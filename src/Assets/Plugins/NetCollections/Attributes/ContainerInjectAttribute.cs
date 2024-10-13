using System;

namespace OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false)]
    public class ContainerInjectAttribute : Attribute
    {
    }
}
