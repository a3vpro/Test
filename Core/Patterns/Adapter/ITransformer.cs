namespace VisionNet.Core.Patterns
{
    public interface ITransformer<T>
    {
        void Transform(ref T productResult);
    }
}
