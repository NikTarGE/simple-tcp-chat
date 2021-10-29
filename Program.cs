using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Serv
{
    class Program
    {
        static void Main(string[] args)
        {
              const string ip = "127.0.0.1";
              const int port = 8080;

               IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

               Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server_socket.Bind(tcpEndPoint);
            server_socket.Listen(10);
            while (true)
              {
                  var client_socket = server_socket.Accept();
                  var buffer = new byte[256];
                  var size = 0;
                  var StrBuilder = new StringBuilder();  
                  do
                  {
                      size = client_socket.Receive(buffer);
                      StrBuilder.Append(Encoding.UTF8.GetString(buffer, 0, size));
                  }
                  while (client_socket.Available > 0);
                  Console.WriteLine(StrBuilder);
                  client_socket.Shutdown(SocketShutdown.Both);
                  client_socket.Close();

            }

        }
    }
}
