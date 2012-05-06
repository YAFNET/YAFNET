/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The yaf db logger.
  /// </summary>
  public class YafDbLogger : ILogger
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="YafDbLogger"/> class.
    /// </summary>
    /// <param name="logType">
    /// The log type.
    /// </param>
    public YafDbLogger([CanBeNull] Type logType)
    {
      this.Type = logType;
    }

#if (DEBUG)

    /// <summary>
    /// The _is debug.
    /// </summary>
    private bool _isDebug = true;
#else
    private bool _isDebug = false;
#endif

    #region Implemented Interfaces

    #region ILogger

    /// <summary>
    ///   Gets a value indicating whether IsDebugEnabled.
    /// </summary>
    public bool IsDebugEnabled
    {
      get
      {
        return this._isDebug;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsErrorEnabled.
    /// </summary>
    public bool IsErrorEnabled
    {
      get
      {
        return  YafContext.Current.BoardSettings.LogError;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsFatalEnabled.
    /// </summary>
    public bool IsFatalEnabled
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsInfoEnabled.
    /// </summary>
    public bool IsInfoEnabled
    {
      get
      {
          return YafContext.Current.BoardSettings.LogInformation;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsUserSuspendedeEnabled.
    /// </summary>
    public bool IsUserSuspendedEnabled
    {
        get
        {
            return YafContext.Current.BoardSettings.LogUserSuspendedUnsuspended;
        }
    }

    /// <summary>
    ///   Gets a value indicating whether IsUserDeletedEnabled.
    /// </summary>
    public bool IsUserDeletedEnabled
    {
        get
        {
            return YafContext.Current.BoardSettings.LogUserDeleted;
        }
    }

    /// <summary>
    ///   Gets a value indicating whether IsLogBannedIP.
    /// </summary>
    public bool IsLogBannedIP
    {
        get
        {
            return YafContext.Current.BoardSettings.LogBannedIP;
        }
    }

    /// <summary>
    ///   Gets a value indicating whether IsTraceEnabled.
    /// </summary>
    public bool IsTraceEnabled
    {
      get
      {
        return this._isDebug;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsWarnEnabled.
    /// </summary>
    public bool IsWarnEnabled
    {
      get
      {
          return YafContext.Current.BoardSettings.LogWarning;
      }
    }

    /// <summary>
    ///   Gets a value indicating the logging type.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// The log.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="logTypes">
    /// The log types.
    /// </param>
    private void Log([NotNull] string message, EventLogTypes logTypes)
    {
      string typeName = "Unknown";

      if (this.Type != null)
      {
        typeName = this.Type.FullName;
      }

      // TODO: come up with userid if the database is available.
      LegacyDb.eventlog_create(null, typeName, message, logTypes);
    }

      /// <summary>
      /// The log.
      /// </summary>
      /// <param name="userId">
      /// The userId.
      ///  </param>
      /// <param name="message">
      /// The message.
      /// </param>
      /// <param name="logTypes">
      /// The log types.
      /// </param>
      private void Log(int userId, [NotNull] string message, EventLogTypes logTypes)
    {
        string typeName = "Unknown";

        if (this.Type != null)
        {
            typeName = this.Type.FullName;
        }

        // TODO: come up with userid if the database is available.
        LegacyDb.eventlog_create(userId, typeName, message, logTypes);
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
    public void Debug(string format, params object[] args)
    {
      if (this.IsDebugEnabled)
      {
        System.Diagnostics.Debug.WriteLine(String.Format(format, args));
      }
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
    public void Debug(Exception exception, string format, params object[] args)
    {
      if (this.IsDebugEnabled)
      {
        System.Diagnostics.Debug.WriteLine(String.Format(format, args));
        System.Diagnostics.Debug.WriteLine(exception.ToString());
      }
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
    public void Error(string format, params object[] args)
    {
      if (this.IsErrorEnabled)
      {
        this.Log(String.Format(format, args), EventLogTypes.Error);
      }
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
    public void Error(Exception exception, string format, params object[] args)
    {
      if (this.IsErrorEnabled)
      {
        this.Log(String.Format(format, args) + "\r\n" + exception, EventLogTypes.Error);
      }
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
    public void Fatal(string format, params object[] args)
    {
      if (this.IsFatalEnabled)
      {
        this.Log(String.Format(format, args), EventLogTypes.Error);
      }
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
    public void Fatal(Exception exception, string format, params object[] args)
    {
      if (this.IsFatalEnabled)
      {
        this.Log(String.Format(format, args) + "\r\n" + exception, EventLogTypes.Error);
      }
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
    public void Info(string format, params object[] args)
    {
      if (this.IsInfoEnabled)
      {
        this.Log(String.Format(format, args), EventLogTypes.Information);
      }
    }

      /// <summary>
      /// The info.
      /// </summary>
      /// <param name="userId">
      /// The userId.
      ///  </param>
      /// <param name="format">
      /// The format.
      /// </param>
      /// <param name="args">
      /// The args.
      /// </param>
      public void Info(int userId, string format, params object[] args)
    {
        if (this.IsInfoEnabled)
        {
            this.Log(userId,String.Format(format, args), EventLogTypes.Information);
        }
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
    public void Info(Exception exception, string format, params object[] args)
    {
      if (this.IsInfoEnabled)
      {
        this.Log(String.Format(format, args) + "\r\n" + exception, EventLogTypes.Information);
      }
    }

    /// <summary>
    /// The UserUnsuspended.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param> 
    public void UserUnsuspended(int userId, string source,  string format, params object[] args)
    {
      if (this.IsUserSuspendedEnabled)
      {
          LegacyDb.eventlog_create(userId, source, String.Format(format, args), EventLogTypes.UserUnsuspended);
      }
    }

    /// <summary>
    /// The User Suspended.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param> 
    public void UserSuspended(int userId, string source, string format, params object[] args)
    {
        if (this.IsUserSuspendedEnabled)
        {
            LegacyDb.eventlog_create(userId, source, String.Format(format, args), EventLogTypes.UserSuspended);
        }
    }

    /// <summary>
    /// The User Deleted.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public void UserDeleted(int userId, string source, string format, params object[] args)
    {
        if (this.IsUserDeletedEnabled)
        {
            LegacyDb.eventlog_create(userId, source, String.Format(format, args), EventLogTypes.UserDeleted);
        }
    }

    /// <summary>
    /// The Ip Ban Set.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public void IpBanSet(int userId, string source, string format, params object[] args)
    {
        if (this.IsLogBannedIP)
        {
            LegacyDb.eventlog_create(userId, source, String.Format(format, args), EventLogTypes.IpBanSet);
        }
    }

    /// <summary>
    /// The Ip Ban Lifted.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public void IpBanLifted(int userId, string source, string format, params object[] args)
    {
        if (this.IsLogBannedIP)
        {
            LegacyDb.eventlog_create(userId, source, String.Format(format, args), EventLogTypes.IpBanLifted);
        }
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
    public void Trace(string format, params object[] args)
    {
      if (this.IsTraceEnabled)
      {
        System.Diagnostics.Trace.TraceInformation(String.Format(format, args));
      }
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
    public void Trace(Exception exception, string format, params object[] args)
    {
      if (this.IsTraceEnabled)
      {
        System.Diagnostics.Trace.TraceInformation(String.Format(format, args));
        System.Diagnostics.Trace.TraceError(exception.ToString());
      }
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
    public void Warn(string format, params object[] args)
    {
      if (this.IsWarnEnabled)
      {
        this.Log(String.Format(format, args), EventLogTypes.Warning);
      }
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
    public void Warn(Exception exception, string format, params object[] args)
    {
      if (this.IsWarnEnabled)
      {
        this.Log(String.Format(format, args) + "\r\n" + exception, EventLogTypes.Warning);
      }
    }

    #endregion

    #endregion
  }
}