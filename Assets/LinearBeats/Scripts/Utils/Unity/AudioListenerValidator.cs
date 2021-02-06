using UnityEngine;
using Utils.Extensions;

namespace Utils.Unity
{
    public sealed class AudioListenerValidator
    {
        private readonly AudioListener[] audioListeners = null;

        public AudioListenerValidator()
        {
            audioListeners = Object.FindObjectsOfType<AudioListener>();

            if (audioListeners.IsNullOrEmpty())
            {
                NullOrEmptyAudioListenerMessage();
            }
        }

        private void NullOrEmptyAudioListenerMessage()
        {
            Debug.LogWarning($"There are no Audio Listeners in the scene!");
        }

        public void ValidUniqueWithTag(string audioListenerTag)
        {
            if (audioListeners.IsNullOrEmpty())
            {
                NullOrEmptyAudioListenerMessage();
                return;
            }

            bool foundFirstAudioListenerWithTag = false;
            foreach (var audioListener in audioListeners)
            {
                if (audioListener.CompareTag(audioListenerTag))
                {
                    if (foundFirstAudioListenerWithTag)
                    {
                        audioListener.enabled = false;
                    }
                    else
                    {
                        audioListener.enabled = true;
                        foundFirstAudioListenerWithTag = true;
                    }
                }
                else
                {
                    audioListener.enabled = false;
                }
            }

            if (!foundFirstAudioListenerWithTag)
            {
                Debug.LogWarning($"There are no Audio Listeners with tag:{audioListenerTag} in the scene!");
            }
        }

        public void ValidUnique()
        {
            if (audioListeners.IsNullOrEmpty())
            {
                NullOrEmptyAudioListenerMessage();
                return;
            }

            bool foundFirstAudioListener = false;
            foreach (var audioListener in audioListeners)
            {
                if (foundFirstAudioListener)
                {
                    audioListener.enabled = false;
                }
                else
                {
                    audioListener.enabled = true;
                    foundFirstAudioListener = true;
                }
            }
        }

        public void MakeUnique(AudioListener uniqueAudioListener)
        {
            if (!audioListeners.IsNullOrEmpty())
            {
                foreach (var audioListener in audioListeners)
                {
                    audioListener.enabled = false;
                }
            }
            uniqueAudioListener.enabled = true;
        }
    }
}