using System;
using System.Dynamic;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace RSIST950
{
    class Program
    {
        public delegate void EventHandler();
        static bool _continue;
        //static SerialPort _serialPort;

       // private static SerialPort port = new SerialPort("COM3", 19200, Parity.None, 8, StopBits.One);

        static void Main(string[] args)
        {
            string name;
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            //    Thread readThread = new Thread(Read);

            // Create a new SerialPort object with default settings.
            //_serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            //    _serialPort.PortName = SetPortName(_serialPort.PortName);
            //     _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            //    _serialPort.Parity = SetPortParity(_serialPort.Parity);
            //    _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            //     _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            //      _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
            //_serialPort.PortName = "COM3";
            //_serialPort.BaudRate = 19200;
            //_serialPort.Parity = Parity.None;
            //_serialPort.StopBits = StopBits.One;
            //_serialPort.DataBits = 8;
            //_serialPort.RtsEnable = true;
            //_serialPort.DtrEnable = true;
            // _serialPort.Handshake = Handshake.None;

            // Set the read/write timeouts
            

            // _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            SerialPort port = new SerialPort("COM3", 19200, Parity.None, 8, StopBits.One);

            port.ReadTimeout = 200;
            port.WriteTimeout = 200;
            port.RtsEnable = true;
            port.DtrEnable = true;

            // _serialPort.Open();
            _continue = true;
            //    readThread.Start();

            //  Console.Write("Name: ");
            //  name = Console.ReadLine();


            Console.WriteLine("Incoming Data:");
            // Attach a method to be called when there
            // is data waiting in the port's buffer 
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            // Begin communications 
            port.Open();
            // Enter an application loop to keep this thread alive 
            

          //  Console.WriteLine("Type QUIT to exit");
            //_serialPort.Write("13");
            //_serialPort.Write("13");
            //_serialPort.Write("13");
            byte[] bytestosend = { 0x0D };


            port.Write(bytestosend, 0, bytestosend.Length);
            port.BaseStream.Flush();
        //    port.Write(bytestosend, 0, bytestosend.Length);
          //  port.Write(bytestosend, 0, bytestosend.Length);

            //while (_continue)
            //{
            //    message = Console.ReadLine();

            //    if (stringComparer.Equals("q", message))
            //    {
            //        _continue = false;
            //    }
            //    else
            //    {
            //        byte[] bytestosend = { 0x0D };


            //        _serialPort.Write(bytestosend, 0, bytestosend.Length);
            //    }
            //}

            //     readThread.Join();

          //    port.Close();

            /*

            var t = Utils.ConvertHexaToBinary("7");


            Entity entity = Utils.GetData();

            

            using (var ms = Utils.ConvertToXML(entity))
            {
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    var xml = sr.ReadToEnd();
                    Console.WriteLine("xml = " + xml);
                }
            }
               */

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }

        private static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            // Show all the incoming data in the port's buffer
            Console.WriteLine(port.ReadExisting());
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            string indata = port.ReadExisting();
            Console.WriteLine("Data received");
            Console.WriteLine(indata);
            Console.ReadKey();
        }

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

    }
}
