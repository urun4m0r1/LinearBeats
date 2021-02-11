#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Input;
using LinearBeats.Script;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public static class NoteJudgement
    {
        private static readonly Dictionary<Judge, ulong> judgeOffsetTable = new Dictionary<Judge, ulong>
        {
            [Judge.Perfect] = 30,
            [Judge.Great] = 60,
            [Judge.Good] = 100,
            [Judge.Miss] = 130,
            [Judge.Null] = 0,
        };
        public static void UpdateNoteJudgement(Note[] notes, ulong currentPulse)
        {
            //TODO: NoteJudge dequeuing
            foreach (var note in notes)
            {
                Judge noteJudgement = JudgeInputTiming(note, currentPulse);
                DisplayNoteJudgement(note, noteJudgement, currentPulse);
            }

            static void DisplayNoteJudgement(Note note, Judge noteJudgement, ulong currentPulse)
            {
                if (noteJudgement != Judge.Null && noteJudgement != Judge.Miss)
                {
                    Debug.Log($"{noteJudgement}Row:{note.PositionRow}, Col:{note.PositionCol} " +
                    $"/ Note: {note.Pulse}, At: {currentPulse}");
                }
            }
        }
        public static Judge JudgeInputTiming(Note note, ulong currentPulse)
        {
            if (InputHandler.IsNotePressed(note))
            {
                if (WithinNoteJudgeTiming(note, currentPulse, judgeOffsetTable[Judge.Perfect]))
                {
                    return Judge.Perfect;
                }
                else if (WithinNoteJudgeTiming(note, currentPulse, judgeOffsetTable[Judge.Great]))
                {
                    return Judge.Great;
                }
                else if (WithinNoteJudgeTiming(note, currentPulse, judgeOffsetTable[Judge.Good]))
                {
                    return Judge.Good;
                }
            }

            if (WithinNoteJudgeTiming(note, currentPulse, judgeOffsetTable[Judge.Miss]))
            {
                return Judge.Miss;
            }

            return Judge.Null;

            static bool WithinNoteJudgeTiming(Note note, ulong currentPulse, ulong offset)
            {
                var diff = currentPulse - note.Pulse;
                return Mathf.Abs(diff) <= offset;
            }
        }
    }
}
