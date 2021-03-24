#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using Lean.Pool;
using LinearBeats.Input;
using LinearBeats.Judgement;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Visuals
{

    public sealed class LaneEffect : SerializedMonoBehaviour
    {
#pragma warning disable IDE0044
        [Required]
        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField]
        private LaneBeam[] _laneBeams = new LaneBeam[Keyboard.Cols];

        [OdinSerialize]
        private Dictionary<Judge, LeanGameObjectPool> _judgeEffects = new Dictionary<Judge, LeanGameObjectPool>
        {
            [Judge.Perfect] = null,
            [Judge.Great] = null,
            [Judge.Good] = null,
            [Judge.Bad] = null,
            [Judge.Miss] = null,
        };
#pragma warning restore IDE0044

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

        public void OnJudge(NoteBehaviour noteBehaviour, Judge judge)
        {
            var judgeEffectPosition = new Vector3(
                noteBehaviour.JudgeEffectAnchor.position.x,
                noteBehaviour.JudgeEffectAnchor.position.y,
                0f);

            _judgeEffects[judge].Spawn(
                judgeEffectPosition,
                noteBehaviour.JudgeEffectAnchor.rotation,
                _judgeEffects[judge].transform);
        }
    }
}
