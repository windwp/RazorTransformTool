using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RazorTransformLibary.Data
{
    public class RazorFileTemplate
    {
        private string _inputFilePath;

        /// <summary>
        /// output file path generate code with custom tool it only set extension of file
        /// </summary>
        public string OutPutFile { get; set; }
        /// <summary>
        /// Get extension of output file
        /// </summary>
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
        public Dictionary<string, string> CustomVariables { get; set; }
        public string Name { get; set; }
        public bool IsRun { get; set; }
        /// <summary>
        /// add [auto generate] string in header of output file
        /// </summary>
        public bool IsHeader { get; set; }

        /// <summary>
        /// template file 
        /// </summary>
        public string InputFilePath
        {
            get { return _inputFilePath; }
            set
            {
                _inputFilePath = value;
                if (!string.IsNullOrEmpty(InputFilePath))
                {
                    var fileInfo = new FileInfo(InputFilePath);
                    if (fileInfo.Exists)
                    {
                        if (fileInfo.Directory != null) InputFolder = fileInfo.Directory.FullName;
                    }
                }
            }
        }

        /// <summary>
        /// get directory parent of inputfile
        /// </summary>
        public string InputFolder { get; set; }

        /// <summary>
        /// template content
        /// </summary>
        public string TemplateData { get; set; }
        /// <summary>
        /// Folder contain libary for compiler template
        /// </summary>
        public string InputDllFolder { get; set; }
        public ICollection<string> ListImportFile { get; set; }
        public RazorFileTemplate()
        {
            InputFolder = String.Empty;
            ListImportFile = new HashSet<string>();
            CustomVariables = new Dictionary<string, string>();
            IsRun = true;
            IsHeader = false;
        }

    }

}
