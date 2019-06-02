namespace YAF.Data.MsSql
{
    using Autofac;

    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// The ms sql module.
    /// </summary>
    public class MsSqlModule : Module
    {
        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MsSqlDbAccess>()
                .Named<IDbAccess>(MsSqlDbAccess.ProviderTypeName)
                .InstancePerLifetimeScope();
        }
    }
}