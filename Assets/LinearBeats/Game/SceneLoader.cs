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

#pragma warning disable IDE0044
        [Range(24, 240)]
        [SerializeField]
        private readonly byte _targetFps = 60;

        [Scene]
        [Required]
        [Header("Migration")]
        [SerializeField]
        private readonly string _dstScenePath = null;

        [SerializeField]
        private readonly LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        [Tag]
        [SerializeField]
        private readonly string _audioListenerTag = "MainCamera";

        [SerializeField]
        private List<GameObject> _migratingObjects = new List<GameObject>();
#pragma warning restore IDE0044

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

            MigrateExistingGameObjectsToScene(dstScene, _migratingObjects);

            if (_loadSceneMode == LoadSceneMode.Single)
            {
                UnloadAllScenesExcept(dstScene);
            }

            ValidateAudioListenerUniqueWithTag(_audioListenerTag);
        }

        private static void MigrateExistingGameObjectsToScene(Scene dstScene, List<GameObject> gameObjects)
        {
            if (!gameObjects.IsNullOrEmpty())
            {
                foreach (var migratingObject in gameObjects)
                {
                    if (migratingObject != null)
                    {
                        SceneManager.MoveGameObjectToScene(migratingObject, dstScene);
                    }
                }
            }
        }

        private static void UnloadAllScenesExcept(Scene targetScene)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene != targetScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        private static void ValidateAudioListenerUniqueWithTag(string tag)
        {
            var audioValidator = new AudioListenerValidator();
            audioValidator.ValidUniqueWithTag(tag);
        }
    }
}
