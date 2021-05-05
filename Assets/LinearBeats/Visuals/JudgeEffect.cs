using Lean.Pool;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public sealed class JudgeEffect : MonoBehaviour
    {
        public void OnParticleSystemStopped()
        {
            LeanPool.Despawn(gameObject);
        }
    }
}
