using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestModels.Interfaces;
using static System.Collections.Specialized.BitVector32;

namespace TestModels.Services
{
    public class ServerActionService : IServerAction
    {
        public ServerAction CreateAction<T>(EnableAction action, T data)
        {
            ServerAction serverAction = new()
            {
                Name = action,
                JsonObject = JsonSerializer.Serialize(data)
            };
            return serverAction;
        }

        public ServerAction GetAction(string action)
        {
            ServerAction serverAction = JsonSerializer.Deserialize<ServerAction>(action);
            return serverAction;
        }

        public T GetObject<T>(ServerAction serverAction)
        {
            T obj = JsonSerializer.Deserialize<T>(serverAction.JsonObject);
            return obj;
        }
    }
}
