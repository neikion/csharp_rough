using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using lab4_server;
namespace RequestSendClient_1
{
    class Client1
    {
        static void Main(string[] args)
        {

            asyncClient c1 = new asyncClient(1);
            c1.StartClient();
            Console.ReadLine();

        }
    }

    public class clientstate
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
        public int id;
    }
    public class asyncClient
    {
        // The port number for the remote device.  
        private const int port = 80;
        private int id;
        private bool clientexit = false;
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;
        public asyncClient(int reid)
        {
            id = reid;
        }
        public void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the
                // remote device is "host.contoso.com".  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                Stateobj cl = new Stateobj();
                cl.id = this.id;
                cl.sk = client;
                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                // Send test data to the remote device.  
                //Send((Stateobj)cl, id+": this is test. hello world! <EOF>");
                //sendDone.WaitOne();


                Task te = Task.Factory.StartNew((s) => send(s), cl);

                Task te2 = Task.Factory.StartNew((c) => Receive(c), client);


                /*
            // Receive the response from the remote device.  
            Receive(client);
            receiveDone.WaitOne();
                */
                // Write the response to the console.  
                Console.WriteLine("Response received : {0}", response);

                /*
                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                */
                Task.Delay(20000).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void send(object cl)
        {
            string data;
            string input;
            while (!clientexit)
            {
                input = Console.ReadLine();
                if (input.Equals(ConsoleKey.Escape))
                {
                    clientexit = true;
                    ((Stateobj)cl).sk.Shutdown(SocketShutdown.Both);
                    ((Stateobj)cl).sk.Close();
                    break;
                }
                data = id + " : " + input;
                Console.WriteLine(((Stateobj)cl).sk.Connected);
                // Send test data to the remote device.  
                Send((Stateobj)cl, data + "<EOF>");
                sendDone.WaitOne();
            }
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;
                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(object cli)
        {
            while (!clientexit)
            {
                try
                {
                    Socket client = (Socket)cli;
                    // Create the state object.  
                    clientstate state = new clientstate();
                    state.workSocket = client;

                    // Begin receiving the data from the remote device.  
                    client.BeginReceive(state.buffer, 0, clientstate.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                receiveDone.WaitOne();
            }

        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                clientstate state = (clientstate)ar.AsyncState;

                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, clientstate.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                        Console.WriteLine(response);
                        state.sb.Clear();
                    }

                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Stateobj client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.sk.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                //Socket client = (Socket)ar.AsyncState;

                Socket client = ((Stateobj)ar.AsyncState).sk;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
