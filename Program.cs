using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;




namespace RSIST950
{
    class Program
    {
        public delegate void EventHandler();
        static bool _continue;
        //static SerialPort _serialPort;

        // private static SerialPort port = new SerialPort("COM3", 19200, Parity.None, 8, StopBits.One);

        public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(5000);
        private bool cancel { get; set; } = false;

        //private IChannel _c;
        // private AutoResetEvent _messageReceived;

        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        static AutoResetEvent autoReset = new AutoResetEvent(false);

        static object _syncRoot = new object();

        static SerialPort port;


        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<RSIST950Worker>();
                });

       // static  async Task Main(string[] args)
       // {

       //     IConfiguration config = new ConfigurationBuilder()
       //    .AddJsonFile("appsettings.json", true, true)
       //    .Build();
       //     var SerialPortConfig = config.GetSection("SerialPortConfig");
       //     //var port = ";";


       //     StringBuilder sb = new StringBuilder();

       //     string name;
       //     string message;
       //     StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
       //     /*
       //     port  = new SerialPort(SerialPortConfig["Port"], 19200, Parity.None, 8, StopBits.One);

       //     port.ReadTimeout = 200;
       //         port.WriteTimeout = 200;
       //         port.RtsEnable = true;
       //         port.DtrEnable = true;
       //     port.Open();

       //     byte[] request = new byte[] { 0x0D }; 
       //     TimeSpan timeout = new TimeSpan(0, 0, 0, 0, 250);

       //     var sendTask = Task.Run(() => SendMessage(request, timeout));
       //     try
       //     {
       //         await await Task.WhenAny(sendTask, Task.Delay(timeout));
       //     }
       //     catch (TaskCanceledException)
       //     {
       //         throw new TimeoutException();
       //     }
       //     byte[] response = await sendTask;


       //     var requestUDP = new byte[] { 0x0d, 0x0a, 0x55, 0x44, 0x50, 0x3d, 0x31, 0x39, 0x32, 0x0D };

       //     var sendTask2 = Task.Run(() => SendMessage(requestUDP, timeout));
       //     try
       //     {
       //         await await Task.WhenAny(sendTask2, Task.Delay(timeout));
       //     }
       //     catch (TaskCanceledException)
       //     {
       //         throw new TimeoutException();
       //     }
       //     byte[] response2 = await sendTask2;


       //     var requestStatud = new byte[] { 0xBB, 0x07, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x0D };

       //     var sendTask3 = Task.Run(() => SendMessage(requestStatud, timeout));
       //     try
       //     {
       //         await await Task.WhenAny(sendTask3, Task.Delay(timeout));
       //     }
       //     catch (TaskCanceledException)
       //     {
       //         throw new TimeoutException();
       //     }
       //     byte[] response3 = await sendTask3;

       //     _continue = true;

       //     Console.WriteLine("response1 =" + BitConverter.ToString(response));
       //     Console.WriteLine("response2 =" + BitConverter.ToString(response2));
       //     Console.WriteLine("response3 =" + BitConverter.ToString(response3));
       //   //  port.Close();

       //     */

       //     var str = "bb 3c 00 05 00 00 00 07 10 00 00 00 00 00 00 00 00 00 00 18 63 e2 03 3a 00 00 00 80 00 00 00 c5 00"
       //         + " 00 00 c5 00 00 00 00 01 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 02 09 00 00 02 00 09 d9 03";


       //     var entity = Utils.GetData(str);

       //     using (var ms = Utils.ConvertToXML(entity))
       //     {
       //         ms.Position = 0;
       //         using (var sr = new StreamReader(ms))
       //         {
       //             var xml = sr.ReadToEnd();
       //             Console.WriteLine("xml = " + xml);
       //         }
       //     }


       //     Console.WriteLine("Press enter to close...");
       //     Console.ReadLine();

            
       //     return;
       //     // Console.WriteLine("Incoming Data:");

       //     //     port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
       //     // Begin communications 
       ////     port.Open();
       //     // Enter an application loop to keep this thread alive 

       //    // var mre = new AutoResetEvent(false);
       //    // var buffer = new StringBuilder();

       //    // port.DataReceived += (s, e) =>
       //    // {
       //    //     SerialPort port1 = (SerialPort)s;
       //    //     sb.Append("port =" + port1.BytesToRead.ToString() + "**" + port1.ReadExisting());
       //    //     buffer.Append(port.ReadExisting());
       //    //     if (buffer.ToString().IndexOf("\r\n") >= 0)
       //    //     {
       //    //         Console.WriteLine("Got response: {0}", buffer);

       //    //         mre.Set(); //allow loop to continue
       //    //             buffer.Clear();
       //    //     }
       //    //     else if (buffer.ToString().IndexOf("\r") >= 0)
       //    //     {
       //    //         Console.WriteLine("Got response: {0}", buffer);

       //    //         mre.Set(); //allow loop to continue
       //    //         buffer.Clear();
       //    //     }
       //    //     else if (buffer.ToString().Length > 0)
       //    //     {
       //    //         Console.WriteLine("Got response: {0}", buffer);

                    
                   

       //    //         int bytes = port.BytesToRead;
       //    //         //create a byte array to hold the awaiting data
       //    //         byte[] comBuffer = new byte[bytes];
       //    //         //read the data and store it
       //    //         port.Read(comBuffer, 0, bytes);
       //    //         Console.WriteLine("Got response 22 : {0}", comBuffer);

       //    //         mre.Set(); //allow loop to continue
       //    //         buffer.Clear();
       //    //         var t = 0;
       //    //     }
       //    // };
       //    // Thread.Sleep(1000);
       //    // //      var command = new byte[] { 0x0D };
       //    // //     port.Write(command, 0, command.Length);
       //    // //      Thread.Sleep(200);
       //    // //       port.Write(command, 0, command.Length);
       //    // //Thread.Sleep(1000);
       //    // //port.Write(command, 0, command.Length);
       //    // //Thread.Sleep(1000);
       //    // //port.Write(command, 0, command.Length);
       //    // //Thread.Sleep(1000);
       //    // //port.Write(command, 0, command.Length);
       //    // //Thread.Sleep(1000);
       //    // //     }
       //    // // port.BaseStream.Flush();

       //    // byte[][] commandsToSend = new byte[2][]
       //    //{
       //    //      new byte[] { 0x0D },
       //    //     // new byte[] { 0x0D },
       //    //      new byte[] { 0x0d, 0x0a, 0x55, 0x44, 0x50, 0x3d, 0x31, 0x39, 0x32, 0x0D},
       //    //     // new byte[] { 0xBB, 0x07, 0x00 ,0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x0D }

       //    // };
       //    // //
       //    // ////////////////////////
       //    // ///



       //    // //var commandsToSend = new string[] { "AT", "AT", "AT+CSQ" };
       //    // var responseTimeout = TimeSpan.FromSeconds(1);

       //    // foreach (var command in commandsToSend)
       //    // {
       //    //     try
       //    //     {
       //    //         Console.WriteLine("Write '{0}' to {1}", command[0], port.PortName);
       //    //         //port.WriteLine(command);
       //    //          port.Write(command, 0, command.Length);
       //    //         //  port.BaseStream.Flush();

       //    //         Console.WriteLine("Waiting for response...");

       //    //         //this is where we block
       //    //         if (!mre.WaitOne(responseTimeout))
       //    //         {
       //    //             Console.WriteLine("Did not receive response");
       //    //             //do something
       //    //         }
       //    //     }
       //    //     catch (TimeoutException)
       //    //     {
       //    //         Console.WriteLine("Write took longer than expected");
       //    //     }
       //    //     catch
       //    //     {
       //    //         Console.WriteLine("Failed to write to port");
       //    //     }
       //    //     //////////////////////////
       //    // }

       //     /*
       //     byte[] bytesToSendEnter = { 0x0D };
       //     port.Write(bytesToSendEnter, 0, bytesToSendEnter.Length);
       //     port.BaseStream.Flush();

       //     //autoResetEvent.WaitOne(100);

       //     byte[] bytesToSendUDP = { 0x0d, 0x0a, 0x55, 0x44, 0x50, 0x3d, 0x31, 0x39, 0x32, 0x0D };
       //     port.Write(bytesToSendEnter, 0, bytesToSendEnter.Length);
       //     port.BaseStream.Flush();
       //     */

       //     //    port.Write(bytestosend, 0, bytestosend.Length);
       //     //  port.Write(bytestosend, 0, bytestosend.Length);

       //     //while (_continue)
       //     //{
       //     //    message = Console.ReadLine();

       //     //    if (stringComparer.Equals("q", message))
       //     //    {
       //     //        _continue = false;
       //     //    }
       //     //    else
       //     //    {
       //     //        byte[] bytestosend = { 0x0D };


       //     //        _serialPort.Write(bytestosend, 0, bytestosend.Length);
       //     //    }
       //     //}

       //     //     readThread.Join();

       //     //var et = new byte[] { 0xBB, 0x07, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x0D };
       //     //port.Write(et, 0, et.Length);
       //     //port.BaseStream.Flush();

       //     /*

       //     var t = Utils.ConvertHexaToBinary("7");


       //     Entity entity = Utils.GetData();



       //     using (var ms = Utils.ConvertToXML(entity))
       //     {
       //         ms.Position = 0;
       //         using (var sr = new StreamReader(ms))
       //         {
       //             var xml = sr.ReadToEnd();
       //             Console.WriteLine("xml = " + xml);
       //         }
       //     }
       //        */
       //     //  port.Close();

       //     Thread.Sleep(1000);

       //     Console.WriteLine("sb..."  + sb);

       //    // Console.WriteLine("Press enter to close...");
       //    //Console.ReadLine();
           
           
       // }
        static byte[] SendMessage(byte[] message, TimeSpan timeout)
        {
            // Use stopwatch to update SerialPort.ReadTimeout and SerialPort.WriteTimeout 
            // as we go.
            var stopwatch = Stopwatch.StartNew();

            // Organize critical section for logical operations using some standard .NET tool.
            lock (_syncRoot)
            {
                var originalWriteTimeout = port.WriteTimeout;
                var originalReadTimeout = port.ReadTimeout;
                try
                {
                    // Start logical request.
                    port.WriteTimeout = (int)Math.Max((timeout - stopwatch.Elapsed).TotalMilliseconds, 0);
                    port.Write(message, 0, message.Length);

                    // Expected response length. Look for the constant value from 
                    // the device communication protocol specification or extract 
                    // from the response header (first response bytes) if there is 
                    // any specified in the protocol.
                    int count = port.BytesToRead;
                    byte[] buffer = new byte[count];
                    int offset = 0;
                    // Loop until we recieve a full response.
                    while (count > 0)
                    {
                        port.ReadTimeout = (int)Math.Max((timeout - stopwatch.Elapsed).TotalMilliseconds, 0);
                        var readCount = port.Read(buffer, offset, count);
                        offset += readCount;
                        count -= readCount;
                    }
                    return buffer;
                }
                finally
                {
                    // Restore SerialPort state.
                    port.ReadTimeout = originalReadTimeout;
                    port.WriteTimeout = originalWriteTimeout;
                }
            }
        }
        private static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            // Show all the incoming data in the port's buffer
            Console.WriteLine(port.ReadExisting());
        }
        static void AutoResetMethod()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\t\t***************** AutoResetEvent ***************");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\t\tRunning Thread: {0}", Thread.CurrentThread.Name);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Executing Task1......");
            Console.WriteLine("Press Enter to Pause Task1..");
            //Wait for first task:  
          //  autoReset.WaitOne();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\t\tRunning Thread: {0}", Thread.CurrentThread.Name);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n===================================");
            Console.WriteLine("Executing Task1.....");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Task1 executing.....");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Execution of Task1 completed..");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine("===================================");
            Console.WriteLine("Executing Task2......");
            Console.WriteLine("Press Enter to Pause Task2..");
            //Wait for second task:  
       //     autoReset.WaitOne();
       //     Console.ForegroundColor = ConsoleColor.Magenta;
       //     Console.WriteLine("\t\t\tRunning Thread: {0}", Thread.CurrentThread.Name);
       //     Console.ForegroundColor = ConsoleColor.Yellow;
       //     Console.WriteLine("Execution of Task2 completed...");
       //     Console.WriteLine("===================================");
            //Console.ForegroundColor = prevColor;
        }

        private static void Worker()
        {
            //while (!cancel)
            //{
            //    Console.WriteLine("Worker is working");
            //    Thread.Sleep(1000);
            //}
            Console.WriteLine("Worker thread ending");
        }

        //private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        //{
        //    SerialPort port = (SerialPort)sender;
        //    string indata = port.ReadExisting();
        //    Console.WriteLine("Data received");
        //    Console.WriteLine(indata);
        //    Console.ReadKey();
        //}

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                   // string message = _serialPort.ReadExisting();
                   // Console.WriteLine(message);
                  //  Console.WriteLine(" Read()");
                }
                catch (TimeoutException) { }
            }
        }

        public string Send(string str)
        {
            // this._c.Send(m);
            // wait for MaxWaitInMs to get an event from _c.MessageReceived
            // return the message or null if no message was received in response


            // This will wait for up to 5000 ms, then throw an exception.
            //_messageReceived.WaitOne(MaxWait);

            return null;
        }

    }
}
