using System;
using System.Collections.Generic;



namespace WhatsAppBotNodejsBased.Bot
{
    public class GotUnReadedMessagesList : EventArgs
    {
        public GotUnReadedMessagesList(List<Message> messages)
        {
            this.messages = messages;
        }
        public List<Message> messages { get; private set; }
    }


}
