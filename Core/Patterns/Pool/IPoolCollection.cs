namespace VisionNet.Core.Patterns
{
    public interface IPoolCollection<T>
    {
        T GetFromPool(string index);
    }
}
