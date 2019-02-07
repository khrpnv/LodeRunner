using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using LodeRunnerForm;
using System.Threading;
using System.Media;

namespace LodeRunnerForm
{
    class Game
    {
        public static bool AskName = true;
        private static bool _fallingDown = false;
        public static void PlayMusic()
        {
            Random rnd = new Random();
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = "Data\\Sounds\\sound"+rnd.Next(1, 9)+".wav";
            player.PlayLooping();
        }
        public void GameProcess(LodeRunner GameForm, string name)
        {
            Panel MainPanel = new Panel()
            {
                Size = new Size(1100, 650),
                Location = new Point(20, 70),
                BackColor = Color.Blue
            };
            GameField GameField1 = new GameField();
            GameForm.Controls.Add(MainPanel);
            Player.AmountOfSteps = 0;
            Gold.SetGoldCount();
            GameField.TeleportCoords.Clear();
            Player.SetPrivateFields();
            SecondPlayer.SetSecondPrev();
            GameField1.GenerateField(MainPanel, name);
            Player.SetThreadFlag(true);
            GameCell[,] field = GameField1.curField;
            Gold.GetAmount(GameForm);
            GameForm.KeyUp += new KeyEventHandler(Key);
            void Key(object sender, KeyEventArgs e)
            {
                if (GameCell.FindPosition(field, "SecondPlayer")[0] != 0)
                {
                    SecondPlayer.MoveSecondHero(e, field, GameField1.curPics, MainPanel);
                    new Thread(() => Fall("SecondPlayer", new SecondPlayer())).Start();
                }
                if (_fallingDown == false)
                    Player.MoveHero(e, field, GameField1.curPics, MainPanel);
                Gold.GetAmount(GameForm);
                new Thread(() => Fall("Player", new Player())).Start();
                void Fall(string type, Player person)
                {
                    int[] coords = GameCell.FindPosition(field, type);
                    int i = coords[0];
                    int j = coords[1];
                    while (field[i + 1, j].Type == "FreeArea" || field[i + 1, j].Type == "Destructed" || field[i + 1, j].Type == "Gold")
                    {
                        _fallingDown = true;
                        Thread.Sleep(250);
                        Player.Falling(field, GameField1.curPics, i, j, person, MainPanel);
                        i++;
                    }
                    _fallingDown = false;
                }
            }
            if (GameCell.FindPosition(field, "SimpleEnemy")[0] != 0 && GameCell.FindPosition(field, "SimpleEnemy")[1] != 0)
            {
                SimpleEnemy.SetSimplePrev();
                new Thread(() => SimpleEnemy.ForThread(field, GameField1.curPics, MainPanel)).Start();
            }
            if (GameCell.FindPosition(field, "BlindEnemy")[0] != 0 && GameCell.FindPosition(field, "BlindEnemy")[1] != 0)
            {
                BlindEnemy.SetPrevBlind();
                new Thread(() => BlindEnemy.SimpleMovement(field, GameField1.curPics, "BlindEnemy", MainPanel)).Start();
            }
            if (GameCell.FindPosition(field, "Coin")[0] != 0 && GameCell.FindPosition(field, "Coin")[1] != 0)
            {
                Coin.SetPrevCoin();
                new Thread(() => Coin.ForThread(field, GameField1.curPics,MainPanel)).Start();
            }
        }
    }
    public abstract class GameCell
    {
        public string Type;
        public string Display;
        public string Definition;
        public string Image;
        public static int[] FindPosition(GameCell[,] gameField, string type)
        {
            int[] result = new int[2];
            for (int i = 0; i < gameField.GetLength(0); i++)
            {
                for (int j = 0; j < gameField.GetLength(1); j++)
                {
                    if (gameField[i, j].Type == type)
                    {
                        result[0] = i;
                        result[1] = j;
                    }
                }
            }
            return result;
        }
    }
    public class Ground : GameCell
    {
        public Ground()
        {
            Type = "Ground";
            Display = "#";
            Definition = "Just a ground.";
            Image = "Data\\Icons\\wall.png";
        }
    }
    public class FreeArea : GameCell
    {
        public FreeArea()
        {
            Type = "FreeArea";
            Display = " ";
            Definition = "Free area.";
            Image = "Data\\Icons\\freeArea.png";
        }
    }
    public class Ladder : GameCell
    {
        public Ladder()
        {
            Type = "Ladder";
            Display = "|";
            Definition = "Ladder.";
            Image = "Data\\Icons\\ladder.png";
        }
    }
    public class Bar : GameCell
    {
        public Bar()
        {
            Type = "Bar";
            Display = "_";
            Definition = "Bar, which you can use to move over free area.";
            Image = "";
        }
    }
    public class Cursor : Player
    {
        public Cursor()
        {
            Type = "Cursor";
            Display = "I";
            Image = "Data\\Icons\\cursor.png";
        }
    }
    public class Gold : GameCell
    {
        private static int _goldCount;
        public Gold()
        {
            Type = "Gold";
            Display = "@";
            Definition = "Gold, which you need to collect.";
            Image = "Data\\Icons\\gold.png";
        }
        public static int IncreaseCount(int points)
        {
            _goldCount += points;
            return _goldCount;
        }
        public static void SetGoldCount()
        {
            _goldCount = 0;
        }
        public static void GetAmount(LodeRunner MainPanel)
        {
            foreach (Control b in MainPanel.Controls)
            {
                if (b is Label && b.Name == "label3")
                {
                    b.Text = "" + _goldCount;
                }
            }
        }
    }
    public class Destructed : FreeArea
    {
        public Destructed()
        {
            Type = "Destructed";
            Display = " ";
            Definition = "Destructed area.";
            Image = "Data\\Icons\\freeArea.png";
        }
        public static void ReviveCell(GameCell[,] gameField)
        {
            List<int> coords = Player.GetCoords();
            for (int i = 0; i < coords.Count; i += 2)
            {
                gameField[coords[i], coords[i + 1]] = new Ground();
                GameField.SetRecover();
                Player.SetClick();
            }
        }
    }
    public class StrongGround : Ground
    {
        public StrongGround()
        {
            Type = "StrongGround";
            Display = "%";
            Definition = "Strong ground, that can be broken only by harmer.";
            Image = "Data\\Icons\\strongground.png";
        }
    }
    public class Harmer : GameCell
    {
        public Harmer()
        {
            Type = "Harmer";
            Display = "!";
            Definition = "Harmer in order to break strong ground.";
            Image = "Data\\Icons\\jackhammer.png";
        }
    }
    public class RopeTrap : GameCell
    {
        public RopeTrap()
        {
            Type = "RopeTrap";
            Display = "®";
            Definition = "Rope trap to kill enemies.";
            Image = "Data\\Icons\\trap.png";
        }
    }
    public class UsedRopeTrap : RopeTrap
    {
        public UsedRopeTrap()
        {
            Type = "UsedRopeTrap";
            Display = "©";
            Definition = "Already used rope trap.";
            Image = "Data\\Icons\\usedtrap.png";
        }
    }
    public class Door : GameCell
    {
        public Door()
        {
            Type = "Door";
            Display = "D";
            Image = "Data\\Icons\\door.png";
            Definition = "Door.";
        }
    }
    public class Boost : GameCell
    {
        public Boost()
        {
            Type = "Boost";
            Display = ">";
            Definition = "Boost. If you take it, you will be able to move faster.";
            Image = "Data\\Icons\\rocket.png";
        }
    }
    public class Helmet : GameCell
    {
        public Helmet()
        {
            Type = "Helmet";
            Display = "+";
            Definition = "Helmet let you move trough walls for some time.";
            Image = "Data\\Icons\\helmet.png";
        }
    }
    public class Key : GameCell
    {
        public Key()
        {
            Type = "Key";
            Display = "Ω";
            Definition = "Key.";
            Image = "Data\\Icons\\key.png";
        }
    }
    public class ExitDoor : GameCell
    {
        public ExitDoor()
        {
            Type = "ExitDoor";
            Display = "O";
            Definition = "Exit door.";
            Image = "Data\\Icons\\exitdoor.png";
        }
    }
    public class Pill : GameCell
    {
        public Pill()
        {
            Type = "Pill";
            Display = "*";
            Definition = "The pill, that disorientates you for some time.";
            Image = "Data\\Icons\\pill.png";
        }
    }
    public class Scrap : GameCell
    {
        public Scrap()
        {
            Type = "Scrap";
            Display = "?";
            Definition = "Scrap can break walls.";
            Image = "Data\\Icons\\scrap.png";
        }
    }
    public class Teleport : GameCell
    {
        public Teleport()
        {
            Type = "Teleport";
            Display = "¤";
            Definition = "Simple teleport.";
            Image = "Data\\Icons\\teleport.png";
        }
    }
    public class Unbreakable : Ground
    {
        public Unbreakable()
        {
            Type = "Unbreakable";
            Display = "▤";
            Definition = "Unbreakable material.";
            Image = "Data\\Icons\\unbreakable.png";
        }
    }
    public class RandomPrize : Gold
    {
        public RandomPrize()
        {
            Type = "RandomPrize";
            Display = "Ꮻ";
            Definition = "Random prize. Take it and explore, what will happen.";
            Image = "Data\\Icons\\random.png";
        }
    }
    class Matrix
    {
        public int H;
        public int W;
        public string[,] Field;
    }
    public class GameField
    {
        private static int _cellRecover;
        public GameCell[,] curField = new GameCell[0, 0];
        public PictureBox[,] curPics = new PictureBox[0, 0];
        public static readonly List<int[]> TeleportCoords = new List<int[]>();
        private static readonly Dictionary<char, GameCell> FieldGenerator = new Dictionary<char, GameCell>(){
            { '#', new Ground()}, { 'X', new Player()}, { ' ', new FreeArea()}, { '|', new Ladder()},{'_', new Bar()},
            { '@',new Gold()}, {'%', new StrongGround()} ,{ '!',new Harmer()},{ 'I',new FreeArea()},
            { '©',new UsedRopeTrap()},{ 'D',new Door()},{ 'O',new ExitDoor()},{ 'Ω',new Key()},{ '?',new Scrap()},
            { '¤', new Teleport()}, {'▤', new Unbreakable()}, {'&', new SimpleEnemy()}, {'♖', new BlindEnemy()}, {'^', new Coin()},
            { 'Σ', new SecondPlayer()}, {'Ꮻ', new RandomPrize()}, {'>', new Boost()}, {'*', new Pill()}, {'+', new Helmet()}
        };
        public void GenerateField(Panel MainPanel, string name)
        {
            string ars = File.ReadAllText("Data\\Levels\\" + name + ".json");
            Matrix field = JsonConvert.DeserializeObject<Matrix>(ars);
            string[,] gameField = field.Field;
            int he = field.H;
            int wi = field.W;
            GameCell[,] myField = new GameCell[he, wi];
            PictureBox[,] pictureField = new PictureBox[he, wi];
            for (int i = 0; i < he; i++)
            {
                for (int j = 0; j < wi; j++)
                {
                    myField[i, j] = FieldGenerator[Convert.ToChar(gameField[i, j])];
                    pictureField[i, j] = new PictureBox()
                    {
                        Size = new Size(30, 30),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Location = new Point(j * 30, i * 30),
                        Image = new Bitmap(myField[i, j].Image)
                    };
                    MainPanel.Controls.Add(pictureField[i, j]);
                    if (myField[i, j].Type == "Teleport")
                    {
                        int[] coordInts = { i, j };
                        TeleportCoords.Add(coordInts);
                    }
                }
            }
            curField = myField;
            curPics = pictureField;
        }
        public static int GetRecover()
        {
            return _cellRecover;
        }
        public static int IncreaseRecover()
        {
            _cellRecover++;
            return _cellRecover;
        }
        public static void SetRecover()
        {
            _cellRecover = 0;
        }
    }
}
  