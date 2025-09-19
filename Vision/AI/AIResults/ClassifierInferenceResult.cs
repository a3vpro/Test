namespace VisionNet.Vision.AI
{
    public class ClassifierInferenceResult: RemoteClientInferenceResult
    {
        public int ClassIndex { get; set; }
        public string ClassName { get; set; }
        public double Score { get; set; }
    }
}
