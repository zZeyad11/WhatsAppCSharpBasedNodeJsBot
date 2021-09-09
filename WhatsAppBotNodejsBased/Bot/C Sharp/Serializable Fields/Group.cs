using System;
using System.Collections.Generic;



namespace WhatsAppBotNodejsBased.Bot
{
    [Serializable]
    public class Group
    {
        public string name;
        public bool isReadOnly;
        public string GroupLink;
        public List<Member> Members;

        [Serializable]
        public class Member
        {
            public string Name;
            public string Phone;
            public bool IsAdmin;
        }


        public int GetMembersCount()
        {
            return Members.Count;
        }
    }
}
