using System;

namespace OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Models
{
    internal struct MemberKey
    {
        #region Variables

        public readonly Type Type;
        public readonly string Name;
        public readonly Type[] ParameterTypes;

        #endregion

        #region Constructors

        public MemberKey(Type type, string name, Type[] parameterTypes)
        {
            Type = type;
            Name = name;
            ParameterTypes = parameterTypes;
        }

        #endregion
    }
}
