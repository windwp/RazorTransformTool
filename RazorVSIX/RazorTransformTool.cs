using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RazorEngine.Templating;
using RazorTransformLibary;
using RazorTransformLibary.Generator;
using RazorTransformLibary.Utils;
using TransformCodeGenerator;
using VSLangProj80;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace RazorVSIX
{
    [ComVisible(true)]
    [Guid("C3AA784D-84FC-45F2-B757-5CC7B53A9FF0")]
    [CodeGeneratorRegistration(typeof(RazorTransformTool), "C# Razor Transform Tool", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(RazorTransformTool), "VB Razor Transform Tool", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(RazorTransformTool))]
    public class RazorTransformTool : IVsSingleFileGenerator
    {
#pragma warning disable 0414
        //The name of this generator (use for 'Custom Tool' property of project item)
        internal static string name = "RazorTransformTool";
#pragma warning restore 0414
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

        private readonly Project project;
        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
           // SwitchDomainForRazorEngine();
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
                int iFound;
                uint itemId;
                ProjectItem item;
                VSDOCUMENTPRIORITY[] pdwPriority = new VSDOCUMENTPRIORITY[1];

                // obtain a reference to the current project as an IVsProject type
                IVsProject vsProject = VsHelper.ToVsProject(project);
                // this locates, and returns a handle to our source file, as a ProjectItem
                vsProject.IsDocumentInProject(wszInputFilePath, out iFound, pdwPriority, out itemId);

                // if our source file was found in the project (which it should have been)
                if (iFound != 0 && itemId != 0)
                {
                    IServiceProvider oleSp;
                    vsProject.GetItemContext(itemId, out oleSp);
                    if (oleSp != null)
                    {
                        ServiceProvider sp = new ServiceProvider(oleSp);
                        // convert our handle to a ProjectItem
                        item = sp.GetService(typeof(ProjectItem)) as ProjectItem;
                    }
                    else
                        throw new ApplicationException("Unable to retrieve Visual Studio ProjectItem");
                }
                else
                    throw new ApplicationException("Unable to retrieve Visual Studio ProjectItem");

                var generator = new RazorGenerator(wszInputFilePath,bstrInputFileContents, model);
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
                //Display error in result template
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
                return VSConstants.S_FALSE;// Change to E_Fail will display error in error list
            }
            catch (Exception ex)
            {
                var messageBuilder =new StringBuilder( ex.Message);
                messageBuilder.AppendLine();
                if (ex.Source != null) messageBuilder.Append(ex.Source);
                messageBuilder.Append(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    messageBuilder.AppendLine();
                    messageBuilder.Append(ex.InnerException.Message + ex.InnerException.StackTrace);
                }
                resultBytes = Encoding.UTF8.GetBytes(messageBuilder.ToString());
                int outputLength = resultBytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
                Marshal.Copy(resultBytes, 0, rgbOutputFileContents[0], outputLength);
                pcbOutput = (uint) outputLength;
                return VSConstants.S_FALSE;// Change to E_Fail will display error in error list
            }
            //finally
            //{
            //    //unload domain for unload dll loaded from InputDllFolder
            //    if (_domain != null) AppDomain.Unload(_domain);
            //}

        }
    }
}
