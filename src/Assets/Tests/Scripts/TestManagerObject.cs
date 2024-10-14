using OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Attributes;
using OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Ports;
using OSK.Unity3D.NetCollections.UnitTests.Assets.Tests.Helpers;
using System;
using UnityEngine;

namespace OSK.Unity3D.NetCollections.UnitTests.Assets.Tests.Scripts
{
    public class TestManagerObject : MonoBehaviour, IContainerBehaviour
    {
        [ContainerInject]
        public void Initialize(FakeApi fakeApi)
        {
            if (fakeApi == null)
            {
                throw new ArgumentNullException(nameof(fakeApi));
            }
        }
    }
}
