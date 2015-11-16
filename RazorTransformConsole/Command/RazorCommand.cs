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
        public RazorCommand() : base("Razor", "generate")
        {

        }
        public override async Task<bool> InvokeAsync(string paramList)
        {
            if (File.Exists(paramList.Trim()))
            {
                try
                {
                    var generator = new RazorGenerator(paramList.Trim(), new object());
                    generator.Init();
                    var result = generator.Render();
                    generator.OutPut();
                    OutputInformation("Entered string was: {0}", result);
                }
                catch (TemplateCompilationException exception)
                {
                   OutputError(MRazorUtil.GetError(exception));
                }
            }
            else
            {
                OutputError("Source file {0} not found",paramList);

            }
           
            return true;
        }



    }
}
