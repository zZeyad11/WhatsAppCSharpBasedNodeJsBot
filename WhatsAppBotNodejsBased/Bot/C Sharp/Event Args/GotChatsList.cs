using System;
using System.Collections.Generic;



namespace WhatsAppBotNodejsBased.Bot
{
    public class GotChatsList : EventArgs
    {
        public GotChatsList(List<Chat> Chats)
        {
            this.Chats = Chats;
        }
        public List<Chat> Chats { get; private set; }
    }


}
