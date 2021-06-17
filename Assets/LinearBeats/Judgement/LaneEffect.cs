using System.Collections.Generic;
using Lean.Pool;
using LinearBeats.Input;
using LinearBeats.Input.Keyboard;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Judgement
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

        public void OnJudge(Transform effectAnchor, Judge judge)
        {
            if (judge == Judge.Null) return;

            var effectPosition = new Vector3(effectAnchor.position.x, 0f, 0f);
            _judgeEffects[judge].Spawn(effectPosition, Quaternion.identity, _judgeEffects[judge].transform);
        }
    }
}
