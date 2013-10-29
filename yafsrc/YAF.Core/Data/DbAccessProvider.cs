/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF.Core.Data
{
    #region Using

    using System;

    using Autofac.Features.Indexed;

    using YAF.Classes;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The db connection provider base.
    /// </summary>
    public class DbAccessProvider : IDbAccessProvider
    {
        #region Fields

        private readonly IIndex<string, IDbAccess> _dbAccessProviders;

        private readonly SafeReadWriteProvider<IDbAccess> _dbAccessSafe;

        private readonly IServiceLocator _serviceLocator;

        private string _providerName = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbAccessProvider" /> class.
        /// </summary>
        /// <param name="dbAccessProviders">
        ///     The db access providers.
        /// </param>
        /// <param name="serviceLocator">
        ///     The service locator.
        /// </param>
        public DbAccessProvider(IIndex<string, IDbAccess> dbAccessProviders, IServiceLocator serviceLocator)
        {
            this._dbAccessProviders = dbAccessProviders;
            this._serviceLocator = serviceLocator;

            this._dbAccessSafe = new SafeReadWriteProvider<IDbAccess>(
                () =>
                    {
                        IDbAccess dbAccess;

                        // attempt to get the provider...
                        if (this._dbAccessProviders.TryGetValue(this.ProviderName, out dbAccess))
                        {
                            // first time...
                            this._serviceLocator.Get<IRaiseEvent>().Raise(new InitDatabaseProviderEvent(this.ProviderName, dbAccess));
                        }
                        else
                        {
                            throw new NoValidDbAccessProviderFoundException(
                                @"Unable to Locate Provider Named ""{0}"" in Data Access Providers (DLL Not Located in Bin Directory?).".FormatWith(
                                    this.ProviderName));
                        }

                        return dbAccess;
                    });
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the instance of the IDbAccess provider.
        /// </summary>
        /// <returns> </returns>
        /// <exception cref="NoValidDbAccessProviderFoundException">
        ///     <c>NoValidDbAccessProviderFoundException</c>
        ///     .
        /// </exception>
        [CanBeNull]
        public IDbAccess Instance
        {
            get
            {
                return this._dbAccessSafe.Instance;
            }

            set
            {
                this._dbAccessSafe.Instance = value;
                if (value != null)
                {
                    this.ProviderName = value.Information.ProviderName;
                }
            }
        }

        /// <summary>
        ///     Gets or sets ProviderName.
        /// </summary>
        public string ProviderName
        {
            get
            {
                return this._providerName ?? (this._providerName = Config.ConnectionProviderName);
            }
            set
            {
                this._providerName = value;
                this._dbAccessSafe.Instance = null;
            }
        }

        #endregion
    }

    /// <summary>
    ///     The no valid db access provider found exception.
    /// </summary>
    public class NoValidDbAccessProviderFoundException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NoValidDbAccessProviderFoundException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public NoValidDbAccessProviderFoundException(string message)
            : base(message)
        {
        }

        #endregion
    }
}