using Force.DeepCloner;

namespace VisionNet.Core.Cloning
{
    public static class CloneExtensions
    {
        public static T Clone<T>(this T source, bool deepCopy = false)
        {
            if (source == null) return default;

            return deepCopy
                ? source.DeepClone()
                : source.ShallowClone();
        }
    }
}
