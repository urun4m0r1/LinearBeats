using UnityEngine;

namespace LinearBeats.Scrolling
{
    public sealed class DividerBehaviour : RailBehaviour
    {
        protected override Vector3 Position => new Vector3(0f, 0f, RailObject?.StartPosition ?? 0f);
        protected override Vector3 Scale => Vector3.one;

        protected override bool RailDisabled
        {
            get
            {
                if (RailObject == null) return false;

                return RailObject.CurrentTime > RailObject.EndTime;
            }
        }
    }
}
