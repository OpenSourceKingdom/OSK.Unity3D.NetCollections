using System;
using System.Collections.Generic;

namespace OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Models
{
    internal class MemberKeyComparer : IEqualityComparer<MemberKey>
    {
        public static readonly MemberKeyComparer Instance = new MemberKeyComparer();

        public bool Equals(MemberKey x, MemberKey y)
        {
            return x.Type == y.Type &&
                   StringComparer.Ordinal.Equals(x.Name, y.Name);
        }

        public int GetHashCode(MemberKey key)
        {
            var typeCode = key.Type.GetHashCode();
            var methodCode = key.Name.GetHashCode();
            return CombineHashCodes(typeCode, methodCode);
        }

        // From System.Web.Util.HashCodeCombiner
        private static int CombineHashCodes(int h1, int h2)
        {
            return (h1 << 5) + h1 ^ h2;
        }
    }
}
