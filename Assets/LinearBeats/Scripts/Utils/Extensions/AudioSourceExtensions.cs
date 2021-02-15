using UnityEngine;

namespace Utils.Extensions
{
    public static class AudioSourceExtensions
    {
        public static void Reset(this AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.Play();
            audioSource.Pause();
        }
    }
}
