using System;
using UnityEngine;
using Utils.Extensions;

namespace Utils.Unity
{
    public static class AudioUtils
    {
        public static int[] GetSamplesPerTimes(AudioSource[] audioSources)
        {
            int[] audioFrequencies = new int[audioSources.Length];
            for (var i = 0; i < audioFrequencies.Length; ++i)
            {
                audioFrequencies[i] = audioSources[i].clip.frequency;
            }
            return audioFrequencies;
        }
    }
}
