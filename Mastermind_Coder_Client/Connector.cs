using System.Net.Sockets;
using System.Text;

namespace Mastermind_Client
{
    public class Connector
    {

        public static void Connect(NetworkStream stream, string message) // Отправка сообщения
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}