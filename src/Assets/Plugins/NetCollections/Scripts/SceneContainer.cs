using Microsoft.Extensions.DependencyInjection;
using OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Attributes;
using OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Models;
using OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Scripts
{
    public abstract class SceneContainer : MonoBehaviour, ISceneContainer
    {
        #region Variables

        private static Dictionary<Type, EfficientInvoker> _invokers = new();

        [SerializeField]
        private bool _initializeScene;
        [SerializeField]
        private bool _includeInactive;

        private IServiceProvider _serviceProvider;

        #endregion

        #region ISceneContainer

        public TBehaviour InitializeComponent<TBehaviour>(GameObject gameObject)
            where TBehaviour : MonoBehaviour
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("Unable to initialize component on null gameObject.");
            }

            var component = gameObject.AddComponent<TBehaviour>();
            if (TryGetEfficientInvoker(typeof(TBehaviour), out var invoker))
            {
                invoker.Invoke(component, invoker.ParameterTypes.Select(_serviceProvider.GetRequiredService).ToArray());
            }

            return component;
        }

        #endregion

        #region MonoBehaviour

        void Awake()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ISceneContainer>(_ => this);
            Configure(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            if (!_initializeScene)
            {
                return;
            }

            var containerObjects = gameObject.scene.GetRootGameObjects()
                .SelectMany(o => o.GetComponentsInChildren<MonoBehaviour>(_includeInactive))
                .OfType<IContainerBehaviour>();
            foreach (var behaviour in containerObjects)
            {
                if (TryGetEfficientInvoker(behaviour.GetType(), out var invoker))
                {
                    invoker.Invoke(behaviour, invoker.ParameterTypes.Select(_serviceProvider.GetRequiredService).ToArray());
                }
            }
        }

        #endregion

        #region Helpers

        private bool TryGetEfficientInvoker(Type type, out EfficientInvoker invoker)
        {
            if (_invokers.TryGetValue(type, out invoker))
            {
                return true;
            }

            var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .FirstOrDefault(m => m.GetCustomAttribute<ContainerInjectAttribute>() != null);
            if (method == null)
            {
                return false;
            }

            invoker = EfficientInvoker.FromMethod(type, method);
            if (invoker == null)
            {
                return false;
            }

            _invokers.Add(type, invoker);
            return true;
        }

        protected abstract void Configure(IServiceCollection services);

        #endregion
    }
}