using System;

namespace VisionNet.Core.Comparisons
{
    /// <summary>
    /// Permite indicar si una propiedad debe ser tenida en cuenta por el ChangeTracker.
    /// Por defecto, todas las propiedades son trackeadas, salvo que se indique explícitamente lo contrario.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class TrackChangesAttribute : Attribute
    {
        public bool Track { get; }

        public TrackChangesAttribute(bool track = true)
        {
            Track = track;
        }
    }
}
