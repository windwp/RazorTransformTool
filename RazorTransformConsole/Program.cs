using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using RazorTransformConsole.Command;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace RazorTransformConsole
{
    public class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
            //LoadedAssembly();
            if (args.Length == 0) SwitchDomainForRazorEngine();
            var console = new ClientConsole();
            var command = new RootCommand(console);
            command.RegisterCommand(new RazorCommand());
            var commandEngine = new CommandEngine(command);
            commandEngine.Run(args);
        }

        private static void SwitchDomainForRazorEngine()
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                // RazorEngine cannot clean up from the default appdomain...
                //Console.WriteLine("Switching to secound AppDomain, for RazorEngine...");
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                var current = AppDomain.CurrentDomain;
                // You only need to add strongnames when your appdomain is not a full trust environment.
                var strongNames = new StrongName[0];

                var domain = AppDomain.CreateDomain(
                    "MyMainDomain", null,
                    current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                    strongNames);
                domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            }
        }
        private static void LoadedAssembly()
        {
            if (Directory.Exists("lib"))
            {
                var fileDllList = Directory.GetFiles("./lib/", "*.dll");
                foreach (var dllPath in fileDllList)
                {
                    var file = Path.GetFullPath(dllPath);
                    Assembly.LoadFile(file);
                }
            }
        }
    }
}
