using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace LodeRunnerForm
{
    public class LevelsCreator : Player
    {
        public static PictureBox[,] picField;
        public void RedactorProcess(Form2 GameForm, int num1, int num2, string name)
        {
            Panel NewPanel = new Panel()
            {
                Size = new Size(1100, 650),
                Location = new Point(250, 20),
                BackColor = Color.Green
            };
            GameField GameFieldCreator = new GameField();
            GameForm.Controls.Add(NewPanel);
            GameFieldCreator.curField = GenerateStartField(NewPanel, num1, num2);
            GameForm.KeyUp += new KeyEventHandler(Key);
            void Key(object sender, KeyEventArgs e)
            {
                if (e.KeyValue == 27) GameForm.Close();
                MoveCursor(e, GameFieldCreator.curField, picField, NewPanel, name);
            }
        }
        private static GameCell _prev = new FreeArea();
        private static bool _player = true, _exitDoor = true;
        public static void UpdateCreator(int i, int j, int x, int y, GameCell[,] gameField, PictureBox[,] pictureField, GameCell p1, GameCell p2, Panel MainPanel)
        {
            pictureField[i, j].Image = new Bitmap(p1.Image);
            pictureField[i + y, j + x].Image = new Bitmap(p2.Image);
        }
        public  static GameCell[,] GenerateStartField(Panel MainPanel, int width, int height)
        {
            GameCell[,]gameField = new GameCell[height,width];
            picField = new PictureBox[height,width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 || i ==height - 1 || j == 0 || j == width-1)
                        gameField[i,j] = new Ground();
                    else if (i == 1 && j == 1)
                    {
                        gameField[i,j] = new Cursor();
                    }
                    else
                    {
                        gameField[i, j] = new FreeArea();
                    }
                    picField[i, j] = new PictureBox()
                    {
                        Size = new Size(30, 30),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Location = new Point(j * 30, i * 30),
                        Image = new Bitmap(gameField[i, j].Image)
                    };
                    MainPanel.Controls.Add(picField[i, j]);
                }
            }
            return gameField;
        }
        protected static void SkipElementsCreator(GameCell[,] gameField, PictureBox[,] pictureField, int i, int j, int x, int y, GameCell p, Panel MainPanel)
        {
            gameField[i, j] = _prev;
            UpdateCreator(i, j, x, y, gameField, pictureField, _prev, new Cursor(), MainPanel);
            _prev = gameField[i + y, j + x];
            gameField[i + y, j + x] = new Cursor();
        }
        private static void CreateElement(int i, int j, int y, int x, GameCell[,] gameField, GameCell elem, Panel MainPanel, PictureBox[,] pictureField)
        {
            SkipElementsCreator(gameField, pictureField, i, j, x, y, new Cursor(), MainPanel);
            gameField[i, j] = elem;
            UpdateCreator(i, j, x, y, gameField, pictureField, elem, new Cursor(), MainPanel);
        }
        private static void DeleteElement(int i, int j, int deltay, int deltax, GameCell[,] gameField, Panel MainPanel, PictureBox[,] pictureField)
        {
            if (i + deltay == gameField.GetLength(0)-1 || j + deltax == 0) return;
            gameField[i + deltay, j + deltax] = new FreeArea();
            gameField[i, j] = new Cursor();
            UpdateCreator(i,j,deltax,deltay,gameField,pictureField,new Cursor(), new FreeArea(), MainPanel);
        }
        private static void ArrowKeys(GameCell[,] gameField, int i, int j, int y, int x, int param1, int param2, Panel MainPanel, PictureBox[,] pictureField)
        {
            if (param1 > param2)
            {
                SkipElementsCreator(gameField,pictureField,i,j,x,y,new Cursor(), MainPanel);
            }
        }
        private static void AllOtherKeys(GameCell[,] gameField, int i, int j, GameCell elem, Panel MainPanel, PictureBox[,] pictureField)
        {
            if (j == gameField.GetLength(1) - 2 && i == gameField.GetLength(0) - 2)
            {
                CreateElement(i, j, -1, 0, gameField, elem, MainPanel, pictureField);
            }
            else if (j == gameField.GetLength(1) - 2)
            {
                CreateElement(i, j, 1, 0, gameField, elem, MainPanel, pictureField);
            }
            else
            {
                CreateElement(i, j, 0, 1, gameField, elem, MainPanel, pictureField);
            }
        }
        public static void MoveCursor(KeyEventArgs e, GameCell[,] gameField, PictureBox[,] pictureField, Panel MainPanel, string name)
        {
            int[] coords = FindPosition(gameField, "Cursor");
            int i = coords[0];
            int j = coords[1];
            switch (e.KeyValue)
            {
                case 37: 
                    ArrowKeys(gameField, i, j, 0, -1, j, 1, MainPanel, pictureField);
                    break;
                case 39: 
                    ArrowKeys(gameField, i, j, 0, 1,gameField.GetLength(1)-2,j, MainPanel, pictureField);
                    break;
                case 38: 
                    ArrowKeys(gameField, i, j, -1, 0,i,1, MainPanel, pictureField);
                    break;
                case 40: 
                    ArrowKeys(gameField, i, j, 1, 0,gameField.GetLength(0)-2,i, MainPanel, pictureField);
                    break;
                case 51: 
                    AllOtherKeys(gameField, i,j,new Ground(), MainPanel, pictureField);
                    break;
                case 50: 
                    AllOtherKeys(gameField, i,j,new Gold(), MainPanel, pictureField);
                    break;
                case 111: 
                    AllOtherKeys(gameField, i,j,new Ladder(), MainPanel, pictureField);
                    break;
                case 53:
                    AllOtherKeys(gameField, i,j,new StrongGround(), MainPanel, pictureField);
                    break;
                case 49:
                    AllOtherKeys(gameField, i,j,new Harmer(), MainPanel, pictureField);
                    break;
                case 9:
                    AllOtherKeys(gameField, i,j,new Bar(), MainPanel, pictureField);
                    break;
                case 71 when _player == true:
                    AllOtherKeys(gameField, i,j,new Player(), MainPanel, pictureField);
                    _player = false;
                    break;
                case 69:
                    AllOtherKeys(gameField, i,j,new SimpleEnemy(), MainPanel, pictureField);
                    break;
                case 84:
                    AllOtherKeys(gameField, i,j,new UsedRopeTrap(), MainPanel, pictureField);
                    break;
                case 68:
                    AllOtherKeys(gameField, i,j,new Door(), MainPanel, pictureField);
                    break;
                case 75:
                    AllOtherKeys(gameField, i,j,new Key(), MainPanel, pictureField);
                    break;
                case 79:
                    AllOtherKeys(gameField, i,j,new ExitDoor(), MainPanel, pictureField);
                    _exitDoor = false;
                    break;
                case 76:
                    AllOtherKeys(gameField, i,j,new Scrap(), MainPanel, pictureField);
                    break;
                case 70:
                    AllOtherKeys(gameField, i, j, new Teleport(), MainPanel, pictureField);
                    break;
                case 85:
                    AllOtherKeys(gameField, i, j, new Unbreakable(), MainPanel, pictureField);
                    break;
                case 80:
                    AllOtherKeys(gameField, i, j, new BlindEnemy(), MainPanel, pictureField);
                    break;
                case 89:
                    AllOtherKeys(gameField, i, j, new SecondPlayer(), MainPanel, pictureField);
                    break;
                case 73:
                    AllOtherKeys(gameField, i, j, new RandomPrize(), MainPanel, pictureField);
                    break;
                case 66:
                    AllOtherKeys(gameField, i, j, new Boost(), MainPanel, pictureField);
                    break;
                case 81:
                    AllOtherKeys(gameField, i, j, new Pill(), MainPanel, pictureField);
                    break;
                case 72:
                    AllOtherKeys(gameField, i, j, new Helmet(), MainPanel, pictureField);
                    break;
                case 67:
                    AllOtherKeys(gameField, i, j, new Coin(), MainPanel, pictureField);
                    break;
                case 8:
                    DeleteElement(i,j,0,-1,gameField, MainPanel, pictureField);
                    break;
                case 90:
                    DeleteElement(i,j,1,0,gameField, MainPanel, pictureField);
                    break;
                case 27 when !_player && !_exitDoor:
                    CreateLevel(gameField, name);
                    break;
                case 27 when _player && _exitDoor:
                    MessageBox.Show("The level wasn`t created, because you have forgotten to draw player and/or exit door.");
                    break;
            }
        }
        private static string[,] ConvertField(GameCell[,] gamefield)
        {
            string[,] stringField = new string[gamefield.GetLength(0),gamefield.GetLength(1)];
            for (int i = 0; i < stringField.GetLength(0); i++)
            {
                for (int j = 0; j < stringField.GetLength(1); j++)
                {
                    stringField[i, j] = gamefield[i, j].Display;
                }
            }
            return stringField;
        }
        public static void CreateLevel(GameCell[,] gameField, string name)
        {
            Matrix matrix = new Matrix{
                H = gameField.GetLength(0),
                W = gameField.GetLength(1),
                Field = ConvertField(gameField)
            };
            name += ".json";
            string data = JsonConvert.SerializeObject(matrix);
            if (!File.Exists("Data\\Levels\\"+name))
            {
                StreamWriter output = new StreamWriter("Data\\Levels\\"+name);
                output.WriteLine(data);
                output.Close();
                Application.Restart();
                Environment.Exit(0);
            }
        }
    }
}