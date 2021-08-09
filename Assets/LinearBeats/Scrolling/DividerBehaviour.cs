using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    public sealed class DividerBehaviour : RailBehaviour
    {
        [ShowInInspector, ReadOnly]
        protected override Vector3 Position => new Vector3(0f, 0f, RailObject?.StartPosition ?? 0f);

        [ShowInInspector, ReadOnly]
        protected override Vector3 Scale => Vector3.one;

        [ShowInInspector, ReadOnly]
        protected override bool RailDisabled => RailObject != null && RailObject.CurrentTime > RailObject.EndTime;
    }
}
