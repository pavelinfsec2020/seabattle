using System;
using System.Collections.Generic;

using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;

namespace SeaBattle.Controllers
{
    internal static class NetController
    {
      public  static Viewers.ConnectionViewer _conView = new Viewers.ConnectionViewer();
        private static int _countOfClicks = 0;
       public static  SoundPlayer _startMusic = new SoundPlayer(Properties.Resources.fone);
     
        public static Models.TcpConnection connection = new Models.TcpConnection();

       
    
        public static void StartController()
        {
            _startMusic.PlayLooping(); 
            InicializeButtonEvents();
            _conView.ShowDialog();     
        }
        
        private static void InicializeButtonEvents()
        {
            _conView.button1.Click += (e, EventArgs) =>
            {
                
                connection.StartServer();              
            };
            _conView.button2.Click += (e, EventArgs) =>
            {
                connection.StartClient();
            };
            _conView.button3.Click += (e, EventArgs) =>
            {
                _conView.Close();
            };
            _conView.button4.Click += (e, EventArgs) =>
            {
                if (_countOfClicks % 2 == 0)
                {
                    _conView.button4.BackgroundImage = Properties.Resources.volumeOff;
                    _startMusic.Stop();
                }
                else
                {
                    _conView.button4.BackgroundImage = Properties.Resources.volumeOn;
                    _startMusic.Play();
                }
            _countOfClicks++;
            };
        }
    }
}
