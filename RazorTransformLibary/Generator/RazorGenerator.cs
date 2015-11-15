using System;
using System.Globalization;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using RazorTransformLibary.Data;
using RazorTransformLibary.Utils;

namespace RazorTransformLibary.Generator
{
    public class RazorGenerator : IBaseGenerator
    {
        private string _filePath;
        private object _modeldata;
        private readonly bool _isPath;
        private ITemplateSource _templateSource;
        private ITemplateKey _templateKey;
        private IRazorEngineService _engine;
        private FileTemplateSetting _template;
        private string _result;
        private string _fileContent;
        public string Extensions { get; set; }
        public RazorGenerator()
        {

        }
        public RazorGenerator(string filePathOrCotent, object modeldata, bool isPath = true)
        {
            _filePath = _fileContent = filePathOrCotent;
            _modeldata = modeldata;
            _isPath = isPath;
        }

        public void Init()
        {
            if (_isPath)
            {
                _fileContent = FileUtils.ReadFileContent(_filePath);
            }
            //load file setting with config in header
            _template = FileTemplateManager.LoadFileTemplateSetting(_fileContent, false);
            Extensions = _template.OutPutExtension;
            //create config for razor engine
            //http://antaris.github.io/RazorEngine/
            var config = new TemplateServiceConfiguration();
            config.EncodedStringFactory = new RawStringFactory();
            config.BaseTemplateType = typeof(CustomTemplateBase<>);
            config.TemplateManager = new DelegateTemplateManager();
            var referenceResolver = new MyIReferenceResolver();
            referenceResolver.DllFolder = _template.InputDllFolder;
            config.ReferenceResolver = referenceResolver;
            _engine = RazorEngineService.Create(config);
            _templateKey = Engine.Razor.GetKey(MyTemplateKey.MAIN_TEMPLATE);
            //check template is compile
            if (!_engine.IsTemplateCached(_templateKey, null))
            {
                var data = _template.TemplateData;
                _templateSource = new LoadedTemplateSource(data);
                _engine.AddTemplate(_templateKey, _templateSource);
                _engine.Compile(_templateKey, _modeldata.GetType());
            }
        }

        public string Render()
        {
            _result = _engine.Run(_templateKey, _modeldata.GetType(), _modeldata).Trim();
            return _result;
        }

        public void OutPut()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_template.OutPutFile))
                {
                    FileUtils.WriteToFile(_template.OutPutFile, _result.Trim());
                }
            }
            catch (Exception)
            {

                throw new Exception("Can't write file with path +" + _template.OutPutFile);
            }

        }
    }
    [Serializable]
    public class CustomTemplateBase<X> : TemplateBase<X>
    {
        private string _tabString = "\t";

        /// <summary>
        /// Simple write @ for .cshtml file
        /// </summary>
        /// <returns></returns>
        public string R2()
        {
            return "@";
        }
        /// <summary>
        /// Write Raw String file
        /// </summary>
        /// <param name="str">Raw</param>
        /// <returns></returns>
        public string R(string str)
        {
            return str;
        }
        public string NL
        {
            get { return Environment.NewLine; }
        }

        public string T
        {
            get
            {
                return _tabString;
            }
        }
        /// <summary>
        /// Write line with string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string WF(string str)
        {
            return WF(0, str);
        }
        /// <summary>
        /// Write line and add some \t in start of line
        /// </summary>
        /// <param name="numTab">number of \t added</param>
        /// <param name="data">line data</param>
        /// <returns></returns>
        public string WF(int numTab, string data = default(string))
        {
            string result = "";
            for (var i = 0; i < numTab; i++)
            {
                result += _tabString;
            }
            return result + data + Environment.NewLine;
        }

        public string WF(string str, params string[] value)
        {
            return WF(0, str, value);
        }

        public string WF(int num, string str, params string[] value)
        {
            string result = "";
            for (var i = 0; i < num; i++)
            {
                result += _tabString;
            }
            return result + string.Format(str, value) + Environment.NewLine;
        }

        public string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}
