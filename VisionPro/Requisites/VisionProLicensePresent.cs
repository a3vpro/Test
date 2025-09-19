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
using VisionNet.Core.Abstractions;
using VisionNet.Core.Exceptions;
using Cognex.VisionPro;
using VisionNet.Core.Requisites;

namespace VisionNet.Vision.VisionPro
{
    public class VisionProLicensePresent : IApplicationRequisite
    {
        public RequisiteType Type => RequisiteType.License;

        public void Configure(ApplicationRequisitesOptions options)
        {
        }

        public void Check()
        {
            if (!TryCheck(out var result))
                throw new CheckException(result);
        }

        public bool TryCheck(out InvalidCheckResult result)
        {
            result = new InvalidCheckResult();

            bool success = false;
            try
            {
                var licensedFeatures = CogLicense.GetLicensedFeatures(false, false);

                foreach (string feature in licensedFeatures)
                    System.Console.WriteLine($"Found Cognex VisionPro license for tool {feature}");

                success = licensedFeatures.Count > 0;
            }
            catch 
            { 
                // license is not valid
            }

            if (!success)
            {
                result.CauseDescription = $"VisionPro dongle is not present.";
                result.NeedDescription = "Please, check that the VisionPro licence is correctly installed.";
                return false;
            }
            return true;
        }

        public void CleanUp()
        {
        }
    }
}
