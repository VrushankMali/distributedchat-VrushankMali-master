using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server();
            //track how many users are connected, helps us make unique temporary names
            int counter = 0;

            //set up server side socket
            TcpListener serverSocket = new TcpListener(s.getIP(), 0);

            serverSocket.Start();
            //print out connection information
            //Needed for client to connect!
            Console.WriteLine("IP/Port: " + serverSocket.LocalEndpoint);

            //run forever!
            while (true)
            {
                //This will wait until a client tries to connect!
                //when a client connects, create a new socket to talk to them directly
                TcpClient newClient = serverSocket.AcceptTcpClient();

                //create a connection to talk to the client - don't know name yet
                Connection c = new Connection("temp" + counter, newClient);
                counter++;

                //create a reader on the socket stream, made to work with strings (Encoding.ASCII)
                StreamReader sr = new StreamReader(c.client.GetStream(), Encoding.ASCII);
                StreamWriter sw = new StreamWriter(c.client.GetStream(), Encoding.ASCII);
                //TODO: create a StreamWriter on the socket stream, made to work with strings (Encoding.ASCII)

                //read what the user sent!
                string sData = sr.ReadLine();

                //strip REGISTER from the beginning of message (magic number 8)
                String name = sData.Substring(8);
                //sanity check
                Console.WriteLine("IN REGISTER, NAME: " + name);

                //update connection with actual name
                c.userName = name;

                //attempt to register the user
                String res = s.registerConnection(c);

                sw.WriteLine(res + " >: " + sData);
                //TODO: write res to the StreamWriter (and end with a newline)

                sw.Flush();
                //TODO: Flush() your StreamWriter
            }
        }
    }
}
