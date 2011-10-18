// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventPageLoad.cs" company="">
//   
// </copyright>
// <summary>
//   The page load event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.EventProxies
{
	using System.Collections.Generic;
	using System.Dynamic;

	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Extensions;

	/// <summary>
	/// The page load event.
	/// </summary>
	public class InitPageLoadEvent : IAmEvent
	{
		#region Constants and Fields

		/// <summary>
		/// The _the expando data.
		/// </summary>
		private readonly ExpandoObject _theExpandoData = new ExpandoObject();

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="InitPageLoadEvent"/> class.
		/// </summary>
		public InitPageLoadEvent()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets Data.
		/// </summary>
		public dynamic Data
		{
			get
			{
				return this._theExpandoData;
			}
		}

		/// <summary>
		///   Gets or sets PageLoadData.
		/// </summary>
		public IDictionary<string, object> DataDictionary
		{
			get
			{
				return (IDictionary<string, object>)this._theExpandoData;
			}
		}

		#endregion
	}
}