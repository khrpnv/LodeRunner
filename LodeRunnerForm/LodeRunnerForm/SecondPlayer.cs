using System;
using System.Drawing;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public class SecondPlayer:Player
    {
        private static GameCell _prevSecond = new FreeArea();
        public SecondPlayer()
        {
            Type = "SecondPlayer";
            Display = "Σ";
            Image = "Data\\Icons\\secondplayer.png";
        }
        private static void SkipSecondElements(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, Panel MainPanel)
        {
            gameField[i, j] = _prevSecond;
            SecondUpdate(i, j, x, y, gameField, pictureField, _prevSecond, new SecondPlayer(), MainPanel);
            _prevSecond = gameField[i + y, j + x];
            gameField[i + y, j + x] = new SecondPlayer();
        }
        private static void SecondUpdate(int i, int j, int x, int y, GameCell[,] gameField, PictureBox[,] pictureField, GameCell p1, GameCell p2, Panel MainPanel)
        {
            pictureField[i, j].Image = new Bitmap(p1.Image);
            pictureField[i + y, j + x].Image = new Bitmap(p2.Image);
        }
        private static void Teleportation(GameCell[,] gameField,PictureBox[,] pictureField, int i, int j, int delta, GameCell p, Panel MainPanel)
        {
            if (GameField.TeleportCoords.Count <= 1)
            {
                SkipSecondElements(gameField,pictureField,i,j,delta,0,MainPanel);
                return;
            }
            gameField[i,j + delta] = new Teleport();
            gameField[i,j] = new FreeArea();
            int[] curCoords = {i, j + delta};
            SecondUpdate(i,j,delta,0,gameField, pictureField, new FreeArea(), new Teleport(), MainPanel);
            var nextCoords = ArrayIndexInList(GameField.TeleportCoords, curCoords[0], curCoords[1]) == GameField.TeleportCoords.Count-1 ? GameField.TeleportCoords[0] : GameField.TeleportCoords[ArrayIndexInList(GameField.TeleportCoords, curCoords[0], curCoords[1]) + 1];
            gameField[nextCoords[0], nextCoords[1]] = p;
            gameField[curCoords[0], curCoords[1]] = new Teleport();
            SecondUpdate(nextCoords[0],nextCoords[1],0,0,gameField,pictureField, new FreeArea(), p, MainPanel);
            _prevSecond = new Teleport();
        }
        private static void DifferentElements(GameCell[,] gameField,PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            switch (gameField[i, j + delta].Type)
            {
                case "Gold":
                    Gold.IncreaseCount(10);
                    SkipSecondElements(gameField,pictureField, i, j, delta, 0, MainPanel);
                    _prevSecond = new FreeArea();
                    SecondUpdate(i, j, delta, 0, gameField, pictureField, _prevSecond, new SecondPlayer(), MainPanel);
                    break;
                case "SimpleEnemy":
                case "BlindEnemy":
                    DieFromEnemy(gameField, pictureField, i, j, delta, MainPanel);
                    break;
                case "ExitDoor":
                    ExitGame(gameField, pictureField, i, j, delta, new SecondPlayer(), MainPanel);
                    break;
                case "Teleport":
                    Teleportation(gameField, pictureField, i, j, delta, new SecondPlayer(), MainPanel);
                    break;
                case "RandomPrize":
                    RandomPrize(gameField, pictureField, i, j, delta, 0, new SecondPlayer(), MainPanel);
                    break;
                default:
                    SkipSecondElements(gameField,pictureField, i, j, delta, 0,MainPanel);
                    break;
            }
        }
        public static void MoveSecondHero(KeyEventArgs e, GameCell[,] gameField, PictureBox[,] pictureField, Panel MainPanel)
        {
            int[] coords = FindPosition(gameField, "SecondPlayer");
            int k = coords[0];
            int f = coords[1];
            if (k == 0 && f == 0) { MessageBox.Show("YOU LOOSE!"); return; }
            else
            {
                switch (e.KeyValue)
                {
                    case 90: 
                        DifferentElements(gameField, pictureField, k,f,-1, MainPanel);
                        break;
                    case 67: 
                        DifferentElements(gameField, pictureField,k,f,1, MainPanel);
                        break;
                    case 83:
                        UpArrowMovement(gameField,pictureField, k, f, new SecondPlayer(), MainPanel);                       
                        break;
                    case 88:
                        DownArrowMovement(gameField,pictureField, k,f, new SecondPlayer(), MainPanel);
                        break;
                }
            }
        }
        public static void SetSecondPrev()
        {
            _prevSecond = new FreeArea();
        }
    }
}