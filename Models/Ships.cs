namespace SeaBattle.Models
{
    internal class Ship
    {
        private readonly int _size;
        public int[] _decksHealth;
        public bool _isDied = false;
        public int Size
        {
            get { return _size; }
        }
        public Ship(int size)
        {
            _size = size;
            _decksHealth = new int[_size];
            inicializeShip(); ;
        }
        public void inicializeShip()
        {
            for (int i = 0; i < _size; i++) _decksHealth[i] = 1;
        }
        public void TakeDamage(int deckNumber)
        {
            _decksHealth[deckNumber] = 2;
            for (int i = 0; i < _size; i++)
            {
                if (_decksHealth[i] == 1)
                    return;
            }
            _isDied = true;
        }
        
    }
}
