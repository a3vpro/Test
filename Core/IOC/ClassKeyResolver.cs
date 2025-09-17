namespace VisionNet.Core.IOC
{
    public abstract class ClassKeyResolver<T>
    {
        protected abstract string ClassType { get; }

        public abstract string Resolve(T type, string className = "");

        //public static string Resolve(T type, string className = "")
        //{
        //    return InternalResolve(type, className);
        //}
    }
}
