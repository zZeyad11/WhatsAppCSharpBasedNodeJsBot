using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using WhatsAppBotNodejsBased.Bot.C_Sharp.Event_Handler;
using System.Threading;

namespace WhatsAppBotNodejsBased.Bot
{
    public class Bot
    {
        //Is Whats App Signed In
        public bool IsSignedIn { get; private set; }

        //Profile Picture
        public Bitmap ProfilePic { get; private set; }

        //Is The Bot Still Working
        public bool ISWorking
        {
            get
            {
                return System.Diagnostics.Process.GetProcessesByName("node").Length > 0;
            }
        }

        //Bot Listening Port
        public int Port;



        private System.Diagnostics.Process process;
        private UDPConnection.UdpSocketClient udpSocket;
        private QR_Preview _Preview;



        public event BotStautsChangedEventHandler BotStautsChangedEvent;
        public event GotChatsListEventHandler GotChatsListEvent;
        public event GotContactsListEventHandler GotContactsListEvent;
        public event GotGroupsListEventHandler GotGroupsListEvent;
        public event GotNewMessageEventHandler GotNewMessageEvent;
        public event GotProfilePicEventHandler GotProfilePicEvent;
        public event GotUnReadedMessagesListEventHandler GotUnReadedMessagesListEvent;
        public event WhatsUpLoggedInEventHandler WhatsUpLoggedInEvent;



        public Bot(int Port = 20567)
        {
            this.Port = Port;
            StartBotInBackGround(Port);
            udpSocket = new UDPConnection.UdpSocketClient();
            udpSocket.ConnectAsync("localhost", Port);
            udpSocket.MessageReceived += GotResponse;
            _Preview = new QR_Preview();
            IsSignedIn = false;
        }


        public string GET_ID(string PhoneNumberWithCountryCodeWithoutThePlusSign,ChatType type) //Only For Chat not Group
        {
            if(type == ChatType.Chat)
            {
                return PhoneNumberWithCountryCodeWithoutThePlusSign + "@c.us";
            }
            else
            {
                return null;
            }

        }

        private async void GetStautsInBackground()
        {
            while (true)
            {
                await udpSocket.SendAsync(Encoding.ASCII.GetBytes("getStauts"));
                await Task.Delay(1000);
            }
        }

        public void Close()
        {
            foreach (var node in System.Diagnostics.Process.GetProcessesByName("node"))
            {
                node.Kill();
            }
        }

