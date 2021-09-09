using System;



namespace WhatsAppBotNodejsBased.Bot
{
    [Serializable]
    public class Message
    {
        public string from;
        public string body;
        private ChatType Type;

        public Message()
        {
        }

        public ChatType GetChatType()
        {
            if (from.Contains("@c.us"))
            {
                Type = ChatType.Chat;
            }
            else if (from.Contains("@g.us"))
            {
                Type = ChatType.Group;
            }
            return Type;
        }
        public string GetSenderNumber()
        {
            //Return The Phone Number
            if (from.Contains("@c.us"))
            {
                return "+" + (from.Replace("@c.us", ""));
            }
            else if (from.Contains("@g.us"))
            {
                return "+" + (from.Replace("@g.us", ""));
            }
            else
            {
                return "+" + from;
            }

        }
    }
}
