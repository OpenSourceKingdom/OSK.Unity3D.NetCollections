using OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Attributes;
using OSK.Unity3D.NetCollections.UnitTests.Assets.Tests.Helpers;
using UnityEngine;

namespace OSK.Unity3D.NetCollections.UnitTests.Assets.Tests.Scripts
{
    public class TestScript: MonoBehaviour
    {
        public FakeApi Api { get; private set; }

        public TestManagerObject Manager { get; private set; }

        [ContainerInject]
        private void Initialize(FakeApi fakeApi, TestManagerObject manager)
        {
            Api = fakeApi;
            Manager = manager;
        }
    }
}
