namespace VisionNet.Vision.Core
{
    public enum VisionPipelineStatus
    {
        Initial,  // Not working | Not terminating | Input messages are not allowed | Waiting for opened    |
        Opened,   // Working     | Not terminating | Input messages are allowed     | Waiting for closed    |
        Closed,   // Working     | Not terminating | Input messages are not allowed | Waiting for completed | Pending messages queued
        Purging,  // Working     | Terminating     | Input messages are not allowed | Waiting for completed | Pending messages queued
        Completed // Not working | Not terminating | Input messages are not allowed | Waiting for opened    | Not pending messages queued
    }
}
