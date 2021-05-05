using JetBrains.Annotations;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    public sealed class DividerBehaviour : RailBehaviour
    {
        protected override Vector3 Position => new Vector3(0f, 0f, RailObject?.StartPosition ?? 0f);
        protected override Vector3 Scale => Vector3.one;
    }
}
