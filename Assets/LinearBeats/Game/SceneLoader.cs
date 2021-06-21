using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LinearBeats.Game
{
    public sealed class SceneLoader
    {
        [Scene] [NotNull] private readonly string _dstScenePath;
        [ShowInInspector, ReadOnly] private readonly LoadSceneMode _loadSceneMode;
        [ShowInInspector, ReadOnly] [CanBeNull] private readonly IReadOnlyCollection<GameObject> _migratingObjects;

        public SceneLoader([NotNull] string dstScenePath,
            LoadSceneMode loadSceneMode = LoadSceneMode.Additive,
            [CanBeNull] List<GameObject> migratingObjects = null)
        {
            _dstScenePath = dstScenePath;
            _loadSceneMode = loadSceneMode;
            _migratingObjects = migratingObjects;
        }

        public void LoadScene()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadSceneAsync(_dstScenePath, LoadSceneMode.Additive);
        }

        private void SceneLoaded(Scene dstScene, LoadSceneMode loadMode)
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            MigrateExistingGameObjectsToScene(dstScene, _migratingObjects);
            if (_loadSceneMode == LoadSceneMode.Single) UnloadAllScenesExcept(dstScene);
        }

        private static void MigrateExistingGameObjectsToScene(Scene dstScene,
            [CanBeNull] IReadOnlyCollection<GameObject> gameObjects)
        {
            if (gameObjects == null || gameObjects.Count <= 0) return;

            foreach (var migratingObject in gameObjects.Where(v => v != null))
                SceneManager.MoveGameObjectToScene(migratingObject, dstScene);
        }

        private static void UnloadAllScenesExcept(Scene targetScene)
        {
            for (var i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene != targetScene) SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}
