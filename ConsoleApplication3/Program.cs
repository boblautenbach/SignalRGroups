using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ConsoleApplication3
{
    class Program
    {
        static IHubProxy proxy;
        static HubConnection hubConnection;

        static int msgCnt = 1;

        public class HubMessage
        {
            public string Msg { get; set; }
            public string Group { get; set; }
            public string Sender { get; set; }
        }


        static void Main(string[] args)
        {
            try
            {
                ConnectToSignalR();
     
                while (true) // Loop indefinitely
                {
                    Console.WriteLine("hit enter to send another hub messsage"); // Prompt
                    string line = Console.ReadLine(); // Get string from user
                    if (line == "exit") // Check string
                    {
                        break;
                    }
                    SendMessage();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendMessage()
        {
            proxy.Invoke<HubMessage>("SendMessage", new HubMessage() { Msg = "This is message " + msgCnt, Group = "foo" });
            msgCnt += 1;
        }

        private static void ConnectToSignalR()
        {
            try
            {
                hubConnection = new HubConnection("http://pitalkersignal.azurewebsites.net");

                proxy = hubConnection.CreateHubProxy("MessageHub");

                hubConnection.StateChanged += hubConnection_StateChanged;
                hubConnection.Error += new Action<Exception>(errorHandler);

                hubConnection.TraceLevel = TraceLevels.All;
                hubConnection.TraceWriter = Console.Out;

                proxy.On<HubMessage>("OnMessageSent", (x) =>
                {
                        Console.WriteLine(x.Msg);
                });

                hubConnection.Start().ContinueWith((x) =>
                {
                    proxy.Invoke("Join", "foo").ContinueWith((y) =>{
                         SendMessage();
                    });
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void errorHandler(Exception obj)
        {
            Console.WriteLine("error " + obj.Message);
        }

 
        private static void hubConnection_StateChanged(StateChange obj)
        {
            try
            {
                if (obj.NewState == ConnectionState.Disconnected)
                {
                    Console.WriteLine("Disconnected to the SignalR Cloud");
                }

                if (obj.NewState == ConnectionState.Connecting)
                {
                    Console.WriteLine("Connecting to the SignalR Cloud");
                }

                if (obj.NewState == ConnectionState.Connected)
                {
                   Console.WriteLine("Connected....Connect Id " + hubConnection.ConnectionId.ToString() );
                }
            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


  
    }
}
 