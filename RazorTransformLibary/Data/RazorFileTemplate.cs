using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RazorTransformLibary.Data
{
    public class RazorFileTemplate
    {
        /// <summary>
        /// output file path generate code with custom tool it only set extension of file
        /// </summary>
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
        public bool IsRun { get; set; }
        /// <summary>
        /// add [auto generate] string in header of output file
        /// </summary>
        public bool IsHeader { get; set; }
        public string FilePath { get; set; }
        public string TemplateData { get; set; }
        public string InputDllFolder { get; set; }

        public RazorFileTemplate()
        {
            CustomVariables=new Dictionary<string, string>();
            IsRun = true;
            IsHeader = false;
        }

    }

}
