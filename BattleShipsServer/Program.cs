using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BattleShipsPlayerClient;

namespace BattleShipsServer
{
    internal class Program
    {
        private static TcpClient player1;
        private static TcpClient player2;
        private static TcpListener server;

        public static void Main()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, 80); // Создаем TCP сервер на порту 9999
                server.Start();
                Console.WriteLine("Server started...");

                player1 = server.AcceptTcpClient(); // Принимаем клиента
                Console.WriteLine("First player connected...");
                player2 = server.AcceptTcpClient(); // Принимаем клиента
                Console.WriteLine("Second player connected...");

                // Создаем токен отмены для задач
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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

        static async Task HandleClient(TcpClient sender, TcpClient receiver, CancellationToken token)
        {
            try
            {
                NetworkStream senderStream = sender.GetStream(); // получение потока отправителя
                NetworkStream receiverStream = receiver.GetStream(); // получение потока получателя
                byte[] buffer = new byte[1024];

                while (true)
                {
                    token.ThrowIfCancellationRequested(); // Проверяем токен отмены

                    // Читаем сообщение от отправителя
                    int bytesRead = await senderStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Если клиент отключился, выходим из цикла
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    // Пересылаем сообщение получателю
                    await receiverStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}