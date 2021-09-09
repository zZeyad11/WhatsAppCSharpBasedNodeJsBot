using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WhatsAppBotNodejsBased.Bot;

namespace Example
{
    public partial class Form1 : Form
    {
        Bot Bot;
        public Form1()
        {
            InitializeComponent();
            Bot = new Bot();
            Bot.BotStautsChangedEvent += Bot_BotStautsChangedEvent;
            Bot.GotProfilePicEvent += Bot_GotProfilePicEvent;
            Bot.WhatsUpLoggedInEvent += Bot_WhatsUpLoggedInEvent;
            Bot.GotChatsListEvent += Bot_GotChatsListEvent;
            Bot.GotContactsListEvent += Bot_GotContactsListEvent;
            Bot.GotGroupsListEvent += Bot_GotGroupsListEvent;
            Bot.GotNewMessageEvent += Bot_GotNewMessageEvent;
        }

        private void Bot_GotNewMessageEvent(Bot sender, GotNewMessage e)
        {


            var MessageText = e.NewMessage.body; //Text Body
            var MessageFrom = e.NewMessage.GetSenderNumber(); //Sender Phone Number
        }

        private void Bot_GotGroupsListEvent(Bot sender, GotGroupsList e)
        {
            //You Must Call The request Funcation of it First "Bot.RequestGroupsList();"

            foreach (Group group in e.Groups)
            {
                var InviteLink = group.GroupLink; //URL Link
                var Name = group.name; //Name
                var members = group.Members; //Members
                var CanSendMessage = group.isReadOnly; //Can Send Message or not (Admin Stopped Messsages)
            }
        }

        private void Bot_GotContactsListEvent(Bot sender, GotContactsList e)
        {
            //You Must Call The request Funcation of it First "Bot.RequestContactsList();"


            foreach (Contact Contact in e.Contacts)
            {
                var Phone = Contact.PhoneNumber; //Phone Number
                var Name = Contact.Name; //Name
            }
        }

        private void Bot_GotChatsListEvent(Bot sender, GotChatsList e)
        {
            //You Must Call The request Funcation of it First "Bot.RequestChatsList();"


            foreach (Chat Chat in e.Chats)
            {
                var ID = Chat.id.user; //Chat ID
                var Phone = Chat.GetPhoneNumber(); //Phone Number
                var Name = Chat.name; //Name

            }
        }

        private void Bot_WhatsUpLoggedInEvent(Bot sender, WhatsUpLoggedIn e)
        {
            //Logged in
        }

        private void Bot_GotProfilePicEvent(Bot sender, GotProfilePic e)
        {
            Invoke(new Action(delegate { pictureBox1.BackgroundImage = e.Pic; }));

            // Bot.SendMessage(Bot.GET_ID("201067528903", ChatType.Chat), "Hello from c#", MessageType.Text); //Send Simple Message
            // Bot.SendMessage(Bot.GET_ID("201067528903",ChatType.Chat),"Path To Imamge",MessageType.Image); //Send Image Message
            // Bot.SendMessage(Bot.GET_ID("201067528903",ChatType.Chat), "Path To Video", MessageType.Video); //Send Simple Message
        }

        private void Bot_BotStautsChangedEvent(Bot sender, BotStautsChanged e)
        {
            Invoke(new Action(delegate
            {
                if (e.NewStauts)
                {
                    Stauts.Text = "Working";
                    Stauts.ForeColor = Color.Green;
                }
                else
                {
                    Stauts.Text = "Passive";
                    Stauts.ForeColor = Color.Red;
                }
            }));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Start New Whatsapp Bot From Scratch
            Bot.InitializeNewBot(textBox1.Text);
            button1.Enabled = false;
            button2.Enabled = false;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Open Existing Session
            Bot.OpenSessionBot(textBox1.Text);
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Close The bot //Must Be Done to avoid Problems at the end of your app
            Bot.Close();
        }
    }
}
