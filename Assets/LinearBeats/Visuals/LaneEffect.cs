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
        private LaneBeam[] laneBeams = new LaneBeam[Keyboard.Length];

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
            for (KeyType lane = 0; lane < (KeyType) Keyboard.Length; ++lane)
            {
                var isHoldingLayer = InputHandler.IsHolding(lane);

                laneBeams[(int) lane].ToggleLayerEffectWhenHolding(lane, isHoldingLayer);
            }
        }

        public void OnJudge(Vector3 effectPosition, Judge judge)
        {
            if (judge != Judge.Null)
                _judgeEffects[judge].Spawn(effectPosition, Quaternion.identity, _judgeEffects[judge].transform);
        }
    }
}
