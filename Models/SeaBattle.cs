using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using controller = SeaBattle.Controllers.GameController;
using tcp = SeaBattle.Controllers.NetController;

namespace SeaBattle.Models
{
    internal class SeaBattle
    {
        private const int ONE_DECK_NUMBER = 4;
        private const int TWO_DECK_NUMBER = 3;
        private const int THREE_DECK_NUBMER = 2;
        private const int FOUR_DECK_NUMBER = 1;
        private const int ALL_SHIPS_NUMBER = 10;
        private BinaryFormatter binary = new BinaryFormatter();
        private NetworkStream stream;
        private int[] _countAddedShips = new int[4];
        private BattleField battleField = new BattleField();
        private delegate void statusMoveText(string newText);
        private delegate void rivalFieldActivate(bool status);
        private delegate void setColorOnCell(Color color);
        private delegate void ClearDataGridV(DataGridView DG);
        private delegate void ChangeColorOfCell(int[] coordinate);
        private delegate void SetActiveNewGameBttn(bool isVisible);
        private delegate void EnableAddShipsButtns(bool isEnabled);
        private Thread ListenCommands;
        private int _shipSize;
        private int yourSuccessShots = 0;
        private int yourBadShots = 0;
        private int rivalSuccessShots = 0;
        private int rivalBadShots = 0;

        ~SeaBattle()
         {
           if(ListenCommands!=null)
            ListenCommands.Abort();
            ListenCommands = null;
         }
        public void SetShipOnFiled(int shipSize)
        {         
            _shipSize = shipSize;
            controller._battleView.dataGridView1.Enabled = true;
            controller._battleView.dataGridView1.MouseClick += (e, MouseEventArgs ) =>
            {
               if(MouseEventArgs.Button == MouseButtons.Right)
               {
                    if (CheckPaintedShip(_shipSize))
                    {
                        battleField.SetShip(new Ship(_shipSize), GetCoordsOfSelectedCells());
                        PaintAddedShip();
                        
                        controller._battleView.dataGridView1.ClearSelection();
                    }
                    else 
                    {
                        
                        controller._battleView.dataGridView1.ClearSelection();
                        return;
                    }
                   
                    switch (_shipSize)
                    {

                        case 1:
                           
                            _countAddedShips[_shipSize - 1]++;
                            controller._battleView.dataGridView1.Enabled = false;
                            if (_countAddedShips[_shipSize - 1] == ONE_DECK_NUMBER)
                            {
                                controller._battleView.button4.Enabled = false;                              
                            }
                            else
                            { 
                               
                            }
                            break;
                        case 2:
                           
                            _countAddedShips[_shipSize - 1]++;
                            controller._battleView.dataGridView1.Enabled = false;
                            if (_countAddedShips[_shipSize - 1] == TWO_DECK_NUMBER)
                            {
                                controller._battleView.button3.Enabled = false;
                            }
                            else
                            {

                            }
                            break;
                        case 3:
                            
                            _countAddedShips[_shipSize - 1]++;
                            controller._battleView.dataGridView1.Enabled = false;
                            if (_countAddedShips[_shipSize - 1] == THREE_DECK_NUBMER)
                            {
                                controller._battleView.button2.Enabled = false;
                            }
                            else
                            {

                            }
                            break;
                        case 4:
                           
                            _countAddedShips[_shipSize - 1]++;
                            controller._battleView.dataGridView1.Enabled = false;
                            if (_countAddedShips[_shipSize - 1] == FOUR_DECK_NUMBER)
                            {
                                controller._battleView.button5.Enabled = false;
                            }
                            else
                            {

                            }
                            break;
                         
                    }
                   
                }
            };
           
        }
        private void ActiveButtnNewGame(bool isVisible)
        {
            controller._battleView.button6.Invoke(new SetActiveNewGameBttn(A => controller._battleView.button6.Visible = A),isVisible);
        }
        private void UpdateBattleInfo(string info, Label gameLabel)
        {
            gameLabel.Invoke(new statusMoveText(A =>
                      gameLabel.Text = A),
                      info);
        }
        private void EnableButton(Button button)
        {
            button.Invoke(new EnableAddShipsButtns(A => button.Enabled = A), true);
        }
        private void ClearSelectedCell()
        {
            controller._battleView.dataGridView2.Invoke(new setColorOnCell(A =>
                       controller._battleView.dataGridView2.ClearSelection()
                      ) );
        }
       
