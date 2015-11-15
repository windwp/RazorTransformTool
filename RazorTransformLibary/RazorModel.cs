using System;

namespace RazorTransformLibary
{
    [Serializable]
    public class RazorModel
    {
        public string DefaultNameSpace { get; set; }
        public string FileName { get; set; }
    }
}
