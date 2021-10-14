using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace SeaBattle.Viewers
{
      
    public partial class ConnectionViewer : Form
    {
       
        public ConnectionViewer()
        {
            InitializeComponent();
          pictureBox1.Image = Image.FromStream(new WebClient().OpenRead("https://c.tenor.com/AjPDwjtHGuAAAAAC/world-of-warships-battleship.gif"));
            
                 
            ImageAnimator.Animate(pictureBox1.Image, OnFrameChanged);
        }
        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() => OnFrameChanged(sender, e)));
                return;
            }
            ImageAnimator.UpdateFrames();
            Invalidate(false);
        
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                try
                {
                    open.ShowDialog();
                    Image avatar = Image.FromFile(open.FileName);
                    avatar = (Image)(new Bitmap(avatar, new Size(300, 300)));
                    pictureBox2.BackgroundImage = avatar;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

            }
        }
    }
}
