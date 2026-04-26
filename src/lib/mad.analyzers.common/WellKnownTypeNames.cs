using System;
using System.Collections.Generic;
using System.Text;

namespace mad.analyzers.common
{
    public static class WellKnownNamespaces
    {
        public const string System = "System";
        public const string SystemCollections = "System.Collections";
        public const string SystemCollectionsGeneric = "System.Collections.Generic";
        public const string SystemThreadingTasks = "System.Threading.Tasks";
        public const string SystemRuntimeCompilerServices = "System.Runtime.CompilerServices";
        public const string NLog = "NLog";
    }

    public static class WellKnownTypeNames
    {
        #region System

        public const string SystemIDisposable = "System.IDisposable";
        public const string SystemIAsyncDisposable = "System.IAsyncDisposable";
        public const string Span1 = "System.Span`1";
        public const string ReadOnlySpan1 = "System.ReadOnlySpan`1";

        #endregion

        #region System.Collections

        public const string SystemCollectionsICollection = "System.Collections.ICollection";
        public const string SystemCollectionsGenericICollection1 = "System.Collections.Generic.ICollection`1";
        public const string SystemCollectionsGenericIReadOnlyCollection1 = "System.Collections.Generic.IReadOnlyCollection`1";

        #endregion

        #region System.Threading

        public const string SystemThreadingTasksValueTask = "System.Threading.Tasks.ValueTask";
        public const string SystemThreadingTasksTask = "System.Threading.Tasks.Task";
        public const string SystemThreadingTasksTask1 = "System.Threading.Tasks.Task`1";

        #endregion

        #region System.Runtime.CompilerServices

        public const string SystemRuntimeCompilerServicesConfiguredAsyncDisposable = "System.Runtime.CompilerServices.ConfiguredAsyncDisposable";
        public const string SystemRuntimeCompilerServicesConfiguredValueTaskAwaitable = "System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable";

        #endregion

        #region NLog

        public const string NLogLogger = "NLog.Logger";
        public const string NLogILogger = "NLog.ILogger";
        public const string NLogILoggerExtensions = "NLog.ILoggerExtensions";

        #endregion
    }
}
