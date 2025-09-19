using System;

namespace VisionNet.Vision.AI
{
    [Serializable]
    public class RemoteClientInferenceResult
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
        public double ProcessingTime { get; set; }
    }
}
