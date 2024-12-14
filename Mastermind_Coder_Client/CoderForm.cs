using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mastermind;
using Mastermind_Client;

namespace Mastermind_Coder_Client
{
    public partial class CoderForm : Form
    {
        private int index = 0;
        private int currentAttempt;
        
        private TcpClient client;
        private NetworkStream stream;
        
        private PagButton usedButton = null;
        
        private Dictionary<int, List<PagButton>> gradeAttempts; //Словарь "ход - кнопки оценки"
        private Dictionary<int, List<PagButton>> decodeAttempts; //Словарь "ход - кнопки расшифровки"
        
        public CoderForm(TcpClient client)
        {
            InitializeComponent();
            
            Enabled = false;
            new MysteryForm().Show();
            
            this.client = client;
            stream = client.GetStream();
            
            gradeAttempts = new Dictionary<int, List<PagButton>>()
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
        
        public void SetPagButton1(PagButton button)
        {
            pagButton100.BackColor = button.BackColor;
        }
        public void SetPagButton2(PagButton button)
        {
            pagButton99.BackColor = button.BackColor;
        }
        public void SetPagButton3(PagButton button)
        {
            pagButton98.BackColor = button.BackColor;
        }
        public void SetPagButton4(PagButton button)
        {
            pagButton97.BackColor = button.BackColor;
        }

        private void ansButton_Click(object sender, EventArgs e) //Пометка оценивающих кнопок
        {
            PagButton button = (PagButton)sender;
            if (button != usedButton && usedButton != null) index = 0;
            List<Color> list = new List<Color>() { Color.Black , Color.White, Color.Transparent};
            
            if (index == 3)
            {
                index = 0;
            }
            button.BackColor = list[index++];
            usedButton = button;
        }


        private void resultButton_Click(object sender, EventArgs e) // Завершение хода
        {
            string message =  gradeAttempts[currentAttempt][0].BackColor.Name + " " 
                             + gradeAttempts[currentAttempt][1].BackColor.Name + " " 
                             + gradeAttempts[currentAttempt][2].BackColor.Name + " " 
                             + gradeAttempts[currentAttempt][3].BackColor.Name;
            Connector.Connect(stream, message);
            if (message == "White White White White") // Обработка "конца игры"
            {
                progressBar1.Visible = false;
                new ResultForm(true, currentAttempt).Show();
            }
            else
            {
                progressBar1.Visible = true;
            }

            if (currentAttempt == 12 && message != "White White White White")
            {
                new ResultForm(false, currentAttempt).Show();
            }
            
            panel1.Enabled = false;
        }

        private void CoderForm_Load(object sender, EventArgs e)
        {
            //Получение и обработка сообщений
            Task task = Task.Run(async () =>
            {
                    byte[] buffer = new byte[1024];

                    while (true)
                    {
                        foreach (var item in gradeAttempts) //Активация кнопок текущего хода
                        {
                            if (item.Key == currentAttempt)
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
                        
                        //Получение сообщения и применение его содержимого на форме
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        string[] response = message.Split(' ');
                        currentAttempt = Int32.Parse(response[0]);
                        List<string> buttons = response.ToList();
                        buttons.RemoveAt(0);
                        int iter = 0;
                        foreach (var item in decodeAttempts[currentAttempt])
                        {
                            Invoke(new Action(() => item.BackColor = Color.FromName(buttons[iter++])));
                        }

                        iter = 0;
                        progressBar1.Visible = false;
                        panel1.Enabled = true;
                    }
            });
        }
    }
}