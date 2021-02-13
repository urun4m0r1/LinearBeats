#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Input;
using LinearBeats.Script;
using LinearBeats.Visuals;
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
        Miss,
    }

    [HideReferenceObjectPicker]
    public sealed class NoteJudgement
    {
#pragma warning disable IDE0044
        [DictionaryDrawerSettings(IsReadOnly = true)]
        [OdinSerialize]
        private Dictionary<Judge, ulong> _judgeTiming = new Dictionary<Judge, ulong>
        {
            [Judge.Perfect] = 30,
            [Judge.Great] = 60,
            [Judge.Good] = 100,
            [Judge.Miss] = 150,
        };

        [SerializeField]
        private LaneEffect _laneEffect = null;
#pragma warning restore IDE0044

        public void JudgeNote(NoteBehaviour noteBehaviour, ulong currentPulse)
        {
            if (ShouldJudgeNote(noteBehaviour.Note, currentPulse))
            {
                Judge noteJudgement = GetJudge(noteBehaviour.Note, currentPulse);
                _laneEffect.OnJudge(noteBehaviour, noteJudgement);
            }
        }

        public Judge GetJudge(Note note, ulong currentPulse)
        {
            if (InputHandler.IsNotePressed(note))
            {
                if (WithinNoteJudgeTiming(note, currentPulse, _judgeTiming[Judge.Perfect]))
                {
                    return Judge.Perfect;
                }
                else if (WithinNoteJudgeTiming(note, currentPulse, _judgeTiming[Judge.Great]))
                {
                    return Judge.Great;
                }
                else if (WithinNoteJudgeTiming(note, currentPulse, _judgeTiming[Judge.Good]))
                {
                    return Judge.Good;
                }
            }
            return Judge.Miss;
        }

        public bool ShouldJudgeNote(Note note, ulong currentPulse)
        {
            return WithinNoteJudgeTiming(note, currentPulse, _judgeTiming[Judge.Miss]);
        }

        private static bool WithinNoteJudgeTiming(Note note, ulong currentPulse, ulong offset)
        {
            var diff = currentPulse - note.Pulse;
            return Mathf.Abs(diff) <= offset;
        }
    }
}
