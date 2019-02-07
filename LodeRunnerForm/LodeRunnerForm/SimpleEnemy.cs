using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public class SimpleEnemy:Player
    {
        private static char _direction = 'l';
        private static GameCell _prevEnemy = new FreeArea();
        public static new void Update(int i, int j, int x, int y, GameCell[,] gameField, PictureBox[,] pictureField, GameCell p1, GameCell p2, Panel MainPanel)
        {
            pictureField[i, j].Image = new Bitmap(p1.Image);
            pictureField[i + y, j + x].Image = new Bitmap(p2.Image);
        }

        protected delegate void Function(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, Panel MainPanel);
        public SimpleEnemy()
        {
            Type = "SimpleEnemy";
            Display = "&";
            Image = "Data\\Icons\\simpleenemy.png";
        }
        private static void SkipElements(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, Panel MainPanel)
        {
            gameField[i, j] = _prevEnemy;
            Update(i,j,x,y,gameField,pictureField, _prevEnemy,new SimpleEnemy(), MainPanel);
            _prevEnemy = gameField[i + y, j + x];
            gameField[i + y, j + x] = new SimpleEnemy();
        }
        protected static void Die(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            gameField[i, j + delta] = new RopeTrap();
            gameField[i,j] = new FreeArea();
            Update(i,j,delta,0,gameField,pictureField,new FreeArea(), new RopeTrap(), MainPanel);
        }
        protected static void SimpleMovement(GameCell[,] gameField, PictureBox[,] pictureField, string type,Function func, Panel MainPanel)
        {
            int[] enemyCoords = FindPosition(gameField, type);
            int k = enemyCoords[0];
            int f = enemyCoords[1];
            if (k == 0 && f == 0) return;
            if (gameField[k, f - 1] is Ground) _direction = 'r';
            if (gameField[k, f + 1] is Ground) _direction = 'l';
            if (gameField[k, f - 1].Type == "RopeTrap")
            {
                Die(gameField,pictureField, k, f, -1,MainPanel);
                return;
            }
            if (gameField[k, f + 1].Type == "RopeTrap")
            {
                Die(gameField,pictureField, k, f, 1,MainPanel);
                return;
            }
            if (gameField[k, f + 1].Type == "Player")
            {
                func(gameField, pictureField, k, f, 1, 0, MainPanel);
                _prevEnemy = new FreeArea();
                return;
            }
            else if (gameField[k, f + 1].Type == "Player")
            {
                func(gameField, pictureField, k, f, 1, 0, MainPanel);
                _prevEnemy = new FreeArea();
                return;
            }
            if (_direction == 'r')
            {
                func(gameField,pictureField, k,f,1,0,MainPanel);
            }
            else
            {
                func(gameField,pictureField, k,f,-1,0,MainPanel);
            }
        }

        private static readonly Function Func = SkipElements;
        public static void ForThread(GameCell[,] gameField, PictureBox[,] pictureField, Panel MainPanel)
        {
            while (ThreadFlag)
            {
                SimpleMovement(gameField,pictureField, "SimpleEnemy", Func, MainPanel);
                Thread.Sleep(300);
                if (FindPosition(gameField, "Player")[0] != 0) continue;
                Killed = true;
                break;
            }
        }
        public static void SetSimplePrev()
        {
            _prevEnemy = new FreeArea();
        }
    }
}