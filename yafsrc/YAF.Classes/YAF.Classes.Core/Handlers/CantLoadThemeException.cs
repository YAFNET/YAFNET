namespace YAF.Classes.Core
{
  using System;

  public class CantLoadThemeException : Exception
  {
    public CantLoadThemeException(string message)
      :base(message)
    {
      
    }
  }
}