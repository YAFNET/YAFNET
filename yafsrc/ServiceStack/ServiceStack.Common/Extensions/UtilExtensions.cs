namespace ServiceStack.Extensions
{
    using System;

    /// <summary>
    /// Move conflicting extension methods to ServiceStack.Extensions namespace 
    /// </summary>
    public static class UtilExtensions
    {
        public static Exception GetInnerMostException(this Exception ex)
        {
            //Extract true exception from static initializers (e.g. LicenseException)
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }
    }
}