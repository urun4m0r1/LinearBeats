#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Input;
using LinearBeats.Script;
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
#pragma warning disable IDE0044
        [DictionaryDrawerSettings(IsReadOnly = true)]
        [OdinSerialize]
        private Dictionary<Judge, ulong> _judgeOffset = new Dictionary<Judge, ulong>
        {
            [Judge.Perfect] = 20,
            [Judge.Great] = 40,
            [Judge.Good] = 60,
            [Judge.Bad] = 80,
        };

        [SerializeField]
        private LaneEffect _laneEffect = null;
#pragma warning restore IDE0044

        public bool JudgeNote(NoteBehaviour noteBehaviour, ulong currentPulse)
        {
            Judge? noteJudgement = GetJudge(noteBehaviour.Note, currentPulse);
            if (noteJudgement != null)
            {
                _laneEffect.OnJudge(noteBehaviour, (Judge)noteJudgement);
                return true;
            }
            return false;
        }

        public Judge? GetJudge(Note note, ulong currentPulse)
        {
            ulong pulsePassedAfterNote;
            ulong pulseLeftBeforeNote;
            if (currentPulse >= note.Pulse)
            {
                pulsePassedAfterNote = currentPulse - note.Pulse;
                pulseLeftBeforeNote = ulong.MaxValue;
            }
            else
            {
                pulsePassedAfterNote = 0;
                pulseLeftBeforeNote = note.Pulse - currentPulse;
            }

            if (InputHandler.IsNotePressed(note))
            {
                if (WithinJudge(_judgeOffset[Judge.Perfect]))
                {
                    return Judge.Perfect;
                }
                else if (WithinJudge(_judgeOffset[Judge.Great]))
                {
                    return Judge.Great;
                }
                else if (WithinJudge(_judgeOffset[Judge.Good]))
                {
                    return Judge.Good;
                }
                else if (WithinJudgeRange(_judgeOffset[Judge.Bad], _judgeOffset[Judge.Good]))
                {
                    return Judge.Bad;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (MissedJudge(_judgeOffset[Judge.Good]))
                {
                    return Judge.Miss;
                }
                else
                {
                    return null;
                }
            }

            bool WithinJudge(ulong judgeOffset)
            {
                return !(MissedJudge(judgeOffset) || PreJudge(judgeOffset));
            }

            bool WithinJudgeRange(ulong judgeOffsetStart, ulong judgeOffsetEnd)
            {
                Assert.IsTrue(judgeOffsetStart >= judgeOffsetEnd);
                return MissedJudge(judgeOffsetStart) && PreJudge(judgeOffsetEnd);
            }

            bool PreJudge(ulong judgeOffset)
            {
                return pulseLeftBeforeNote >= judgeOffset;
            }

            bool MissedJudge(ulong judgeOffset)
            {
                return pulsePassedAfterNote >= judgeOffset;
            }
        }
    }
}
