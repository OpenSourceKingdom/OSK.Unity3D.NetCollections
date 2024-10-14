using System.Collections;
using NUnit.Framework;
using OSK.Unity3D.NetCollections.UnitTests.Assets.OSK.Unity3D.NetCollections.UnitTests.Scripts;
using OSK.Unity3D.NetCollections.UnitTests.Assets.Tests.Scripts;
using UnityEngine;
using UnityEngine.TestTools;

namespace OSK.Unity3D.NetCollections.UnitTests
{
    public class SceneContainerTests
    {
        [UnityTest]
        public IEnumerator SceneContainer_Awakens_InitializesComponentsOfIContainerBehaviour()
        {
            // Arrange
            var gameObject = new GameObject();
            var testManager = gameObject.AddComponent<TestManagerObject>();
            var sceneContainer = gameObject.AddComponent<TestSceneContainer>();
            sceneContainer.TestManagerObject = testManager;

            yield return null;

            var otherObject = new GameObject();

            // Act
            var script = sceneContainer.InitializeComponent<TestScript>(otherObject);

            // Assert
            Assert.IsNotNull(script);
            Assert.AreEqual(testManager, script.Manager);

            Assert.IsNotNull(script.Api);
        }
    }
}
