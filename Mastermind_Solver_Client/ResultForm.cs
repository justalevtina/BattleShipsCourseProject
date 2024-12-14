using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mastermind_Client
{
    public partial class ResultForm : Form
    {
        public ResultForm(bool isWon, int attempt)
        {
            InitializeComponent();
            if (isWon)
            {
                BackgroundImage = Image.FromFile("win.png");
                pictureBox1.BackgroundImage = Image.FromFile("won.png");
                label1.Text = "Победа";
                label2.Text = $"Вы взломали код за {attempt} попытку(-ок)";
            }
            else
            {
                pictureBox1.BackgroundImage = Image.FromFile("lose.png");
                label1.Text = "Поражение";
                label2.Text = "Вы так и не смогли расшифровать код";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}