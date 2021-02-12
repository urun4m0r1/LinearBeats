#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Script;
using LinearBeats.Visuals;
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

    public sealed class InputHandler : MonoBehaviour
    {
#pragma warning disable IDE0044
        [Required]
        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField]
        private LaneBeam[] _laneBeams = new LaneBeam[Keyboard.Cols];
#pragma warning restore IDE0044

        private static readonly UserInputListener s_pressedListener = new UserInputListener(new PressedReceiver());
        private static readonly UserInputListener s_holdingListener = new UserInputListener(new HoldingReceiver());

        private void Update()
        {
            UpdateLaneEffects();
        }

        private void UpdateLaneEffects()
        {
            for (byte layer = 0; layer < Keyboard.Rows; ++layer)
            {
                for (byte lane = 0; lane < Keyboard.Cols; ++lane)
                {
                    bool isHoldingLayer = IsHolding(row: layer, col: lane);
                    _laneBeams[lane].ToggleLayerEffectWhenHolding(layer, isHoldingLayer);
                }
            }

            static bool IsHolding(byte row, byte col)
            {
                return s_holdingListener.IsInputInvoked(row, col);
            }
        }

        public static bool IsNotePressed(Note note)
        {
            return s_pressedListener.GetNoteInvoked(note).Exist;
        }
    }
}
