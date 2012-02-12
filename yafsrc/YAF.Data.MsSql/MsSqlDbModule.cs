// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDbModule.cs" company="">
//   
// </copyright>
// <summary>
//   The ms sql db module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Data.MsSql
{
	using Autofac;

	using YAF.Types;
	using YAF.Types.Interfaces;

	/// <summary>
	/// The ms sql db module.
	/// </summary>
	public class MsSqlDbModule : Module
	{
		#region Methods

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="builder">
		/// The builder.
		/// </param>
		protected override void Load([NotNull] ContainerBuilder builder)
		{
			builder.RegisterType<MsSqlDbAccess>().Named<IDbAccess>("System.Data.SqlClient").InstancePerDependency().
				PreserveExistingDefaults();
		}

		#endregion
	}
}