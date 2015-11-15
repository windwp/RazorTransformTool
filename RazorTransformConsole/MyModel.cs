using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorGenConsole
{
    [Serializable]
    public class MyModel
    {
        public MyModel()
        {
            TestString1 = GenerateString(5);
            TestString2 = GenerateString(5);
            TestInt1 = GenerateInt();
            TestInt2 = GenerateInt();

            ListData=new List<string>(5);
            for (int i = 0; i < 5; i++)
            {
                ListData.Add(GenerateString(5));
            }
        }
        public string TestString1 { get; set; }
        public string TestString2 { get; set; }
        public int TestInt1 { get; set; }
        public int TestInt2 { get; set; }

        public IList<string> ListData { get; set; }

        public static string GenerateString(int stringSize = 8)
        {
            string chars = "abcdefghijkmnopqrstuvwxyz1234567890";
            StringBuilder result = new StringBuilder(stringSize);
            int count = 0;



            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                result.Append(chars[b % (chars.Length - 1)]);
                count++;
                if (count >= stringSize)
                    return result.ToString();
            }
            return result.ToString();
        }


        /// Generates a unique numeric ID. Generated off a GUID and
        /// returned as a 64 bit long value
        /// <returns></returns>
        public static int GenerateInt()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
