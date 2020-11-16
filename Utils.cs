using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace RSIST950
{
    public static class Utils
    {
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
                CycleSecond = 12,
                ProgramNumber = 13,
                StageNumber = 14,
                Phases = new List<int>()
                        {
                            1, 0, 1, 1, 0, 0, 0
                        },
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

        public static string ConvertHexaToBinary(string hexaValue)
        {
            return Convert.ToString(Convert.ToInt32(hexaValue, 16), 2).PadLeft(8, '0');
        }
    }
}
