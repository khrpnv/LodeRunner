using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public class Coin : Player
    {
        private static int _changeDir = 0;
        public Coin()
        {
            Type = "Coin";
            Display = "^";
            Image = "Data\\Icons\\coin.png";
        }
        private static int _direction;
        private static GameCell _prevCoin = new FreeArea();
        public static new void Update(int i, int j, int x, int y, GameCell[,] gameField, PictureBox[,] pictureField, GameCell p1, GameCell p2, Panel MainPanel)
        {
            pictureField[i, j].Image = new Bitmap(p1.Image);
            pictureField[i + y, j + x].Image = new Bitmap(p2.Image);
        }
        private static void SkipElements(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, Panel MainPanel)
        {
            _changeDir++;
            gameField[i, j] = _prevCoin;
            Update(i, j, x, y, gameField, pictureField, _prevCoin, new Coin(), MainPanel);
            _prevCoin = gameField[i + y, j + x];
            gameField[i + y, j + x] = new Coin();
        }
        protected static void Die(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            gameField[i, j] = new FreeArea();
            Update(i, j, delta, 0, gameField, pictureField, new FreeArea(), new RopeTrap(), MainPanel);
        }
        private static void SimpleMovement(GameCell[,] gameField, PictureBox[,] pictureField, Panel MainPanel)
        {
            if (_changeDir >= 6)
                _changeDir = 0;
            int[] coinCoords = FindPosition(gameField, "Coin");
            int k = coinCoords[0];
            int f = coinCoords[1];
            if (k == 0 && f == 0) return;
            if (_prevCoin is Player)
            {
                gameField[k, f] = new Player();
                pictureField[k, f].Image = new Bitmap("Data\\Icons\\player.png");
                ThreadFlag = false;
                return;
            }
            if (_changeDir == 0)
            {
                Random rnd = new Random();
                if (gameField[k + 1, f] == gameField[k - 1, f])
                {
                    _direction = rnd.Next(0, 2);
                }
                else
                {
                    _direction = rnd.Next(0, 4);
                }
            }
            if (_direction == 0 && gameField[k + 1, f] is Ground)
            {
                if (gameField[k, f + 1] is Ground) { _direction = 1; return; }
                SkipElements(gameField, pictureField, k, f, 1, 0, MainPanel);
            }
            else if (_direction == 1 && gameField[k + 1, f] is Ground)
            {
                if (gameField[k, f - 1] is Ground) { _direction = 0; return; }
                SkipElements(gameField, pictureField, k, f, -1, 0, MainPanel);
            }
            else if (_direction == 2)
            {
                if (gameField[k - 1, f] is Ground) { _direction = 3; return; }
                SkipElements(gameField, pictureField, k, f, 0, -1, MainPanel);
            }
            else if (_direction == 3)
            {
                if (gameField[k + 1, f] is Ground)
                {
                    _changeDir = 0;
                    return;
                }
                SkipElements(gameField, pictureField, k, f, 0, 1, MainPanel);
            }
        }
        public static void ForThread(GameCell[,] gameField, PictureBox[,] pictureField, Panel MainPanel)
        {
            while (ThreadFlag)
            {
                SimpleMovement(gameField, pictureField, MainPanel);
                Thread.Sleep(200);
            }
        }
        public static void SetPrevCoin()
        {
            _prevCoin = new FreeArea();
        }
    }
}
