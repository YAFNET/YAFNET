/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Core.Data
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;

    using Omu.ValueInjecter;

    using ServiceStack.DataAnnotations;

    using YAF.Types;

    /// <summary>
    ///     The data record injection.
    /// </summary>
    public class DataRecordInjection : KnownSourceValueInjection<IDataRecord>
    {
        #region Methods

        /// <summary>
        ///     Handles injection of an IDataRecord
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        protected override void Inject([NotNull] IDataRecord source, object target)
        {
            CodeContracts.VerifyNotNull(source, "source");

            var props = target.GetProps();

            var aliasMapping = props.OfType<PropertyDescriptor>()
                .Where(p => p.Attributes.OfType<AliasAttribute>().Any())
                .ToDictionary(k => k.Attributes.OfType<AliasAttribute>().FirstOrDefault().Name, v => v.Name);

            var nameMap = new Func<string, string>(inputName => aliasMapping.ContainsKey(inputName) ? aliasMapping[inputName] : inputName);

            for (var i = 0; i < source.FieldCount; i++)
            {
                PropertyDescriptor activeTarget = props.GetByName(nameMap(source.GetName(i)), true);

                if (activeTarget == null)
                {
                    continue;
                }

                var value = source.GetValue(i);
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