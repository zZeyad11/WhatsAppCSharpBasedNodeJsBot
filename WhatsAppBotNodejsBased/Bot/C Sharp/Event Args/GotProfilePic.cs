using System;
using System.Drawing;



namespace WhatsAppBotNodejsBased.Bot
{
    public class GotProfilePic : EventArgs
    {
        public GotProfilePic(Bitmap Image)
        {
            Pic = Image;
        }
        public Bitmap Pic { get; private set; }
    }


}
