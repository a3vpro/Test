using System.Collections.Generic;
using System.Linq;
using VisionNet.Core.Enums;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Clase conversora entre el tipo IParameter a NamedValue
    /// </summary>
    public class MeasurableAdapter : IAdapter<IReadonlyParameter, NamedValue>
    {
        public NamedValue Convert(IReadonlyParameter value)
        {
            var result = new NamedValue();
            Convert(value, ref result);
            return result;
        }

        public void Convert(IReadonlyParameter value, ref NamedValue result)
        {
            result = new NamedValue
            {
                Name = value.Name,
                Value = value.Value,
                Type = value.DataType,                    
            };
        }
    }
}
