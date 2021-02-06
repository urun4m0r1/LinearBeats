#pragma warning disable IDE0090

using System;
using System.Threading;

namespace Utils.Templates
{
    public class Singleton<T> where T : class
    {
        private static readonly Lazy<T> s_lazyInstance = new Lazy<T>(
            CreateInstance,
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static T Instance
        {
            get => s_lazyInstance.Value;
        }

        private static T CreateInstance()
        {
            CheckPublicConstructorsCount();
            return CreateInstanceWithPrivateConstructor();
        }

        private static void CheckPublicConstructorsCount()
        {
            if (typeof(T).GetConstructors().Length > 0)
            {
                throw new InvalidOperationException(InvaildOperationMessage());
            }
        }

        private static string InvaildOperationMessage()
        {
            return
                $"{typeof(T).Name} has at least one accessible constructor. " +
                $"Impossible to make singleton behaviour.";
        }

        private static T CreateInstanceWithPrivateConstructor()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }
    }
}
