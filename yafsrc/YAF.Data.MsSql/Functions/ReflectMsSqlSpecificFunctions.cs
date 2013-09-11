namespace YAF.Data.MsSql.Functions
{
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces.Data;

    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class ReflectMsSqlSpecificFunctions : BaseReflectedSpecificFunctions
    {
        public ReflectMsSqlSpecificFunctions(IDbAccess dbAccess)
            : base(typeof(MsSqlSpecificFunctions), dbAccess)
        {
        }

        public override int SortOrder
        {
            get
            {
                return 1000;
            }
        }
    }
}