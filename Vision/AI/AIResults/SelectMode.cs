using System;
using System.Collections.Generic;
using System.Text;

namespace VisionNet.Vision.AI
{
    public enum SelectMode
    {
        // Select all values lower than a maximum
        LowerThanMax,
        
        // Select all values upper than a minium
        UpperThanMin,

        // Select all values between the minimum and maximium value
        BetweenMinMax,

        // Select all values outside the minimum and maximium value
        OutsideMinMax,
    }
}
