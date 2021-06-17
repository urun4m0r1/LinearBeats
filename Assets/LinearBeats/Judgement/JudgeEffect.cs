using Lean.Pool;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public sealed class JudgeEffect : MonoBehaviour
    {
        public void OnParticleSystemStopped()
        {
            LeanPool.Despawn(gameObject);
        }
    }
}
