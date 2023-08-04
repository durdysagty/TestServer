using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestModels;

namespace TestServer.Services
{
    internal static class OnlineUsers
    {
        public static IList<User> List = new List<User>();
    }
}
