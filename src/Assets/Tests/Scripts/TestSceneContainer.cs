using Microsoft.Extensions.DependencyInjection;
using OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Scripts;
using OSK.Unity3D.NetCollections.UnitTests.Assets.Tests.Helpers;
using OSK.Unity3D.NetCollections.UnitTests.Assets.Tests.Scripts;

namespace OSK.Unity3D.NetCollections.UnitTests.Assets.OSK.Unity3D.NetCollections.UnitTests.Scripts
{
    public class TestSceneContainer : SceneContainer
    {
        #region Variables

        public TestManagerObject TestManagerObject;

        #endregion

        protected override void Configure(IServiceCollection services)
        {
            services.AddSingleton(_ => TestManagerObject);
            services.AddTransient<FakeApi, FakeApi>();
        }
    }
}
