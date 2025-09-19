using AutoMapper;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Provides an adapter pattern for converting objects of type <typeparamref name="TSource"/> to <typeparamref name="TDestiny"/>.
    /// Implements the IAdapter interface to define the conversion behavior.
    /// </summary>
    /// <typeparam name="TSource">The source type to be converted.</typeparam>
    /// <typeparam name="TDestiny">The target type after conversion.</typeparam>
    public class Adapter<TSource, TDestiny> : IAdapter<TSource, TDestiny>
    {
        private static Mapper _mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestiny>()));

        /// <summary>
        /// Converts a value of type <typeparamref name="TSource"/> to <typeparamref name="TDestiny"/> using the static map.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="TSource"/> to convert.</param>
        /// <returns>A converted value of type <typeparamref name="TDestiny"/>.</returns>
        public static TDestiny ConvertFrom(TSource value)
        {
            return _mapper.Map<TDestiny>(value);
        }

        /// <summary>
        /// Converts a value of type <typeparamref name="TSource"/> to <typeparamref name="TDestiny"/> using the instance map.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="TSource"/> to convert.</param>
        /// <returns>A converted value of type <typeparamref name="TDestiny"/>.</returns>
        public TDestiny Convert(TSource value)
        {
            return _mapper.Map<TDestiny>(value);
        }
    }
}
