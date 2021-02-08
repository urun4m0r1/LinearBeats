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
            get => UserInputListener.BindingProvider as Keyboard;
            set => UserInputListener.BindingProvider = value;
        }

        [Required]
        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField]
        private Lane[] _lanes = new Lane[Keyboard.Cols];

        private static readonly UserInputListener s_pressedListener = new UserInputListener(new PressedReceiver());
        private static readonly UserInputListener s_releasedListener = new UserInputListener(new ReleasedReceiver());
        private static readonly UserInputListener s_holdingListener = new UserInputListener(new HoldingReceiver());

        private static readonly Dictionary<Judge, ulong> judgeOffsetTable = new Dictionary<Judge, ulong>
        {
            [Judge.Perfect] = 30,
            [Judge.Great] = 60,
            [Judge.Good] = 100,
            [Judge.Miss] = 130,
            [Judge.Null] = 0,
        };

        private void Start()
        {
            UserInputListener.BindingProvider = CurrentKeyboard;
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
                    return s_holdingListener.IsInputInvoked(row, col);
                }
            }
        }

        public static Judge JudgeNote(Note note, ulong currentPulse)
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
