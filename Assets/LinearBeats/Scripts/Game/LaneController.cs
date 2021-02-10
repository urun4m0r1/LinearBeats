using LinearBeats.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Game
{
    public sealed class LaneController : MonoBehaviour
    {
        [Required]
        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField]
        private GameObject[] _lanePressedEffects = new GameObject[Keyboard.Rows];

        public void ToggleLayerEffectWhenHolding(byte layer, bool isHolding)
        {
            _lanePressedEffects[layer].SetActive(isHolding);
        }
    }
}
