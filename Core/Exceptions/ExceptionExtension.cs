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
using System.Diagnostics;
using System.Text;

namespace VisionNet.Core.Exceptions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Logs the details of the specified exception to the console.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="sourceClass">The name of the class where the exception occurred (optional, defaults to "Unknown").</param>
        public static void LogToConsole(this Exception ex, string sourceClass = "Unknown")
        {
            // Log the exception to the console
            Console.WriteLine($"An error occurred in {sourceClass}:");
            Console.Write(ComposeExceptionString(ex));

            //Console.WriteLine($"An error occurred in {sourceClass}: {ex.Message}");
            //if (ex.InnerException != null)
            //{
            //    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            //}
            //Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        /// <summary>
        /// Extrae la información de la excepción
        /// </summary>
        /// <param name="exception">Excepción de la que queremos extraer la información</param>
        /// <param name="assembly">Ensamblado</param>
        /// <param name="file">Nombre del archivo donde se ha provocado la excepción</param>
        /// <param name="className">Nombre de la clase donde se ha provocado la excepción</param>
        /// <param name="methode">Método donde se ha provocado la excepción</param>
        /// <param name="line">Linea donde se ha provocado la excepción</param>
        /// <param name="stackTrace">Pila de llamadas</param>
        public static void GetExceptionInfo(this Exception exception, out string assembly, out string file, out string className, out string methode, out int line, out string stackTrace)
        {
            assembly = string.Empty;
            file = string.Empty;
            className = string.Empty;
            methode = string.Empty;
            stackTrace = string.Empty;
            line = 0;

            StackTrace st = new StackTrace(exception, true);
            foreach (StackFrame sf in st.GetFrames())
            {
                if (!string.IsNullOrEmpty(sf.GetFileName()))
                {
                    assembly = sf.GetMethod().Module.Assembly.GetName().Name;
                    file = sf.GetFileName();
                    className = sf.GetMethod().ReflectedType.FullName;
                    methode = sf.GetMethod().Name;
                    line = sf.GetFileLineNumber();
                    break;
                }
            }
            stackTrace = exception.StackTrace + Environment.NewLine + ComposeInnerExceptionString(exception);
        }

        /// <summary> The ComposeExceptionString function composes a string containing the exception message, file name, class name, method name and line number where the exception occurred.</summary>
        /// <param name="exception"> The exception.</param>
        /// <returns> A string that contains the exception message, file name, class name, method name and line number where the exception occurred.</returns>
        public static string ComposeExceptionString(this Exception exception)
        {
            GetExceptionInfo(exception, out var assembly, out var file, out var className, out var method, out var line, out var callStack);

            var message = new StringBuilder();
            message.AppendLine("- Message:");
            message.AppendLine(exception.Message);
            message.AppendLine("- File:");
            message.AppendLine(file);
            message.AppendLine("- Class name:");
            message.AppendLine(className);
            message.AppendLine("- Method:");
            message.AppendLine(method);
            message.AppendLine("- Line:");
            message.AppendLine(line.ToString());
            message.AppendLine("- Call stach:");
            message.AppendLine(callStack);
            message.AppendLine(ComposeInnerExceptionString(exception));

            return message.ToString();
        }

        /// <summary> The ComposeExceptionString function composes a string containing the exception message, file name, class name, method name and line number where the exception occurred.</summary>
        /// <param name="exception"> The exception.</param>
        /// <returns> A string that contains the exception message, file name, class name, method name and line number where the exception occurred.</returns>
        public static string ComposeInnerExceptionString(this Exception exception)
        {
            var message = new StringBuilder();

            var innerEx = exception.InnerException;
            while (innerEx != null)
            {
                message.AppendLine("- Derived from exception:");
                message.AppendLine(innerEx.Message);
                innerEx = innerEx.InnerException;
            }

            return message.ToString();
        }

        /// <summary> The ComposeExceptionString function composes a string containing the exception message, file name, class name, method name and line number where the exception occurred.</summary>
        /// <param name="exception"> The exception.</param>
        /// <returns> A string that contains the exception message, file name, class name, method name and line number where the exception occurred.</returns>
        public static string GetFirstInnerExceptionString(this Exception exception)
        {
            string message = string.Empty;

            var innerEx = exception.InnerException;
            while (innerEx != null)
            {
                message = innerEx.Message;
                innerEx = innerEx.InnerException;
            }

            return message;
        }
    }
}