        private  void GetAndSendNetCommands()
        {
            while ( tcp.connection._client.Connected)
            {
                string command = "";
                try { command = (String)binary.Deserialize(stream); }
                catch (Exception) { }

                switch (command)
                {
                   
                    case "HisMove" :
                        UpdateBattleInfo("Rival's move!", controller._battleView.label3);

                        int[] gettedCoord = (int[])binary.Deserialize(stream);
                        int[,] answer = battleField.GetBangFromRival(gettedCoord);
                        bool rezultFromRival = GetRezultOfRivalsShot(answer);

                        if (battleField.LoseStatus)
                        {
                            new SoundPlayer(Properties.Resources.lose).Play();
                            UpdateBattleInfo(
                               "You Lose!",
                               controller._battleView.label3
                               );
                            answer[0, 0] = -20;
                            binary.Serialize(stream,answer );
                            binary.Serialize(stream, "YouWin");
                            ActiveButtnNewGame(true);
                            //break;
                        }
                        else
                        {
                            binary.Serialize(stream, answer);
                            binary.Serialize(stream, "HisMove");
                        }
                       
                        if (rezultFromRival)
                        {
                           
                            rivalSuccessShots++;
                            UpdateBattleInfo(
                                "GoodShots: " + rivalSuccessShots,
                                controller._battleView.label7
                                );
                        }
                        else
                        {
                            new SoundPlayer(Properties.Resources.bulk).Play();
                            SetColorInCell(gettedCoord,Color.White,controller._battleView.dataGridView1);
                            rivalBadShots++;
                            UpdateBattleInfo(
                                "BadShots: " + rivalBadShots,
                                controller._battleView.label6
                                );
                        }
                        break;
                    case "YourMove":
                        UpdateBattleInfo("Your move!", controller._battleView.label3);
                    
                        controller._battleView.dataGridView2.Invoke(new rivalFieldActivate(A =>
                       controller._battleView.dataGridView2.Enabled = A), true);
                        int[,] coordsFromRival = (int[,])binary.Deserialize(stream);
                        if (coordsFromRival[0, 0] == -20)
                        {
                            yourSuccessShots++;
                            UpdateBattleInfo(
                                "GoodShots: " + yourSuccessShots,
                                controller._battleView.label4
                                );
                            break;
                        } 
                        bool rezFromRivalShot = GetRezultOfRivalsShot(coordsFromRival);
                        SetColorFromRivalShot(coordsFromRival);
                        if (rezFromRivalShot)
                        {
                        
                            yourSuccessShots++;
                            UpdateBattleInfo(
                                "GoodShots: " +yourSuccessShots,
                                controller._battleView.label4
                                ); 
                        }
                        else
                        {
                            new SoundPlayer(Properties.Resources.bulk).Play();
                            yourBadShots++;
                            UpdateBattleInfo(
                                "BadShots: " + yourBadShots,
                                controller._battleView.label5
                                );
                        }
                        controller._battleView.dataGridView2.ClearSelection();
                        binary.Serialize(stream, "YourMove");
                        break;
                    case "YouWin":
                        new SoundPlayer(Properties.Resources.win).Play();
                        UpdateBattleInfo(
                               "You Win!",
                               controller._battleView.label3
                               );
                        ActiveButtnNewGame(true);
                        break;
                    case "RequestForNewGame":
                       ActiveButtnNewGame(false);
                        ResponseForNewGame();                      
                        break;
                    case "StartNewGame":
                        StartNewGame();
                        break;
                }
            }
        }
        private void StartNewGame()
        {           
            _countAddedShips = new int[4];
            battleField = new BattleField();
            yourSuccessShots = 0;
            yourBadShots = 0;
            rivalSuccessShots = 0;
            rivalBadShots = 0;
            UpdateBattleInfo("GoodShots: ",controller._battleView.label4);
            UpdateBattleInfo("GoodShots: ", controller._battleView.label7);
            UpdateBattleInfo("BadShots: ", controller._battleView.label5);
            UpdateBattleInfo("BadShots: ", controller._battleView.label6);
            UpdateBattleInfo("Position your ships!: ", controller._battleView.label3);
            for (int i = 0; i < 10; i++)
                for (int j = 0; j<10; j++)
                {
                    controller._battleView.dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(0, 0, 64);
                    controller._battleView.dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(0, 0, 64);
                }
          
            for (int i = 1; i < 6; i++)
               EnableButton(
                   (Button) controller._battleView.Controls.Find("button" + i, false)[0]
                   );
            ListenCommands.Abort();
        }
        private void ResponseForNewGame()
        { 
            Viewers.NewGame newGame = new Viewers.NewGame();
            newGame.button1.Click += (e, EventArgs) =>
            {
                binary.Serialize(stream,"StartNewGame");
                StartNewGame();
                newGame.Close();
            };
            newGame.button2.Click += (e, EventArgs) =>
            {
                newGame.Close();
            };
            newGame.ShowDialog();
        }
        private int[,] ReverseCellsCollection(int[,] coordinates, bool isHorizontal)
        {
            int n = coordinates.GetLength(0);
            int[,] reversedArray = new int[n, 2];
            int[] indexes;
           
            if (isHorizontal) 
            {
                if (coordinates[0, 1] < coordinates[1, 1])
                    return coordinates;
               else indexes = new int[] { 0, 1 }; 
            }
            else
            {
                if (coordinates[0, 0] < coordinates[1, 0])
                    return coordinates;
              else  indexes = new int[] { 1, 0 };
            
            }
                
            for (int i = 0; i < n; i++)
            {            
                    reversedArray[i, indexes[0]] = coordinates[i, indexes[0]];
                    reversedArray[(n - 1) - i, indexes[1]] = coordinates[i, indexes[1]];               
            }
            return reversedArray;
        }
        private List<int[]> GetNearestCells(int[,] cells)
        {
            
            int n = cells.GetLength(0);
            if (n == 1)
            {
                return GetNearestCellsForHorizontalShip(cells);
            }
            else 
            {
                if (cells[0, 0] == cells[1, 0])
                {
                    return GetNearestCellsForHorizontalShip(
                           ReverseCellsCollection(cells, true)
                           );
                }
                else 
                {
                    return GetNearestCellsForVerticalShip(
                          ReverseCellsCollection(cells, false)
                          );
                }
              
            }
           
        }
        private List<int[]> GetNearestCellsForVerticalShip(int[,] cells)
        {
            string temp = "";
             List<int[]> nearestCells = new List<int[]>();
            //для боковых левой  и правой сторон
            int n = cells.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                if (cells[i, 1] - 1 > -1)
                    nearestCells.Add(new int[] { cells[i, 0] , cells[i, 1]-1 });
                if (cells[i, 1] + 1 < 10)
                    nearestCells.Add(new int[] { cells[i, 0] , cells[i, 1]+1 });
              
            }
          
