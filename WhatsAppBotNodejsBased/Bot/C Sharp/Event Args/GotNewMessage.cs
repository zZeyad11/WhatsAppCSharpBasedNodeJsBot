using System;



namespace WhatsAppBotNodejsBased.Bot
{
    public class GotNewMessage : EventArgs
    {
        public GotNewMessage(Message M)
        {
            NewMessage = M;
        }
        public Message NewMessage { get; private set; }
    }


}
