namespace YAF.Core.Data
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Omu.ValueInjecter.Injections;
    using Omu.ValueInjecter.Utils;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Extensions;

    public class DataRowInjection : KnownSourceInjection<DataRow>
    {
        #region Methods

        protected override void Inject(DataRow source, object target)
        {
            var props = target.GetProps();

            var aliasMapping = props.OfType<PropertyDescriptor>()
                .Where(p => p.Attributes.OfType<AliasAttribute>().Any())
                .ToDictionary(k => k.Attributes.OfType<AliasAttribute>().FirstOrDefault().Name, v => v.Name);

            var nameMap = new Func<string, string>(inputName => aliasMapping.ContainsKey(inputName) ? aliasMapping[inputName] : inputName);

            for (var i = 0; i < source.ItemArray.Length; i++)
            {
                var activeTarget = props.FirstOrDefault(p => p.Name == nameMap(source.Table.Columns[i].ColumnName));

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