using System.IO;
using System.Text;
using RazorEngine.Templating;

namespace RazorTransformLibary.Utils
{
    public class MRazorUtil
    {

        public static string GetError(TemplateCompilationException exception)
        {

            var errorStrBuilder = new StringBuilder();
            foreach (var compilerError in exception.CompilerErrors)
            {
                errorStrBuilder.AppendLine();
                errorStrBuilder.AppendFormat("Error Message : {0} {1}", compilerError.ErrorText,compilerError.ErrorNumber);
                errorStrBuilder.AppendLine();
                errorStrBuilder.AppendLine("################");
                errorStrBuilder.AppendFormat("Source Line {0} :{1}\r\n", compilerError.Line - 1, GetLine(exception.SourceCode, compilerError.Line - 1));
                errorStrBuilder.AppendFormat("Error  Line {0} :{1}\r\n", compilerError.Line, GetLine(exception.SourceCode,compilerError.Line));
                errorStrBuilder.AppendFormat("Source Line {0} :{1}\r\n", compilerError.Line + 1, GetLine(exception.SourceCode, compilerError.Line +1));
                errorStrBuilder.AppendLine("################");
            }


            return errorStrBuilder.ToString();
        }

        public static string  GetLine(string data, int line)
        {
            if (string.IsNullOrEmpty(data)) return "Line Data is null";
            using (var sr = new StringReader(data))
            {
                for (int i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }


    }
}
