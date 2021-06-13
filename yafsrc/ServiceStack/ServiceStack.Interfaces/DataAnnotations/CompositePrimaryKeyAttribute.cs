namespace ServiceStack.DataAnnotations
{
    using System;
    using System.Collections.Generic;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class CompositePrimaryKeyAttribute : AttributeBase
    {
        public CompositePrimaryKeyAttribute()
        {
            this.FieldNames = new List<string>();
        }

        public CompositePrimaryKeyAttribute(params string[] fieldNames)
        {
            this.FieldNames = new List<string>(fieldNames);
        }

        public List<string> FieldNames { get; set; }

        public string Name { get; set; }
    }
}