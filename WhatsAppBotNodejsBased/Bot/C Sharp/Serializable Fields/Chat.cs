using System;



namespace WhatsAppBotNodejsBased.Bot
{
    [Serializable]
    public class Chat
    {
        public string name;
        public ID id;
        


        public string GetPhoneNumber()
        {
            return id.user.Contains("+")? id.user :  "+" + id.user;
        }

        [Serializable]
        public class ID
        {
            public string user;
        }
    }
}
