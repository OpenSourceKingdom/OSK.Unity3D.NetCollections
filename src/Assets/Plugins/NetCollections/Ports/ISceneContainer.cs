using UnityEngine;

namespace OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Ports
{
    public interface ISceneContainer
    {
        TBehaviour InitializeComponent<TBehaviour>(GameObject gameObject)
            where TBehaviour : MonoBehaviour;
    }
}
