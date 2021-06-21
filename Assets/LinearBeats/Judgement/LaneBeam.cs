using JetBrains.Annotations;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public sealed class LaneBeam : MonoBehaviour
    {
        [SerializeField] [CanBeNull] private GameObject lanePressedEffect;
        public void ToggleEffect(bool isHolding)
        {
            if (lanePressedEffect) lanePressedEffect.SetActive(isHolding);
        }
    }
}
