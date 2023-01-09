namespace FarsiLibrary.Utils.Exceptions;

using System;

public class InvalidPersianDateException : Exception
{
    public InvalidPersianDateException() : base(string.Empty)
    {
    }

    public InvalidPersianDateException(string message)
        : base(message)
    {
    }

    public InvalidPersianDateException(string message, object value)
    {
        this.InvalidValue = value;
    }

    public object InvalidValue
    {
        get;
    }
}