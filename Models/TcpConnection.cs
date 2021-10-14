using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using controller = SeaBattle.Controllers.NetController;
namespace SeaBattle.Models
{
    internal class TcpConnection
    {
        private static int _port=1000 ;
        private static string _hostName="127.0.0.1";
        private  TcpListener _server;
        private NetworkStream stream;
        private BinaryFormatter binary = new BinaryFormatter();
        public TcpClient _client;
        public bool isServer;
        delegate void labelText(string newText);
        private async  void WaitForClient()
        {
            _server = new TcpListener(IPAddress.Any, _port);
            _server.Start();
            _client = new TcpClient();
            while (_client.Connected == false)
            {
                try 
                { 
                    _client = await _server.AcceptTcpClientAsync(); 
                } 
                catch (Exception e) 
                {
                    MessageBox.Show(e.Message.ToString());
                    return;
                }
            }
            
            controller._conView.label2.Text = "Game started!";          
            Controllers.GameController.StartController
               (
                 StartGameBeforeConnection()
               );
        
        }
        private async  void ConnectToServer()
        {
               _client = new TcpClient();
                while (_client.Connected == false) 
                try 
                { 
                    await _client.ConnectAsync(_hostName, _port); 
                } 
                catch (Exception e) 
                {
                    MessageBox.Show(e.Message.ToString());
                    return;
                }

            
            controller._conView.label2.Invoke(new labelText(A => controller._conView.label2.Text = A ), "Game started!");
            Controllers.GameController.StartController
                (
                  StartGameBeforeConnection()
                );
       
        }
        private  Dictionary<string,Image> StartGameBeforeConnection()
        {
            Image rivalAvatar;
            string rivalNick;
            Dictionary<string, Image> rivalInfo =  new Dictionary<string, Image>();
            rivalInfo.Add("You", controller._conView.pictureBox2.BackgroundImage);
           
            stream = _client.GetStream();         
            binary.Serialize(stream, controller._conView.textBox1.Text);
            rivalNick = (string) binary.Deserialize(stream);
            Thread.Sleep(200);
            binary.Serialize(stream, controller._conView.pictureBox2.BackgroundImage);
            rivalAvatar = (Image)binary.Deserialize(stream);
      
            rivalInfo.Add(rivalNick,rivalAvatar);
            return rivalInfo;
        }
        public  void StartServer()
        {
            Point[] points = CreateSettingsField("     Server settings");
            isServer = true;

            //добавление на экран ip адреса сервера 
            GetLocalIPAddress();
            controller._conView.groupBox1.Controls.Add
              (
                CreateFormsElement
                (
                new Label(),
                "ipText",
                "IP:\t " + _hostName,
                points[0],
                new Size(200,40)
                )
              );
           // добавление на экран поля для ввода порта сервера
            controller._conView.groupBox1.Controls.Add
           (
             CreateFormsElement
             (
             new TextBox(),
             "portBox",
             "Enter port...",
             points[1],
             new Size(200, 40)
             )
           );
            // добавление на экран кнопки старта сервера
            controller._conView.groupBox1.Controls.Add
           (
             CreateFormsElement
             (
             new Button(),
             "buttonStart",
             "Start server",
             points[2],
             new Size(200, 40)
             )
           );
           //Событие созданной выше кнопки
            Button button = (Button) controller._conView.groupBox1.Controls.Find("buttonStart",false)[0];
            button.Click += (e, EventArgs) =>
            {
                controller._conView.label2.Text = "Waiting for client...";
                string enteredPort = ((TextBox) controller._conView.groupBox1.Controls.Find("portBox", false)[0]).Text;
                Int32.TryParse(enteredPort,out _port);
                WaitForClient();
            };
        }
        public  void StartClient()
        {
            Point[] points = CreateSettingsField("      Client settings");
            isServer = false;
            //добавление на экран ip адреса сервера 
            controller._conView.groupBox1.Controls.Add
              (
                CreateFormsElement
                (
                new TextBox(),
                "ipTextBox",
                "server's IP...",
                points[0],
                new Size(200, 40)
                )
              );
            // добавление на экран поля для ввода порта сервера
            controller._conView.groupBox1.Controls.Add
           (
             CreateFormsElement
             (
             new TextBox(),
             "portTextBox",
             " server's port...",
             points[1],
             new Size(200, 40)
             )
           );
            // добавление на экран кнопки старта сервера
            controller._conView.groupBox1.Controls.Add
           (
             CreateFormsElement
             (
             new Button(),
             "buttonConnect",
             "Connect",
             points[2],
             new Size(200, 40)
             )
           );
            //Событие созданной выше кнопки
            Button button = (Button)controller._conView.groupBox1.Controls.Find("buttonConnect", false)[0];
            button.Click += (e, EventArgs) =>
            {
                controller._conView.label2.Text = "Connecting...";

                _hostName = ((TextBox)controller._conView.groupBox1.Controls.Find("ipTextBox", false)[0]).Text;
                string enteredPort = ((TextBox)controller._conView.groupBox1.Controls.Find("portTextBox", false)[0]).Text;
                
                Int32.TryParse(enteredPort, out _port);
                ConnectToServer();
            };
        }
        private  Point[] CreateSettingsField(string topText)
        {
            Point temp = controller._conView.button1.Location;
            Point[] points = new Point[3];
            points[0] =  new Point(temp.X+30,temp.Y);
            points[1] =  new Point(points[0].X,points[0].Y+40);
            points[2] = new Point(points[1].X, points[1].Y + 40);

            controller._conView.label2.Text = topText;
            controller._conView.button1.Dispose();
            controller._conView.button2.Dispose();
            return points;
        }
        private  TControl CreateFormsElement<TControl> 
             (
            TControl control ,  
           string name,
            string text,
                Point location,
                Size size
            ) where TControl : Control
        {
            control.Name = name;
            control.Size = size;
            control.Text = text;
            control.Location = location;
            control.ForeColor = Color.White;
            control.BackColor = Color.FromArgb(0, 0, 54);
            control.Font = new Font("Times New Roman", 16, FontStyle.Bold);
            return control;
            
        }
        private  void GetLocalIPAddress()
        {
           
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _hostName = ip.ToString();

                }
            }
           
        }
    }
}
