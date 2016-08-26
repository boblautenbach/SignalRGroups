# signalR Groups

I have 2 separate clients connecting. One client will register to join group "foo" (ClientA-foo) and the other will join the same group(clientB-foo). Both clients are .Net-based and running the latest version of the signalr client (2.2.1.0).  

When I have clientA-foo Invoke SendMessage("foo", "Hello") from its proxy, my ClientB-foo doesn't always get the message on the On(OnMessageSent). Again, its random...Sometimes its works and sometimes its doesn't.   Is there something I am missing to make it more consistent?


I actually 'tailed" the traces to my log pc to watch.  The hub methods are being called but it seems like the hub thinks the 2 clients are not in the same group (which they are).

The way to replicate......Spin up one and then spin up a second one.  You may have to spin up and shut down the second one a few times to see this issue but eventually you will see it.


Is it at all possible that the Server Hub is creating multiple instance of itself, thereby causing some connections to join the group in one instance and other connections to join the group in another instance.  For example, (I have tried this), I have 4 console apps up running: console1 and console3 can send and receive messages to/from group "foo" and console2 and console4 can send and send/receive to/from group "foo".  But they only work in those pairs, meaning all 4 of the console apps should see all the messages (they are in the same group) but that is not happening.  Its as if each pair of consoles apps is connected to a completely different hub instance.


Here is my server code for MessageHub: Hub 

Startup.cs and Hub code below: 

        {
           app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
 
            app.MapSignalR();
        }


        public Task Join(string name)
        {
            //maybe check that it is a valid center?
            return Groups.Add(Context.ConnectionId, name);
        }

        public void SendMessage(string name, string message)
        {
            Clients.Group(name).OnMessageSent(message);
        }

