using System.Net.Sockets;
using System.Text;
using SharedClass;
using static SharedClass.FormBlockClass;

namespace PlayerClient
{
    public partial class PlayerForm : Form
    {
        private NetworkStream stream;

        private const string alphabet = "ABCDEFGHIJ";
        private const int MapSize = 11;
        private const int CellSize = 30;
        private const int ShipsNumber = 3;
        private int MyShipsCount = 0;
        private int EnemyShipsCount = 0;

        private bool[,] MyShipsArray = new bool[MapSize + 1, MapSize + 1];
        private bool[,] EnemyShipsArray = new bool[MapSize + 1, MapSize + 1];

        private Button[,] MyMapArray = new Button[MapSize, MapSize];
        private Button[,] EnemyMapArray = new Button[MapSize, MapSize];

        public PlayerForm(TcpClient client)
        {
            InitializeComponent();

            stream = client.GetStream();

            Size = new Size(2 * CellSize * (MapSize + 2), CellSize * (MapSize + 4));

            Label MyMapLable = new Label();
            MyMapLable.Text = "Player map";
            MyMapLable.AutoSize = true;
            MyMapLable.Location = new Point(CellSize, CellSize / 3);
            MyMapLable.Font = new Font("", 12);
            Controls.Add(MyMapLable);
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                {
                    Button MyMapCell = new Button();
                    MyMapCell.Location = new Point(CellSize * (j + 1), CellSize * (i + 1));
                    MyMapCell.Size = new Size(CellSize, CellSize);

                    if (j == 0 || i == 0)
                    {
                        if (j > 0)
                            MyMapCell.Text = alphabet[j - 1].ToString();
                        else if (i > 0)
                            MyMapCell.Text = i.ToString();
                        MyMapCell.Enabled = false;
                    }
                    else
                        MyMapCell.Click += new EventHandler(MyShips);

                    MyMapCell.BackColor = Color.LightGray;

                    MyMapArray[i, j] = MyMapCell;
                    Controls.Add(MyMapCell);
                }

            Label EnemyMapLable = new Label();
            EnemyMapLable.Text = "Opponent map";
            EnemyMapLable.AutoSize = true;
            EnemyMapLable.Font = new Font("", 12);
            EnemyMapLable.Location = new Point(CellSize * (MapSize + 2), CellSize / 3);
            this.Controls.Add(EnemyMapLable);
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    Button EnemyMapCell = new Button();
                    EnemyMapCell.Location = new Point(CellSize * (MapSize + 2 + j), CellSize * (i + 1));
                    EnemyMapCell.Size = new Size(CellSize, CellSize);

                    if (j == 0 || i == 0)
                    {
                        if (j > 0)
                            EnemyMapCell.Text = alphabet[j - 1].ToString();
                        else if (i > 0)
                            EnemyMapCell.Text = i.ToString();
                        EnemyMapCell.Enabled = false;
                    }
                    else
                        EnemyMapCell.Click += new EventHandler(MyMove);

                    EnemyMapCell.Enabled = false;
                    EnemyMapCell.BackColor = Color.LightGray;

                    EnemyMapArray[i, j] = EnemyMapCell;
                    Controls.Add(EnemyMapCell);
                }
            }

            Button StartButton = new Button();
            StartButton.Location = new Point(CellSize, CellSize * (MapSize + 1));
            StartButton.Font = new Font("", 12);
            StartButton.Text = "Start";
            StartButton.AutoSize = true;
            StartButton.Click += new EventHandler(StartButtonClick);
            this.Controls.Add(StartButton);

            ActiveControl = StartButton;
        }

        public void MyShips(object sender, EventArgs e)
        {
            Button PressedButton = sender as Button;

            int y = PressedButton.Location.Y / CellSize - 1;
            int x = PressedButton.Location.X / CellSize - 1;

            // placing player ships on their field algorithm
            if (MyShipsArray[y, x] && MyShipsCount <= ShipsNumber)
            {
                MyMapArray[y, x].BackColor = Color.LightGray;
                MyShipsArray[y, x] = false;
                MyShipsCount--;
            }
            else if (!MyShipsArray[y, x] && MyShipsCount < ShipsNumber)
            {
                MyMapArray[y, x].BackColor = Color.Black;
                MyShipsArray[y, x] = true;
                MyShipsCount++;
            }
        }

        public void MyMove(object sender, EventArgs e)
        {
            Console.WriteLine("i am in my move");
            Button PressedButton = sender as Button;

            int i = PressedButton.Location.Y / CellSize - 1;
            int j = (PressedButton.Location.X - CellSize * (MapSize + 1)) / CellSize - 1;

            EnemyMapArray[i, j].Enabled = false;

            if (!EnemyShipsArray[i, j])
            {
                EnemyMapArray[i, j].Text = ".";
            }
            else if (EnemyShipsArray[i, j])
            {
                EnemyMapArray[i, j].Text = "x";
                EnemyShipsCount++;
            }

            string array = EnemyMapArray[i, j].Text + Convert.ToString(i) + Convert.ToString(j);
            byte[] data = Encoding.UTF8.GetBytes(array);
            stream.Write(data, 0, data.Length);

            /*if (EnemyShipsCount == ShipsNumber) // End game
            {
                ResultForm resultForm = new ResultForm();
                resultForm.Show();
            }*/

        }
        private async void StartButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine("start but");
            try
            {
                if (MyShipsCount < ShipsNumber)
                    MessageBox.Show("Не все корабли расставлены", "Внимание");
                else
                {
                    // отправка своего поля на сервер

                    Console.WriteLine("toserver");
                    string MyMap = "";
                    for (int i = 1; i < MapSize; i++)
                        for (int j = 1; j < MapSize; j++)
                            MyMap += Convert.ToString(MyShipsArray[i, j]) + " ";
                    byte[] data = Encoding.UTF8.GetBytes(MyMap);
                    await stream.WriteAsync(data, 0, data.Length);

                    // получение поля противника с сервера
                    CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
                    CancellationToken token = cancelTokenSource.Token;

                    Console.WriteLine("before server");
                    string EnemyMap = "";
                    Task task = Task.Run(async () =>
                    {
                        Console.WriteLine("sever");
                        byte[] buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                        int k = 0;
                        EnemyMap = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        string[] enemyMap = EnemyMap.Split(' ');
                        for (int i = 1; i < MapSize; i++)
                            for (int j = 1; j < MapSize; j++)
                            { EnemyShipsArray[i, j] = Convert.ToBoolean(enemyMap[k]); k++; }
                        for (int i = 1; i < MapSize; i++)
                            for (int j = 1; j < MapSize; j++)
                                Console.WriteLine(EnemyShipsArray[i, j]);

                        for (int i = 1; i < MapSize; i++)
                            for (int j = 1; j < MapSize; j++)
                                EnemyMapArray[i, j].Enabled = true;
                    }, token);

                    // Создаем экземпляр класса
                    SharedClass.FormBlockClass formBlockClass = new();

                    // Блокируем элементы карты игрока
                    formBlockClass.BlockMapElements(MyMapArray);

                    // Разблокируем элементы карты противника
                    formBlockClass.UnblockMapElements(EnemyMapArray);

                    // отмена задачи при закрытии формы
                    FormClosing += (s, ea) => cancelTokenSource.Cancel();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("error");
                MessageBox.Show(exception.Message, "");
            }
        }

    }
}