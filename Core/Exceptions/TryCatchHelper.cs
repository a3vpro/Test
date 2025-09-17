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

namespace VisionNet.Core.Exceptions
{
    /// <summary>
    /// A static helper class that provides methods for safely executing actions or converting objects, 
    /// handling exceptions, and retrying operations.
    /// </summary>
    public static class TryCatchHelper
    {
        /// <summary>
        /// Attempts to execute an action on an object, and returns a boolean indicating whether the action was successful.
        /// If the action fails, it returns false and sets the exception parameter to contain information about the failure.
        /// </summary>
        /// <param name="obj">The object on which the action is performed.</param>
        /// <param name="action">The action to be performed on the object.</param>
        /// <param name="exception">An output parameter that will contain the exception details if the action fails.</param>
        /// <returns>A boolean value indicating whether the action was successful.</returns>
        public static bool TryDo(this object obj, Action<object> action, out Exception exception)
        {
            return TryDo(obj, action, out exception, 1);
        }

        /// <summary>
        /// Attempts to execute an action on an object, and returns a boolean indicating whether the action was successful.
        /// If the TryDo function fails, it returns false and sets the exception parameter to contain information about the failure.
        /// </summary>
        /// <param name="obj">The object on which the action is performed.</param>
        /// <param name="action">The action to be performed on the object.</param>
        /// <returns>A boolean value indicating whether the action was successful.</returns>
        public static bool TryDo(this object obj, Action<object> action)
        {
            return TryDo(obj, action, out var exception, 1);
        }

        /// <summary>
        /// Attempts to execute an action on an object, and returns a boolean indicating whether the action was successful.
        /// If the action fails, it returns false and sets the exception parameter to contain information about the failure.
        /// </summary>
        /// <param name="obj">The object on which the action is performed.</param>
        /// <param name="action">The action to be performed on the object.</param>
        /// <param name="maxRetries">The maximum number of times to attempt the action in case of failure.</param>
        /// <returns>A boolean value indicating whether the action was successful.</returns>
        public static bool TryDo(this object obj, Action<object> action, int maxRetries)
        {
            return TryDo(obj, action, out var exception, maxRetries);
        }

        /// <summary>
        /// Attempts to execute an action on an object, and returns a boolean indicating whether the action was successful.
        /// If the action fails, it returns false and sets the exception parameter to contain information about the failure.
        /// </summary>
        /// <param name="obj">The object on which the action is performed.</param>
        /// <param name="action">The action to be performed on the object.</param>
        /// <param name="exception">An output parameter that will contain the exception details if the action fails.</param>
        /// <param name="maxRetries">The maximum number of times to attempt the action in case of failure.</param>
        /// <returns>A boolean value indicating whether the action was successful.</returns>
        public static bool TryDo(this object obj, Action<object> action, out Exception exception, int maxRetries = 1)
        {
            if (maxRetries <= 0)
            {
                exception = new ArgumentException($"{nameof(maxRetries)} must be greater than 0.");
                return false;
            }

            exception = null;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    action(obj);
                    exception = null;
                    return true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to execute an action on an object, and returns a boolean indicating whether the action was successful.
        /// If the action fails, it returns false and sets the exception parameter to contain information about the failure.
        /// A custom exception action is provided to handle the exception.
        /// </summary>
        /// <param name="obj">The object on which the action is performed.</param>
        /// <param name="action">The action to be performed on the object.</param>
        /// <param name="exceptionAction">A custom action that processes the exception if the action fails.</param>
        /// <param name="maxRetries">The maximum number of times to attempt the action in case of failure.</param>
        /// <returns>A boolean value indicating whether the action was successful.</returns>
        public static bool TryDo(this object obj, Action<object> action, Action<Exception> exceptionAction, int maxRetries = 1)
        {
            Exception exception = null;
            if (maxRetries <= 0)
            {
                exception = new ArgumentException($"{nameof(maxRetries)} must be greater than 0.");
            }

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    action(obj);
                    return true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            exceptionAction(exception);
            return false;
        }

        /// <summary>
        /// Attempts to convert an object to a specified type. If the conversion fails, 
        /// it returns false and sets the exception parameter to the thrown exception.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <param name="action">The function used to convert the object to the target type.</param>
        /// <param name="result">The result of the conversion.</param>
        /// <param name="exception">An output parameter that will contain the exception details if the conversion fails.</param>
        /// <returns>A boolean value indicating whether the conversion was successful.</returns>
        public static bool TryConvert<T>(this object obj, Func<object, T> action, out T result, out Exception exception)
        {
            var success = false;
            result = default(T);
            exception = null;

            try
            {
                result = action(obj);
                success = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return success;
        }

        /// <summary>
        /// Attempts to convert an object to a specified type. If the conversion fails, 
        /// it returns false and sets the exception parameter to the thrown exception.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <param name="action">The function used to convert the object to the target type.</param>
        /// <param name="result">The result of the conversion.</param>
        /// <returns>A boolean value indicating whether the conversion was successful.</returns>
        public static bool TryConvert<T>(this object obj, Func<object, T> action, out T result)
        {
            return TryConvert(obj, action, out result, out var exception);
        }
    }
}
