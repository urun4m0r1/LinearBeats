using System.Collections.Generic;
using JetBrains.Annotations;
using LinearBeats.Script;
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
        Null,
    }

    [CreateAssetMenu(menuName = "LinearBeats/JudgeRange")]
    public sealed class JudgeRange : SerializedScriptableObject
    {
        [OdinSerialize, DisableContextMenu, DictionaryDrawerSettings(IsReadOnly = true)]
        [NotNull] private Dictionary<Judge, float> _judgeRangeInSeconds = new Dictionary<Judge, float>
        {
            [Judge.Perfect] = 0.033f,
            [Judge.Great] = 0.067f,
            [Judge.Good] = 0.133f,
            [Judge.Bad] = 0.267f,
        };

        public Second Range(Judge judge) => _judgeRangeInSeconds[judge];
    }
}
