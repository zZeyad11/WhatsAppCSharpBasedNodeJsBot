using System;
using System.Collections.Generic;



namespace WhatsAppBotNodejsBased.Bot
{
    public class GotGroupsList : EventArgs
    {
        public GotGroupsList(List<Group> Groups)
        {
            this.Groups = Groups;
        }
        public List<Group> Groups { get; private set; }
    }


}
