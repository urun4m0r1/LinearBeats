#pragma warning disable IDE0090
#pragma warning disable IDE0051

using LinearBeats.Input;
using LinearBeats.Script;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public static class NoteJudgement
    {
        public static void UpdateNoteJudgement(Note[] notes, ulong currentPulse)
        {
            //TODO: NoteJudge dequeuing
            foreach (var note in notes)
            {
                Judge noteJudgement = InputHandler.JudgeNote(note, currentPulse);
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
    }
}
