namespace YAF.Core.Data
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;

    using Omu.ValueInjecter;

    using ServiceStack.DataAnnotations;

    public class DataRowInjection : KnownSourceValueInjection<DataRow>
    {
        #region Methods

        protected override void Inject(DataRow source, object target)
        {
            var props = target.GetProps();

            var aliasMapping = props.OfType<PropertyDescriptor>()
                .Where(p => p.Attributes.OfType<AliasAttribute>().Any())
                .ToDictionary(k => k.Attributes.OfType<AliasAttribute>().FirstOrDefault().Name, v => v.Name);

            var nameMap = new Func<string, string>(inputName => aliasMapping.ContainsKey(inputName) ? aliasMapping[inputName] : inputName);

            for (var i = 0; i < source.ItemArray.Count(); i++)
            {
                PropertyDescriptor activeTarget = props.GetByName(nameMap(source.Table.Columns[i].ColumnName), true);

                if (activeTarget == null)
                {
                    continue;
                }

                var value = source.ItemArray[i];
                if (value == DBNull.Value)
                {
                    continue;
                }

                if (activeTarget.PropertyType == value.GetType())
                {
                    activeTarget.SetValue(target, value);
                }
                else
                {
                    Type conversionType = activeTarget.PropertyType;

                    if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        conversionType = (new NullableConverter(conversionType)).UnderlyingType;
                    }

                    activeTarget.SetValue(target, Convert.ChangeType(value, conversionType));
                }
            }
        }

        #endregion
    }
}