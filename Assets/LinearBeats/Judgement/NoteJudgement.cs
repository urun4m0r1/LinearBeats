using System.Collections.Generic;
using JetBrains.Annotations;
using LinearBeats.Input;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

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

    [HideReferenceObjectPicker]
    public sealed class NoteJudgement
    {
        [DictionaryDrawerSettings(IsReadOnly = true)]
        [OdinSerialize]
        private Dictionary<Judge, float> _judgeRangeInSeconds = new Dictionary<Judge, float>
        {
            [Judge.Perfect] = 0.033f,
            [Judge.Great] = 0.066f,
            [Judge.Good] = 0.133f,
            [Judge.Bad] = 0.150f,
        };

        [SerializeField] private LaneEffect _laneEffect = null;

        //TODO: 늦게치면 무조건 miss인 현상 해결
        //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
        public bool JudgeNote([NotNull] RailObject railObject, Shape noteShape, Vector3 effectPosition)
        {
            var noteJudgement = GetJudge(railObject.CurrentTime, railObject.StartTime, noteShape);
            if (noteJudgement == null) return false;

            _laneEffect.OnJudge(effectPosition, (Judge) noteJudgement);
            return true;
        }

        private Judge? GetJudge(FixedTime currentTime, FixedTime targetTime, Shape noteShape)
        {
            Second elapsedTime;
            Second remainingTime;
            if (currentTime >= targetTime)
            {
                elapsedTime = currentTime - targetTime;
                remainingTime = float.MaxValue;
            }
            else
            {
                elapsedTime = 0f;
                remainingTime = targetTime - currentTime;
            }

            if (InputHandler.IsNotePressed(noteShape))
            {
                if (WithinJudge(_judgeRangeInSeconds[Judge.Perfect])) return Judge.Perfect;
                if (WithinJudge(_judgeRangeInSeconds[Judge.Great])) return Judge.Great;
                if (WithinJudge(_judgeRangeInSeconds[Judge.Good])) return Judge.Good;
                if (WithinJudgeRange(_judgeRangeInSeconds[Judge.Bad], _judgeRangeInSeconds[Judge.Good])) return Judge.Bad;

                return null;
            }

            if (MissedJudge(_judgeRangeInSeconds[Judge.Good])) return Judge.Miss;

            return null;

            bool WithinJudge(Second judgeOffset) => !(MissedJudge(judgeOffset) || PreJudge(judgeOffset));

            bool WithinJudgeRange(Second judgeOffsetStart, Second judgeOffsetEnd)
            {
                Assert.IsTrue(judgeOffsetStart >= judgeOffsetEnd);

                return MissedJudge(judgeOffsetStart) && PreJudge(judgeOffsetEnd);
            }

            bool PreJudge(Second judgeOffset) => remainingTime >= judgeOffset;

            bool MissedJudge(Second judgeOffset) => elapsedTime >= judgeOffset;
        }
    }
}
