namespace YAF.Core.Data.Profiling
{
    using System;

    using YAF.Types.Interfaces;

    /// <summary>
    ///     Implements IProfileQuery and creates QueryWatchers.
    /// </summary>
    public class QueryProfile : IProfileQuery
    {
        #region Public Methods and Operators

        IDisposable IProfileQuery.Start(string currentStep)
        {
            return new QueryWatcher(currentStep);
        }

        public static IDisposable Start(string currentStep)
        {
            return new QueryWatcher(currentStep);
        }

        #endregion
    }
}