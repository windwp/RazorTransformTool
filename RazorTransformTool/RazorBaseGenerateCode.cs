using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using RazorEngine.Templating;
using RazorTransformLibary;
using RazorTransformLibary.Generator;
using RazorTransformLibary.Utils;

namespace RazorTransformTool
{
    [ComVisible(true)]
    [Guid("C3AA784D-84FC-45F2-B757-5CC7B53A9FF0")]
    //[Microsoft.VisualStudio.Shell.CodeGeneratorRegistration(typeof(RazorBaseGenerateCode), "RazorTransformTool", "FAE04EC1-301F-11D3-BF4B-00C04F79EFBC")]
    public class RazorBaseGenerateCode : IVsSingleFileGenerator
    {
        private static AppDomain _domain;
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

                _domain = AppDomain.CreateDomain(
                    "MyMainDomain", null,
                    current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                    strongNames);
                _domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            }
        }
        private string _extenstion = ".cs";
      

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = _extenstion;
            return VSConstants.S_OK;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            //SwitchDomainForRazorEngine();
            byte[] resultBytes;
            try
            {
                var model = new RazorModel();

                //set file name and namespace for model using
                model.DefaultNameSpace = wszDefaultNamespace;
                var info=new FileInfo(wszInputFilePath);
                if (info.Exists)
                {
                    model.FileName = info.Name;
                }
         
                var generator = new RazorGenerator(wszInputFilePath, model);
                generator.Init();
                //get extension from header file
                if (!string.IsNullOrEmpty(generator.RazorTemplate.OutPutExtension))
                {
                    _extenstion = generator.RazorTemplate.OutPutExtension;
                }
                //generate code
                var result = generator.Render();
                resultBytes = Encoding.UTF8.GetBytes(result);
                int outputLength = resultBytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
                Marshal.Copy(resultBytes, 0, rgbOutputFileContents[0], outputLength);
                pcbOutput = (uint) outputLength;
                return VSConstants.S_OK;
            }
            catch (TemplateCompilationException tex)
            {
                foreach (var compilerError in tex.CompilerErrors)
                {
                    pGenerateProgress.GeneratorError(0, 1, compilerError.ErrorText, (uint) compilerError.Line,
                        (uint) compilerError.Column);
                }
                var message = MRazorUtil.GetError(tex);
                resultBytes = Encoding.UTF8.GetBytes(message);
                int outputLength = resultBytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
                Marshal.Copy(resultBytes, 0, rgbOutputFileContents[0], outputLength);
                pcbOutput = (uint) outputLength;
                return VSConstants.S_FALSE;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                resultBytes = Encoding.UTF8.GetBytes(message);
                int outputLength = resultBytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
                Marshal.Copy(resultBytes, 0, rgbOutputFileContents[0], outputLength);
                pcbOutput = (uint) outputLength;
                return VSConstants.S_FALSE;
            }
            //finally
            //{
            //    //unload domain for unload dll loaded from InputDllFolder
            //    if (_domain != null) AppDomain.Unload(_domain);
            //}

        }
    }
}
