using System;



namespace WhatsAppBotNodejsBased.Bot
{
    public class BotStautsChanged : EventArgs
    {
        public BotStautsChanged(bool Stauts)
        {
            NewStauts = Stauts;
        }

        public bool NewStauts { get; private set; }

    }


}
