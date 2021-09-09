using System;
using System.Collections.Generic;



namespace WhatsAppBotNodejsBased.Bot
{
    public class GotContactsList : EventArgs
    {
        public GotContactsList(List<Contact> Contacts)
        {
            this.Contacts = Contacts;
        }
        public List<Contact> Contacts { get; private set; }
    }


}
