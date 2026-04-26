/*
This code was copied from https://github.com/dotnet/sdk and maybe slightly modified.
Code was originally distributed under the MIT license, see LICENSE.TXT in the project root for license information.
*/

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace mad.analyzers.common.util
{
    /// <summary>
    /// Compares objects based upon their reference identity.
    /// </summary>
    public class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        private ReferenceEqualityComparer()
        {
        }

        bool IEqualityComparer<object>.Equals(object a, object b)
        {
            return a == b;
        }

        int IEqualityComparer<object>.GetHashCode(object a)
        {
            return GetHashCode(a);
        }

        public static int GetHashCode(object a)
        {
            return RuntimeHelpers.GetHashCode(a);
        }
    }
}
