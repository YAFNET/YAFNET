namespace ServiceStack.OrmLite.SqlServer.Converters
{
    using System;

    using ServiceStack.OrmLite.Converters;

    public class SqlServerGuidConverter : GuidConverter
    {
        public override string ColumnDefinition => "UniqueIdentifier";

        public override string ToQuotedString(Type fieldType, object value)
        {
            var guidValue = (Guid)value;
            return $"CAST('{guidValue}' AS UNIQUEIDENTIFIER)";
        }
    }
}