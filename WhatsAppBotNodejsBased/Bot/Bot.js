const { Client, MessageMedia } = require('./index');
var udp = require('dgram');
const fs = require('fs');




var server = udp.createSocket('udp4');
var WClient = null;
var SESSION_FILE_PATH = "";
let sessionData = null;
var SenderPort = null;


// emits when any error occurs
server.on('error', function(error) {
    console.log('Error: ' + error);
    server.close();
});

// emits on new datagram msg
server.on('message', async function(msg, info) {
    //console.log('Data received from client : ' + msg.toString());

    //Start New WhatsWeb Instance
    if (msg.toString().includes('startnew')) {
        var Name = msg.toString().replace("startnew", "");
        SESSION_FILE_PATH = `./${Name}.json`;


        if (fs.existsSync(SESSION_FILE_PATH)) {
            sessionData = require(SESSION_FILE_PATH);
        }
        WClient = new Client({
            takeoverOnConflict: true
        });


        SetWhatsAppClient(info);
        //Opens New WhatsWeb Instance
    } else if (msg.toString().includes('opensession')) {
        var Name = msg.toString().replace("opensession", "");
        SESSION_FILE_PATH = `./${Name}.json`;

        if (fs.existsSync(SESSION_FILE_PATH)) {
            sessionData = require(SESSION_FILE_PATH);
        }
        WClient = new Client({
            session: sessionData,
            takeoverOnConflict: true
        });
        SetWhatsAppClient(info);
    } else if (msg.toString().includes('getStauts')) {
        try {
            var s = await WClient.getState();
            server.send("stauts:" + s, SenderPort, 'localhost', function(error) {});
        } catch (e) {

        }


    } else if (msg.toString().includes('getProfilePicUrl')) {
        try {
            var s = await WClient.getProfilePicUrl(WClient.info.wid.user);
            server.send("pic:" + s, SenderPort, 'localhost', function(error) {});
        } catch (e) {

        }


    } else if (msg.toString().includes('sendtext')) {
        var Raw = msg.toString().replace("sendtext", "").split(',')[1];
        var ID = msg.toString().replace("sendtext", "").split(',')[0];
        await (await WClient.getChatById(ID)).sendMessage(Raw);



    } else if (msg.toString().includes('sendimg')) {
        var Raw = msg.toString().replace("sendimg", "").split(',')[1];
        var ID = msg.toString().replace("sendimg", "").split(',')[0];
        var media = MessageMedia.fromFilePath(Raw);
        await (await WClient.getChatById(ID)).sendMessage(media);

    } else if (msg.toString().includes('sendvid')) {
        var Raw = msg.toString().replace("sendvid", "").split(',')[1];
        var ID = msg.toString().replace("sendvid", "").split(',')[0];
        var media = MessageMedia.fromFilePath(Raw);
        await W(await WClient.getChatById(ID)).sendMessage(media);


    } else if (msg.toString().includes('getchats')) {
        var Chats = JSON.stringify((await WClient.getChats()).filter(ch => ch.isGroup == false));
        server.send("chats:" + Chats, SenderPort, 'localhost', function(error) {});

    } else if (msg.toString().includes('getUnreadMessages')) {
        var Chats = (await WClient.getChats()).filter(ch => ch.unreadCount > 0); // Get UnreadChats
        var UnReadedMessages = [];

        for (var I = 0; I < Chats.length; I++) {
            var UnreadedMessages = (await Chats[I].fetchMessages({ limit: Chats[I].unreadCount })).map((m) => {
                return {
                    from: m.from,
                    body: m.body
                }
            });
            UnreadedMessages.forEach(message => {
                UnReadedMessages.push(message);
            });
        }
        server.send("UnReadedMessages:" + JSON.stringify(UnReadedMessages), SenderPort, 'localhost', function(error) {});

    } else if (msg.toString().includes('getgroups')) {

        var Chats = (await WClient.getChats()).filter(ch => ch.isGroup == true);
        var ModifiedChats = [];
        for (let chatnum = 0; chatnum < Chats.length; chatnum++) {
            var ILink = null;
            try {
                ILink = "https://chat.whatsapp.com/" + await (Chats[chatnum].getInviteCode());
            } catch {}
            var MemberList = [];

            //Get Each Member of Each Group
            for (let MemberNum = 0; MemberNum < Chats[chatnum].groupMetadata.participants.length; MemberNum++) {
                var Current = Chats[chatnum].groupMetadata.participants[MemberNum];
                var Name = await WClient.getContactById(Current.id._serialized);
                var Member = {
                    Name: Name.pushname,
                    Phone: Current.id.user,
                    IsAdmin: Current.isAdmin
                };
                MemberList.push(Member);
            }

            //Create Formated Group Info
            var GroupChat = {
                name: Chats[chatnum].name,
                isReadOnly: Chats[chatnum].isReadOnly,
                GroupLink: ILink,
                Members: MemberList
            };
            ModifiedChats.push(GroupChat);

        }

        server.send("groups:" + JSON.stringify(ModifiedChats), SenderPort, 'localhost', function(error) {});

    } else if (msg.toString().includes('getcontacts')) {

        var Contacts = (await WClient.getContacts()).map(a => {
            return {
                Name: a.name,
                PhoneNumber: a.number
            }
        });
        server.send("contacts:" + JSON.stringify(Contacts), SenderPort, 'localhost', function(error) {});
    }
});






//Start Listeing On port
server.on('listening', function() {
    var address = server.address();
    var port = address.port;
    var family = address.family;
    var ipaddr = address.address;
    console.log('Server is listening at port' + port);
    console.log('Server ip :' + ipaddr);
    console.log('Server is IP4/IP6 : ' + family);
});

//Port Closed
server.on('close', function() {
    console.log('Socket is closed !');
});


server.bind(process.argv[3]); //Start Listeing On 29874

function SetWhatsAppClient(info) {
    if (SenderPort == null) {
        SenderPort = info.port;
        //Auto Save session and Send Feedback to the C#
        WClient.on('authenticated', (session) => {
            sessionData = session;
            fs.writeFile(SESSION_FILE_PATH, JSON.stringify(session), (err) => {
                if (err) {
                    console.error(err);
                }
            });
            server.send("authenticated", SenderPort, 'localhost', function(error) {});
        });

        WClient.on('qr', (qr) => {
            server.send("qr:" + qr, SenderPort, 'localhost', function(error) {});

        });

        WClient.on('auth_failure', msg => {
            // Fired if session restore was unsuccessfull
            server.send("auth_failure", SenderPort, 'localhost', function(error) {});
        });

        WClient.on('ready', () => {
            server.send("ready", SenderPort, 'localhost', function(error) {});
        });

        WClient.on('message', (message) => {
            server.send("message:" + JSON.stringify(message), SenderPort, 'localhost', function(error) {});
        });

        WClient.initialize();
    }
}