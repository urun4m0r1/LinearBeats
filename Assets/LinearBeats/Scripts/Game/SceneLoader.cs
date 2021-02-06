#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Extensions;
using Utils.Unity;

namespace LinearBeats.Game
{
    public sealed class SceneLoader : MonoBehaviour
    {
        [Range(24, 240)]
        [SerializeField]
        private byte _targetFps = 60;

        [Scene]
        [Required]
        [Header("Migration")]
        [SerializeField]
        private string _dstScenePath = null;

        [SerializeField]
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        [Tag]
        [SerializeField]
        private string _audioListenerTag = "MainCamera";

        [SerializeField]
        private List<GameObject> _migratingObjects = new List<GameObject>();

        private void Awake()
        {
            Application.targetFrameRate = _targetFps;
        }

        [DisableInEditorMode]
        [Button("LoadScene")]
        public void LoadScene()
        {
            SceneManager.sceneLoaded += SceneLoaded;

            SceneManager.LoadSceneAsync(_dstScenePath, LoadSceneMode.Additive);
        }

        private void SceneLoaded(Scene dstScene, LoadSceneMode loadMode)
        {
            SceneManager.sceneLoaded -= SceneLoaded;

            MigrateExistingGameObjectsToScene(dstScene);

            if (_loadSceneMode == LoadSceneMode.Single)
            {
                UnloadAllScenesExcept(dstScene);
            }

            ValidateAudioListenerUniqueWithTag();
        }

        private void MigrateExistingGameObjectsToScene(Scene dstScene)
        {
            if (!_migratingObjects.IsNullOrEmpty())
            {
                foreach (var migratingObject in _migratingObjects)
                {
                    if (migratingObject != null)
                    {
                        SceneManager.MoveGameObjectToScene(migratingObject, dstScene);
                    }
                }
            }
        }

        private void UnloadAllScenesExcept(Scene targetScene)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene != targetScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        private void ValidateAudioListenerUniqueWithTag()
        {
            var audioValidator = new AudioListenerValidator();
            audioValidator.ValidUniqueWithTag(_audioListenerTag);
        }
    }
}
