#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinearBeats.Visuals
{
    [Serializable]
    public sealed class RailScroll
    {
#pragma warning disable IDE0044
        [Range(0.0001f, 10)]
        [SerializeField]
        private float _meterPerPulse = 0.01f;
#pragma warning restore IDE0044

        private readonly List<Queue<RailBehaviour>> _railsBehaviours = new List<Queue<RailBehaviour>>();

        public void AddRailBehaviours(Queue<RailBehaviour> railBehaviours)
        {
            _railsBehaviours.Add(railBehaviours);
        }

        public void UpdateRailPosition(ulong currentPulse)
        {
            foreach (var railBehaviours in _railsBehaviours)
            {
                foreach (var railBehaviour in railBehaviours)
                {
                    //TODO: BPM 정지 구현 ulong a = _script.Timings[0].PulseStopDuration;
                    //TODO: BPM 역스크롤 구현 ulong a = _script.Timings[0].PulseReverseDuration (like a folded timeline!)

                    float positionInMeter = _meterPerPulse * (railBehaviour.Pulse - currentPulse);
                    railBehaviour.SetZPosition(positionInMeter);
                }
            }
        }
    }
}

