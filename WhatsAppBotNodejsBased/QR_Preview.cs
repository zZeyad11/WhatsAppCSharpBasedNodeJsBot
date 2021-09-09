using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace WhatsAppBotNodejsBased
{
    public partial class QR_Preview : Form
    {
        public bool IShown;
        public QR_Preview()
        {
            InitializeComponent();

        }
        public void ShowQR(string QR)
        {
            Thread GetPic = new Thread(() =>
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(QR, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                Invoke(new Action(delegate
                {
                    pictureBox1.BackgroundImage = qrCodeImage;
                }));
            });


            GetPic.Start();

        }

        private void QR_Preview_Shown(object sender, EventArgs e)
        {
            IShown = true;
        }
    }
}
