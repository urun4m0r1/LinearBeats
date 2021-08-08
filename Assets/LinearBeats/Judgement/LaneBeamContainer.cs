using System.Collections.Generic;
using JetBrains.Annotations;
using LinearBeats.Controls;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace LinearBeats.Judgement
{
    public sealed class LaneBeamContainer : SerializedMonoBehaviour
    {
        [OdinSerialize, DisableContextMenu, DictionaryDrawerSettings(IsReadOnly = true)] [NotNull]
        private Dictionary<KeyType, LaneBeam> _laneBeams = new Dictionary<KeyType, LaneBeam>
        {
            [KeyType.LShift] = null,
            [KeyType.Z] = null,
            [KeyType.X] = null,
            [KeyType.C] = null,
            [KeyType.V] = null,
            [KeyType.B] = null,
            [KeyType.N] = null,
            [KeyType.M] = null,
            [KeyType.Comma] = null,
            [KeyType.Period] = null,
            [KeyType.Slash] = null,
            [KeyType.RShift] = null,
            [KeyType.Space] = null,
            [KeyType.LAlt] = null,
            [KeyType.RAlt] = null,
        };

        //TODO: 판정에 따라 색 다르게 하기
        public void UpdateAll([NotNull] InputReceiver inputReceiver)
        {
            for (KeyType i = 0; i < (KeyType) _laneBeams.Count; ++i)
            {
                var isHoldingLayer = inputReceiver.GetBindingOrAlternative(i);
                var laneBeam = _laneBeams[i];

                if (laneBeam) laneBeam.ToggleEffect(isHoldingLayer);
            }
        }
    }
}
