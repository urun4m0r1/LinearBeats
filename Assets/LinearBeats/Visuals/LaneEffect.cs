using System.Collections.Generic;
using Lean.Pool;
using LinearBeats.Input;
using LinearBeats.Input.Keyboard;
using LinearBeats.Judgement;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public sealed class LaneEffect : SerializedMonoBehaviour
    {
        [Required] [ListDrawerSettings(IsReadOnly = true)] [SerializeField]
        private LaneBeam[] _laneBeams = new LaneBeam[Keyboard.Cols];

        [OdinSerialize] private Dictionary<Judge, LeanGameObjectPool> _judgeEffects = new Dictionary<Judge, LeanGameObjectPool>
        {
            [Judge.Perfect] = null,
            [Judge.Great] = null,
            [Judge.Good] = null,
            [Judge.Bad] = null,
            [Judge.Miss] = null,
        };

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
                    bool isHoldingLayer = InputHandler.IsHolding(row: layer, col: lane);
                    _laneBeams[lane].ToggleLayerEffectWhenHolding(layer, isHoldingLayer);
                }
            }
        }

        public void OnJudge(Vector3 effectPosition, Judge judge)
        {
            if (judge != Judge.Null)
                _judgeEffects[judge].Spawn(effectPosition, Quaternion.identity, _judgeEffects[judge].transform);
        }
    }
}
