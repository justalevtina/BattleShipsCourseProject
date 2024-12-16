using PlayerClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace PlayerClient
{
    public partial class ConnectionForm : Form
    {
        private string server = "";
        private PlayerForm playerForm;

        public ConnectionForm()
        {
            InitializeComponent();

            Size = new Size(424, 225);

            Label ServerChoiceLabel = new Label();
            ServerChoiceLabel.Text = "Choose label";
            ServerChoiceLabel.AutoSize = true;
            ServerChoiceLabel.Location = new Point(36, 18);
            ServerChoiceLabel.Font = new Font("", 12);
            Controls.Add(ServerChoiceLabel);

            ServerComboBox.Name = "ServerComboBox";
            ServerComboBox.Items.Add("127.0.0.1");
            ServerComboBox.Location = new Point(36, 73);
            ServerComboBox.Font = new Font("", 12);
            Controls.Add(ServerComboBox);

            Button ConnectionButton = new Button();
            ConnectionButton.Location = new Point(43, 105);
            ConnectionButton.Font = new Font("", 12);
            ConnectionButton.Text = "Start";
            ConnectionButton.AutoSize = true;
            ConnectionButton.Click += new EventHandler(ConectionButtonClick);
            Controls.Add(ConnectionButton);

            ActiveControl = ConnectionButton;
        }

        private void ConectionButtonClick(object sender, EventArgs e)
        {
            TcpClient client;
            IPAddress chosenAddr = IPAddress.Parse("0.0.0.0");

            server = ServerComboBox.Text;
            if (server != "127.0.0.1")
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

            playerForm = new PlayerForm(client);

            this.Close();
            playerForm.Show();
        }
    }
}