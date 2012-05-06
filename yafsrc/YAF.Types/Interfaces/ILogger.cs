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
namespace YAF.Types.Interfaces
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The logger interface
  /// </summary>
  public interface ILogger
  {
    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsDebugEnabled.
    /// </summary>
    bool IsDebugEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether IsErrorEnabled.
    /// </summary>
    bool IsErrorEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether IsFatalEnabled.
    /// </summary>
    bool IsFatalEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether IsInfoEnabled.
    /// </summary>
    bool IsInfoEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether IsTraceEnabled.
    /// </summary>
    bool IsTraceEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether IsWarnEnabled.
    /// </summary>
    bool IsWarnEnabled { get; }

    /// <summary>
    /// Gets a value indicating the logging type.
    /// </summary>
    Type Type { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The debug.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    void Debug([NotNull] string format, [NotNull] params object[] args);

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
    void Debug([NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args);

    /// <summary>
    /// The error.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    void Error([NotNull] string format, [NotNull] params object[] args);

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
    void Error([NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args);

    /// <summary>
    /// The fatal.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    void Fatal([NotNull] string format, [NotNull] params object[] args);

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
    void Fatal([NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args);

    /// <summary>
    /// The info.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    void Info([NotNull] string format, [NotNull] params object[] args);

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
    void Info([NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args);

   /// <summary>
   /// The info.
   /// </summary>
   /// <param name="userId">
   /// The userId.
   /// </param>
   /// <param name="format">
   /// The format.
   /// </param>
   /// <param name="args">
   /// The args.
   /// </param>
    void Info(int userId, string format, params object[] args);

    /// <summary>
    /// The trace.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    void Trace([NotNull] string format, [NotNull] params object[] args);

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
    void Trace([NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args);

    /// <summary>
    /// The warn.
    /// </summary>
    /// <param name="format">
    /// The format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    void Warn([NotNull] string format, [NotNull] params object[] args);

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
    void Warn([NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args);

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
    void UserUnsuspended(int userId, string source, string format, params object[] args);

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
    void UserSuspended(int userId, string source, string format, params object[] args);

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
      void UserDeleted(int userId, string source, string format, params object[] args);

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
      void IpBanSet(int userId, string source, string format, params object[] args);

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
      void IpBanLifted(int userId, string source, string format, params object[] args);

      #endregion
  }
}