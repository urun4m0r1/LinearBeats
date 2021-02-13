
#pragma warning disable IDE0051
#pragma warning disable IDE0090

using LinearBeats.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public sealed class LaneBeam : MonoBehaviour
    {
#pragma warning disable IDE0044
        [Required]
        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField]
        private GameObject[] _lanePressedEffects = new GameObject[Keyboard.Rows];
#pragma warning restore IDE0044

        public void ToggleLayerEffectWhenHolding(byte layer, bool isHolding)
        {
            _lanePressedEffects[layer].SetActive(isHolding);
        }
    }
}
