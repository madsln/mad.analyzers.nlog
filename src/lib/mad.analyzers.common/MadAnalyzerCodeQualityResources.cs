using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace mad.analyzers.common
{
    public partial class MadAnalyzerCodeQualityResources
    {
        private static readonly Type s_resourcesType = typeof(MadAnalyzerCodeQualityResources);

        public static LocalizableResourceString CreateLocalizableResourceString(string nameOfLocalizableResource)
            => new(nameOfLocalizableResource, ResourceManager, s_resourcesType);

        public static LocalizableResourceString CreateLocalizableResourceString(string nameOfLocalizableResource, params string[] formatArguments)
            => new(nameOfLocalizableResource, ResourceManager, s_resourcesType, formatArguments);
    }
}
