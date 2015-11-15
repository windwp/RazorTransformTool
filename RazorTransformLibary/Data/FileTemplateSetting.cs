using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RazorTransformLibary.Data
{
    public class FileTemplateSetting
    {
        public string OutPutFile { get; set; }

        public string OutPutExtension
        {
            get
            {
                try
                {
                    return Regex.Match(OutPutFile, @"[^\\]*(\.\w+)$", RegexOptions.Multiline).Groups[1].Value;
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
            
        }
        public Dictionary<string,string> CustomVariables { get; set; }
        public string Name { get; set; }
        public bool IsGenerate { get; set; }
        public string FilePath { get; set; }
        public string TemplateData { get; set; }
        public string InputDllFolder { get; set; }

        public FileTemplateSetting()
        {
            CustomVariables=new Dictionary<string, string>();
        }

    }

}
