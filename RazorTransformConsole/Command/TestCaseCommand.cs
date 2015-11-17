using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace RazorTransformConsole.Command
{
    class TestCaseCommand: ActionCommandBase
    {
        private readonly ActionCommandBase _razorcommand;
        public TestCaseCommand(ActionCommandBase razorcommand) : base("Test","Test case command")
        {
            _razorcommand = razorcommand;
            //RegisterCommand(new )
        }

        public override Task<bool> InvokeAsync(string paramList)
        {
            switch (paramList)
            {
                case "1":
                    return _razorcommand.InvokeAsync("template\\test.cshtml");
                case "2":
                    return _razorcommand.InvokeAsync("template\\testdatabase.cshtml");
                case "3":
                    return _razorcommand.InvokeAsync("template\\testinclude.cshtml");
                case "4":
                    return _razorcommand.InvokeAsync("template\\testrenderpartial.cshtml");
            }
            OutputInformation("Sorry,we not found any test case");
            return Task.FromResult(false);
        }
    }

}