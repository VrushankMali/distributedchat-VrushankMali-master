using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp
{
    class Client
    {
        String userName; //keep track of user name
        TcpClient client; //actual TCP connection to server
        StreamReader sr; //for reading strings from the TCP connection
        StreamWriter sw; //fopr writing strings to the TCP connection
        Boolean running; //used to control loops, make sure we connected correctly
        Thread messageListener; //Thread to handle incoming messages from the server
        Thread consoleListener; //Thread to handle interacting with user

        public Client(String name, String ip, int port)
        {
            userName = name;
            //create TCP connection
            client = new TcpClient();
            //Connect to the server
            client.Connect(ip, port);
            sr = new StreamReader(name, Encoding.ASCII);
            sw = new StreamWriter(client.GetStream(), Encoding.ASCII);
            //TODO: initialize your StreamReader and Writer!  They will connect to the socket stream, and use plain text (Encoding.ASCII)
            

            running = start();

            if (running)
            {
                messageListener = new Thread(this.chatListener);
                //TODO: Initialize and start the messageListener.  This thread should run on the function this.chatListener()
                consoleListener = new Thread(this.handleTyping);
                //TODO: Initalize and start the consoleListerner.  This thread should run on the function this.handleTyping()
                
            }
        }

        public bool start()
        {
            //generate and send a registration message!
            //kill and exit if bad user name

            sw.WriteLine("REGISTER" + userName);
            sw.Flush();

            String result = sr.ReadLine();
            Console.WriteLine("Server: " + result);
            if (result.ToLower().StartsWith("error"))
            {
                return false;
            }
            else return true;
        }

        //thread to forever listen for strings coming from the server
        public void chatListener()
        {
            while (running)
            {
                Console.ReadLine();
                //TODO: read a line of text from the socket
                Console.WriteLine(sw);
                //TODO: write the line of text to the console
           
            }
        }

        //helper to make it easy to write to the socket
        public void sendData(String data)
        {
            Console.WriteLine(data);
            //TODO: Write a line of text (data) to the socket, and flush()
            sw.Flush();
        }

        //function for thread to listen to console input
        public void handleTyping()
        {
            //Menu of commands
            string helpInfo = "Welcome to my Awesome Chat Program!\n" +
                    "help or HELP - prints this message\n" +
                    "quit or QUIT - gracefully exits the program\n" +
                    "show users - queries the server for a list of connected users\n" +
                    "\\whisper <user>: - sends message to only the user specified\n" +
                    "anything else entered is assumed to be a mass broadcast message and will be sent to all connected users";

            Console.WriteLine(helpInfo);
            //TODO: Display helpInfo on startup

            while (running)
            {
                String info = Console.ReadLine();

                if (info.ToLower().Equals("help"))
                {
                    Console.WriteLine(helpInfo);
                }
                else if (info.ToLower().Equals("quit"))
                {
                    //exit gracefully...
                    Console.WriteLine("LOGGOF");
                    Console.ReadLine();
                    //TODO: Send the server a logoff message ("LOGOFF")  Don't forget to Flush()!
                    sw.Flush();

                    running = false;
                }
                else if (info.ToLower().Equals("show users"))
                {
                    Console.WriteLine(userName);
                    //TODO: send user name request to server ("USERLIST")  Don't forget to Flush()!
                    sw.Flush();
                }
                else if (info.ToLower().StartsWith("\\whisper"))
                {
                    //send whisper
                    int idx = info.IndexOf(" ");
                    //TODO: strip off "\whisper" (HINT: you'll want to use Substring() and idx from above)

                    //TODO: send the whisper to the server!  Don't forget to Flush()!
                   
                }
                else
                {
                    //otherwise it is a broadcast message!
                    //TODO: send broadcast message to the server.  append the user input to the word ("BROADCAST").  Don't forget to Flush()!
                    
                }
            }

        }


    }
}
