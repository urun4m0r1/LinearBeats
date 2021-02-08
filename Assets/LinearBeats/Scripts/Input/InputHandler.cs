#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Game;
using LinearBeats.Script;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Input
{
    public enum Judge : byte
    {
        Perfect,
        Great,
        Good,
        Miss,
        Null,
    }

    public sealed class InputHandler : SerializedMonoBehaviour
    {
        [PropertyOrder(-1)]
        [Required]
        [OdinSerialize]
        public Keyboard CurrentKeyboard
        {
            get => InputListener.BindingProvider as Keyboard;
            set => InputListener.BindingProvider = value;
        }

        [Required]
        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField]
        private Lane[] _lanes = new Lane[Keyboard.Cols];

        private static readonly InputListener s_pressedListener = new InputListener(new PressedReceiver());
        private static readonly InputListener s_releasedListener = new InputListener(new ReleasedReceiver());
        private static readonly InputListener s_holdingListener = new InputListener(new HoldingReceiver());

        private void Start()
        {
            InputListener.BindingProvider = CurrentKeyboard;
        }

        [HideInEditorMode]
        [Button]
        public void ChangeKeyboard(Keyboard keyboard)
        {
            CurrentKeyboard = keyboard;
        }

        private void Update()
        {
            UpdateLaneEffects();

            void UpdateLaneEffects()
            {
                for (byte layer = 0; layer < Keyboard.Rows; ++layer)
                {
                    for (byte lane = 0; lane < Keyboard.Cols; ++lane)
                    {
                        _lanes[lane].ToggleLayerEffectWhenHolding(layer, IsHolding(row: layer, col: lane));
                    }
                }

                static bool IsHolding(byte row, byte col)
                {
                    return s_holdingListener.IsBindingInvoked(row, col);
                }
            }
        }

        public static Judge JudgeNote(Note note, ulong currentPulse, Dictionary<Judge, ulong> judgeOffsetTable)
        {
            bool isNotePressed = s_pressedListener.GetNoteInvoked(note).Exist;
            if (isNotePressed)
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
                return note.Pulse <= currentPulse + offset && currentPulse <= note.Pulse + offset;
            }
        }
    }
}
