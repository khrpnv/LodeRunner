using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public class Player:GameCell
    {
        public static bool Killed = false;
        private static int _click, _harmerAmount, _keysAmount, _doorSide, _boost, _counter, _pill ,_activePill, _helmet, _helmetCount;
        protected static int ScrapAmount, RopeTrapAmount;
        protected static bool ThreadFlag = true;
        public static int AmountOfSteps = 0;
        private static GameCell _prev = new FreeArea();
        private static readonly List<int> DestrCoords = new List<int>();
        private Player(int x, int y)
        {
            Type = "Player";
            Display = "X";
        }
        public Player()
        {
            Type = "Player";
            Display = "X";
            Image = "Data\\Icons\\player.png";
        }
        private static void ShowDescription(int i, int j, GameCell[,] gameField)
        {
            MessageBox.Show("LEFT: "+gameField[i,j-1].Definition+"\n"+ "RIGHT: " + gameField[i, j + 1].Definition + "\n" + 
                "DOWN: " + gameField[i + 1, j].Definition + "\n" + "UP: " + gameField[i - 1, j].Definition + "\n" + "Press OK to close the window.");
            return;
        }
        public static void Update(int i, int j, int x, int y,GameCell[,] gameField, PictureBox[,] pictureField, GameCell p1, GameCell p2, Panel MainPanel)
        {
            pictureField[i, j].Image = new Bitmap(gameField[i, j].Image);
            pictureField[i + y, j + x].Image = new Bitmap(gameField[i + y, j + x].Image);
            if (GetClick() != 0 && p1.Type == "Player" || GetClick() != 0 && p2.Type == "Player")
                GameField.IncreaseRecover();
            if (GameField.GetRecover() == 7)
            {
                Destructed.ReviveCell(gameField);
                List<int> coords = GetCoords();
                for (int k = 0; k < coords.Count; k += 2)
                {
                    gameField[coords[k], coords[k + 1]] = new Ground();
                    Update(coords[k], coords[k+1],0,0, gameField, pictureField, new FreeArea(), new Ground(), MainPanel);
                }
            }
        }
        protected static void MoveOnFreeArea(int i, int j, int y, int x, GameCell[,] gameField, GameCell p)
        {
            gameField[i + y, j + x] = p;
            gameField[i, j] = new FreeArea();
        }
        private static void MoveOnLadder(int i, int j, int y, int x, GameCell[,] gameField, Player p)
        {
            gameField[i + y, j + x] = p;
            gameField[i, j] = new Ladder();
        }
        public static void FallDown(int i, int j, GameCell[,] gameField, GameCell p)
        {
            gameField[i + 1, j] = p;
            if (gameField[i + 1, j].Type == "Destructed")
            {
                gameField[i, j] = new Destructed();
            }
            else
            {
                gameField[i, j] = new FreeArea();
            }
        }
        protected static void DieFromEnemy(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            gameField[i, j + delta] = _prev;
            gameField[i,j] = new FreeArea();
            Update(i,j,delta,0, gameField, pictureField, new FreeArea(), _prev, MainPanel);
        }
        private static void LeftRightArrowMovement(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            if (j + delta == 0 || j + delta == gameField.GetLength(1) - 1) return;
            if (_counter > _boost*7) _boost--;
            if (_activePill > _pill*20) _pill--;
            if (_helmetCount > _helmet*4) _helmet--;
            if (_boost > 0)
            {
                if (gameField[i, j + 2*delta] is Ground) return;
                SkipElements(gameField, pictureField, i, j, 2*delta, 0, new Player(), MainPanel);
                pictureField[i, j + 2 * delta].Image = new Bitmap("Data\\Icons\\player.png");
                gameField[i, j + 2 * delta] = new Player();
                _counter++;
                return;
            }
            if (gameField[i, j + delta].Type == "SimpleEnemy" || i == 0)
            {
                DieFromEnemy(gameField,pictureField, i, j, delta, MainPanel);
            }
            if (gameField[i, j + 2*delta].Type == "Door")
            {
                _doorSide = delta;
            }
            else if (gameField[i, j + delta].Type == "Teleport")
            {
                Teleportation(gameField,pictureField, i, j, delta, new Player(), MainPanel);
                return;
            }
            if(_helmet > 0)
            {
                _helmetCount++;
                if (DetectItems(gameField, pictureField, i, j, delta, MainPanel))
                {
                    SkipElements(gameField, pictureField, i, j, delta, 0, new Player(), MainPanel);
                    _prev = new FreeArea();
                }
                else
                {
                    SkipElements(gameField, pictureField, i, j, delta, 0, new Player(), MainPanel);
                }
                Update(i, j, delta, 0, gameField, pictureField, _prev, new Player(), MainPanel);
                return;
            }
            if (gameField[i - 1, j].Type == "Ladder" && gameField[i, j + delta].Type != "Ground" || gameField[i + 1, j].Type == "Ladder" && gameField[i, j + delta].Type != "Ground")
            {
                MoveOnLadder(i, j, 0, delta, gameField, new Player());
                Update(i,j,delta,0,gameField, pictureField, new Player(), new Ladder(), MainPanel);
            }
            else if (gameField[i, j+delta].Type != "Ground" && gameField[i, j+delta].Type != "StrongGround" && gameField[i, j+delta].Type != "Door" && gameField[i, j+delta].Type != "Unbreakable" && j+delta>0)
            {
                if (DetectItems(gameField,pictureField, i, j, delta, MainPanel))
                {
                    SkipElements(gameField,pictureField,i,j,delta,0,new Player(), MainPanel);
                    _prev = new FreeArea();
                }
                else
                {
                    SkipElements(gameField,pictureField,i,j,delta,0,new Player(), MainPanel);
                }
                Update(i, j, delta,0, gameField, pictureField, _prev, new Player(), MainPanel);
            }
        }
        protected static void UpArrowMovement(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, Player p, Panel MainPanel)
        {
            if ((gameField[i + 1, j].Type != "Ladder" || gameField[i - 1, j].Type == "FreeArea" || gameField[i - 1, j] is Ground) && !(gameField[i + 1, j] is Ground) || gameField[i - 1, j].Type != "Ladder") return;
            MoveOnLadder(i, j, -1, 0, gameField, p);
            Update(i, j, 0, -1, gameField, pictureField, p, new Ladder(), MainPanel);
        }
        protected static void DownArrowMovement(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, Player p, Panel MainPanel)
        {
            if (gameField[i + 1, j].Type != "Ladder") return;
            MoveOnLadder(i, j, 1, 0, gameField, p);
            Update(i,j,0,1,gameField,pictureField,p, new Ladder(),MainPanel);
        }
        protected static void ExitGame(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Player p, Panel MainPanel)
        {
            ThreadFlag = false;
            var rnd = new Random();
            int index = rnd.Next(1, 4);
            MoveOnFreeArea(i, j, 0, delta, gameField, p);
            Update(i,j,delta,0,gameField,pictureField, new FreeArea(), p, MainPanel);
            if (index == 1)
            {
                WindowsFormsApp1.Form1 mini = new WindowsFormsApp1.Form1();
                mini.Show();
            }
            else if (index == 2)
            {
                MatchingGame.Form1 miniGame = new MatchingGame.Form1();
                miniGame.Show();
            }
            else if(index == 3)
            {
                WindowsFormsApp4.Form1 codes = new WindowsFormsApp4.Form1();
                codes.Show();
            }
        }
        private static void GeneralDestroy(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int deltay, int deltax, Panel MainPanel)
        {
            gameField[i + deltay, j + deltax] = new Destructed();
            DestrCoords.Add(i + deltay);
            DestrCoords.Add(j + deltax);
            _click++;
            Update(i,j,deltax,deltay,gameField,pictureField,new Destructed(), new Player(), MainPanel);
        }        
        private static void DestroyCells(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            if (gameField[i + 1, j + delta].Type != "Ground" || gameField[i, j + delta].Type != "FreeArea" ||
                gameField[i, j + delta].Type == "Ground" || i + 1 == gameField.GetLength(0) - 1 || gameField[i,j+delta].Type == "SimpleEnemy") return;
            GeneralDestroy(gameField,pictureField,i,j,1,delta, MainPanel);
            
        }
        private static void DestroyStrong(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, Panel MainPanel)
        {
            if (gameField[i + 1, j].Type != "StrongGround" || gameField[i, j].Type == "Ladder" ||
                i + 1 == gameField.GetLength(0) - 1 || _harmerAmount <= 0) return;
            _harmerAmount--;
            GeneralDestroy(gameField,pictureField, i, j,1, 0, MainPanel);
        }
        protected static void DestroyWall(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            if ((gameField[i, j + delta].Type != "Ground" || ScrapAmount == 0) &&
                (gameField[i, j + delta].Type != "StrongGround" || ScrapAmount == 0)) return;
            GeneralDestroy(gameField,pictureField, i, j, 0, delta, MainPanel);
            ScrapAmount--;
        }
        private static void RopeOrScrap(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            if (j + delta == 0 || j + delta == gameField.GetLength(1) - 1) return;
            if(RopeTrapAmount > 0)
                PutTrap(gameField,pictureField,i,j, delta, MainPanel);
            else 
                DestroyWall(gameField,pictureField, i, j ,delta, MainPanel);
        }
        protected static void PutTrap(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            if (gameField[i, j + delta].Type != "FreeArea" || RopeTrapAmount <= 0) return;
            gameField[i,j+delta] = new RopeTrap();
            RopeTrapAmount--;
            Update(i,j,delta,0,gameField,pictureField, new Player(i,j),new RopeTrap(), MainPanel);
        }
        protected static void SkipElements(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, GameCell p, Panel MainPanel)
        {
            gameField[i, j] = _prev;
            Update(i,j,x,y,gameField,pictureField, p, _prev, MainPanel);
            _prev = gameField[i + y, j + x];
            gameField[i + y, j + x] = p;
        }
        protected static void RandomPrize(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, GameCell p, Panel MainPanel)
        {
            MoveOnFreeArea(i, j,y,x, gameField, p);
            Update(i, j, x, y, gameField,pictureField, new FreeArea(), p, MainPanel);
            var rnd = new Random();
            int index = rnd.Next(1,5);
            switch (index)
            {
                case 1:
                    for (int k = 0; k < 5; k++) Gold.IncreaseCount(20);
                    break;
                case 2:
                    Gold.SetGoldCount();
                    break;
                case 3:
                    MessageBox.Show("YOU LOOSE!");
                    break;
            }
        }
        private static bool DetectItems(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, Panel MainPanel)
        {
            switch (gameField[i, j + delta].Type)
            {
                case "Harmer" when RopeTrapAmount + ScrapAmount != 0:
                case "UsedRopeTrap" when _harmerAmount + ScrapAmount != 0:
                case "Scrap" when _harmerAmount + RopeTrapAmount != 0:
                case "RopeTrap":
                case "SimpleEnemy":
                case "BlindEnemy":
                case "SecondPlayer":
                case "Ground" when _helmet != 0:
                case "Gold" when FindPosition(gameField, "SecondPlayer")[0] != 0:
                    return false;
                case "ExitDoor":
                    ExitGame(gameField,pictureField, i, j, delta, new Player(), MainPanel);
                    break;
                case "Gold":
                    Gold.IncreaseCount(10);
                    break;
                case "Coin":
                    Gold.IncreaseCount(40);
                    break;
                case "Pill":
                    _pill++;
                    break;
                case "Helmet":
                    _helmet++;
                    break;
                case "Harmer" when RopeTrapAmount == 0 && ScrapAmount == 0:
                    _harmerAmount++;
                    break;
                case "Key":
                    _keysAmount++;
                    break;
                case "Boost":
                    _boost++;
                    _counter = 0;
                    break;
                case "RandomPrize":
                    RandomPrize(gameField, pictureField, i, j, delta, 0, new Player(), MainPanel);
                    break;
                case "UsedRopeTrap" when _harmerAmount == 0 && ScrapAmount == 0:
                    RopeTrapAmount++;
                    break;
                case "Scrap" when _harmerAmount == 0 && RopeTrapAmount == 0:
                    ScrapAmount++;
                    break;
            }
            AmountOfSteps++;
            return true;
        }
        private static void OpenDoor(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, Panel MainPanel)
        {
            if (gameField[i, j + _doorSide].Type != "Door" || _keysAmount <= 0) return;
            gameField[i,j+_doorSide] = new FreeArea();
            _keysAmount--;
            Update(i,j,_doorSide,0,gameField,pictureField,new FreeArea(), new Player(), MainPanel);
        }
        protected static int ArrayIndexInList(List<int[]> list, int i, int j)
        {
            for (int k = 0; k < list.Count; k++)
            {
                if (list[k][0] == i && list[k][1] == j)
                    return k;
            }
            return -1;
        }
        public static void Falling(GameCell[,] field, PictureBox[,] pictureField, int i, int j, Player p, Panel MainPanel)
        {
            if (field[i + 1, j].Type == "Gold")
            {
                Gold.IncreaseCount(10);
            }
            if (field[i + 1, j].Type == "Coin")
            {
                Gold.IncreaseCount(40);
            }
            FallDown(i, j, field, p);
            Update(i, j, 0, 1, field, pictureField, p, new FreeArea(), MainPanel);
        }
        private static void Teleportation(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int delta, GameCell p, Panel MainPanel)
        {
            if (GameField.TeleportCoords.Count <= 1)
            {
                SkipElements(gameField,pictureField,i,j,delta,0, p, MainPanel);
                return;
            }
            gameField[i,j + delta] = new Teleport();
            gameField[i,j] = new FreeArea();
            int[] curCoords = {i, j + delta};
            Update(i,j,delta,0,gameField,pictureField, new Teleport(), new FreeArea(), MainPanel);
            var nextCoords = ArrayIndexInList(GameField.TeleportCoords, curCoords[0], curCoords[1]) == GameField.TeleportCoords.Count-1 ? GameField.TeleportCoords[0] : GameField.TeleportCoords[ArrayIndexInList(GameField.TeleportCoords, curCoords[0], curCoords[1]) + 1];
            gameField[nextCoords[0], nextCoords[1]] = p;
            gameField[curCoords[0], curCoords[1]] = new Teleport();
            Update(nextCoords[0],nextCoords[1],0,0,gameField,pictureField,new FreeArea(), p, MainPanel);
            _prev = new Teleport();
        }
        public static void MoveHero(KeyEventArgs e, GameCell[,] gameField, PictureBox[,] pictureField, Panel MainPanel)
        {
            int[] coords = FindPosition(gameField, "Player");
            int i = coords[0];
            int j = coords[1];
            if (i == 0 || j == 0) { MessageBox.Show("YOU LOOSE!"); return; }
            if (_pill > 0) _activePill++;
            switch (e.KeyValue)
            {
                case 37:
                    if(_pill > 0) LeftRightArrowMovement(gameField, pictureField, i, j, 1, MainPanel);
                    else LeftRightArrowMovement(gameField, pictureField, i, j, -1, MainPanel);
                    break;
                case 39:
                    if (_pill > 0) LeftRightArrowMovement(gameField, pictureField, i, j, -1, MainPanel);
                    else LeftRightArrowMovement(gameField, pictureField, i, j, 1, MainPanel);
                    break;
                case 32:
                    OpenDoor(gameField, pictureField, i, j, MainPanel);
                    break;
                case 40:
                    if(_pill > 0) UpArrowMovement(gameField, pictureField, i, j, new Player(), MainPanel);
                    else DownArrowMovement(gameField, pictureField, i, j, new Player(), MainPanel);
                    break;
                case 38:
                    if(_pill > 0) DownArrowMovement(gameField, pictureField, i, j, new Player(), MainPanel);
                    else UpArrowMovement(gameField, pictureField, i, j, new Player(), MainPanel);
                    break;
                case 81:
                    DestroyCells(gameField, pictureField, i, j, -1, MainPanel);
                    break;
                case 69:
                    DestroyCells(gameField,pictureField, i, j, 1, MainPanel);
                    break;
                case 9:
                    DestroyStrong(gameField,pictureField, i, j, MainPanel);
                    break;
                case 65:
                    RopeOrScrap(gameField,pictureField, i, j, -1,MainPanel);
                    break;
                case 68:
                    RopeOrScrap(gameField,pictureField, i, j, 1, MainPanel);
                    break;
                case 8:
                    ShowDescription(i, j, gameField);
                    break;
            }            
        }
        private static int GetClick()
        {
            return _click;
        }
        public static void SetClick()
        {
            _click = 0;
        }
        public static List<int> GetCoords()
        {
            return DestrCoords;
        }
        public static void Bag(GameCell[,] gameField)
        {
            Console.SetCursorPosition(0,gameField.GetLength(0)+2);
            Console.WriteLine("\nYOUR BACKPACK:");
            Console.WriteLine("1. harmers: " + _harmerAmount);
            Console.WriteLine("2. traps: " + RopeTrapAmount);
            Console.WriteLine("3. scraps: " + ScrapAmount);
            Console.WriteLine("4. keys: " + _keysAmount);
            Console.SetCursorPosition(100,100);        
        }
        public static void SetPrivateFields()
        {
            _prev = new FreeArea();
            _harmerAmount = 0;
            RopeTrapAmount = 0;
            _keysAmount = 0;
            _doorSide = 0;
            ScrapAmount = 0;
            _boost = 0;
            _click = 0;
            _boost = 0;
            _counter = 0;
            _pill = 0;
            _activePill = 0;
            _helmet = 0;
            _helmetCount = 0;
            Killed = false;
        }
        public static void SetThreadFlag(bool type)
        {
            ThreadFlag = type;
        }
    }
}