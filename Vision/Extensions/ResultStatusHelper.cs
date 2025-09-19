//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 23-06-2023
// Description      : v1.7.0
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.            
//-----------------------------------------------
using VisionNet.Core.Enums;

namespace VisionNet.Vision.Core
{
    public static class ResultStatusHelper
    {
        public static bool IsEnabled(this ResultStatus resultStatus) => !IsDisabled(resultStatus);

        public static bool IsDisabled(this ResultStatus resultStatus) => resultStatus == ResultStatus.Disabled;

        public static bool IsExecuted(this ResultStatus resultStatus) => resultStatus.IsIn(ResultStatus.Pass, ResultStatus.Rejected, ResultStatus.Warning);

        public static bool IsNotExecuted(this ResultStatus resultStatus) => !IsExecuted(resultStatus);

        public static bool IsError(this ResultStatus resultStatus) => resultStatus == ResultStatus.Error;

        public static bool IsNotError(this ResultStatus resultStatus) => !IsError(resultStatus);

        public static bool IsOK(this ResultStatus resultStatus) => resultStatus.IsIn(ResultStatus.Disabled, ResultStatus.Pass, ResultStatus.Warning);

        public static bool IsNOK(this ResultStatus resultStatus) => resultStatus.IsIn(ResultStatus.Error, ResultStatus.Rejected, ResultStatus.PrevInspectionRejected);
        public static bool IsPrevInspectionRejected(this ResultStatus resultStatus) => resultStatus == ResultStatus.PrevInspectionRejected;

        public static ResultStatus Create(bool result, bool enabled = true, bool success = true, bool prevResult = true, bool warning = false)
        {
            if (!enabled)
                return ResultStatus.Disabled;
            else if (!prevResult)
                return ResultStatus.PrevInspectionRejected;
            else if (!success)
                return ResultStatus.Error;
            else if (!result)
                return ResultStatus.Rejected;
            else if (warning)
                return ResultStatus.Warning;
            else
                return ResultStatus.Pass;
        }

        public static T Default<T>(ResultStatus resultStatus, T value, T defaultValue)
        {
            return resultStatus.IsEnabled() && resultStatus.IsNotError() && !resultStatus.IsPrevInspectionRejected() ? value : defaultValue;
        }
    }
}
