using System;
using System.IO;
using System.Threading.Tasks;
using RazorEngine.Templating;
using RazorTransformLibary.Generator;
using RazorTransformLibary.Utils;
using Tharga.Toolkit.Console.Command.Base;

namespace RazorTransformConsole.Command
{
    public class RazorCommand : ActionCommandBase
    {
        public RazorCommand() : base("Razor", "Generate code")
        {

        }
        public override  Task<bool> InvokeAsync(string paramList)
        {
            if (File.Exists(paramList.Trim()))
            {
                try
                {
                    var generator = new RazorGenerator(paramList.Trim(),string.Empty, new object());
                    generator.Init();
                    var result = generator.Render();
                    generator.OutPut();
                    OutputInformation("Entered string was: {0}", result);
                }
                catch (TemplateCompilationException exception)
                {
                    OutputError(MRazorUtil.GetError(exception));
                }
                catch (Exception ex)
                {
                    OutputError("Error {0}", ex.Message);
                }
            }
            else
            {
                OutputError("Source file {0} not found",paramList);

            }
           
            return Task.FromResult(true);
        }



    }
}