            //для боковой левой стороны 
            if (cells[0, 0] - 1 > -1 && cells[0, 1] - 1 > -1)
                nearestCells.Add(new int[] { cells[0, 0] - 1, cells[0, 1] - 1 });

            if (cells[0, 0] - 1 > -1)
                nearestCells.Add(new int[] { cells[0, 0]-1, cells[0, 1] });

            if (cells[0, 0] - 1 > -1 && cells[0, 1] + 1 < 10)
                nearestCells.Add(new int[] { cells[0, 0] - 1, cells[0, 1] + 1 });

            //для боковой правой стороны           
            if (cells[n - 1, 0] + 1 < 10 && cells[n - 1, 1] - 1 >-1)
                nearestCells.Add(new int[] { cells[n - 1, 0] + 1, cells[n - 1, 1] - 1 });

            if (cells[n - 1, 0] + 1 < 10)
                nearestCells.Add(new int[] { cells[n - 1, 0]+1, cells[n - 1, 1] });

            if (cells[n - 1, 0] + 1 < 10 && cells[n - 1, 1] + 1 < 10)
                nearestCells.Add(new int[] { cells[n - 1, 0] + 1, cells[n - 1, 1] + 1 });
            return nearestCells;
        }
        private List<int[]> GetNearestCellsForHorizontalShip(int[,] cells)
        {
            string temp = "";
             List<int[]> nearestCells = new List<int[]>();
           //для верхней и нижней стороны
          int n = cells.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                if (cells[i, 0] - 1 > -1)
                    nearestCells.Add(new int[] { cells[i, 0] - 1, cells[i, 1] });
                if (cells[i, 0] + 1 < 10)
                    nearestCells.Add(new int[] { cells[i, 0] + 1, cells[i, 1] });
                temp += cells[i, 0] + "," + cells[i, 1] + ";";
            }
            
