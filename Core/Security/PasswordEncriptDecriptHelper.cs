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
using System.Security;

namespace VisionNet.Core.Security
{
    public static class PasswordEncriptDecriptHelper
    {
        // Encripta una cadena
        public static string Encript(this SecureString source, EncriptMethod encriptMethod = EncriptMethod.Base64)
            => new PasswordEncriptDecript().Encript(source, encriptMethod);

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public static SecureString Decript(this string source, EncriptMethod encriptMethod = EncriptMethod.Base64)
            => new PasswordEncriptDecript().Decript(source, encriptMethod);
    }
}