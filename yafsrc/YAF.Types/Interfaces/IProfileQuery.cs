namespace YAF.Types.Interfaces
{
    using System;

    public interface IProfileQuery
    {
        IDisposable Start(string currentStep);
    }
}