            //для боковой левой стороны 
            if (cells[0, 0] - 1 > -1 && cells[0, 1] - 1 > -1)
                nearestCells.Add(new int[] { cells[0, 0] - 1, cells[0, 1] - 1 });

            if (cells[0, 1] - 1 > -1)
                nearestCells.Add(new int[] { cells[0, 0], cells[0, 1] - 1 });

            if (cells[0, 0] + 1 < 10 && cells[0, 1] - 1 > -1)
                nearestCells.Add(new int[] { cells[0, 0] + 1, cells[0, 1] - 1 });

            //для боковой правоq стороны           
            if (cells[n-1, 0] - 1 > -1 && cells[n-1, 1] + 1 < 10)
                nearestCells.Add(new int[] { cells[n-1, 0] - 1, cells[n-1, 1] + 1 });

            if (cells[n-1, 1] + 1 < 10)
                nearestCells.Add(new int[] { cells[n-1, 0], cells[n-1, 1] + 1 });

            if (cells[n-1, 0] + 1 < 10 && cells[n-1, 1] + 1 < 10)
                nearestCells.Add(new int[] { cells[n-1, 0] + 1, cells[n-1, 1] + 1 });
            return nearestCells;
        }
        private void ClearGameField(DataGridView DG)
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    DG.Invoke(new setColorOnCell(A =>
                          DG.Rows[i].Cells[j].Style.BackColor = A),
                            Color.FromArgb(0, 0, 64));
                }                   
        }
        private void SetColorInCell(int[] coordinate, Color color,DataGridView DG)
        {    
            if (coordinate[0] > 9)
            {
                coordinate[0] -= 10;
                DG.Invoke(new setColorOnCell(A =>
                        DG.Rows[coordinate[0]].Cells[coordinate[1]].Style.BackColor = A),
                          Color.Red);
                int[,] oneDeck = new int[,] { { coordinate[0],coordinate[1] } };
                List<int[]> nearCelss =  GetNearestCells(oneDeck);
                for (int i = 0; i < nearCelss.Count; i++)
                    SetColorInCell(nearCelss[i], Color.White, DG);
            }
            else 
            {
                DG.Invoke(new setColorOnCell(A =>
                        DG.Rows[coordinate[0]].Cells[coordinate[1]].Style.BackColor = A),
                          color);
            } 
        }
        private void PaintDiedShip(int[,] answer)
        {
            for (int i = 0; i < answer.GetLength(0); i++)
            {
                SetColorInCell(new int[] { answer[i, 0], answer[i, 1] }, Color.Red, controller._battleView.dataGridView2);
            }
            List<int[]> nearestCellsOfKilledShip = GetNearestCells(answer);
            for (int i = 0; i < nearestCellsOfKilledShip.Count; i++)
            {
                SetColorInCell(new int[] {
                        nearestCellsOfKilledShip[i][0],
                        nearestCellsOfKilledShip[i][1]
                    },
                    Color.White,
                    controller._battleView.dataGridView2);
            }
        }
        private void SetColorFromRivalShot(int[,] answer)
        {
            if (answer.GetLength(0) == 1)
            {
                if (answer[0, 0] != -1)
                {
                    if (answer[0, 0] > 10)
                    {
                        new SoundPlayer(Properties.Resources.killed).Play();
                        answer[0, 0] -= 10;
                        PaintDiedShip(answer);
                    }
                    else
                    {
                        new SoundPlayer(Properties.Resources.goodShot).Play();
                        SetColorInCell(new int[] { answer[0, 0], answer[0, 1] }, Color.Yellow, controller._battleView.dataGridView2);
                    }
                }
                else
                {
                    int rowIndex = controller._battleView.dataGridView2.SelectedCells[0].RowIndex;
                    int columnIndex = controller._battleView.dataGridView2.SelectedCells[0].ColumnIndex;
                    SetColorInCell(new int[] {rowIndex , columnIndex }, Color.White, controller._battleView.dataGridView2);
                }
            }
            else
            {
                new SoundPlayer(Properties.Resources.killed).Play();
                PaintDiedShip(answer);
            }
        }
        private bool GetRezultOfRivalsShot (int[,] answer)
        {
            if (answer.GetLength(0) == 1)
            {
                if (answer[0, 0] != -1) return true;
                else return false;
            }
            else return true;
        }

        public void StartExpectOpponet()
        {
           
            if (CheckAddedShipsCount())
            {
                new SoundPlayer(Properties.Resources.START).Play();
                controller._battleView.label3.Text = "Waiting for rival...";
                 stream = tcp.connection._client.GetStream();
                if (tcp.connection.isServer)
                {
                    Thread.Sleep(500);
                    binary.Serialize(stream, "HisMove");
                }
                else
                {
                    Thread.Sleep(300);
                    binary.Serialize(stream, "YourMove");
                }
                controller._battleView.button1.Enabled = false;
                ListenCommands = new Thread(GetAndSendNetCommands);
                ListenCommands.Start();
            }
            else
            {
                MessageBox.Show("You have positioned not all ships!","Warning!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }
        private bool CheckAddedShipsCount()
        {
            if (battleField._shipsCoordinates.Count == ALL_SHIPS_NUMBER)
                return true;
            else return false;
        }
        private int[,] GetCoordsOfSelectedCells()
        {
            int countSelectedCells = controller._battleView.dataGridView1.SelectedCells.Count;
            int[,] coords = new int[countSelectedCells, 2];
            for (int i = 0; i < countSelectedCells; i++)
            {
                coords[i, 0] = controller._battleView.dataGridView1.SelectedCells[i ].RowIndex;
                coords[i, 1] = controller._battleView.dataGridView1.SelectedCells[i].ColumnIndex;
            }
            return coords;
        }
        private int[,] ConvertSelectedCellsInArray(DataGridView field)
        {
            int n = field.SelectedCells.Count;
            int[,] coordinates = new int[n, 2];
          
            for (int i = 0; i < n; i++)
            {
                coordinates[i, 0] = field.SelectedCells[i].RowIndex;
                coordinates[(n-1)-i, 1] = field.SelectedCells[i].ColumnIndex;             
            }          
            return coordinates;
        } 
        private bool CheckPaintedShip(int shipSize)
        {

            int[,] coordinates = ConvertSelectedCellsInArray(controller._battleView.dataGridView1);
            int countSelectedCells = coordinates.GetLength(0);
            if (countSelectedCells == shipSize)
            {
                List<int[]> nearestCells = GetNearestCells(coordinates);
                for (int i = 0; i < nearestCells.Count; i++)
                {
                    if (battleField._field[nearestCells[i][0], nearestCells[i][1]] == 1)
                        return false;

                }
                if (shipSize == 4)
                {
                    if (shipSize != countSelectedCells) return false;
                    for (int i = 1; i < 4; i++)
                        if ((coordinates[0, 1] != coordinates[i, 1]) &&
                            (coordinates[0, 0] != coordinates[i, 0])
                            ) return false;
                    
                }
                return true;
            }
            else return false;
        }
        private void PaintAddedShip()
        {
 
            foreach (DataGridViewCell cell in controller._battleView.dataGridView1.SelectedCells)
            {
 
               cell.Style.BackColor = Color.Aqua;
            }
        }
    }
}
