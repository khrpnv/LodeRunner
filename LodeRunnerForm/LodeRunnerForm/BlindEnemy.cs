using System;
using System.Threading;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public class BlindEnemy:SimpleEnemy
    {
        private static GameCell _prevBlindEnemy = new FreeArea();
        public BlindEnemy()
        {
            Type = "BlindEnemy";
            Display = "♖";
            Image = "Data\\Icons\\blindenemy.png";
        }
        private static void SkipElements(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, Panel MainPanel)
        {
            gameField[i, j] = _prevBlindEnemy;
            Update(i,j,x,y,gameField,pictureField, _prevBlindEnemy, new BlindEnemy(), MainPanel);
            _prevBlindEnemy = gameField[i + y, j + x];
            gameField[i + y, j + x] = new BlindEnemy();
        }
        private static void UsePlayersItems(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            switch (gameField[i, j + delta].Type)
            {
                case "Ground" when ScrapAmount > 0:
                    DestroyWall(gameField,pictureField, i, j, delta,MainPanel);
                    break;
                case "StrongGround" when ScrapAmount > 0:
                    DestroyWall(gameField,pictureField, i, j, delta, MainPanel);
                    break;
                default:
                    if(RopeTrapAmount > 0)
                        PutTrap(gameField,pictureField, i, j, -delta, MainPanel);
                    break;
            }
        }
        private static readonly Function Func = SkipElements;
        public static void SimpleMovement(GameCell[,] gameField, PictureBox[,] pictureField, string type, Panel MainPanel)
        {
            while (ThreadFlag)
            {
                int[] enemyCoords = FindPosition(gameField, "BlindEnemy");
                int[] playerCoords = FindPosition(gameField, "Player");
                if (gameField[enemyCoords[0], enemyCoords[1] + 1].Type == "Ground" &&
                    gameField[enemyCoords[0], enemyCoords[1] - 1].Type == "Ground")
                {
                    return;
                }
                if (gameField[enemyCoords[0], enemyCoords[1] - 1].Type == "RopeTrap")
                {
                    Die(gameField, pictureField, enemyCoords[0], enemyCoords[1], -1, MainPanel);
                    return;
                }
                if (gameField[enemyCoords[0], enemyCoords[1] + 1].Type == "RopeTrap")
                {
                    Die(gameField, pictureField, enemyCoords[0], enemyCoords[1], 1, MainPanel);
                    return;
                }
                if (gameField[enemyCoords[0] + 1, enemyCoords[1]].Type == "Destructed")
                {
                    SkipElements(gameField, pictureField, enemyCoords[0], enemyCoords[1], 0, 1, MainPanel);
                    _prevBlindEnemy = new FreeArea();
                    continue;
                }
                if (GameField.GetRecover() < 6 && gameField[enemyCoords[0], enemyCoords[1]+1].Type == "Ground" && gameField[enemyCoords[0], enemyCoords[1]-1].Type == "Ground")
                {
                    Thread.Sleep(300);
                    if(enemyCoords[1] > playerCoords[1])
                        SkipElements(gameField, pictureField, enemyCoords[0], enemyCoords[1],1,-1, MainPanel);
                    else
                        SkipElements(gameField, pictureField, enemyCoords[0], enemyCoords[1],-1,-1, MainPanel);
                    continue;
                }
                if (enemyCoords[0] != playerCoords[0])
                {
                    SimpleMovement(gameField, pictureField, "BlindEnemy", Func, MainPanel);
                    Thread.Sleep(400);
                }
                else if (enemyCoords[0] == playerCoords[0] && enemyCoords[1] < playerCoords[1])
                {
                    UsePlayersItems(gameField, pictureField, enemyCoords[0], enemyCoords[1], 1, MainPanel);
                    SkipElements(gameField, pictureField, enemyCoords[0], enemyCoords[1], 1, 0, MainPanel);
                    Thread.Sleep(200);
                }
                else if (enemyCoords[0] == playerCoords[0] && enemyCoords[1] > playerCoords[1])
                {
                    UsePlayersItems(gameField, pictureField, enemyCoords[0], enemyCoords[1], 1, MainPanel);
                    SkipElements(gameField, pictureField, enemyCoords[0], enemyCoords[1], -1, 0, MainPanel);
                    Thread.Sleep(200);
                }
                if (FindPosition(gameField, "Player")[0] != 0) continue;
                Killed = true;
                break;
            }
        }
        public static void SetPrevBlind()
        {
            _prevBlindEnemy = new FreeArea();
        }
    }
}