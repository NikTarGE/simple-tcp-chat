using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Chat_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

public partial class MainWindow : Window
    {
        //tb- text box
        // lb- label
        //butt - button(ass) ^_^
        TextBox chat_tb;
        TextBox message_tb;
        TextBox ip_tb;
        TextBox port_tb;
        Button send_butt;
        Button connect_butt;
        Label connected_ip_lb;
        Label connected_port_lb;

        public MainWindow()
        {
            InitializeComponent();

            chat_tb = (TextBox)FindName("Chat_Box");
            ip_tb = (TextBox)FindName("IP_Text_Box");
            port_tb = (TextBox)FindName("PORT_Text_Box");
            message_tb = (TextBox)FindName("Message_Input");

            send_butt = (Button)FindName("Send_button");
            connect_butt = (Button)FindName("Connect_Button");

            connected_ip_lb = (Label)FindName("Connected_IP_Lable");
            connected_port_lb = (Label)FindName("Connected_Port_Lable");

            send_butt.Click += Send_button_Click;
            connect_butt.Click += Try_Connect_to_Socket;
            Loaded += OnLoad;
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            try {
                if (File.Exists("endpoint.json"))
                {
                    endpoint_json endpoint = JsonConvert.DeserializeObject<endpoint_json>(File.ReadAllText("endpoint.json"));
                    connected_ip_lb.Content = endpoint.ip;
                    connected_port_lb.Content = endpoint.port;
                }
                   
                }
            catch
            {
                MessageBox.Show("Deserialize Error");
            }
        }

            private async void Send_button_Click(object sender, RoutedEventArgs e)
        {
            Socket client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if (File.Exists("endpoint.json"))
                {
                    endpoint_json endpoint_json = JsonConvert.DeserializeObject<endpoint_json>(File.ReadAllText("endpoint.json"));
                    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(endpoint_json.ip), endpoint_json.port);
                    await client_socket.ConnectAsync(iPEndPoint);

                }
            }
            catch
            {
                MessageBox.Show("Socket connection error");
            }
            if (client_socket.Connected)
            { 
            string message = message_tb.Text;
            var buffer = Encoding.UTF8.GetBytes(message);
            client_socket.Send(buffer);

            var data = new byte[256];
            var size = 0;
            var answer = new StringBuilder();

          /*  do
            {            
                size = client_socket.Receive(data);
                answer.Append(Encoding.UTF8.GetString(data, 0, size));
            }
            while (client_socket.Available > 0);*/
             Console.WriteLine(answer.ToString());
            chat_tb.Text += answer + "\n";
            message_tb.Text = "";

            client_socket.Disconnect(true);
            }
            else
            {
                MessageBox.Show("Socket is not connected");
            }
        }

        public class endpoint_json
        {
            public endpoint_json(string ip, int port)
            {
                this.ip = ip;
                this.port = port;
            }
           public string ip { get; set; }
           public int port { get; set; }
        }
        private async void Try_Connect_to_Socket(object sender, RoutedEventArgs e)
        {
            //127.0.0.1
            //8080

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ip = ip_tb.Text;
            string port_str = port_tb.Text;
            bool is_correct_input = true;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0"),0);
            try
            {
                 endPoint =  new IPEndPoint(IPAddress.Parse(ip_tb.Text), int.Parse(port_str));
            }
            catch { MessageBox.Show("Incorrect input"); is_correct_input = false; }
            if (is_correct_input) 
            { 
            try { await socket.ConnectAsync(endPoint); }  
                catch
                {
                    MessageBox.Show("Connection error");
                }
                if (socket.Connected)
                {
                    endpoint_json endpoint_json = new endpoint_json(ip, int.Parse(port_str));
                    string endPoint_json = JsonConvert.SerializeObject(endpoint_json);
                    if (!File.Exists("endpoint.json"))
                        File.Create("endpoint.json");
                    File.WriteAllText("endpoint.json", endPoint_json);
                    ip_tb.Text = "";
                    port_tb.Text = "";
                    MessageBox.Show("Connected");
                    connected_ip_lb.Content = ip;
                    connected_port_lb.Content = port_str;
                    socket.Disconnect(true);

            }

            }

        }

    }

    
}
