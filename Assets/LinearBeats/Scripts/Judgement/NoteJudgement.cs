#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System;
using System.Collections.Generic;
using LinearBeats.Input;
using LinearBeats.Script;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

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
        [InlineProperty(LabelWidth = 90)]
        private struct JudgeData
        {
            [OdinSerialize]
            public ulong Timing { get; private set; }
            [SerializeField]
            public readonly UnityEvent JudgeEvent;

            public JudgeData(ulong timing)
            {
                Timing = timing;
                JudgeEvent = new UnityEvent();
            }
        }

#pragma warning disable IDE0044
        [DictionaryDrawerSettings(IsReadOnly = true)]
        [OdinSerialize]
        private Dictionary<Judge, JudgeData> _judgeDataTable = new Dictionary<Judge, JudgeData>
        {
            [Judge.Perfect] = new JudgeData(30),
            [Judge.Great] = new JudgeData(60),
            [Judge.Good] = new JudgeData(100),
            [Judge.Miss] = new JudgeData(150),
        };
#pragma warning restore IDE0044

        public NoteJudgement()
        {

        }

        public void JudgeNote(Note note, ulong currentPulse)
        {
            Judge noteJudgement = JudgeInputTiming(note, currentPulse);
            DisplayNoteJudgement(note, noteJudgement, currentPulse);


            static void DisplayNoteJudgement(Note note, Judge noteJudgement, ulong currentPulse)
            {
                if (noteJudgement != Judge.Miss)
                {
                    Debug.Log($"{noteJudgement}Row:{note.PositionRow}, Col:{note.PositionCol} " +
                    $"/ Note: {note.Pulse}, At: {currentPulse}");
                }
            }
        }
        public Judge JudgeInputTiming(Note note, ulong currentPulse)
        {
            if (InputHandler.IsNotePressed(note))
            {
                if (WithinNoteJudgeTiming(note, currentPulse, _judgeDataTable[Judge.Perfect].Timing))
                {
                    _judgeDataTable[Judge.Perfect].JudgeEvent.Invoke();
                    return Judge.Perfect;
                }
                else if (WithinNoteJudgeTiming(note, currentPulse, _judgeDataTable[Judge.Great].Timing))
                {
                    _judgeDataTable[Judge.Great].JudgeEvent.Invoke();
                    return Judge.Great;
                }
                else if (WithinNoteJudgeTiming(note, currentPulse, _judgeDataTable[Judge.Good].Timing))
                {
                    _judgeDataTable[Judge.Good].JudgeEvent.Invoke();
                    return Judge.Good;
                }
            }
            _judgeDataTable[Judge.Miss].JudgeEvent.Invoke();
            return Judge.Miss;
        }

        public bool ShouldJudgeNote(Note note, ulong currentPulse)
        {
            return WithinNoteJudgeTiming(note, currentPulse, _judgeDataTable[Judge.Miss].Timing);
        }

        private static bool WithinNoteJudgeTiming(Note note, ulong currentPulse, ulong offset)
        {
            var diff = currentPulse - note.Pulse;
            return Mathf.Abs(diff) <= offset;
        }
    }
}
