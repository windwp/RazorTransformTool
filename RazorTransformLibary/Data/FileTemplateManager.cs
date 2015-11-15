using System;
using System.IO;
using System.Text.RegularExpressions;
using RazorTransformLibary.Utils;

namespace RazorTransformLibary.Data
{
    class FileTemplateManager
    {
       
        private static string _headerpattern = @"/\*config([^(*/)]*)\*/";
        public static string LoadFileContent(string filePathOrContent, bool isFilePath = true)
        {
            var data = filePathOrContent;
            if (isFilePath)
            {
               data = FileUtils.ReadFileContent(filePathOrContent);
            }
            //delete header comment
            return Regex.Replace(data, _headerpattern, string.Empty, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Read config Header of file
        ///  Sample
        ///  /*config
        ///     Name=ABC;
        ///     OutPutFile=out/file.cs
        ///     InputDllFolder=./lib/
        ///  */
        /// </summary>
        /// <param name="filePathOrContent"></param>
        /// <param name="isFilePath"></param>
        /// <returns></returns>
        public static RazorFileTemplate LoadFileTemplate(string filePathOrContent,bool isFilePath=true)
        {
            var setting = new RazorFileTemplate();
            var fileContent = filePathOrContent;
            if (isFilePath)
            {
                var fileInfo = new FileInfo(filePathOrContent);
                setting.Name = fileInfo.Name.Replace("." + fileInfo.Extension, "");
                setting.FilePath = filePathOrContent;
                fileContent = FileUtils.ReadFileContent(filePathOrContent);
            }
            //read header setting file
            var fileHeaderSetting = Regex.Match(fileContent,_headerpattern, RegexOptions.IgnorePatternWhitespace).Groups[1].Value;
            try
            {
                fileHeaderSetting = fileHeaderSetting.Replace("\r\t", "\r\n\t");
                Regex regexObj = new Regex("(.*)=(.*)", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                Match matchResults = regexObj.Match(fileHeaderSetting);
                while (matchResults.Success)
                {
                    Group groupNameObj = matchResults.Groups[1];
                    Group groupDataObj = matchResults.Groups[2];
                    switch (groupNameObj.Value.Trim())
                    {
                        case "IsRun":
                            setting.IsRun = groupDataObj.Value.Trim()=="true"||groupDataObj.Value.Trim()=="1";
                            break;
                        case "IsHeader":
                            setting.IsHeader = groupDataObj.Value.Trim() == "true" || groupDataObj.Value.Trim() == "1";
                            break;
                        case "Name":
                            setting.Name = groupDataObj.Value.Trim();
                            break;
                        case "OutPutFile":
                            setting.OutPutFile = groupDataObj.Value.Trim();
                            break;
                        case "InputDllFolder":
                            setting.InputDllFolder = groupDataObj.Value.Trim();
                            break;
                        default:
                            setting.CustomVariables.Add(groupNameObj.Value.Trim(), groupDataObj.Value.Trim());
                            break;
                    }
                    matchResults = matchResults.NextMatch();
                }
            }
            catch (ArgumentException )
            {
                // Syntax error in the regular expression
            }
            fileContent = Regex.Replace(fileContent, _headerpattern, string.Empty, RegexOptions.IgnorePatternWhitespace);
            setting.TemplateData = fileContent;
            return setting;
        }
     

    }
}
