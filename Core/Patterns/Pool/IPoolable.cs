namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Simple poolable pattern
    /// </summary>
    public interface IPoolable
    {
        bool IsOccupied { get; }

        void Hold();

        void Release();
    }
}
