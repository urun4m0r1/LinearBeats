using System.Collections.Generic;
using LinearBeats.Time;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public enum Judge : byte
    {
        Perfect,
        Great,
        Good,
        Bad,
        Miss,
    }

    [CreateAssetMenu(menuName = "LinearBeats/JudgeRange")]
    public sealed class JudgeRange : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<Judge, float> _judgeRangeInSeconds = new Dictionary<Judge, float>
        {
            [Judge.Perfect] = 0.033f,
            [Judge.Great] = 0.067f,
            [Judge.Good] = 0.133f,
            [Judge.Bad] = 0.267f,
        };

        public Second Range(Judge judge) => _judgeRangeInSeconds[judge];
    }
}
