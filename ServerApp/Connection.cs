using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp
{


    //Helper class: Ties a username to a socket
    class Connection
    {

        public String userName;
        public TcpClient client;

        public Boolean connected;


        public Connection(String n, TcpClient c)
        {

            userName = n;

            client = c;
            connected = true;
        }

        //reach out and send a message to this client
        //the client should also be told who sent it!
        public void sendMessage(String from, String msg)
        {
            StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.ASCII);
            sw.WriteLine(from + " >: " + msg);
            //forces a write to the stream
            sw.Flush();
        }

    }
}
