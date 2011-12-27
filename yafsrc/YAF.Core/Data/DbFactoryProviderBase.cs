namespace YAF.Core.Data
{
	#region Using

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
		/// The _db access providers.
		/// </summary>
		private readonly IIndex<string, IDbAccess> _dbAccessProviders;

		/// <summary>
		/// The _last provider name.
		/// </summary>
		private readonly string _lastProviderName = string.Empty;

		/// <summary>
		/// The _service locator.
		/// </summary>
		private readonly IServiceLocator _serviceLocator;

		/// <summary>
		/// The _db access.
		/// </summary>
		private IDbAccess _dbAccess;

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
		public DbAccessProvider(IIndex<string, IDbAccess> dbAccessProviders, IServiceLocator serviceLocator)
		{
			this._dbAccessProviders = dbAccessProviders;
			this._serviceLocator = serviceLocator;
			this.ProviderName = Config.ConnectionProviderName;
		}

		#endregion

		#region Properties

		/// <summary>
		///   The create.
		/// </summary>
		/// <returns>
		/// </returns>
		[NotNull]
		public IDbAccess Instance
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

				return this._dbAccess ?? (this._dbAccess = this._serviceLocator.Get<DbAccessBase>());
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
		/// Gets or sets ProviderName.
		/// </summary>
		public string ProviderName { get; set; }

		#endregion
	}
}