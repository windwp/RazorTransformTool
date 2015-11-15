using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace RazorTransformLibary.Utils
{
    public class FileUtils
    {

        public static void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            string content = ReadFileContent(filePath);

            CountReplacements(searchText, content);

            content = Regex.Replace(content, searchText, replaceText);

            WriteToFile(filePath, content);

        }

        public static string ReadFileContent(string filePath)
        {
            StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();
            return content;
        }
        public static void MakeDirectory(string directory)
        {
            var folderlist = directory.Replace(".", "").Split('\\');
            var currentDir = ".\\";
            foreach (var s in folderlist)
            {
                if (string.IsNullOrEmpty(s)) continue;
                if (s.IndexOf(".") != -1) break;
                if (!Directory.Exists(Path.Combine(currentDir, s)))
                {
                    Directory.CreateDirectory(Path.Combine(currentDir, s));
                }

                currentDir = Path.Combine(currentDir, s);
            }
        }

        public static void WriteToFile(string filePath, string content)
        {
            var directoryPath = filePath.Substring(0, filePath.LastIndexOf("\\"));
            if (!Directory.Exists(directoryPath))
            {
                MakeDirectory(directoryPath);
            }
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(content);
                writer.Close();
            }
        }

        public static void CountReplacements(string searchText, string content)
        {
            int counter = 0;

            foreach (char found in content)
            {
                var charArr = searchText.ToCharArray();
                int i = charArr.Count();

                if (i == 1)
                {
                    if (found == charArr[0])
                    {
                        counter++;
                    }
                }

            }
        }
        static public void SerializeToXML(string fileName, object data)
        {
            XmlSerializer serializer = new XmlSerializer(data.GetType());
            TextWriter textWriter = new StreamWriter(fileName);
            serializer.Serialize(textWriter, data);
            textWriter.Close();
        }
        public static TType DeserializeXml<TType>(string filename)
        {
            XmlSerializer _xmlSerializer = new XmlSerializer(typeof(TType));
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var result = _xmlSerializer.Deserialize(stream);
            stream.Close();
            return (TType)result;

        }
        public static void Serialize(string fileName, object data)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter pFormatter = new BinaryFormatter();
            pFormatter.Serialize(stream, data);
            stream.Close();
        }
        public static object DeSerialize(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryFormatter pFormatter = new BinaryFormatter();
            object data = pFormatter.Deserialize(fs);
            fs.Close();
            return data;
        }

       
    }
}
