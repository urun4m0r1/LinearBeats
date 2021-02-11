#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;

namespace LinearBeats.Visuals
{
    public static class RailScroll
    {
        public static void UpdateRailPosition<T>(Queue<T> railBehaviours, ulong currentPulse, float meterPerPulse) where T : RailBehaviour
        {
            foreach (var railBehaviour in railBehaviours)
            {
                //TODO: BPM 정지 구현 ulong a = _script.Timings[0].PulseStopDuration;
                //TODO: BPM 역스크롤 구현 ulong a = _script.Timings[0].PulseReverseDuration (like a folded timeline!)
                //TODO: position multiplyer 대신에 note절대적인 거리 저장
                //TODO: 지금 상태면 currentPulse와의 거리에 비례해서 이동하는 비례식 구조

                float positionInMeter = meterPerPulse * (railBehaviour.Pulse - currentPulse);
                float zPosition = railBehaviour.PositionMultiplyer * positionInMeter;
                railBehaviour.SetZPosition(zPosition);
            }
        }
    }
}

