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

namespace YAF.Core.Data
{
	#region Using

	using System;

	using Autofac.Features.Indexed;

	using YAF.Classes;
	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The db connection provider base.
	/// </summary>
	public class DbAccessProvider : IDbAccessProvider
	{
		#region Constants and Fields

		/// <summary>
		///   The _db access providers.
		/// </summary>
		private readonly IIndex<string, IDbAccessV2> _dbAccessProviders;

		/// <summary>
		///   The _last provider name.
		/// </summary>
		private readonly string _lastProviderName = string.Empty;

		/// <summary>
		///   The _service locator.
		/// </summary>
		private readonly IServiceLocator _serviceLocator;

		/// <summary>
		///   The _db access.
		/// </summary>
		private IDbAccessV2 _dbAccess;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DbAccessProvider"/> class.
		/// </summary>
		/// <param name="dbAccessProviders">
		/// The db access providers. 
		/// </param>
		/// <param name="serviceLocator">
		/// The service locator. 
		/// </param>
		public DbAccessProvider(IIndex<string, IDbAccessV2> dbAccessProviders, IServiceLocator serviceLocator)
		{
			this._dbAccessProviders = dbAccessProviders;
			this._serviceLocator = serviceLocator;
			this.ProviderName = Config.ConnectionProviderName;
		}

		#endregion

		#region Public Properties

		/// <summary>
		///   The create.
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="NoValidDbAccessProviderFoundException">
		///   <c>NoValidDbAccessProviderFoundException</c>
		///   .</exception>
		[CanBeNull]
		public IDbAccessV2 Instance
		{
			get
			{
				if (this._dbAccess != null && !this._lastProviderName.Equals(this.ProviderName))
				{
					this._dbAccess = null;
				}

				if (this._dbAccess == null && this.ProviderName.IsSet())
				{
					// attempt to get the provider...
					this._dbAccessProviders.TryGetValue(this.ProviderName.ToLower(), out this._dbAccess);
				}

				if (this._dbAccess == null)
				{
					throw new NoValidDbAccessProviderFoundException(
						@"No Valid Database Access Module Found for Provider Named ""{0}"".".FormatWith(this.ProviderName));
				}

				return this._dbAccess;
			}

			set
			{
				this._dbAccess = value;
				if (this._dbAccess != null)
				{
					this.ProviderName = this._dbAccess.ProviderName;
				}
			}
		}

		/// <summary>
		///   Gets or sets ProviderName.
		/// </summary>
		public string ProviderName { get; set; }

		#endregion
	}

	/// <summary>
	/// The no valid db access provider found exception.
	/// </summary>
	public class NoValidDbAccessProviderFoundException : Exception
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="NoValidDbAccessProviderFoundException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public NoValidDbAccessProviderFoundException(string message)
			: base(message)
		{
		}

		#endregion
	}
}