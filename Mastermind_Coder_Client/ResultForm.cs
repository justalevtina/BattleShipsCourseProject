using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mastermind
{
    public partial class ResultForm : Form
    {
        public ResultForm(bool isWon, int attempt)
        {
            InitializeComponent();
            if (isWon)
            {
                pictureBox1.BackgroundImage = Image.FromFile("lose.png");
                label1.Text = "Поражение";
                label2.Text = $"Ваш код взломали за {attempt} попытку(-ок)";
            }
            else
            {
                BackgroundImage = Image.FromFile("win.png");
                pictureBox1.BackgroundImage = Image.FromFile("won.png");
                label1.Text = "Победа";
                label2.Text = "Ваш код остался нерасшифрованным";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}