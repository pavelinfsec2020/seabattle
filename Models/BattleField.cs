using System.Collections.Generic;
using System.Drawing;
using System.Media;

namespace SeaBattle.Models
{
    internal class BattleField
    {
       const int ROW_COUNT = 10;
        public Dictionary<int[,],Ship> _shipsCoordinates;
        public int[,] _field;
        private int countOfDiedShips = 0;
        public bool LoseStatus
        {
            get {
                if (countOfDiedShips == 10) return true;
                else return false;
            }
        }

        public BattleField()
        {
            _field = new int[ROW_COUNT,ROW_COUNT];
            _shipsCoordinates = new Dictionary<int[,],Ship>();
        }
        public void SetShip(Ship ship,int[,] coordinates)
        {
            _shipsCoordinates.Add(coordinates,ship);
            for (int i = 0; i < coordinates.GetLength(0); i++)
            {
                _field[coordinates[i, 0], coordinates[i, 1]] = 1;
            }
        }
       
        private void PaintDiedShip(int[,] coordinates) 
        { 
           for(int i=0;i<coordinates.GetLength(0);i++)
                Controllers.GameController._battleView.dataGridView1.Rows[coordinates[i,0]].Cells[coordinates[i,1]].Style.BackColor = Color.Red;
        }
        public int[,] GetBangFromRival(int[] coordinate)
        {
            int[,] answerCoordsToRival = new int[1, 2];
            if (_field[coordinate[0], coordinate[1]] == 1)
            {
                Controllers.GameController._battleView.dataGridView1.Rows[coordinate[0]].Cells[coordinate[1]].Style.BackColor = Color.Yellow;
                foreach (KeyValuePair<int[,], Ship> coord in _shipsCoordinates)
                {
                    for (int i = 0; i < coord.Key.GetLength(0); i++)
                    {
                        if (coord.Key[i, 0] == coordinate[0] && coord.Key[i, 1] == coordinate[1])
                        {
                            coord.Value.TakeDamage(i);
                            if (coord.Value._isDied)
                            {
                                PaintDiedShip(coord.Key);
                                countOfDiedShips++;
                                if (coord.Value.Size == 1)
                                {
                                    coord.Key[0, 0] += 10;
                                }
                                new SoundPlayer(Properties.Resources.killed).Play();
                                return coord.Key;
                            }
                            new SoundPlayer(Properties.Resources.goodShot).Play();
                            answerCoordsToRival[0, 0] = coordinate[0];
                            answerCoordsToRival[0, 1] = coordinate[1];

                            return answerCoordsToRival;
                        }
                    }
                   
                }
               
            }
            else 
            {
                answerCoordsToRival[0, 0] = -1;
                answerCoordsToRival[0, 1] = -1;
               
            }
            return answerCoordsToRival;
        }
    }
}
