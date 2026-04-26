/*
This code was copied from https://github.com/dotnet/sdk and maybe slightly modified.
Code was originally distributed under the MIT license, see LICENSE.TXT in the project root for license information.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace mad.analyzers.common
{
    public static class DiagnosticCategory
    {
        public const string Design = nameof(Design);
        public const string Globalization = nameof(Globalization);
        public const string Interoperability = nameof(Interoperability);
        public const string Mobility = nameof(Mobility);
        public const string Performance = nameof(Performance);
        public const string Reliability = nameof(Reliability);
        public const string Security = nameof(Security);
        public const string Usage = nameof(Usage);
        public const string Naming = nameof(Naming);
        public const string Library = nameof(Library);
        public const string Documentation = nameof(Documentation);
        public const string Maintainability = nameof(Maintainability);
    }
}
