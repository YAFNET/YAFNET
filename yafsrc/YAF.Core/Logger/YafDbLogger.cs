namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Classes.Data;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The yaf db logger.
  /// </summary>
  public class YafDbLogger : ILogger
  {
    public YafDbLogger()
    {
      
    }

#if (DEBUG)
    private bool _isDebug = true;
#else
    private bool _isDebug = false;
#endif
    

    #region Implemented Interfaces

    #region ILogger

    /// <summary>
    /// Gets a value indicating whether IsDebugEnabled.
    /// </summary>
    public bool IsDebugEnabled
    {
      get
      {
        return !_isDebug;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsErrorEnabled.
    /// </summary>
    public bool IsErrorEnabled
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsFatalEnabled.
    /// </summary>
    public bool IsFatalEnabled
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsInfoEnabled.
    /// </summary>
    public bool IsInfoEnabled
    {
      get
      {
        return !_isDebug;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsTraceEnabled.
    /// </summary>
    public bool IsTraceEnabled
    {
      get
      {
        return !_isDebug;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsWarnEnabled.
    /// </summary>
    public bool IsWarnEnabled
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating the logging type.
    /// </summary>
    public Type Type
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// The debug.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Debug(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The debug.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Debug(Exception exception, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The error.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Error(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The error.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Error(Exception exception, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The fatal.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Fatal(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The fatal.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Fatal(Exception exception, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The info.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Info(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The info.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Info(Exception exception, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The trace.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Trace(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The trace.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Trace(Exception exception, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The warn.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Warn(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The warn.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void Warn(Exception exception, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    #endregion

    #endregion
  }
}