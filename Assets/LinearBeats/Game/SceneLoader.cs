using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LinearBeats.Game
{
    public sealed class SceneLoader : MonoBehaviour
    {
        [Range(24, 240)] [SerializeField] private byte targetFps = 60;
        [Scene] [Required] [Header("Migration")] [SerializeField] private string dstScenePath;
        [SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;
        [SerializeField] private List<GameObject> migratingObjects = new List<GameObject>();

        private void Awake()
        {
            Application.targetFrameRate = targetFps;
        }

        [DisableInEditorMode] [Button("LoadScene")] public void LoadScene()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadSceneAsync(dstScenePath, LoadSceneMode.Additive);
        }

        private void SceneLoaded(Scene dstScene, LoadSceneMode loadMode)
        {
            SceneManager.sceneLoaded -= SceneLoaded;

            MigrateExistingGameObjectsToScene(dstScene, migratingObjects);

            if (loadSceneMode == LoadSceneMode.Single) UnloadAllScenesExcept(dstScene);
        }

        private static void MigrateExistingGameObjectsToScene(Scene dstScene, List<GameObject> gameObjects)
        {
            if (gameObjects?.Count <= 0) return;

            foreach (var migratingObject in gameObjects.Where(migratingObject => migratingObject != null))
                SceneManager.MoveGameObjectToScene(migratingObject, dstScene);
        }

        private static void UnloadAllScenesExcept(Scene targetScene)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene != targetScene) SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}
