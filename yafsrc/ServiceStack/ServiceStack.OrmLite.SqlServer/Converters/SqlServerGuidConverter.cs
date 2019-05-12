namespace ServiceStack.OrmLite.SqlServer.Converters
{
    using System;

    using ServiceStack.OrmLite.Converters;

    public class SqlServerGuidConverter : GuidConverter
    {
        public override string ColumnDefinition
        {
            get { return "UniqueIdentifier"; }
        }

        public override string ToQuotedString(Type fieldType, object value)
        {
            var guidValue = (Guid)value;
            return string.Format("CAST('{0}' AS UNIQUEIDENTIFIER)", guidValue);
        }
    }
}