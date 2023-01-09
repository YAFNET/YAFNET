namespace FarsiLibrary.Utils.Exceptions;

using System;

public class InvalidPersianDateFormatException : Exception
{
    public InvalidPersianDateFormatException(string message)
        : base(message)
    {
    }

    public InvalidPersianDateFormatException() : base(string.Empty)
    { 
    }
}