using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using TestModels;
using TestModels.Interfaces;
using TestModels.Services;
using TestServer.Interfaces;
using TestServer.Services;
using System;
using System.Threading.Tasks;
using System.Linq;

IServerAction serverAction = new ServerActionService();
IPEndPoint ipPoint = new(IPAddress.Any, 8888);
using Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Bind(ipPoint);
socket.Listen();
Console.WriteLine("Сервер ожидает подключения...");
_ = Task.Run(() =>
{
    while (Console.ReadKey().Key != ConsoleKey.Enter)
    {
    }
    socket.Close();
    Environment.Exit(0);
});

while (true)
{
    Socket client = await socket.AcceptAsync();
    Console.WriteLine($"Адрес подключенного клиента: {client.RemoteEndPoint}");
    IRepository repositoryService = new RepositoryService();
    ILogin loginService = new LoginService(repositoryService);
    ITest testService = new TestService(repositoryService);

    _ = Task.Run(async () =>
    {
        byte[] buffer = new byte[51200];
        int bytesReceived;

        User loggedInUser = null;
        try
        {
            while ((bytesReceived = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None)) > 0)
            {
                byte[] receivedBytes = new byte[bytesReceived];
                Array.Copy(buffer, receivedBytes, bytesReceived);
                string receivedString = Encoding.UTF8.GetString(receivedBytes);
                ServerAction action = serverAction.GetAction(receivedString);
                Console.WriteLine(action.Name);
                switch (action.Name)
                {
                    case EnableAction.Login:
                        string userName = serverAction.GetObject<string>(action);
                        if (OnlineUsers.List.Any(u => u.Name == userName))
                        {
                            string message = "Вы уже подключились с другого устройства!";
                            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                            await client.SendAsync(new ArraySegment<byte>(messageBytes, 0, messageBytes.Length), SocketFlags.None);
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                            return;
                        }
                        Console.WriteLine($"{action.Name} - {userName}");
                        loggedInUser = await loginService.LoginAsync(userName);
                        byte[] resultBytes = Encoding.UTF8.GetBytes(loggedInUser.IsTestPassed.ToString());
                        await client.SendAsync(new ArraySegment<byte>(resultBytes, 0, resultBytes.Length), SocketFlags.None);
                        break;
                    case EnableAction.Test:
                        userName = serverAction.GetObject<string>(action);
                        Console.WriteLine($"{action.Name} - {userName}");
                        Test test = await testService.GetTest(userName);
                        string jsonTest = JsonSerializer.Serialize(test);
                        resultBytes = Encoding.UTF8.GetBytes(jsonTest);
                        await client.SendAsync(new ArraySegment<byte>(resultBytes, 0, resultBytes.Length), SocketFlags.None);
                        break;
                    default:
                        UserAnswer userAnswer = serverAction.GetObject<UserAnswer>(action);
                        userName = userAnswer.Name;
                        Console.WriteLine($"{action.Name} - {userName}");
                        bool isPassed = await testService.IsTestPassed(userAnswer);
                        resultBytes = Encoding.UTF8.GetBytes(isPassed.ToString().ToLower());
                        await client.SendAsync(new ArraySegment<byte>(resultBytes, 0, resultBytes.Length), SocketFlags.None);
                        break;
                }
                buffer = new byte[51200];
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Произошла ошибка: {e.Message}");
        }
        finally
        {
            repositoryService.Dispose();
        }
        OnlineUsers.List.Remove(loggedInUser);
        Console.WriteLine($"Клиент отключился: {client.RemoteEndPoint}");
        Console.WriteLine(string.Join(", ", OnlineUsers.List.Select(u => u.Name))); ;
        client.Dispose();
    });
}