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
using System.Linq;
using VisionNet.Core.Comparisons;
using VisionNet.Core.Types;
using VisionNet.Core.Exceptions;
using VisionNet.Core.SafeObjects;
using VisionNet.Image.Bitmaps;
using VisionNet.IO.Paths;
using System.IO;
using System.Collections.Generic;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public class VisionSafeObject : SafeObject, ISafeObject<BasicTypeCode>, IEquatable<object>, IEquatable<SafeObject>, IEquatable<VisionSafeObject>
    {
        public void SetDefaultValue(object defaultValue)
        {
            if (DataType == BasicTypeCode.Image && !IsArray && defaultValue is string pathString)
            {
                if (string.IsNullOrWhiteSpace(pathString))
                    throw new ArgumentException("The specified default value path is null or whitespace.");

                if (pathString.IsValidRelativeDirectoryPath())
                    pathString = pathString.FromExeRelativePathToAbsolutePath();

                if (!pathString.IsValidAbsoluteFilePath() || !File.Exists(pathString))
                    throw new ArgumentException($"The specified default value is not a valid image path: {pathString}");

                try
                {
                    DefaultValue = new ImageBitmap().ReadFile(pathString);
                }
                catch (IOException ioEx)
                {
                    // SAFETY: Wrap I/O failures with context
                    throw new IOException($"Failed to read default image from '{pathString}'.", ioEx);
                }
                catch (UnauthorizedAccessException uaEx)
                {
                    throw new UnauthorizedAccessException($"Access denied reading default image from '{pathString}'.", uaEx);
                }
            }
            else if (DataType == BasicTypeCode.Image && IsArray && defaultValue is IEnumerable<object> objectList)
            {
                var defaultListValue = new List<IImage>();
                foreach (var objectItem in objectList)
                {
                    objectItem.TryChangeType(BasicTypeCode.Image, false, out object resultImgObj);
                    if (resultImgObj is IImage imageItem)
                    {
                        defaultListValue.Add(imageItem);
                    }
                    else
                    {
                        objectItem.TryChangeType(BasicTypeCode.String, false, out object resultStrObj);
                        if (resultStrObj is string pathStringItem)
                        {
                            string pathStringInternal = pathStringItem;

                            if (pathStringItem.IsValidRelativeDirectoryPath())
                                pathStringInternal = pathStringItem.FromExeRelativePathToAbsolutePath();

                            if (!pathStringInternal.IsValidAbsoluteFilePath() || !File.Exists(pathStringInternal))
                                throw new ArgumentException($"The specified default value is not a valid image path: {pathStringInternal}");

                            try
                            {
                                var defaultImage = new ImageBitmap().ReadFile(pathStringInternal) as IImage;
                                defaultListValue.Add(defaultImage);
                            }
                            catch (IOException ioEx)
                            {
                                throw new IOException($"Failed to read default image from '{pathStringInternal}'.", ioEx);
                            }
                            catch (UnauthorizedAccessException uaEx)
                            {
                                throw new UnauthorizedAccessException($"Access denied reading default image from '{pathStringInternal}'.", uaEx);
                            }
                        }
                        else
                        {
                            // GUARD: Invalid list element type for image array defaults
                            throw new ArgumentException("The specified default list contains an item that cannot be converted to an image or valid image path.");
                        }
                    }
                }
                DefaultValue = defaultListValue;
            }
            else
            {
                DefaultValue = defaultValue;
            }
        }

        public new BasicTypeCode DataType { get; set; }

        public bool IsArray { get; set; }


        /// <summary> The SafeObject function is a wrapper for the object type that allows you to set default values and 
        /// perform null checks on objects without having to worry about NullReferenceExceptions. It also provides a 
        /// convenient way of converting between types.</summary>
        /// <returns> A safeobject object.</returns>
        public VisionSafeObject() : base()
        {
            DataType = BasicTypeCode.NotSupported;
        }


        /// <summary> The SafeObject function is a wrapper for the object type that allows you to safely convert between types without throwing exceptions.</summary>
        /// <param name="dataType"> The type of data that the safeobject will hold.</param>
        /// <param name="defaultValue"> What is this used for?</param>
        /// <param name="preferences"> What is the purpose of this parameter?</param>
        /// <returns> The value of the object.</returns>
        public VisionSafeObject(BasicTypeCode dataType, bool isArray, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None)
        {
            if (defaultValue == null)
                defaultValue = dataType.DefaultValue(isArray);
            else if (!defaultValue.TryChangeType(dataType, isArray, out var _))
                throw new ArgumentException($"The specified default value is not valid for the datatype {dataType} and vectorization = {isArray}");

            var baseDataType = TypeCode.Empty;
            dataType.ToTypeCode(ref baseDataType);
            base.DataType = baseDataType;

            DataType = dataType;
            IsArray = isArray;
            Preferences = preferences;
            SetDefaultValue(defaultValue);

            _value = DefaultValue;
        }

        /// <summary> The TrySetValue function attempts to set the value of a variable.
        /// If it is successful, then the function returns true and sets the value of 
        /// _value equal to tmpValue. Otherwise, if it fails, then TrySetValue returns false.</summary>
        /// <param name="value"> The value to be converted</param>
        /// <returns> True if the value was successfully converted to the type of this property. 
        ///otherwise, it returns false.</returns>
        public virtual bool TrySetValue(object value)
        {
            bool canConvert = false;
            try
            {
                lock (_lockObject)
                {
                    canConvert = value.TryChangeType(DataType, IsArray, out var tmpValue, DefaultValue, Preferences);

                    if (canConvert)
                        _value = tmpValue;
                }
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(TrySetValue));
            }
            return canConvert;
        }


        /// <summary> The IsValidValue function is used to determine if the value passed in can be converted into a valid type for this field.
        /// If it cannot, then an exception will be thrown.</summary>
        /// <param name="value"> The value to be converted.</param>
        /// <returns> True if the value can be converted to the datatype property.</returns>
        public virtual bool IsValidValue(object value)
        {
            lock (_lockObject)
                return value.TryChangeType(DataType, IsArray, out var _, DefaultValue, Preferences);
        }

        /// <summary>
        /// The Equals function returns if the value contained into the passed SafeObject is the same than the current one
        /// </summary>
        /// <param name="other">Other instance of SafeObject</param>
        /// <returns>True if the value contained into the passed SafeObject is the same than the current one, false otherwise</returns>
        public bool Equals(VisionSafeObject other)
        {
            return ComparisonExtension.SafeAreEqualTo(_value, other?._value);
        }

        // Implement IEquatable<SafeObject>
        public bool Equals(SafeObject other)
        {
            return ComparisonExtension.SafeAreEqualTo(_value, other?.Value);
        }

        // Implement IEquatable<object> and standard object equality
        public override bool Equals(object obj)
        {
            return ComparisonExtension.Equals(_value, obj);
        }

        public override int GetHashCode()
        {
            // POSTCONDITION: hash consistent with Equals based on the wrapped value.
            lock (_lockObject)
                return _value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(VisionSafeObject left, VisionSafeObject right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(VisionSafeObject left, VisionSafeObject right)
        {
            return !(left == right);
        }
    }
}
