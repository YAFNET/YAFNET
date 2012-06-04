namespace YAF.Types.Interfaces
{
	#region Using

	using System;
	using System.Data.Common;

	#endregion

	/// <summary>
	/// The db unit of work.
	/// </summary>
	public interface IDbUnitOfWork : IDisposable
	{
		#region Properties

		/// <summary>
		/// Gets Transaction.
		/// </summary>
		DbTransaction Transaction { get; }

		#endregion
	}
}