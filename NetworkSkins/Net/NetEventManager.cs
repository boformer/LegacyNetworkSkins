using ICities;

namespace NetworkSkins.Net
{
    public class NetEventManager : LoadingExtensionBase
    {
        public static NetEventManager instance;
        
        public delegate void SegmentTransferDataEventHandler(ushort oldSegment, ushort newSegment);
        public event SegmentTransferDataEventHandler eventSegmentTransferData;

        public delegate void SegmentCreateEventHandler(ushort segment);
        public event SegmentCreateEventHandler eventSegmentCreate;

        public delegate void SegmentReleaseEventHandler(ushort segment);
        public event SegmentReleaseEventHandler eventSegmentRelease;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            instance = this;

            NetManagerDetour.Deploy();
        }

        public override void OnReleased()
        {
            base.OnReleased();

            NetManagerDetour.Revert();

            eventSegmentTransferData = null;
            eventSegmentCreate = null;
            eventSegmentRelease = null;

            instance = null;
        }

        public void PostTransferDataEvent(ushort oldSegment, ushort newSegment) 
        {
            if(eventSegmentTransferData != null) eventSegmentTransferData(oldSegment, newSegment);
        }

        public void PostCreateEvent(ushort segment)
        {
            if(eventSegmentCreate != null) eventSegmentCreate(segment);
        }

        public void PostReleaseEvent(ushort segment)
        {
            if(eventSegmentRelease != null) eventSegmentRelease(segment);
        }
    }
}
