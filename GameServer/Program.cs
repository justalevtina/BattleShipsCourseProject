using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    internal class Program
    {
        public static void Main()
        {
            try
            {
                Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, 80));
                server.Listen(10);
                Console.WriteLine("Server started...");

                Socket player1 = server.AcceptAsync().Result;
                Console.WriteLine("First player connected...");
                Socket player2 = server.AcceptAsync().Result;
                Console.WriteLine("Second player connected...");

                // Создаем токен отмены для задач
                CancellationTokenSource cancellationTokenSource = new();

                // Создаем асинхронные задачи для обработки клиентов
                Task task1 = Task.Run(() => HandleClient(player1, player2, cancellationTokenSource.Token));
                Task task2 = Task.Run(() => HandleClient(player2, player1, cancellationTokenSource.Token));

                // Ждем завершения задач
                Task.WaitAll(task1, task2);

                // Отменяем задачи
                cancellationTokenSource.Cancel();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static async Task HandleClient(Socket sender, Socket receiver, CancellationToken token)
        {
            try
            {
                NetworkStream senderStream = new(sender);
                NetworkStream receiverStream = new(receiver);
                byte[] buffer = new byte[1024];

                while (true)
                {
                    token.ThrowIfCancellationRequested(); // Проверяем токен отмены

                    // Читаем сообщение от отправителя
                    int bytesRead = await senderStream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0) break; // Если клиент отключился, выходим из цикла
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    // Пересылаем сообщение получателю
                    await receiverStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}