        private void GotResponse(object sender, UDPConnection.UdpSocketMessageReceivedEventArgs e)
        {
            var Message = Encoding.ASCII.GetString(e.ByteData, 0, e.ByteData.Length); //Get Decoded Message


            if (Message.StartsWith("qr"))
            {
                Thread t = new Thread(() =>
                {
                    if (_Preview.IShown) _Preview.Close();
                    _Preview = new QR_Preview();
                    var QR_Code = Message.Replace("qr:", ""); //Get QR Code
                    _Preview.ShowQR(QR_Code); // Display QR Code
                    _Preview.ShowDialog();
                });

                t.Start();

            }

            else if (Message.StartsWith("ready"))
            {
                try
                {
                    _Preview.Invoke(new Action(() =>
                    {
                        _Preview.Close(); // End Display QR Code
                }));
                }
                catch { }

                GetStautsInBackground(); //Start Getting Whatapp Stauts
                GetPic(); //Get Profile Pic

                WhatsUpLoggedInEvent?.Invoke(this, new WhatsUpLoggedIn());


            }
            else if (Message.StartsWith("stauts:"))
            {
                var Stauts = Message.Replace("stauts:", "");

                //Raise Event If Stauts Changed
                if ((Stauts == "CONNECTED") == !IsSignedIn)
                {
                    BotStautsChangedEvent?.Invoke(this, new BotStautsChanged((Stauts == "CONNECTED")));
                }

                //Set New Stauts
                IsSignedIn = Stauts == "CONNECTED";



            }
            else if (Message.StartsWith("pic:"))
            {
                var PicURL = Message.Replace("pic:", "");
                using (WebClient wc = new WebClient())
                {
                    using (Stream s = wc.OpenRead(PicURL))
                    {
                        ProfilePic = new Bitmap(s);
                    }
                }


                //Got Profile Pic & Raise Event
                GotProfilePicEvent?.Invoke(this, new GotProfilePic(ProfilePic));
            }

            else if (Message.StartsWith("message:"))
            {
                var Messages = Message.Replace("message:", "");
                Message message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(Messages);


                //Got New Message & Raise Event
                GotNewMessageEvent?.Invoke(this, new GotNewMessage(message));
            }
            else if (Message.StartsWith("chats:"))
            {
                var Messages = Message.Replace("chats:", "");
                List<Chat> Chats = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Chat>>(Messages);

                //Got Chats & Raise Event
                GotChatsListEvent?.Invoke(this, new GotChatsList(Chats));
            }
            else if (Message.StartsWith("groups:"))
            {
                var Messages = Message.Replace("groups:", "");
                List<Group> Groups = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Group>>(Messages);


                //Got Groups & Raise Event
                GotGroupsListEvent?.Invoke(this, new GotGroupsList(Groups));
            }
            else if (Message.StartsWith("contacts:"))
            {
                var Messages = Message.Replace("contacts:", "");
                List<Contact> Contacts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contact>>(Messages);

                //Got Contacts & Raise Event
                GotContactsListEvent?.Invoke(this, new GotContactsList(Contacts));

            }
            else if (Message.StartsWith("UnReadedMessages:"))
            {
                var Messages = Message.Replace("UnReadedMessages:", "");
                List<Message> UnreadMessages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Message>>(Messages);

                //Got Unreaded Messages & Raise Event
                GotUnReadedMessagesListEvent?.Invoke(this, new GotUnReadedMessagesList(UnreadMessages));
            }

        }

        //Start New Instance
        public async void InitializeNewBot(string SessionName)
        {
            await udpSocket.SendAsync(Encoding.ASCII.GetBytes("startnew" + SessionName));
        }

        public async void RequestChatsList()
        {
            await udpSocket.SendAsync(Encoding.ASCII.GetBytes("getchats"));

        }
        public async void RequestUnreadedMessagesList()
        {
            await udpSocket.SendAsync(Encoding.ASCII.GetBytes("getUnreadMessages"));

        }
        public async void RequestGroupsList()
        {
            await udpSocket.SendAsync(Encoding.ASCII.GetBytes("getgroups"));

        }
        public async void RequestContactsList()
        {
            await udpSocket.SendAsync(Encoding.ASCII.GetBytes("getcontacts"));

        }


        public async void SendMessage(string TO_ID, string Content, MessageType type)
        {
            switch (type)
            {
                case MessageType.Text:
                    await udpSocket.SendAsync(Encoding.ASCII.GetBytes("sendtext" + TO_ID + "," + Content));
                    break;

                case MessageType.Image:
                    await udpSocket.SendAsync(Encoding.ASCII.GetBytes("sendimg" + TO_ID + "," + Content));
                    break;

                case MessageType.Video:
                    await udpSocket.SendAsync(Encoding.ASCII.GetBytes("sendvid" + TO_ID + "," + Content));
                    break;

            }
        }

        //Send Get Profile Pic Order
        private async void GetPic()
        {
            await udpSocket.SendAsync(Encoding.ASCII.GetBytes("getProfilePicUrl"));
        }

        //Load Existing Session
        public async void OpenSessionBot(string SessionName)
        {
            await udpSocket.SendAsync(Encoding.ASCII.GetBytes("opensession" + SessionName));
        }



        private void StartBotInBackGround(int Port)
        {
            process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = Environment.CurrentDirectory + "/bot";
            startInfo.Arguments = $"/c call node bot.js /p {Port}";
            process.StartInfo = startInfo;
            process.Start();

        }
    }


}
