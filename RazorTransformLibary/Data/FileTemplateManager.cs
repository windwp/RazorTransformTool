using System;
using System.IO;
using System.Text.RegularExpressions;
using RazorTransformLibary.Utils;

namespace RazorTransformLibary.Data
{
    class FileTemplateManager
    {

        private static string _headerpattern = @"/\*config(.*?)\*/";
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
        /// Sample<para />.
        ///  /*config<para />.
        ///     Name=ABC;<para />.
        ///     OutPutFile=out/file.cs<para />.
        ///     InputDllFolder=./lib/<para />.
        ///  */
        /// </summary>
        /// <param name="filePath">path of file template</param>
        /// <param name="templateContent">content of file template</param>
        /// <param name="isReload">reload file content</param>
        /// <returns></returns>
        public static RazorFileTemplate LoadFileTemplate(string filePath,string templateContent,bool isReload=false)
        {
            var setting = new RazorFileTemplate();
            var fileInfo = new FileInfo(filePath);
            setting.Name = fileInfo.Name.Replace("." + fileInfo.Extension, "");
            setting.InputFilePath = filePath;
            if (string.IsNullOrEmpty(templateContent)||isReload)
            {
                templateContent = FileUtils.ReadFileContent(filePath);
            }
            
            try
            {
                //read header setting file
                var fileHeaderSetting = Regex.Match(templateContent, _headerpattern, RegexOptions.Singleline | RegexOptions.Multiline).Groups[1].Value;
                fileHeaderSetting = fileHeaderSetting.Replace("\r\t", "\r\n\t");
                Regex regexObj = new Regex("(.*)=(.*)", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                Match matchResults = regexObj.Match(fileHeaderSetting);
                while (matchResults.Success)
                {
                    Group groupNameObj = matchResults.Groups[1];
                    Group groupDataObj = matchResults.Groups[2];
                    switch (groupNameObj.Value.Trim().ToLower())
                    {
                        case "isrun":
                            setting.IsRun = groupDataObj.Value.Trim() == "true" || groupDataObj.Value.Trim() == "1";
                            break;
                        case "isheader":
                            setting.IsHeader = groupDataObj.Value.Trim() == "true" || groupDataObj.Value.Trim() == "1";
                            break;
                        case "name":
                            setting.Name = groupDataObj.Value.Trim();
                            break;
                        case "outputfile":
                            setting.OutPutFile = groupDataObj.Value.Trim();
                            break;
                        case "inputdllfolder":
                            setting.InputDllFolder = groupDataObj.Value.Trim();
                            break;
                        case "listincludefile":
                            var listFile = groupDataObj.Value.Trim().Split(',');
                            foreach (var path in listFile)
                            {
                                var importFilePath = FileUtils.GetPartialPath(setting.InputFolder, path);
                                if (File.Exists(importFilePath))
                                {
                                    setting.ListImportFile.Add(importFilePath);
                                }
                                else
                                {
                                    throw new FileNotFoundException("Include file not found at :" + importFilePath, importFilePath);
                                }
                            }
                            break;
                        default:
                            setting.CustomVariables.Add(groupNameObj.Value.Trim(), groupDataObj.Value.Trim());
                            break;
                    }
                    matchResults = matchResults.NextMatch();
                }
            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
            }
            templateContent = Regex.Replace(templateContent, _headerpattern, string.Empty, RegexOptions.Singleline | RegexOptions.Multiline);
            setting.TemplateData = templateContent;
            return setting;
        }


    }
}
