using System;
using System.Windows.Forms;

namespace BattleShipsPlayerClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ConnectionForm connectionForm1 = new ConnectionForm();
            connectionForm1.Show();

            ConnectionForm connectionForm2 = new ConnectionForm();
            connectionForm2.Show();

            Application.Run();
        }
    }
}
