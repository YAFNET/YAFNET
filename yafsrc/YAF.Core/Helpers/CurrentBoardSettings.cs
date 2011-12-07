// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentBoardSettings.cs" company="">
//   
// </copyright>
// <summary>
//   The current board settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Core
{
	#region Using

	using System.Web;

	using YAF.Classes;
	using YAF.Types;
	using YAF.Types.Constants;
	using YAF.Types.Interfaces;

	#endregion

	/// <summary>
	/// The current board settings.
	/// </summary>
	public class CurrentBoardSettings : IReadWriteProvider<YafBoardSettings>
	{
		#region Constants and Fields

		/// <summary>
		///   The _application state base.
		/// </summary>
		private readonly HttpApplicationStateBase _applicationStateBase;

		/// <summary>
		/// The _db function.
		/// </summary>
		private readonly IDbFunction _dbFunction;

		/// <summary>
		///   The _have board id.
		/// </summary>
		private readonly IHaveBoardId _haveBoardId;

		/// <summary>
		///   The _inject services.
		/// </summary>
		private readonly IInjectServices _injectServices;

		/// <summary>
		///   The _treat cache key.
		/// </summary>
		private readonly ITreatCacheKey _treatCacheKey;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CurrentBoardSettings"/> class.
		/// </summary>
		/// <param name="applicationStateBase">
		/// The application state base.
		/// </param>
		/// <param name="injectServices">
		/// The inject services.
		/// </param>
		/// <param name="haveBoardId">
		/// The have board id.
		/// </param>
		/// <param name="treatCacheKey">
		/// </param>
		/// <param name="dbFunction">
		/// The db Function.
		/// </param>
		public CurrentBoardSettings(
			[NotNull] HttpApplicationStateBase applicationStateBase, 
			[NotNull] IInjectServices injectServices, 
			[NotNull] IHaveBoardId haveBoardId, 
			[NotNull] ITreatCacheKey treatCacheKey, [NotNull] IDbFunction dbFunction)
		{
			CodeContracts.ArgumentNotNull(applicationStateBase, "applicationStateBase");
			CodeContracts.ArgumentNotNull(injectServices, "injectServices");
			CodeContracts.ArgumentNotNull(haveBoardId, "haveBoardId");
			CodeContracts.ArgumentNotNull(treatCacheKey, "treatCacheKey");

			this._applicationStateBase = applicationStateBase;
			this._injectServices = injectServices;
			this._haveBoardId = haveBoardId;
			this._treatCacheKey = treatCacheKey;
			this._dbFunction = dbFunction;
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets or sets Object.
		/// </summary>
		public YafBoardSettings Instance
		{
			get
			{
				return this._applicationStateBase.GetOrSet(
					this._treatCacheKey.Treat(Constants.Cache.BoardSettings), 
					() =>
						{
							var boardSettings = new YafLoadBoardSettings(this._haveBoardId.BoardId, this._dbFunction);

							// inject
							this._injectServices.Inject(boardSettings);

							return boardSettings;
						});
			}

			set
			{
				this._applicationStateBase.Set(this._treatCacheKey.Treat(Constants.Cache.BoardSettings), value);
			}
		}

		#endregion
	}
}