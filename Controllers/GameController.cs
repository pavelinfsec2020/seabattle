using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace SeaBattle.Controllers
{
    internal class GameController
    {
        public static Viewers.BattleViewer _battleView = new Viewers.BattleViewer();
        private static Models.SeaBattle _seaBattl;
        private static NetworkStream stream = Controllers.NetController.connection._client.GetStream();
        private static BinaryFormatter binary = new BinaryFormatter();
       
        public static void StartController(Dictionary<string, Image> rivalInfo)      
        { 
            SetNicksAndAvatars(rivalInfo);
            StartNewGame();
            _battleView.ShowDialog();
            
        }
        public static void StartNewGame()
        {
            _seaBattl = new Models.SeaBattle();
            _battleView.label3.Text = "Рosition your ships!";
            InicializeButtonEvents();
            InitializeRivalsField();
          
           
        }
        private static void SetNicksAndAvatars(Dictionary<string, Image> rivalInfo)
        {
            Image yourAvatar;
            rivalInfo.TryGetValue("You", out yourAvatar);
            _battleView.pictureBox1.BackgroundImage = yourAvatar;

            foreach (var info in rivalInfo)
            {
                _battleView.label2.Text = info.Key;
                _battleView.pictureBox2.BackgroundImage = info.Value;
            }
        }
        private static void InicializeButtonEvents()
        {
            _battleView.button1.Click += (e, EventArgs) =>
            {
                _seaBattl.StartExpectOpponet();
            };
            _battleView.button2.Click += (e, EventArgs) =>
            {
                _seaBattl.SetShipOnFiled(3);
            };
            _battleView.button3.Click += (e, EventArgs) =>
            {
                _seaBattl.SetShipOnFiled(2);
            };
            _battleView.button4.Click += (e, EventArgs) =>
            {
                _seaBattl.SetShipOnFiled(1);
            };
            _battleView.button5.Click += (e, EventArgs) =>
            {
                _seaBattl.SetShipOnFiled(4);
            };
            _battleView.button6.Click += (e, EventArgs) =>
            {
              binary.Serialize(stream, "RequestForNewGame");
                _battleView.button6.Visible = false;
            };
        }
        private static void InitializeRivalsField()
        {
            _battleView.dataGridView2.MouseClick += (e, EventArgs) =>
            {
                if (EventArgs.Button == MouseButtons.Right && 
                _battleView.dataGridView2.SelectedCells.Count!=0)
                {                
                    binary.Serialize(
                        stream,
                        new int[] {
                            _battleView.dataGridView2.SelectedCells[0].RowIndex,
                            _battleView.dataGridView2.SelectedCells[0].ColumnIndex
                        }
                   );
                   
                    _battleView.dataGridView2.Enabled = false;
                }
                  
            };
        }
    }
}
