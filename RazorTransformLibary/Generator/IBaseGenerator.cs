namespace RazorTransformLibary.Generator
{
    public interface IBaseGenerator
    {
        void Init();
        string Render();
        void OutPut();
    }
}