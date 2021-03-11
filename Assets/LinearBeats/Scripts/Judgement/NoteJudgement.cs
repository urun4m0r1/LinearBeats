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
        [DictionaryDrawerSettings(IsReadOnly = true)]
        [OdinSerialize]
        private Dictionary<Judge, float> _judgeOffset = new Dictionary<Judge, float>
        {
            [Judge.Perfect] = 0.033f,
            [Judge.Great] = 0.066f,
            [Judge.Good] = 0.133f,
            [Judge.Bad] = 0.150f,
        };

        [SerializeField]
        private LaneEffect _laneEffect = null;
#pragma warning restore IDE0044

        public bool JudgeNote(NoteBehaviour noteBehaviour, Second currentSecond)
        {
            Second noteSecond = noteBehaviour.FixedTime.Second;
            Shape noteShape = noteBehaviour.Note.Shape;
            Judge? noteJudgement = GetJudge(noteSecond, noteShape, currentSecond);
            if (noteJudgement != null)
            {
                _laneEffect.OnJudge(noteBehaviour, (Judge)noteJudgement);
                return true;
            }
            return false;
        }

        public Judge? GetJudge(Second noteSecond, Shape noteShape, Second currentSecond)
        {
            Second elapsed;
            Second remaining;
            if (currentSecond >= noteSecond)
            {
                elapsed = currentSecond - noteSecond;
                remaining = float.MaxValue;
            }
            else
            {
                elapsed = 0f;
                remaining = noteSecond - currentSecond;
            }

            if (InputHandler.IsNotePressed(noteShape))
            {
                if (WithinJudge(_judgeOffset[Judge.Perfect])) return Judge.Perfect;
                else if (WithinJudge(_judgeOffset[Judge.Great])) return Judge.Great;
                else if (WithinJudge(_judgeOffset[Judge.Good])) return Judge.Good;
                else if (WithinJudgeRange(_judgeOffset[Judge.Bad], _judgeOffset[Judge.Good])) return Judge.Bad;
                else return null;
            }
            else
            {
                if (MissedJudge(_judgeOffset[Judge.Good])) return Judge.Miss;
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
                return remaining >= judgeOffset;
            }

            bool MissedJudge(Second judgeOffset)
            {
                return elapsed >= judgeOffset;
            }
        }
    }
}

