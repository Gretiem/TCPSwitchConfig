using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;


namespace TCPSwitchConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }


        #region Variable

        public string strCommandLogin = "1234\r\nen\r\n1234\r\nterm len 0\r\n";
        public string strCommandMakeFile = "copy flash:vlan.dat flash:test.dat\r\n\r\n";
        public string strRemoveFile = "delete /f /r flash:test.dat\r\n\r\n";
        public string strCheckFile = "sh flash:\r\n";
        public int intTelnetPort = 23;
        #endregion End Variable


        #region EventHandler

        private void btnStaticConnection_Click(object sender, RoutedEventArgs e)
        {
            StaticConnectionMethod();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void btnCommand1_Click(object sender, RoutedEventArgs e)
        {
            Command1();
        }

        private void btnCommand2_Click(object sender, RoutedEventArgs e)
        {
            Command2();
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Disconnected");
        }

        private void btnMakeFile_Click(object sender, RoutedEventArgs e)
        {
            MakeFile();
        }

        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            RemoveFile();
        }

        private void btnCheckFile_Click(object sender, RoutedEventArgs e)
        {
            CheckFile();
        }

        #endregion End EventHandler


        #region Methods

        #region Single Thread

        private void StaticConnectionMethod()
        {
            byte[] bytes;
            NetworkStream netStream;
            TcpClient tcpClient;
            Byte[] sendBytes;

            tcpClient = new TcpClient();

            string targetIP = "192.168.0.20";
            int intPingAttempts = 5;

            try
            {
                bool pingable = false;
                bool breakloop = false;
                Ping ping = new Ping();

                while (!pingable && !breakloop)
                {
                    for (int i = 0; i <= intPingAttempts; i++)
                    {
                        string x = ("Attemping connection " + i.ToString() + "of " + intPingAttempts.ToString() + ".");
                        MessageBox.Show(x);


                        //Console.WriteLine("Attemping connection {0} of {1}.", i.ToString(), intPingAttempts);
                        if (i == intPingAttempts)
                        {
                            breakloop = true;
                        }
                        else
                        {
                            try
                            {
                                PingReply reply = ping.Send(targetIP);
                                pingable = reply.Status == IPStatus.Success;
                                if (pingable)
                                {
                                    i = intPingAttempts;
                                }
                            }
                            catch (PingException)
                            {

                            }
                        }
                    }

                    if (pingable)
                    {
                        tcpClient.Connect(targetIP, 23);

                        netStream = tcpClient.GetStream();

                        string login = "1234\r\nen\r\n1234\r\nterm len 0\r\n";
                        string command = "sh int status\r\n";
                        string sh24status = "sh int fa0/24 status\r\n";

                        sendBytes = Encoding.UTF8.GetBytes(login + command);
                        netStream.Write(sendBytes, 0, sendBytes.Length);
                        bytes = new byte[10000];
                        System.Threading.Thread.Sleep(2000);
                        netStream.Read(bytes, 0, 10000);
                        MessageBox.Show(Encoding.UTF8.GetString(bytes));
                        //Console.WriteLine(Encoding.UTF8.GetString(bytes));


                        sendBytes = Encoding.UTF8.GetBytes(sh24status);
                        netStream.Write(sendBytes, 0, sendBytes.Length);
                        bytes = new byte[10000];
                        System.Threading.Thread.Sleep(2000);
                        netStream.Read(bytes, 0, 10000);
                        MessageBox.Show(Encoding.UTF8.GetString(bytes));
                        //Console.WriteLine(Encoding.UTF8.GetString(bytes));






                        tcpClient.Close();
                        netStream.Close();


                        //Console.ReadLine();

                    }
                    else
                    {
                        MessageBox.Show("Connection Failed");
                        //Console.WriteLine("Connection Failed");
                        //Console.ReadLine();
                    }


                }

            }
            catch (Exception)
            {

            }
        }

        #endregion End Single Thread

        #region Async

        private async void Connect()
        {
            bool pingable = false;
            string strTargetIP = "192.168.0.20";

            await Task.Run(async () =>
            {
                pingable = await pingDevice(strTargetIP);
            });

            if(pingable)
            {
                MessageBox.Show(pingable.ToString());

                byte[] bytes;
                NetworkStream netStream;
                TcpClient tcpClient;
                Byte[] sendBytes;

                tcpClient = new TcpClient();

                tcpClient.Connect(strTargetIP, 23);

                netStream = tcpClient.GetStream();

                string login = "1234\r\nen\r\n1234\r\nterm len 0\r\n";
                string command = "sh int status\r\n";
                string sh24status = "sh int fa0/24 status\r\n";

                sendBytes = Encoding.UTF8.GetBytes(login + command);
                netStream.Write(sendBytes, 0, sendBytes.Length);
                bytes = new byte[10000];
                System.Threading.Thread.Sleep(2000);
                netStream.Read(bytes, 0, 10000);
                MessageBox.Show(Encoding.UTF8.GetString(bytes));


                sendBytes = Encoding.UTF8.GetBytes(sh24status);
                netStream.Write(sendBytes, 0, sendBytes.Length);
                bytes = new byte[10000];
                System.Threading.Thread.Sleep(2000);
                netStream.Read(bytes, 0, 10000);
                MessageBox.Show(Encoding.UTF8.GetString(bytes));

                tcpClient.Close();
                netStream.Close();
                
            }
            else
            {
                MessageBox.Show("Connection Failed");
            }

            
        }

        private async void Command1()
        {
            bool pingable = false;
            string strTargetIP = "192.168.0.20";

            await Task.Run(async () =>
            {
                pingable = await pingDevice(strTargetIP);
            });


            if (pingable)
            {
                try
                {
                    byte[] bytes;
                    NetworkStream netStream;
                    TcpClient tcpClient;
                    Byte[] sendBytes;

                    tcpClient = new TcpClient();
                    tcpClient.Connect("192.168.0.20", intTelnetPort);
                    netStream = tcpClient.GetStream();
    
                    string command = "config t\r\ndefault int fa0/1\r\nend\r\nsh int fa0/1 status\r\n";

                    sendBytes = Encoding.UTF8.GetBytes(strCommandLogin + command);
                    netStream.Write(sendBytes, 0, sendBytes.Length);
                    bytes = new byte[10000];
                    System.Threading.Thread.Sleep(2000);
                    netStream.Read(bytes, 0, 10000);
                    tbOutPut.Text = string.Empty;
                    tbOutPut.Text = Encoding.UTF8.GetString(bytes);

                    tcpClient.Close();
                    netStream.Close();
                }
                catch
                {

                }
            }
            else
            {
                MessageBox.Show("Connection Failed");
            }
        }

        private async void Command2()
        {
            string strTargetIP = "192.168.0.20";
            string output = "";
            string command1 = "config t\r\nint fa0/1\r\ndescription Test Port\r\nend\r\nsh int fa0/1 status\r\n";
            string command2 = "sh run\r\n";
            string command3 = "delete /f /r flash:test.dat\r\n\r\n";
            string command4 = "";

            await Task.Run(async () =>
            {
                output = await SendCommand(strTargetIP, command1);
            });

            //MessageBox.Show(output);

            await Task.Run(async () =>
            {
                output += await SendCommand(strTargetIP, command3);
            });

            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;
            //MessageBox.Show(output);

        }

        private async void MakeFile()
        {
            string strTargetIP = "192.168.0.20";
            string output = "";

            await Task.Run(async () =>
            {
                output += await SendCommand(strTargetIP, strCommandMakeFile);
            });

            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;
        }

        private async void RemoveFile()
        {
            string strTargetIP = "192.168.0.20";
            string output = "";

            await Task.Run(async () =>
            {
                output += await SendCommand(strTargetIP, strRemoveFile);
            });

            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;
        }

        private async void CheckFile()
        {
            string strTargetIP = "192.168.0.20";
            string output = "";
            

            await Task.Run(async () =>
            {
                output = await SendCommand(strTargetIP, strCheckFile);
            });


            //trying to come up with a search function
            int last = output.LastIndexOf(".dat");
            string test = output.Substring(last - 10, 20);

            int space = test.IndexOf(" ");
            string next = test.Substring(space);
            MessageBox.Show(next);


            /*
            //Possible solution
            Console.WriteLine(strComdindSample.Contains(".conf"));
            Console.WriteLine(strComdindSample.IndexOf(".conf"));
            
            string[] separators = { " " };
            //string[] separators = { ",", ".", "!", "?", ";", ":", " " };
            string[] words = strComdindSample.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int bytesline = 0;
            foreach (var word in words)
            {
                //Console.WriteLine(word);
                if (word.Contains(".conf") || word.Contains(".dat"))
                {
                    Console.WriteLine("Captured content: " + word);
                    int y = bytesline - 6;
                    Console.WriteLine(words[y]);
                    
                }
                bytesline++;
                
            }


             
             */




            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;
        }

        private async Task<bool> pingDevice(string ipAddress)
        {
            bool pingable = false;
            Ping ping = new Ping();

            int i = 0;
            while (!pingable && i <= 5)
            {                
                try
                {
                    PingReply reply = ping.Send(ipAddress);
                    pingable = reply.Status == IPStatus.Success;
                }
                catch
                {
                    
                }
                i++;
            }

            return pingable;
        }

        private async Task<string> SendCommand(string ipAddress, string command)
        {
            bool pingable = false;
            string strTargetIP = ipAddress;
            string strCommand = command;
            string output = "";


            await Task.Run(async () =>
            {
                pingable = await pingDevice(strTargetIP);
            });

            if (pingable)
            {
                try
                {
                    byte[] bytes;
                    NetworkStream netStream;
                    TcpClient tcpClient;
                    Byte[] sendBytes;

                    tcpClient = new TcpClient();
                    tcpClient.Connect(strTargetIP, intTelnetPort);
                    netStream = tcpClient.GetStream();

                    //string command = "config t\r\nint fa0/1\r\ndescription Test Port\r\nend\r\nsh int fa0/1 status\r\n";


                    sendBytes = Encoding.UTF8.GetBytes(strCommandLogin + strCommand);
                    netStream.Write(sendBytes, 0, sendBytes.Length);
                    bytes = new byte[10000];
                    System.Threading.Thread.Sleep(2000);
                    netStream.Read(bytes, 0, 10000);
                    output = Encoding.UTF8.GetString(bytes);
                    //MessageBox.Show(Encoding.UTF8.GetString(bytes));
                    
                    tcpClient.Close();
                    netStream.Close();
                }
                catch
                {

                }

            }
            else
            {
                MessageBox.Show("Connection Failed");
                output = "Connection Failed";
            }



            return output;
        }

        #endregion End Async

        #endregion End Methods


    }
}
