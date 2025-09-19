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
using VisionNet.Core.States;

namespace VisionNet.Vision.Core
{
    public class PipelineStatusTransition : StatusValidator<VisionPipelineStatus>
    {
        public PipelineStatusTransition()
        {
            AddTransition(VisionPipelineStatus.Initial, VisionPipelineStatus.Opened);
            AddTransition(VisionPipelineStatus.Opened, VisionPipelineStatus.Closed);
            AddTransition(VisionPipelineStatus.Opened, VisionPipelineStatus.Purging);
            AddTransition(VisionPipelineStatus.Closed, VisionPipelineStatus.Purging);
            AddTransition(VisionPipelineStatus.Closed, VisionPipelineStatus.Completed);
            AddTransition(VisionPipelineStatus.Purging, VisionPipelineStatus.Completed);
            AddTransition(VisionPipelineStatus.Completed, VisionPipelineStatus.Opened);
        }
    }
}
