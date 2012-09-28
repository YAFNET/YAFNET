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
namespace YAF.Types.Interfaces.Data
{
    using System;
    using System.Data;

    using YAF.Types.Handlers;

    public interface IDbConnectionManager : IDisposable
  {
    /// <summary>
    /// Gets ConnectionString.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the current DB Connection in any state.
    /// </summary>
    IDbConnection DBConnection { get; }

    /// <summary>
    /// Gets an open connection to the DB. Can be called any number of times.
    /// </summary>
    IDbConnection OpenDBConnection { get; }

    /// <summary>
    /// The info message.
    /// </summary>
    event YafDBConnInfoMessageEventHandler InfoMessage;

    /// <summary>
    /// The init connection.
    /// </summary>
    void InitConnection();

    /// <summary>
    /// The close connection.
    /// </summary>
    void CloseConnection();
  }
}