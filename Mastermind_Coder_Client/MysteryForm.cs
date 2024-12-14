using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mastermind;
using Mastermind_Client;
using Mastermind_Coder_Client;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mastermind
{
    public partial class MysteryForm : Form
    {
        private Color color;
        private TaskCompletionSource<bool> tcs;
        private object syncObject = new object();
        public MysteryForm()
        {
            InitializeComponent();
        }
        
        private async Task chooseColor() //Обработка окрашивания кнопок
        {
            
            EventHandler eventHandler = null;
            eventHandler = (s, e) =>
            {
                lock (syncObject)
                {
                    eventHandler -= colorButton_Click;
                    tcs.SetResult(true);
                }

            };
            eventHandler += colorButton_Click;
        }
        private void colorButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            color = button.BackColor;
        }
        
        private async void pagButton_Click(object sender, EventArgs e)
        {
            PagButton button = (PagButton)sender;
            tcs = new TaskCompletionSource<bool>();
            
            await chooseColor();
            button.BackColor = color;
        }

        private void button7_Click(object sender, EventArgs e) // Составление кода
        {
            if (pagButton97.BackColor.Name == "Transparent" ||
                pagButton98.BackColor.Name == "Transparent" ||
                pagButton99.BackColor.Name == "Transparent" ||
                pagButton100.BackColor.Name == "Transparent")
            {
                MessageBox.Show("Код составлен неверно", "Внимание");
            }
            else
            {
                CoderForm form = (CoderForm)Application.OpenForms[1];
                form.SetPagButton1(pagButton100);
                form.SetPagButton2(pagButton99);
                form.SetPagButton3(pagButton98);
                form.SetPagButton4(pagButton97);
                form.Enabled = true;
                Close();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mastermind_Client
{
    public partial class PlayerForm : Form
    {
        private int attempt = 1;
        private bool isDecoded = false;

        private TaskCompletionSource<bool> tcs;
        private object syncObject = new object();
        Color color;

        private TcpClient client;
        private NetworkStream stream;

        private Dictionary<int, List<PagButton>> attempts; //Словарь "ход - кнопка оценки"
        private Dictionary<int, List<PagButton>> decodeAttempts; //Словарь "ход - кнопка попытки"

        public PlayerForm(TcpClient client)
        {
            InitializeComponent();

            this.client = client;
            stream = client.GetStream();

            attempts = new Dictionary<int, List<PagButton>>()
            {
                {1, new List<PagButton>(){pagButton51, pagButton52, pagButton49, pagButton50}},
                {2, new List<PagButton>(){pagButton55, pagButton56, pagButton53, pagButton54}},
                {3, new List<PagButton>(){pagButton59, pagButton60, pagButton57, pagButton58}},
                {4, new List<PagButton>(){pagButton63, pagButton64, pagButton61, pagButton62}},
                {5, new List<PagButton>(){pagButton67, pagButton68, pagButton65, pagButton66}},
                {6, new List<PagButton>(){pagButton71, pagButton72, pagButton69, pagButton70}},
                {7, new List<PagButton>(){pagButton75, pagButton76, pagButton73, pagButton74}},
                {8, new List<PagButton>(){pagButton79, pagButton80, pagButton77, pagButton78}},
                {9, new List<PagButton>(){pagButton83, pagButton84, pagButton81, pagButton82}},
                {10, new List<PagButton>(){pagButton87, pagButton88, pagButton85, pagButton86}},
                {11, new List<PagButton>(){pagButton91, pagButton92, pagButton89, pagButton90}},
                {12, new List<PagButton>(){pagButton95, pagButton96, pagButton93, pagButton94}},
            };

            decodeAttempts = new Dictionary<int, List<PagButton>>()
            {
                {1, new List<PagButton>(){pagButton4, pagButton3, pagButton2, pagButton1}},
                {2, new List<PagButton>(){pagButton8, pagButton7, pagButton6, pagButton5}},
                {3, new List<PagButton>(){pagButton12, pagButton11, pagButton10, pagButton9}},
                {4, new List<PagButton>(){pagButton16, pagButton15, pagButton14, pagButton13}},
                {5, new List<PagButton>(){pagButton20, pagButton19, pagButton18, pagButton17}},
                {6, new List<PagButton>(){pagButton24, pagButton23, pagButton22, pagButton21}},
                {7, new List<PagButton>(){pagButton28, pagButton27, pagButton26, pagButton25}},
                {8, new List<PagButton>(){pagButton32, pagButton31, pagButton30, pagButton29}},
                {9, new List<PagButton>(){pagButton36, pagButton35, pagButton34, pagButton33}},
                {10, new List<PagButton>(){pagButton40, pagButton39, pagButton38, pagButton37}},
                {11, new List<PagButton>(){pagButton44, pagButton43, pagButton42, pagButton41}},
                {12, new List<PagButton>(){pagButton48, pagButton47, pagButton46, pagButton45}},
            };
        }

        private async void pagButton_Click(object sender, EventArgs e)
        {
            PagButton button = (PagButton)sender;
            tcs = new TaskCompletionSource<bool>();

            await chooseColor();
            button.BackColor = color;
        }

        private async Task chooseColor() //Окрашивание кнопки
        {

            EventHandler eventHandler = null;
            eventHandler = (s, e) =>
            {
                lock (syncObject)
                {
                    eventHandler -= colorButton_Click;
                    tcs.SetResult(true);
                }

            };
            eventHandler += colorButton_Click;
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            color = button.BackColor;
        }

        private void tryButton_Click(object sender, EventArgs e) // Завершение хода
        {
            if (decodeAttempts[attempt][0].BackColor.Name == "Transparent" ||
                decodeAttempts[attempt][1].BackColor.Name == "Transparent" ||
                decodeAttempts[attempt][2].BackColor.Name == "Transparent" ||
                decodeAttempts[attempt][3].BackColor.Name == "Transparent")
            {
                MessageBox.Show("Код составлен неверно", "Внимание");
            }
            else
            {

                string ans = attempt + " " + decodeAttempts[attempt][0].BackColor.Name
                             + " " + decodeAttempts[attempt][1].BackColor.Name
                             + " " + decodeAttempts[attempt][2].BackColor.Name
                             + " " + decodeAttempts[attempt][3].BackColor.Name;
                Connector.Connect(stream, ans);

                panel1.Enabled = false;
                progressBar1.Visible = true;
                label1.Visible = true;
            }
        }

        private void SolverForm_Load(object sender, EventArgs e)
        {
            //Обработка сообщений и модификация формы
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            Task task = Task.Run(async () =>
            {
                try
                {
                    byte[] buffer = new byte[1024];

                    while (true)
                    {
                        foreach (var item in decodeAttempts)//Активация кнопок на этом ходу
                        {
                            if (item.Key == attempt)
                            {
                                item.Value[0].Enabled = true;
                                item.Value[1].Enabled = true;
                                item.Value[2].Enabled = true;
                                item.Value[3].Enabled = true;
                            }
                            else
                            {
                                item.Value[0].Enabled = false;
                                item.Value[1].Enabled = false;
                                item.Value[2].Enabled = false;
                                item.Value[3].Enabled = false;
                            }
                        }
                        //Получение сообщения и модификация формы
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        string[] buttons = message.Split(' ');
                        int iter = 0;
                        foreach (var item in attempts[attempt])
                        {
                            Invoke(new Action(() => item.BackColor = Color.FromName(buttons[iter++])));
                        }
                        //Обработка полученной оценки
                        if (attempts[attempt][0].BackColor.Name == "White" &&
                            attempts[attempt][1].BackColor.Name == "White" &&
                            attempts[attempt][2].BackColor.Name == "White" &&
                            attempts[attempt][3].BackColor.Name == "White")
                        {
                            isDecoded = true;
                            cancelTokenSource.Cancel();
                            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();
                        }
                        else if (attempt == 12 && (attempts[attempt][0].BackColor.Name != "White" ||
                                                   attempts[attempt][1].BackColor.Name != "White" ||
                                                   attempts[attempt][2].BackColor.Name != "White" ||
                                                   attempts[attempt][3].BackColor.Name != "White"))
                        {
                            cancelTokenSource.Cancel();
                            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();
                        }

                        attempt++;
                        iter = 0;

                        panel1.Enabled = true;
                        progressBar1.Visible = false;
                        label1.Visible = false;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    progressBar1.Visible = false;
                    label1.Visible = false;
                    //Обработка победы/поражения
                    Invoke(isDecoded
                        ? new Action(() => { new ResultForm(true, attempt).Show(); })
                        : new Action(() => { new ResultForm(false, attempt).Show(); }));
                }
            }, token);
        }
    }
}