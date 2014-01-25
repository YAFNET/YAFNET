namespace YAF.Data.MsSql
{
    using Autofac;

    using YAF.Types.Interfaces.Data;

    public class MsSqlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MsSqlDbAccess>()
                .Named<IDbAccess>(MsSqlDbAccess.ProviderTypeName)
                .InstancePerLifetimeScope();
        }
    }
}