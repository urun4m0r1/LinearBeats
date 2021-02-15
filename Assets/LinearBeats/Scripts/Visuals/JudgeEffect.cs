using Lean.Pool;
using UnityEngine;

public class JudgeEffect : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        LeanPool.Despawn(gameObject);
    }
}
