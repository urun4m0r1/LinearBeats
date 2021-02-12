#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;

namespace LinearBeats.Visuals
{
    public sealed class RailScroll
    {
        private readonly float _meterPerPulse = 0f;
        private readonly Queue<RailBehaviour>[] _railsBehaviours = null;

        public RailScroll(float meterPerPulse, Queue<RailBehaviour>[] railsBehaviours)
        {
            _meterPerPulse = meterPerPulse;
            _railsBehaviours = railsBehaviours;
        }

        public void UpdateRailPosition(ulong currentPulse)
        {
            foreach (var railBehaviours in _railsBehaviours)
            {
                foreach (var railBehaviour in railBehaviours)
                {
                    //TODO: BPM 정지 구현 ulong a = _script.Timings[0].PulseStopDuration;
                    //TODO: BPM 역스크롤 구현 ulong a = _script.Timings[0].PulseReverseDuration (like a folded timeline!)
                    //TODO: position multiplyer 대신에 note절대적인 거리 저장
                    //TODO: 지금 상태면 currentPulse와의 거리에 비례해서 이동하는 비례식 구조

                    float positionInMeter = _meterPerPulse * (railBehaviour.Pulse - currentPulse);
                    float zPosition = railBehaviour.PositionMultiplyer * positionInMeter;
                    railBehaviour.SetZPosition(zPosition);
                }
            }
        }
    }
}

