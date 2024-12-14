using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mastermind_Client;
using Mastermind_Coder_Client;

namespace Mastermind_Start
{
    public partial class ChoiseForm : Form
    {
        private string server = "";
        private TcpClient client;
        private IPAddress chosenAddr;
        private NetworkStream stream;
        private Task task;
        public ChoiseForm(String server)
        {
            InitializeComponent();
            textBox1.Text = server;
            this.server = server;
            if (this.server != "127.0.0.1")
            {
                IPAddress[] addresses = Dns.GetHostEntry(server).AddressList;
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        chosenAddr = address;
                    }
                }
                client = new TcpClient();
                client.Connect(chosenAddr, 80);
            }
            else
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 80);
            }
            stream = client.GetStream();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Program.context.MainForm = new PlayerForm(client);
            
            Close();
            Program.context.MainForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            Program.context.MainForm = new CoderForm(client);
            Close();
            Program.context.MainForm.Show();
        }
    }
}