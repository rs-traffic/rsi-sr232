using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO.Ports;
using System.Diagnostics;

namespace RSIST950
{
    public static class Utils
    {
        static SerialPort port;
        static object _syncRoot = new object();
        public static async Task<string> GetDataFromControler()
        {
            IConfiguration config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", true, true)
           .Build();
            var SerialPortConfig = config.GetSection("SerialPortConfig");

            port = new SerialPort(SerialPortConfig["Port"], 19200, Parity.None, 8, StopBits.One);

                 port.ReadTimeout = 200;
                     port.WriteTimeout = 200;
                     port.RtsEnable = true;
                     port.DtrEnable = true;
                 port.Open();

                 byte[] request = new byte[] { 0x0D }; 
                 TimeSpan timeout = new TimeSpan(0, 0, 0, 0, 250);

                 var sendTask = Task.Run(() => SendMessage(request, timeout));
                 try
                 {
                     await await Task.WhenAny(sendTask, Task.Delay(timeout));
                 }
                 catch (TaskCanceledException)
                 {
                     throw new TimeoutException();
                 }
                 byte[] response = await sendTask;


                 var requestUDP = new byte[] { 0x0d, 0x0a, 0x55, 0x44, 0x50, 0x3d, 0x31, 0x39, 0x32, 0x0D };

                 var sendTask2 = Task.Run(() => SendMessage(requestUDP, timeout));
                 try
                 {
                     await await Task.WhenAny(sendTask2, Task.Delay(timeout));
                 }
                 catch (TaskCanceledException)
                 {
                     throw new TimeoutException();
                 }
                 byte[] response2 = await sendTask2;


                 var requestStatud = new byte[] { 0xBB, 0x07, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x0D };

                 var sendTask3 = Task.Run(() => SendMessage(requestStatud, timeout));
                 try
                 {
                     await await Task.WhenAny(sendTask3, Task.Delay(timeout));
                 }
                 catch (TaskCanceledException)
                 {
                     throw new TimeoutException();
                 }
                 byte[] response3 = await sendTask3;

            //     _continue = true;

            //     Console.WriteLine("response1 =" + BitConverter.ToString(response));
            //     Console.WriteLine("response2 =" + BitConverter.ToString(response2));
            //     Console.WriteLine("response3 =" + BitConverter.ToString(response3));

            string res = BitConverter.ToString(response) +  BitConverter.ToString(response2)
            + BitConverter.ToString(response3);
            return res;
        }
        public static string GetDataXML( string str)
        {
            string res = string.Empty;

            var entity = Utils.GetData(str);

            using (var ms = Utils.ConvertToXML(entity))
            {
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    var xml = sr.ReadToEnd();
                    Console.WriteLine("xml = " + xml);
                    res = xml;
                }
            }
            return res;
        }
        public static void SendData(string xml)
        {
            byte[] bytes = new byte[1024];

            try
            {
                // Connect to a Remote server  
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = System.Net.IPAddress.Parse("127.0.0.1");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.    
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.    
                try
                {
                    // Connect to Remote EndPoint  
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.    
                    // byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    byte[] msg = Encoding.ASCII.GetBytes(xml);

                    // Send the data through the socket.    
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.    
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.    
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void ExecuteClient()
        {

            
        }
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
        public static MemoryStream ConvertToXML(List<Dictionary<string, string>> lst)
        {


            MemoryStream ms = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("status");
                foreach (var l in lst)
                {
                    writer.WriteStartElement("junction");
                    var dict = l;

                    foreach (var d in dict)
                    {
                        writer.WriteAttributeString(d.Key, d.Value);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            };
            settings = null;

            return ms;

        }
        public static MemoryStream ConvertToXML(Entity entity)
        {
            var props = GetAllProperties(entity);

            StringBuilder builder = new StringBuilder();
            MemoryStream ms = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("status");

                foreach (var prop in props)
                {
                    if(prop.Value.Contains(";"))
                    {
                        WriteFormatData(writer, prop);
                    }
                    else
                    {
                        writer.WriteAttributeString(prop.Key, prop.Value);
                    }
                    
                }
                
                //writer.WriteAttributeString("content", "e");
                writer.WriteEndElement();
                writer.WriteEndDocument();
            };
            settings = null;

            return ms;

        }

        private static void WriteFormatData(XmlWriter writer, KeyValuePair<string, string> prop)
        {
            var values = prop.Value.Split(";");
            string name = prop.Key;
            int i = 1;
            foreach (var val in values)
            {
                writer.WriteAttributeString($"{name}{i}", val);
                i++;
            }
        }

        public static Entity GetData()
        {
            return new Entity
            {
                //CycleSecond = 12,
                ProgramNumber = 13,
                StageNumber = 14,
                //Phases = new List<int>()
                //        {
                //            1, 0, 1, 1, 0, 0, 0
                //        },
                Detectors = new List<int>()
                        {
                            1, 1, 1, 0, 0, 0
                        },
                DL20 = 0,
                Outputs = new List<int>()
                        {
                            1, 1, 1, 0, 0, 0
                        }
            };
        }

        private static Dictionary<string, string> GetProperties1(object obj)
        {
            var props = new Dictionary<string, string>();
            if (obj == null)
                return props;

            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                var val = prop.GetValue(obj, new object[] { });
                var valStr = val == null ? "" : val.ToString();
                props.Add(prop.Name, valStr);
            }

            return props;
        }

        private static Dictionary<string, string> GetAllProperties(object propValue, int level = 0)
        {
            var props = new Dictionary<string, string>();
            if (propValue == null)
                return props;

            var childProps = propValue.GetType().GetProperties();
            foreach (var prop in childProps)
            {
                dynamic value;
                var name = prop.Name;
                if(typeof(IList).IsAssignableFrom(prop.PropertyType))
                {
                    value = string.Join(",", string.Join(";", (List<int>)prop.GetValue(propValue)));
                }
                else
                {
                    value = prop.GetValue(propValue, null);
                }
                
                props.Add(name, value == null ? "" : value.ToString());
            }

            return props;
        }


        public static string ecu(char c)
        {
            return  "f";
        }

        //List<Dictionary<string, string>> lst
        internal static List<Dictionary<string, string>> GetData(string str)
        {
            //var str = "bb 3c 00 05 00 00 00 07 10 00 00 00 00 00 00 00 00 00 00 18 63 e2 03 3a 00 00 00 80 00 00 00 c5 00"
            //   + "00 00 c5 00 00 00 00 01 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 02 09 00 00 02 00 09 d9 03";

            //var dict = FormatResultDictonary(str);
            var arrEntity = str.Split(" ");
            int len = (arrEntity.Length - 2);
            var dest = arrEntity[7..len];

            List<Dictionary<string, string>> lst = new List<Dictionary<string, string>>();

            Dictionary<string, string> dict = new Dictionary<string, string>();


            string CycleSecondSecondJunction = GetStringValueInt(dest[55..56]);  //"09"
            string StageNumberSecondJunction = GetStringValueInt(dest[34..35]);

            dict.Add(Enums.FieldsToXml.CycleSecond.ToString(), GetStringValueInt(dest[50..51]));
            dict.Add(Enums.FieldsToXml.ProgramNumber.ToString(), GetStringValueInt(dest[49..50]));
            dict.Add("StageNumber", GetStringValueInt(dest[33..34]));


            var phases = new string[4][]
            {
                 dest[16..20],
                 dest[20..24],
                 dest[24..28],
                 dest[28..32]
            };

            var phasesXml = BuildPhases(phases[0], phases[1], phases[2], phases[3]);
            foreach (var phase in phasesXml)
            {
                dict.Add(phase.Key, phase.Value);
            }

            //07 10 00 detectors 3*8=24

            var arrDet = GetCharArray(string.Concat(dest[0..3]));// string.Concat(dict.GetValueOrDefault(field.ToString())));
            var detectors = GetDetectors("Detector", arrDet);
            foreach (var det in detectors)
            {
                dict.Add(det.Key, det.Value);
            }

            // 0  1 2  3  4  5
            //07 10 00 00 00 00 00 00 00 00 00 00
            //Port 5,6,7
            //00 00

            //    var values = GetCharArray(string.Concat(dict.GetValueOrDefault(field.ToString())));
            var values = GetCharArray(string.Concat(dest[5..8]));
            int iCount = values.Length; ;
            foreach (Enums.SpecialDetectors sd in Enum.GetValues(typeof(Enums.SpecialDetectors)))
            {
                dict.Add(sd.ToString(), values[iCount - 1].ToString());
            }

            var arrOutputs = GetCharArray(dest[8..9].FirstOrDefault().ToString());
            var outputs = GetDetectors("Output", arrOutputs);
            foreach (var outp in outputs)
            {
                dict.Add(outp.Key, outp.Value);
            }

            lst.Add(dict);
            if (CycleSecondSecondJunction != "0" && StageNumberSecondJunction != "0")
            {
                Dictionary<string, string> dictJunction = new Dictionary<string, string>();

                foreach (var d in dict)
                {
                    if (d.Key == Enums.FieldsToXml.CycleSecond.ToString())
                    {
                        dictJunction.Add(Enums.FieldsToXml.CycleSecond.ToString(), CycleSecondSecondJunction);
                    }
                    else if (d.Key == "StageNumber")
                    {
                        dictJunction.Add(Enums.FieldsToXml.StageNumber.ToString(), StageNumberSecondJunction);
                    }
                    else
                    {
                        dictJunction.Add(d.Key, d.Value);
                    }

                }

                lst.Add(dictJunction);
            }
            //return dict;
            return lst;
        }
        private static char[] GetCharArray(string str)
        {
            var arrBinary = str.AsEnumerable().Select(d => ConvertHexaToBinary(d.ToString()));
            var res = string.Concat(arrBinary);
            return res.ToCharArray();
        }
        private static Dictionary<string, string> GetDetectors(string name,Char[] values)
        {
            var dic = new Dictionary<string, string>();

            int i = 1;
            for(int count = values.Length -1; count >= 0; count --)
            {
                dic.Add($"{name}{i}", values[count].ToString());
                i++;
            }

            return dic;
        }

        private static List<string> GetListValue(string[] value)
        {
            var lst = new List<string>();

            foreach (var val in value)
            {
                var arrBinary = val.AsEnumerable().Select(d => ConvertHexaToBinary(d.ToString()));
                lst.Add(string.Concat(arrBinary));
            }


            return lst;

          //  eeee.ForEach(c => ecu(c));
          //    //    var result = string.Concat(eeee);

            //foreach (var p in dict)
            //{
            //    // var eeee = strin.AsEnumerable().Select(d => ConvertHexaToBinary(d.ToString()));
            //    // eeee.ForEach(c => ecu(c));
            //    //    var result = string.Concat(eeee);

            //    //var result = string.Join(ecu, str.ToCharArray());
            //   // binaryDict.Add(p.Key, p.Value.Select(d => ConvertHexaToBinary(d.ToString())));

            //    if(p.Key == "CycleSecond")
            //    {
            //        binaryList.Add(p.Key, p.Value.First());

            //    }
            //}
        }
        private static string ConvertStringToHexaChar(string str)
        {
            var arrBinary = str.AsEnumerable().Select(d => ConvertHexaToBinary(d.ToString()));
            var res = string.Concat(arrBinary);
            return res;
        }
        private static string GetStringValueInt(string[] value)
        {
            var binaryStr = ConvertStringToHexaChar(value.FirstOrDefault());
            var res = Convert.ToInt32(binaryStr, 2).ToString();
            return res;
        }

        //List<Dictionary<string, string>> lst
        private static List<Dictionary<string, string>> FormatResultDictonary(string str)
        {
            var arrEntity = str.Split(" ");
            int len = (arrEntity.Length - 2);
            var dest = arrEntity[7..len];

            List<Dictionary<string, string>> lst = new List<Dictionary<string, string>>();

            Dictionary<string, string> dict = new Dictionary<string, string>();


            string CycleSecondSecondJunction = GetStringValueInt(dest[55..56]);  //"09"
            string StageNumberSecondJunction =  GetStringValueInt(dest[34..35]);

            dict.Add(Enums.FieldsToXml.CycleSecond.ToString(), GetStringValueInt(dest[50..51]));
            dict.Add(Enums.FieldsToXml.ProgramNumber.ToString(), GetStringValueInt(dest[49..50]));
            dict.Add("StageNumber", GetStringValueInt(dest[33..34]));


            var phases = new string[4][]
            {
                 dest[16..20],
                 dest[20..24],
                 dest[24..28],
                 dest[28..32]
            };

            var phasesXml = BuildPhases(phases[0], phases[1], phases[2], phases[3]);
            foreach (var phase in phasesXml)
            {
                dict.Add(phase.Key, phase.Value);
            }

            //07 10 00 detectors 3*8=24

            var arrDet = GetCharArray(string.Concat(dest[0..3]));// string.Concat(dict.GetValueOrDefault(field.ToString())));
            var detectors = GetDetectors("Detector", arrDet);
            foreach (var det in detectors)
            {
                dict.Add(det.Key, det.Value);
            }

            // 0  1 2  3  4  5
            //07 10 00 00 00 00 00 00 00 00 00 00
            //Port 5,6,7
            //00 00

            //    var values = GetCharArray(string.Concat(dict.GetValueOrDefault(field.ToString())));
            var values = GetCharArray(string.Concat(dest[5..8]));
            int iCount = values.Length; ;
            foreach (Enums.SpecialDetectors sd in Enum.GetValues(typeof(Enums.SpecialDetectors)))
            {
                dict.Add(sd.ToString(), values[iCount - 1].ToString());
            }

            var arrOutputs = GetCharArray(dest[8..9].FirstOrDefault().ToString());
            var outputs = GetDetectors("Output", arrOutputs);
            foreach (var outp in outputs)
            {
                dict.Add(outp.Key, outp.Value);
            }

            lst.Add(dict);
            if (CycleSecondSecondJunction != "0" && StageNumberSecondJunction != "0")
            {
                Dictionary<string, string> dictJunction = new Dictionary<string, string>();

                foreach(var d in dict)
                {
                   if(d.Key == Enums.FieldsToXml.CycleSecond.ToString())
                    {
                        dictJunction.Add(Enums.FieldsToXml.CycleSecond.ToString(), CycleSecondSecondJunction);
                    }
                    else if (d.Key == "StageNumber")
                    {
                        dictJunction.Add(Enums.FieldsToXml.StageNumber.ToString(), StageNumberSecondJunction);
                    }
                    else 
                    {
                        dictJunction.Add(d.Key, d.Value);
                    }

                }

                lst.Add(dictJunction);
            }

            
          //  dict.Add("CycleSecondSecondJunction", dest[55..56]);
            var t = "ggg";

           // break;

            //  dict.Add("Detectors", dest[0..1]);

            //  dict.Add("SpecialDetectors", dest[5..6]);
            //  dict.Add("Outputs", dest[8..9]);



            //  //12 
            //  dict.Add("ports", dest[0..12]);
            //  dict.Add("maps", dest[12..14]);
            ////  dict.Add("CycleSecond", dest[14..15]);
            //  dict.Add("maps2", dest[15..16]);


            //dict.Add("Red", dest[16..20]);
            //dict.Add("Yellow", dest[20..24]);
            //dict.Add("Green", dest[24..28]);
            //dict.Add("FlashingGreen", dest[28..32]);

          //  string[] red = dest[16..20];


            // byte[][] commandsToSend = new byte[2][]
            //{
            //      new byte[] { 0x0D },
            //     // new byte[] { 0x0D },
            //      new byte[] { 0x0d, 0x0a, 0x55, 0x44, 0x50, 0x3d, 0x31, 0x39, 0x32, 0x0D},
            //     // new byte[] { 0xBB, 0x07, 0x00 ,0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x0D }




            // 

            //dict.Add("maps7", dest[32..33]);
            //dict.Add("maps8", dest[33..40]);
            //dict.Add("maps9", dest[40..47]);




            //dict.Add("a1", dest[49..51]);
            //dict.Add("a2", dest[51..52]);
            //dict.Add("CycleSecondSecondJunction", dest[55..56]);


            // return dict;
            return lst;
        }

        private static Dictionary<string, string> BuildPhases(string[] arr1, string[] arr2, string[] arr3, string[] arr4)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int phaseNumNext;
            int phaseNum = 1;

            for (int  i = arr1.Length - 1; i >=0; i--)
            {
                var temp = GetPhases(arr1[i].ToString(), arr2[i].ToString(), arr3[i].ToString(), arr4[i].ToString(), phaseNum, out phaseNumNext);

                if (phaseNumNext > phaseNum) phaseNum = phaseNumNext;
                foreach (var d in temp )
                {
                    dic.Add(d.Key, d.Value);
                }
            }
            
            return dic;
        }

        public static Dictionary<string, string> GetPhases(string str1, string str2, string str3, string str4,int phaseNum ,out int phaseNumNext)
        {
            string name = "Phase";

            Dictionary<string, string> dic = new Dictionary<string, string>();

            var arrRed = GetCharArray(str1);
            var arrYellow = GetCharArray(str2);
            var arrGreen = GetCharArray(str3);
            var arrFlashingGreen = GetCharArray(str4);

            int i = phaseNum;
            for (int count = arrRed.Length - 1; count >= 0; count--)
            {

                var color = GetColor(string.Concat(arrRed[count], arrYellow[count], arrGreen[count], arrFlashingGreen[count]));
                //  var color = Enum.GetName(typeof(Enums.Color),string.Concat(arrRed[count], arrYellow[count], arrGreen[count], arrFlashingGreen[count]));
                //var colorIndexName = Enum.GetName(typeof(Enums.ColorIndex), color);

                if (color != "ee" && color != "")
                {
                    var colorIndexName = (Enums.ColorIndex)System.Enum.Parse(typeof(Enums.ColorIndex), color);

                    dic.Add($"{name}{i}", ((int)colorIndexName).ToString());
                }else if(color == "")
                {
                    //var colorIndexName = (Enums.ColorIndex)System.Enum.Parse(typeof(Enums.ColorIndex), color);

                    dic.Add($"{name}{i}", "0");
                }

                i++;
            }
            phaseNumNext = i;
            return dic;
        }

        public static string GetColor(string number)
        { 
            string res = string.Empty;
            switch (number)
            {  
                case Enums.Color.Red: res = "Red";
                    break;
                case Enums.Color.RedYellow: res = "RedYellow";
                    break;
                case Enums.Color.Green: res = "Green";
                    break;
                case Enums.Color.Yellow: res = "Yellow";
                    break;
                case Enums.Color.FlashingGreen: res = "FlashingGreen";
                    break;
                case Enums.Color.GreenYellow: res = "GreenYellow";
                    break;
            }
  
            return res;
        }
        public static string ConvertHexaToBinary(string hexaValue)
        {
            return Convert.ToString(Convert.ToInt32(hexaValue, 16), 2).PadLeft(4, '0');
        }


    }
}


//var arrDet = GetCharArray(string.Concat(dict.GetValueOrDefault(field.ToString())));
//var detectors = GetDetectors("Detector", arrDet);
//foreach (var det in detectors)
//{
//    formatedSortedDic.Add(det.Key, det.Value);
//}