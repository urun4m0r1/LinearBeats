using System.Collections.Generic;
using JetBrains.Annotations;
using Lean.Pool;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public sealed class JudgeEffectSpawner : SerializedMonoBehaviour
    {
        [OdinSerialize, DisableContextMenu, DictionaryDrawerSettings(IsReadOnly = true)]
        [NotNull] private Dictionary<Judge, LeanGameObjectPool> _judgeEffectSpawners = new Dictionary<Judge, LeanGameObjectPool>
        {
            [Judge.Perfect] = null,
            [Judge.Great] = null,
            [Judge.Good] = null,
            [Judge.Bad] = null,
            [Judge.Miss] = null,
        };

        public void Spawn(Judge judge, Vector3 spawnPosition)
        {
            var spawner = _judgeEffectSpawners[judge];

            if (spawner) spawner.Spawn(spawnPosition, Quaternion.identity, spawner.transform);
        }
    }
}
