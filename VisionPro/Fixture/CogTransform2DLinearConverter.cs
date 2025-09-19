using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionNet.Core.Patterns;
using VisionNet.Image;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.VisionPro
{
    public class CogTransform2DLinearConverter : IBidirectionalAdapter<Fixture2D, CogTransform2DLinear>
    {
        /// <summary>
        /// Crea una nueva instancia de Fixture2D a partir de un objeto CogTransform2DLinear.
        /// </summary>
        /// <param name="cogTransform">Instancia de CogTransform2DLinear con los parámetros de transformación.</param>
        /// <returns>Nueva instancia de Fixture2D.</returns>
        public Fixture2D Convert(CogTransform2DLinear cogTransform)
        {
            return new Fixture2D(
                cogTransform.TranslationX,
                cogTransform.TranslationY,
                cogTransform.Rotation,
                cogTransform.ScalingX,
                cogTransform.ScalingY
                );
        }

        /// <summary>
        /// Crea una nueva instancia de CogTransform2DLinear a partir de un objeto Fixture2D.
        /// </summary>
        /// <param name="fixture">Instancia de Fixture2D con los parámetros de transformación.</param>
        /// <returns>Nueva instancia de CogTransform2DLinear.</returns>
        public CogTransform2DLinear Convert(Fixture2D fixture)
        {
            var cogTransform = new CogTransform2DLinear();
            cogTransform.SetScalingsRotationsTranslation(fixture.ScaleX, fixture.ScaleY, fixture.Angle, fixture.Angle, fixture.X, fixture.Y);
            return cogTransform;
        }
    }
}
