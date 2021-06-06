using System;
using System.Threading;
using JetBrains.Annotations;

namespace LinearBeats.Utils
{
    public class Singleton<T> where T : class
    {
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateInstance, LazyThreadSafetyMode.ExecutionAndPublication);

        public static T Instance => LazyInstance.Value;

        [NotNull]
        private static T CreateInstance()
        {
            var message = $"{typeof(T).Name} has more than one accessible constructor. Impossible to make singleton instance.";

            if (typeof(T).GetConstructors().Length > 0) throw new InvalidOperationException(message);

            return Activator.CreateInstance(typeof(T), true) as T ?? throw new InvalidOperationException(message);
        }
    }
}
