namespace YAF.Data.MsSql
{
    using ServiceStack.OrmLite;
    using ServiceStack.OrmLite.SqlServer;

    using YAF.Classes;
    using YAF.Core.Data;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces;

    /// <summary>
    /// The yaf naming strategy base override.
    /// </summary>
    public class YafNamingStrategyBaseOverride : INamingStrategy
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get column name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetColumnName(string name)
        {
            return name;
        }

        /// <summary>
        /// The get table name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTableName(string name)
        {
            return string.Format("{0}{1}", Config.DatabaseObjectQualifier, name);
        }

        #endregion
    }

    /// <summary>
    /// The yaf sql server orm lite dialect provider.
    /// </summary>
    public class YafSqlServerOrmLiteDialectProvider : SqlServerOrmLiteDialectProvider
    {
        #region Static Fields

        /// <summary>
        /// The instance.
        /// </summary>
        public static YafSqlServerOrmLiteDialectProvider Instance = new YafSqlServerOrmLiteDialectProvider();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSqlServerOrmLiteDialectProvider"/> class.
        /// </summary>
        public YafSqlServerOrmLiteDialectProvider()
        {
            this.NamingStrategy = new YafNamingStrategyBaseOverride();
        }

        #endregion
    }

    /// <summary>
    /// The set ms sql dialect event.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerDependancy, new[] { typeof(IHandleEvent<InitDatabaseProviderEvent>) })]
    public class SetMsSqlDialectEvent : IHandleEvent<InitDatabaseProviderEvent>
    {
        #region Public Properties

        /// <summary>
        ///     Gets the order.
        /// </summary>
        public int Order
        {
            get
            {
                return 1000;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event. 
        /// </param>
        public void Handle(InitDatabaseProviderEvent @event)
        {
            if (@event.ProviderName == MsSqlDbAccess.ProviderTypeName)
            {
                // set the OrmLite dialect provider...
                OrmLiteConfig.DialectProvider = YafSqlServerOrmLiteDialectProvider.Instance;
            }
        }

        #endregion
    }
}