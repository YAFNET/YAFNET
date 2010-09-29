/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;

namespace YAF.Classes.Utils
{
    /// <summary>
    /// The List DataTable Helper Class.
    /// </summary>
    public static class ListTableHelper
    {
        
        ///<summary>
        /// 
        /// Converts an Generics List to a DataTable
        ///</summary>
        ///<param name="list">List to Convert</param>
        ///<returns>The New Created DataTable</returns>
        public static DataTable ListToDataTable(IList list)
        {

            DataTable dt = null;
            Type listType = list.GetType();

            if (listType.IsGenericType)
            {
                Type elementType = listType.GetGenericArguments()[0];

                dt = new DataTable(elementType.Name + "List");

                MemberInfo[] miArray = elementType.GetMembers(
                    BindingFlags.Public | BindingFlags.Instance);

                foreach (MemberInfo mi in miArray)
                {
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Property:
                            {
                                PropertyInfo pi = mi as PropertyInfo;
                                dt.Columns.Add(pi.Name, pi.PropertyType);
                            }
                            break;
                        case MemberTypes.Field:
                            {
                                FieldInfo fi = mi as FieldInfo;
                                dt.Columns.Add(fi.Name, fi.FieldType);
                            }
                            break;
                    }
                }

                IList il = list;

                foreach (object record in il)
                {
                    int i = 0;
                    object[] fieldValues = new object[dt.Columns.Count];

                    foreach (MemberInfo mi in
                        from DataColumn c in dt.Columns select elementType.GetMember(c.ColumnName)[0])
                    {
                        switch (mi.MemberType)
                        {
                            case MemberTypes.Property:
                                {
                                    PropertyInfo pi = mi as PropertyInfo;
                                    fieldValues[i] = pi.GetValue(record, null);
                                }
                                break;
                            case MemberTypes.Field:
                                {
                                    FieldInfo fi = mi as FieldInfo;
                                    fieldValues[i] = fi.GetValue(record);
                                }
                                break;
                        }
                        i++;
                    }
                    dt.Rows.Add(fieldValues);
                }
            }
            return dt;

        }
    }
}