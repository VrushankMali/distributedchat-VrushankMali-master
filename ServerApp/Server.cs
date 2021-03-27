using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp
{
    class Server
    {
        //Ties a username to a connection (how you send messages to people)
        //C#'s dictionary is a HashMap (if you've seen the topic already)
        //This allows you to map a key to a value, offering fast lookup on the key!
        Dictionary<String, Connection> userMap;
        //Map a user name to the thread handling a client
        Dictionary<String, Thread> threadMap;

        //Easy lookup of server connection information
        IPAddress myIP;

        public Server()
        {
            userMap = new Dictionary<String, Connection>();
            threadMap = new Dictionary<String, Thread>();
            //TODO: instantiate threadMap here!

            //a bit of magic for the lab machines
            myIP = Dns.GetHostAddresses(Environment.MachineName)[1];
        }

        public IPAddress getIP()
        {
            return myIP;
        }

        public String registerConnection(Connection c)
        {
            string n = c.userName;
            //don't allow users with the same name
            if (userMap.ContainsKey(n))
            {
                return "ERROR: User " + n + " already exists";
            }
            //add the user to userMap
            userMap.Add(n, c);

            Thread thr = new Thread(this.handleConnection);
            //TODO: create a new thread to manage this connection.  The function it should run is this.handleConnection
            threadMap.Add(n, thr);
            //TODO: add the thread to threadMap
            thr.Start();
            //TODO: Start the new thread!
            
            //report success
            return "SUCCESS: User " + n + " successfully added";
        }

        //Clean way to clean up the user and thread list when a client disconnects
        public String removeConnection(String name)
        {
            //sanity check to make sure we were actually tracking this user
            if (!userMap.ContainsKey(name))
            {
                return "ERROR: User " + name + " not in system";
            }
            userMap.Remove(name);
            threadMap.Remove(name);
            return "SUCCESS: User " + name + " successfully removed";
        }

        //accessor to look up a connection for a user name
        public Connection getConnection(String name)
        {
            if (userMap.ContainsKey(name))
            {
                return userMap[name];
            }
            return null;
        }

        //helper function to manage sending a broadcast message
        public void broadcast(String sender, String msg)
        {
            foreach (String k in userMap.Keys)
            {
                //don't show a broadcast message to the sender
                if (!k.Equals(sender))
                {
                    userMap[k].sendMessage(sender, msg);
                }
            }
        }


        //create a list of all usernames of connected users
        public List<String> getUsers()
        {
            //nifty way to turn the collection of keys into a list!
            List<String> names = userMap.Keys.ToList();
            return names;
        }

        //the function that your threads will run, loop while the client is connected
        //Note: we passed an object to the thread!  Every object in C# inherits from the base Object class
        public void handleConnection(Object obj)
        {
            //cast back up from generic object to Connection!
            Connection c = (Connection)obj;

            //create a stream reader on the socket's stream.  Set it up to read normal strings (Encoding.ASCII)
            StreamReader sr = new StreamReader(c.client.GetStream(), Encoding.ASCII);

            StreamWriter sw = new StreamWriter(c.client.GetStream(), Encoding.ASCII);
            //TODO: create a streamWriter on the socket's stream.  Set it up to read normal strings (Encoding.ASCII)

            //while the client's connected
            while (c.connected)
            {
                //figure out what the user is trying to do...
                String sData = sr.ReadLine();
                if (sData.StartsWith("BROADCAST"))
                {
                    //handle a broadcast message
                    String message = sData.Substring(9); //magic to get past the string: "BROADCAST" at the beginning of the message
                    
                    //call broadcast
                    broadcast(c.userName, message);
                }
                else if (sData.StartsWith("LOGOFF"))
                {
                    //handle logout
                    String res = removeConnection(c.userName);
                    sw.WriteLine(res);
                    sw.Flush();
                    c.connected = false;
                }
                else if (sData.StartsWith("USERLIST"))
                {
                    //tell the client everyone that's connected
                    List<String> users = getUsers();
                    String r = "";
                    foreach (String s in users)
                    {
                        r += s + "\n";
                    }
                    sw.WriteLine("USERS:\n" + r);
                    sw.Flush();
                }
                else
                {
                    //must be a whisper

                    //need a character array for a split() function
                    char[] sp = { ':' };
                    //split on the first ':' you find in a whisper message
                    string[] stuff = sData.Split(sp, 2);
                    //first part will be who you're supposed to connect to
                    Connection to = getConnection(stuff[0]);
                    if (to != null)
                    {
                        to.sendMessage(c.userName, stuff[1]);
                    }
                    else
                    {
                        sw.WriteLine("ERROR: could not find user " + stuff[0] + " to send private message.");
                        sw.Flush();
                    }

                }
            }
        }

    }
}
