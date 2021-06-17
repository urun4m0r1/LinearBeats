using LinearBeats.Input.Keyboard;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public sealed class LaneBeam : MonoBehaviour
    {
        [Required] [SerializeField]
        private GameObject lanePressedEffect;

        public void ToggleLayerEffectWhenHolding(KeyType key, bool isHolding)
        {
            lanePressedEffect.SetActive(isHolding);
        }
    }
}
