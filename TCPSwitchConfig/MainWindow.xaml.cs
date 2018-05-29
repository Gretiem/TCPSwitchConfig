﻿using System;
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
using System.IO;

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
        public string strCommandGetSoftwareVersion = "sh ver | i Soft\r\n";
        public int intTelnetPort = 23;

        public string strFactoryRouterIOSBin = "isr4400-universalk9.03.16.04b.S.155-3.S4b-ext.SPA.bin";
        public string strFactorySwitchIOSBin = "cat3k_caa-universalk9.16.03.05b.SPA.bin";
        public int intFactoryRouterIOSBytes = 491266116;
        public int intFactorySwitchIOSBytes = 539623424;

        public string strProjectRouterIOSBin = "isr4400-universalk9.03.16.06.S.155-3.S6-ext.SPA.bin";
        public string strProjectSwitchIOSBin = "cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin";
        public int intProjectRouterIOSBytes = 494560068;
        public int intProjectSwitchIOSBytes = 303772864;

        

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

        private void btnBCSCheckFile_Click(object sender, RoutedEventArgs e)
        {
            BCSCheckFlash(tbBCSIpAddress.Text);
        }

        private void btnBCSConfigTest_Click(object sender, RoutedEventArgs e)
        {
            //DownloadCorrectIOS("192.168.144.4");
            ConfigureDevice("192.168.144.4", "3650", "s-upc-3.s02020.us");
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
            string[] separators = { " " };
            //string[] separators = { ",", ".", "!", "?", ";", ":", " " };
            string[] tempArray = strComdindSample.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string[] words = strComdindSample.Split(separators, StringSplitOptions.RemoveEmptyEntries); ;
            int bytesline = 0;

            string strFileNameVlan = "vlan.dat";
            string strFileNamePackages = "packages.conf";
            int intFileSizeVlan = 2356;
            int intFileSizePackages = 1236;


            foreach (var word in words)
            {
                string x = word.Replace("\n", "");
                //Console.WriteLine(word);
                

                if (word.Contains(".conf") || word.Contains(".dat"))
                {
                    string strCheckFilePackages = word.Replace(Environment.NewLine, "");
                    string strCheckFileVlan = word.Replace(Environment.NewLine, "");
                    int intIndexPositionOfBytes = 0;


                    if (strCheckFilePackages == strFileNamePackages)
                    {
                        Console.WriteLine("Matching File Located");

                        intIndexPositionOfBytes = bytesline - 6;
                        int intCurrentFileBytes = Convert.ToInt32(words[intIndexPositionOfBytes]);

                        if (intCurrentFileBytes == intFileSizePackages)
                        {
                            Console.WriteLine("File size matches what it needs to.");
                        }
                        else
                        {
                            Console.WriteLine("File incorrect size.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Matching File Located"); ;
                    }

                    if (strCheckFileVlan == strFileNameVlan)
                    {
                        Console.WriteLine("Matching File Located");

                        intIndexPositionOfBytes = bytesline - 6;
                        int intCurrentFileBytes = Convert.ToInt32(words[intIndexPositionOfBytes]);

                        if (intCurrentFileBytes == intFileSizeVlan)
                        {
                            Console.WriteLine("File size matches what it needs to.");
                        }
                        else
                        {
                            Console.WriteLine("File incorrect size.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Matching File Located"); ;
                    }


                    Console.WriteLine("File: " + word + "Bytes: " + words[intIndexPositionOfBytes] + Environment.NewLine);

                }
                
                bytesline++;
                
            }
            }


             
             */




            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;
        }

        #region New BCS Async Code

        private async void ConfigureDevice(string IpAddress, string Model, string RunningConfig)
        {
            string strIpAddress = IpAddress;
            string strModel = Model;
            string strRunningConfig = RunningConfig;
            bool correctIOSInstalled = false;

            string strFileName = "";
            int intFileSize = 0;

            if (strModel == "4451")
            {

            }
            else if (strModel == "3850")
            {
                strFileName = "cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin";
                intFileSize = 303772864;
            }
            else if (strModel == "3650")
            {
                strFileName = "cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin";
                intFileSize = 303772864;

                await Task.Run(async () =>
                {
                    correctIOSInstalled = await IOS3650Switch(strIpAddress);
                });
                //ping - check
                //check soft first
                //delete old flash
                //verify deleted
                //download new IOS
                //verify download
                //verify install


            }
            else
            {
                MessageBox.Show("Something didn't work right");
            }

            MessageBox.Show("switch method done, IOS configured correctly:" + correctIOSInstalled.ToString());
            
        }

        private async Task<bool> IOS3650Switch(string IpAddress)
        {
            string strIpAddress = IpAddress;
            bool pingable = false;
            bool correctIOSInstalled = false;
            string output = "";

            await Task.Run(async () =>
            {
                pingable = await pingDevice(strIpAddress);
            });

            if(pingable)
            {                
                //Sends switch command to check for current software version
                await Task.Run(async () =>
                {
                    output = await BCSCheckCurrentSwitchFlash(strIpAddress);
                });

                //if loop to check if output from BCSCheckCurrentFlash matches the correct IOS
                if (output == "03.06.06E")
                {
                    MessageBox.Show("confirmed version match");
                    correctIOSInstalled = true;
                    return correctIOSInstalled;
                }
                else
                {
                    //Insert method to redo flash
                }

                MessageBox.Show(output);
            }
            else
            {
                MessageBox.Show("Pingable:" + pingable.ToString());
            }

            // Will tell parent method if true or false
            return correctIOSInstalled;
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

        private async Task<string> BCSCheckCurrentSwitchFlash(string IpAddress)
        {
            string strTargetIP = IpAddress;
            string output = "";

            await Task.Run(async () =>
            {
                output = await SendCommand(strTargetIP, strCommandGetSoftwareVersion);
            });

            string[] separators = { " " };
            string[] words = output.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int bytesline = 0;
            

            //IOS Bin for each file
            //sh ver | i soft - get the flash version
            //sh flash: - learn if packages.conf or .bin





            foreach (var word in words)
            {

                if (word.Contains("Version"))
                {
                    string strPossibleMatch = word.Replace(Environment.NewLine, "");
                    int intIndexPositionOfTargetInArrary = 0;

                    if (strPossibleMatch == "Version")
                    {
                        //MessageBox.Show("Matching String Located");
                        intIndexPositionOfTargetInArrary = bytesline + 1;
                        output = words[intIndexPositionOfTargetInArrary];
                        //MessageBox.Show(words[intIndexPositionOfTargetInArrary]);

                    }
                    else
                    {
                        //MessageBox.Show("No Matching File Located");
                    }
                }
                bytesline++;

            }
            return output;
            
        }

        private async void BCSDownloadCorrectIOS(string IpAddress)
        {
            bool pingable = false;
            string strTargetIP = IpAddress;

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

                    Boolean bolDownloadComplete = false;
                    int intDownloadTimeOut = 0;

                    //string strCmd = "copy flash:cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin flash:bat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin \r\n\r\n";
                    string strCmd = "copy tftp://192.168.144.253/Public/IOS/cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin flash: \r\n\r\n";
                    sendBytes = Encoding.UTF8.GetBytes(strCommandLogin + strCmd);
                    netStream.Write(sendBytes, 0, sendBytes.Length);
                    bytes = new byte[10000];
                    
                    while (!bolDownloadComplete)
                    {
                        System.Threading.Thread.Sleep(10000);

                        await Task.Run(async () =>
                        {
                            bolDownloadComplete = await BCSGetDownloadStatus(strTargetIP, "cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin", 303772864);
                        });
                    }

                    if (bolDownloadComplete)
                    {
                        tcpClient.Close();
                        netStream.Close();
                    }
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


            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;
        }

        private async Task<bool> BCSGetDownloadStatus(string ipAddress, string fileName, int bytesize)
        {
            bool IsDownloadComplete = false;


            string strTargetIP = "192.168.144.4";
            string output = "";
            string TargetFileName = fileName;
            int TargetByteSize = bytesize;


            await Task.Run(async () =>
            {
                output = await SendCommand(strTargetIP, strCheckFile);
            });

            string[] separators = { " ", "\n", "\r" };

            string[] words = output.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int bytesline = 0;





            foreach (var word in words)
            {                
                if (word.Contains("cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin"))
                {                    
                    string strCheckFilePackages = word;
                    int intIndexPositionOfBytes = 0;
                    //MessageBox.Show(strCheckFilePackages);

                    if (strCheckFilePackages == TargetFileName)
                    {
                        intIndexPositionOfBytes = bytesline - 6;
                        int intCurrentFileBytes = Convert.ToInt32(words[intIndexPositionOfBytes]);
                        //MessageBox.Show(intCurrentFileBytes.ToString());
                        if (intCurrentFileBytes == TargetByteSize)
                        {
                            IsDownloadComplete = true;
                            MessageBox.Show("file downloaded");
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        MessageBox.Show("No Matching File Located"); ;
                    }
                }
                bytesline++;
            }            
            return IsDownloadComplete;
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

        #endregion New BCS Async Code

        private async void BCSCheckFile(string IpAddress, string SearchParam)
        {
            string strTargetIP = IpAddress;
            string output = "";

            await Task.Run(async () =>
            {
                output = await SendCommand(strTargetIP, strCheckFile);
            });

            string[] separators = { " " };
            //string[] separators = { ",", ".", "!", "?", ";", ":", " " };
            
            string[] words = output.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int bytesline = 0;

            string strFileNameVlan = "vlan.dat";
            string strFileNamePackages = "packages.conf";
            int intFileSizeVlan = 2356;
            int intFileSizePackages = 1236;


            foreach (var word in words)
            {
                string x = word.Replace("\n", "");
                //Console.WriteLine(word);


                if (word.Contains(".conf") || word.Contains(".dat"))
                {
                    string strCheckFilePackages = word.Replace(Environment.NewLine, "");
                    string strCheckFileVlan = word.Replace(Environment.NewLine, "");
                    int intIndexPositionOfBytes = 0;


                    if (strCheckFilePackages == strFileNamePackages)
                    {
                        MessageBox.Show("Matching File Located");

                        intIndexPositionOfBytes = bytesline - 6;
                        int intCurrentFileBytes = Convert.ToInt32(words[intIndexPositionOfBytes]);

                        if (intCurrentFileBytes == intFileSizePackages)
                        {
                            MessageBox.Show("File size matches what it needs to.");
                        }
                        else
                        {
                            MessageBox.Show("File incorrect size.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Matching File Located"); ;
                    }

                    if (strCheckFileVlan == strFileNameVlan)
                    {
                        MessageBox.Show("Matching File Located");

                        intIndexPositionOfBytes = bytesline - 6;
                        int intCurrentFileBytes = Convert.ToInt32(words[intIndexPositionOfBytes]);

                        if (intCurrentFileBytes == intFileSizeVlan)
                        {
                            MessageBox.Show("File size matches what it needs to.");
                        }
                        else
                        {
                            MessageBox.Show("File incorrect size.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Matching File Located"); ;
                    }


                    MessageBox.Show("File: " + word + "Bytes: " + words[intIndexPositionOfBytes] + Environment.NewLine);

                }

                bytesline++;

            }

            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;

        }

        private async void BCSCheckFlash(string IpAddress)
        {
            string strTargetIP = IpAddress;
            string output = "";

            await Task.Run(async () =>
            {
                output = await SendCommand(strTargetIP, strCommandGetSoftwareVersion);
            });

            string[] separators = { " " };
            string[] words = output.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int bytesline = 0;

            //IOS Bin for each file
            //sh ver | i soft - get the flash version
            //sh flash: - learn if packages.conf or .bin


           


            foreach (var word in words)
            {
                
                if (word.Contains("Version"))
                {
                    string strPossibleMatch = word.Replace(Environment.NewLine, "");                    
                    int intIndexPositionOfTargetInArrary = 0;

                    if (strPossibleMatch == "Version")
                    {
                        MessageBox.Show("Matching String Located");      
                        intIndexPositionOfTargetInArrary = bytesline + 1;
                        MessageBox.Show(words[intIndexPositionOfTargetInArrary]);                         
                       
                    }
                    else
                    {
                        MessageBox.Show("No Matching File Located"); ;
                    }
                }          
                bytesline++;

            }
            
            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;
        }       

        private async Task<bool> GetDownloadStatus(string ipAddress, string fileName, int bytesize)
        {
            bool IsDownloadComplete = false;


            string strTargetIP = "192.168.144.4";
            string output = "";
            string TargetFileName = fileName;
            int TargetByteSize = bytesize;


            await Task.Run(async () =>
            {
                output = await SendCommand(strTargetIP, strCheckFile);
            });

            string[] separators = { " ", "\n", "\r" };
            //string[] separators = { ",", ".", "!", "?", ";", ":", " " };
            
            string[] words = output.Split(separators, StringSplitOptions.RemoveEmptyEntries); 
            int bytesline = 0;

            
            


            foreach (var word in words)
            {
                //string x = word.Replace("\n", "");
                //Console.WriteLine(word);
                


                if (word.Contains("cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin"))
                {
                    //string strCheckFilePackages = word.Replace(Environment.NewLine, "");

                    //string strCheckFilePackages = word.Replace(Environment.NewLine, "");
                    string strCheckFilePackages = word;
                    int intIndexPositionOfBytes = 0;

                   //MessageBox.Show(strCheckFilePackages);

                    if (strCheckFilePackages == TargetFileName)
                    {
                        

                        intIndexPositionOfBytes = bytesline - 6;
                        int intCurrentFileBytes = Convert.ToInt32(words[intIndexPositionOfBytes]);
                        //MessageBox.Show(intCurrentFileBytes.ToString());
                        if (intCurrentFileBytes == TargetByteSize)
                        {
                            IsDownloadComplete = true;
                            MessageBox.Show("file downloaded");
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        MessageBox.Show("No Matching File Located"); ;
                    }  
                }
                bytesline++;

            }






            return IsDownloadComplete;
        }

        private async void DownloadCorrectIOS(string ipAddress)
        {
            bool pingable = false;
            string strTargetIP = ipAddress;
            
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

                    //need
                    //timeout, while loop, 
                    bool bolDownloading = false;
                    int intDownloadTimeOut = 0;

                    //string strCmd = "copy flash:cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin flash:bat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin \r\n\r\n";
                    string strCmd = "copy tftp://192.168.144.253/Public/IOS/cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin flash: \r\n\r\n";
                    sendBytes = Encoding.UTF8.GetBytes(strCommandLogin + strCmd);
                    netStream.Write(sendBytes, 0, sendBytes.Length);
                    bytes = new byte[10000];

                    


                    /*
                    string[] separators = { " ", ")" };
                    string[] words = output.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    int len = words.Length;
                    MessageBox.Show(words[len -1].ToString());
                    */

                    //netStream.Read(bytes, 0, 10000);                        
                    //output = Encoding.UTF8.GetString(bytes);
                    

                    Boolean IsaySO = false;

                    while(!IsaySO)
                    { 
                        System.Threading.Thread.Sleep(10000);

                        await Task.Run(async () =>
                        {
                            IsaySO = await GetDownloadStatus(strTargetIP, "cat3k_caa-universalk9.SPA.03.06.06.E.152-2.E6.bin", 303772864);
                        });
                    }

                    if (IsaySO)
                    {
                        tcpClient.Close();
                        netStream.Close();
                    }
                    
                    
                    

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


            tbOutPut.Text = string.Empty;
            tbOutPut.Text = output;

        }
        
        

        #endregion End Async

        #endregion End Methods


    }
}
