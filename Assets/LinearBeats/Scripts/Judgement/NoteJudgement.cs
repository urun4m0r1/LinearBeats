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
        private Dictionary<Judge, FixedTime> _judgeOffset = new Dictionary<Judge, FixedTime>
        {
            [Judge.Perfect] = (Second)0.033f,
            [Judge.Great] = (Second)0.066f,
            [Judge.Good] = (Second)0.133f,
            [Judge.Bad] = (Second)0.150f,
        };

        [SerializeField]
        private LaneEffect _laneEffect = null;
#pragma warning restore IDE0044

        public bool JudgeNote(NoteBehaviour noteBehaviour, FixedTime currentFixedTime)
        {
            FixedTime noteTime = noteBehaviour.FixedTime;
            Shape noteShape = noteBehaviour.Note.Shape;
            Judge? noteJudgement = GetJudge(noteTime, noteShape, currentFixedTime);
            if (noteJudgement != null)
            {
                _laneEffect.OnJudge(noteBehaviour, (Judge)noteJudgement);
                return true;
            }
            return false;
        }

        public Judge? GetJudge(FixedTime noteTime, Shape noteShape, FixedTime currentTime)
        {
            FixedTime elapsedTime;
            FixedTime remainingTime;
            if (currentTime >= noteTime)
            {
                elapsedTime = currentTime - noteTime;
                remainingTime = FixedTime.MaxValue;
            }
            else
            {
                elapsedTime = FixedTime.Zero;
                remainingTime = noteTime - currentTime;
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

            bool WithinJudge(FixedTime judgeOffset)
            {
                return !(MissedJudge(judgeOffset) || PreJudge(judgeOffset));
            }

            bool WithinJudgeRange(FixedTime judgeOffsetStart, FixedTime judgeOffsetEnd)
            {
                Assert.IsTrue(judgeOffsetStart >= judgeOffsetEnd);

                return MissedJudge(judgeOffsetStart) && PreJudge(judgeOffsetEnd);
            }

            bool PreJudge(FixedTime judgeOffset)
            {
                return remainingTime >= judgeOffset;
            }

            bool MissedJudge(FixedTime judgeOffset)
            {
                return elapsedTime >= judgeOffset;
            }
        }
    }
}

