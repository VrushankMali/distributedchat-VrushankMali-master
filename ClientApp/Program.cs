using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //check out program properties to add command line arguments!
            if (args.Length < 3)
            {
                //exit immediately if we don't have enough information to run
                Console.WriteLine("Correct Usage: ChatClient <username> <server ip> <server port>");
                System.Environment.Exit(1);
            }

            Client clt = new Client.Client(String name, String ip, int port);
            //TODO: Create new Client - should start running automatically
        }
    }
}
