#pragma warning disable IDE0090
#pragma warning disable IDE0051
using System.Collections.Generic;
using LinearBeats.Input;
using LinearBeats.Script;
using LinearBeats.Time;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace LinearBeats.Judgement
{
    [HideReferenceObjectPicker]
    public sealed class NoteJudgement
    {
#pragma warning disable IDE0044
        /* NOTE: 직렬화랑 게임 로직을 구분해서 사용하자
        [Serializeable]
        public class MonsterData
        {
            몬스터 관련 변수데이터
            public Vector3 Position => transform.position;
            public int Health { get; set; } ...
        }
        public class Monster
        {
            public MonsterData data;

        }
        */
        [DictionaryDrawerSettings(IsReadOnly = true)]
        [OdinSerialize]
        private Dictionary<Judge, float> _judgeRangeInSeconds = new Dictionary<Judge, float>
        {
            [Judge.Perfect] = 0.033f,
            [Judge.Great] = 0.066f,
            [Judge.Good] = 0.133f,
            [Judge.Bad] = 0.150f,
        };

        [SerializeField]
        private LaneEffect _laneEffect = null;
#pragma warning restore IDE0044

        public bool JudgeNote(NoteBehaviour noteBehaviour, FixedTime currentTime)
        {
            FixedTime noteTime = noteBehaviour.FixedTime;
            Shape noteShape = noteBehaviour.Note.Shape;
            Judge? noteJudgement = GetJudge(noteTime, noteShape, currentTime);
            if (noteJudgement != null)
            {
                _laneEffect.OnJudge(noteBehaviour, (Judge)noteJudgement);
                return true;
            }
            return false;
        }

        public Judge? GetJudge(FixedTime noteTime, Shape noteShape, FixedTime currentTime)
        {
            Second elapsedTime;
            Second remainingTime;
            if (currentTime >= noteTime)
            {
                elapsedTime = currentTime - noteTime;
                remainingTime = float.MaxValue;
            }
            else
            {
                elapsedTime = 0f;
                remainingTime = noteTime - currentTime;
            }

            if (InputHandler.IsNotePressed(noteShape))
            {
                if (WithinJudge(_judgeRangeInSeconds[Judge.Perfect])) return Judge.Perfect;
                else if (WithinJudge(_judgeRangeInSeconds[Judge.Great])) return Judge.Great;
                else if (WithinJudge(_judgeRangeInSeconds[Judge.Good])) return Judge.Good;
                else if (WithinJudgeRange(_judgeRangeInSeconds[Judge.Bad], _judgeRangeInSeconds[Judge.Good])) return Judge.Bad;
                else return null;
            }
            else
            {
                if (MissedJudge(_judgeRangeInSeconds[Judge.Good])) return Judge.Miss;
                else return null;
            }

            bool WithinJudge(Second judgeOffset)
            {
                return !(MissedJudge(judgeOffset) || PreJudge(judgeOffset));
            }

            bool WithinJudgeRange(Second judgeOffsetStart, Second judgeOffsetEnd)
            {
                Assert.IsTrue(judgeOffsetStart >= judgeOffsetEnd);

                return MissedJudge(judgeOffsetStart) && PreJudge(judgeOffsetEnd);
            }

            bool PreJudge(Second judgeOffset)
            {
                return remainingTime >= judgeOffset;
            }

            bool MissedJudge(Second judgeOffset)
            {
                return elapsedTime >= judgeOffset;
            }
        }
    }
}

