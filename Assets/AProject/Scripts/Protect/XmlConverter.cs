using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;


namespace Cryptograph.Xml
{


    public class XmlConverter
    {

        private string _path;
        private XmlConverter(string path) => _path = path;


        public static XmlConverter Create(string fullPath)
        {
            return new XmlConverter(fullPath);
        }

        private string SerializeObjectToSTR<T>(T objectForDesirialization)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlText = new XmlTextWriter(memoryStream, Encoding.Unicode);
            xmlSerializer.Serialize(xmlText, objectForDesirialization);
            memoryStream = (MemoryStream)xmlText.BaseStream;
            return GetStr(memoryStream.ToArray());
        }


        private object DesirializeObject<T>(string str)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(GetBytes(str));
            return xmlSerializer.Deserialize(memoryStream);
        }


        private string LoadStr(string pswrd)
        {
            using (StreamReader reader = File.OpenText(_path))
            {
                string str = reader.ReadToEnd();
                return ProtectorAES.Decrypt(str, pswrd );
            }
        }


        private void SaveStr(string str, string pswrd)
        {
            using (StreamWriter writer = File.CreateText(_path))
            {
                writer.Write(ProtectorAES.Encrypt(str,pswrd));
            }
        }


        private bool HasFile => File.Exists(_path);


        private byte[] GetBytes(string str) => UTF8Encoding.Unicode.GetBytes(str);


        private string GetStr(byte[] bytes) => UTF8Encoding.Unicode.GetString(bytes);
         
         
        public void Save<T>(T objectForSerialization, string pswrd)
        {
            if (_path == null) throw new Exception("You should create KeyGen | Path not found..");
            string str = SerializeObjectToSTR<T>(objectForSerialization);
            SaveStr(str,pswrd);
        }


        public T Load<T>(T objectForDesirialization, string pswrd)
        {
            if (HasFile)
            {

                string str = LoadStr(pswrd);
                return (T)DesirializeObject<T>(str);
            }
            else throw new Exception("File doesn't exist | I can load..");
        }
    }
}