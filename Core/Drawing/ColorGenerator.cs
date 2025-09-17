//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;

namespace VisionNet.Drawing
{
    /// <summary>
    /// The ColorGenerator class generates a sequence of unique colors for visualization purposes.
    /// It maintains a cache of pre-generated colors and provides methods to retrieve them either
    /// as hex strings or Color objects.
    /// </summary>
    public class ColorGenerator
    {
        private int _index = 0;
        private IntensityGenerator _intensityGenerator = new IntensityGenerator();
        private List<string> ColorCache = new List<string>();
        private int _maxColors = 256;

        
        /// <summary> The ColorGenerator function generates a random color string.</summary>
        /// <param name="maxColors"> The maximum number of colors to generate</param>
        /// <returns> A string value</returns>
        public ColorGenerator(int maxColors = 256)
        {
            _maxColors = maxColors;
            for (int i = 0; i < maxColors; i++)
                ColorCache.Add(NextColorString());
            _index = 0;
            _intensityGenerator = new IntensityGenerator();
        }

        
        /// <summary> The NextColorString function returns a string of the next color in the list.
        /// The function also increments _index by 1, so that it will return the next color on 
        /// subsequent calls.</summary>
        /// <returns> The color of the next index in the array</returns>
        public string NextColorString()
        {
            string color = GetColorString(_index);
            _index++;

            return color;
        }

        
        /// <summary> The NextColor function returns a random color from the list of colors in the ColorList.
        /// The function first generates a random number between 0 and 1, then multiplies that by the length of 
        /// ColorList to get an index for which color to return. It then converts that index into a string, and uses 
        /// int.Parse() with System.Globalization.NumberStyles as HexNumber to convert it into an integer value.</summary>
        /// <returns> A color string in hexadecimal format.</returns>
        public Color NextColor()
        {
            string colorString = NextColorString();
            var colorInt = int.Parse(colorString, System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(colorInt);
        }

        
        /// <summary> The GetColor function takes an index and returns a color.
        /// The function uses the ColorCache to get the color from a list of colors.
        /// If there are more indexes than colors, it will loop through the list of colors.</summary>
        /// <param name="index"> The index of the color in the colorcache array</param>
        /// <returns> A color from the colorcache array.</returns>
        public Color GetColor(int index)
        {
            var idx = index % _maxColors;
            var colorString = ColorCache[idx];
            var colorInt = int.Parse(colorString, System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(colorInt);
        }

#pragma warning disable Commentor
        private string GetColorString(int index)
        {
            string color = string.Format(NextPattern(index),
                _intensityGenerator.NextIntensity(index));
            return "FF" + color;
        }

        private string NextPattern(int index)
        {
            switch (index % 7)
            {
                case 0: return "{0}0000";
                case 1: return "00{0}00";
                case 2: return "0000{0}";
                case 3: return "{0}{0}00";
                case 4: return "{0}00{0}";
                case 5: return "00{0}{0}";
                case 6: return "{0}{0}{0}";
                default: throw new Exception("Math error");
            }
        }
#pragma warning restore Commentor
    }

#pragma warning disable Commentor
    internal class IntensityGenerator
    {
        private IntensityValueWalker walker;
        private int current;

        public string NextIntensity(int index)
        {
            if (index == 0)
            {
                current = 255;
            }
            else if (index % 7 == 0)
            {
                if (walker == null)
                {
                    walker = new IntensityValueWalker();
                }
                else
                {
                    walker.MoveNext();
                }
                current = walker.Current.Value;
            }
            string currentText = current.ToString("X");
            if (currentText.Length == 1) currentText = "0" + currentText;
            return currentText;
        }
    }

    internal class IntensityValue
    {

        private IntensityValue mChildA;
        private IntensityValue mChildB;

        public IntensityValue(IntensityValue parent, int value, int level)
        {
            if (level > 7) throw new Exception("There are no more colors left");
            Value = value;
            Parent = parent;
            Level = level;
        }

        public int Level { get; set; }
        public int Value { get; set; }
        public IntensityValue Parent { get; set; }

        public IntensityValue ChildA
        {
            get
            {
                return mChildA ?? (mChildA = new IntensityValue(this, this.Value - (1 << (7 - Level)), Level + 1));
            }
        }

        public IntensityValue ChildB
        {
            get
            {
                return mChildB ?? (mChildB = new IntensityValue(this, Value + (1 << (7 - Level)), Level + 1));
            }
        }
    }

    internal class IntensityValueWalker
    {

        public IntensityValueWalker()
        {
            Current = new IntensityValue(null, 1 << 7, 1);
        }

        public IntensityValue Current { get; set; }

        public void MoveNext()
        {
            if (Current.Parent == null)
            {
                Current = Current.ChildA;
            }
            else if (Current.Parent.ChildA == Current)
            {
                Current = Current.Parent.ChildB;
            }
            else
            {
                int levelsUp = 1;
                Current = Current.Parent;
                while (Current.Parent != null && Current == Current.Parent.ChildB)
                {
                    Current = Current.Parent;
                    levelsUp++;
                }
                if (Current.Parent != null)
                {
                    Current = Current.Parent.ChildB;
                }
                else
                {
                    levelsUp++;
                }
                for (int i = 0; i < levelsUp; i++)
                {
                    Current = Current.ChildA;
                }

            }
        }
    }
#pragma warning restore Commentor
}
