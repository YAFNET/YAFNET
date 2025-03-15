// ***********************************************************************
// <copyright file="SqlBuilder.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

using PropertyAttributes = System.Reflection.PropertyAttributes;

#if !NO_EXPRESSIONS
/// <summary>
/// Nice SqlBuilder class by @samsaffron from Dapper.Contrib:
/// http://samsaffron.com/archive/2011/09/05/Digging+ourselves+out+of+the+mess+Linq-2-SQL+created
/// Modified to work in .NET 3.5
/// </summary>
public class SqlBuilder
{
    /// <summary>
    /// The data
    /// </summary>
    private readonly Dictionary<string, Clauses> data = [];
    /// <summary>
    /// The seq
    /// </summary>
    private int seq;

    /// <summary>
    /// Class Clause.
    /// </summary>
    private class Clause
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql { get; set; }
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public object Parameters { get; set; }
    }

    /// <summary>
    /// Class DynamicParameters.
    /// </summary>
    private class DynamicParameters
    {
        /// <summary>
        /// Class Property.
        /// </summary>
        private class Property
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Property" /> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="type">The type.</param>
            /// <param name="value">The value.</param>
            public Property(string name, Type type, object value)
            {
                this.Name = name;
                this.Type = type;
                this.Value = value;
            }

            /// <summary>
            /// The name
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// The type
            /// </summary>
            public readonly Type Type;
            /// <summary>
            /// The value
            /// </summary>
            public readonly object Value;
        }

        /// <summary>
        /// The properties
        /// </summary>
        private readonly List<Property> properties = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicParameters" /> class.
        /// </summary>
        /// <param name="initParams">The initialize parameters.</param>
        public DynamicParameters(object initParams)
        {
            this.AddDynamicParams(initParams);
        }

        /// <summary>
        /// Adds the dynamic parameters.
        /// </summary>
        /// <param name="cmdParams">The command parameters.</param>
        public void AddDynamicParams(object cmdParams)
        {
            if (cmdParams == null)
            {
                return;
            }

            foreach (var pi in cmdParams.GetType().GetPublicProperties())
            {
                var getterFn = pi.CreateGetter();
                if (getterFn == null)
                {
                    continue;
                }

                var value = getterFn(cmdParams);
                this.properties.Add(new Property(pi.Name, pi.PropertyType, value));
            }
        }

        // The property set and get methods require a special attrs:
        /// <summary>
        /// The get set attribute
        /// </summary>
        private const MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        /// <summary>
        /// Creates the type of the dynamic.
        /// </summary>
        /// <returns>System.Object.</returns>
        public object CreateDynamicType()
        {
            var assemblyName = new AssemblyName { Name = "tmpAssembly" };
#if NET9_0_OR_GREATER
                var typeBuilder =
                    AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                    .DefineDynamicModule("tmpModule")
                    .DefineType("SqlBuilderDynamicParameters", TypeAttributes.Public | TypeAttributes.Class);
#else
            var typeBuilder =
                Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                    .DefineDynamicModule("tmpModule")
                    .DefineType("SqlBuilderDynamicParameters", TypeAttributes.Public | TypeAttributes.Class);
#endif
            var emptyCtor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorIL = emptyCtor.GetILGenerator();

            var unsetValues = new List<Property>();

            // Loop over the attributes that will be used as the properties names in out new type
            foreach (var p in this.properties)
            {
                // Generate a private field
                var field = typeBuilder.DefineField("_" + p.Name, p.Type, FieldAttributes.Private);

                //set default values with Emit for popular types
                if (p.Type == typeof(int))
                {
                    ctorIL.Emit(OpCodes.Ldarg_0);
                    ctorIL.Emit(OpCodes.Ldc_I4, (int)p.Value);
                    ctorIL.Emit(OpCodes.Stfld, field);
                }
                else if (p.Type == typeof(long))
                {
                    ctorIL.Emit(OpCodes.Ldarg_0);
                    ctorIL.Emit(OpCodes.Ldc_I8, (long)p.Value);
                    ctorIL.Emit(OpCodes.Stfld, field);
                }
                else if (p.Type == typeof(string))
                {
                    ctorIL.Emit(OpCodes.Ldarg_0);
                    ctorIL.Emit(OpCodes.Ldstr, (string)p.Value);
                    ctorIL.Emit(OpCodes.Stfld, field);
                }
                else
                {
                    unsetValues.Add(p); //otherwise use reflection
                }

                // Generate a public property
                var property = typeBuilder.DefineProperty(p.Name, PropertyAttributes.None, p.Type, [p.Type]);

                // Define the "get" accessor method for current private field.
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + p.Name, GetSetAttr, p.Type, Type.EmptyTypes);

                // Get Property impl
                var currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                // Define the "set" accessor method for current private field.
                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + p.Name, GetSetAttr, null, [p.Type]);

                // Set Property impl
                var currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                // Hook up, getters and setters.
                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
            }

            ctorIL.Emit(OpCodes.Ret);

#if NET9_0_OR_GREATER
                var generetedType = typeBuilder.CreateTypeInfo().AsType();
#else
            var generetedType = typeBuilder.CreateType();
#endif
            var instance = Activator.CreateInstance(generetedType);

            //Using reflection for less property types. Not caching since it's a generated type.
            foreach (var p in unsetValues)
            {
                generetedType.GetProperty(p.Name).GetSetMethod().Invoke(instance, [p.Value]);
            }

            return instance;
        }
    }

    /// <summary>
    /// Class Clauses.
    /// </summary>
    private class Clauses : List<Clause>
    {
        /// <summary>
        /// The joiner
        /// </summary>
        private readonly string joiner;
        /// <summary>
        /// The prefix
        /// </summary>
        private readonly string prefix;
        /// <summary>
        /// The postfix
        /// </summary>
        private readonly string postfix;

        /// <summary>
        /// Initializes a new instance of the <see cref="Clauses" /> class.
        /// </summary>
        /// <param name="joiner">The joiner.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="postfix">The postfix.</param>
        public Clauses(string joiner, string prefix = "", string postfix = "")
        {
            this.joiner = joiner;
            this.prefix = prefix;
            this.postfix = postfix;
        }

        /// <summary>
        /// Resolves the clauses.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>System.String.</returns>
        public string ResolveClauses(DynamicParameters p)
        {
            foreach (var item in this)
            {
                p.AddDynamicParams(item.Parameters);
            }
            return this.prefix + string.Join(this.joiner, this.Select(c => c.Sql).ToArray()) + this.postfix;
        }
    }

    /// <summary>
    /// Class Template.
    /// Implements the <see cref="ServiceStack.OrmLite.ISqlExpression" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.ISqlExpression" />
    public class Template : ISqlExpression
    {
        /// <summary>
        /// The SQL
        /// </summary>
        private readonly string sql;
        /// <summary>
        /// The builder
        /// </summary>
        private readonly SqlBuilder builder;
        /// <summary>
        /// The initialize parameters
        /// </summary>
        private readonly object initParams;
        /// <summary>
        /// The data seq
        /// </summary>
        private int dataSeq = -1; // Unresolved

        /// <summary>
        /// Initializes a new instance of the <see cref="Template" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        public Template(SqlBuilder builder, string sql, object parameters)
        {
            this.initParams = parameters;
            this.sql = sql;
            this.builder = builder;
        }

        /// <summary>
        /// The regex
        /// </summary>
        private readonly static Regex regex = new(@"\/\*\*.+\*\*\/", RegexOptions.Compiled | RegexOptions.Multiline,
            TimeSpan.FromMilliseconds(100));

        /// <summary>
        /// Resolves the SQL.
        /// </summary>
        private void ResolveSql()
        {
            if (this.dataSeq != this.builder.seq)
            {
                var p = new DynamicParameters(this.initParams);

                this.rawSql = this.sql;

                foreach (var pair in this.builder.data)
                {
                    this.rawSql = this.rawSql.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                }
                this.parameters = p.CreateDynamicType();

                // replace all that is left with empty
                this.rawSql = regex.Replace(this.rawSql, "");

                this.dataSeq = this.builder.seq;
            }
        }

        /// <summary>
        /// The raw SQL
        /// </summary>
        private string rawSql;
        /// <summary>
        /// The parameters
        /// </summary>
        private object parameters;

        /// <summary>
        /// Gets the raw SQL.
        /// </summary>
        /// <value>The raw SQL.</value>
        public string RawSql { get { this.ResolveSql(); return this.rawSql; } }
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public object Parameters { get { this.ResolveSql(); return this.parameters; } }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public List<IDbDataParameter> Params { get; private set; }

        /// <summary>
        /// Converts to selectstatement.
        /// </summary>
        /// <returns>string.</returns>
        public string ToSelectStatement()
        {
            return this.ToSelectStatement(QueryType.Select);
        }

        /// <summary>
        /// Converts to selectstatement.
        /// </summary>
        /// <param name="forType">For type.</param>
        /// <returns>string.</returns>
        public string ToSelectStatement(QueryType forType)
        {
            return this.RawSql;
        }

        /// <summary>
        /// Selects the into.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        public string SelectInto<T>()
        {
            return this.RawSql;
        }

        /// <summary>
        /// Selects the into.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryType">Type of the query.</param>
        /// <returns>System.String.</returns>
        public string SelectInto<T>(QueryType queryType)
        {
            return this.RawSql;
        }
    }

    /// <summary>
    /// Adds the template.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>Template.</returns>
    public Template AddTemplate(string sql, object parameters = null)
    {
        return new Template(this, sql, parameters);
    }

    /// <summary>
    /// Adds the clause.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="joiner">The joiner.</param>
    /// <param name="prefix">The prefix.</param>
    /// <param name="postfix">The postfix.</param>
    private void AddClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "")
    {
        if (!this.data.TryGetValue(name, out var clauses))
        {
            clauses = new Clauses(joiner, prefix, postfix);
            this.data[name] = clauses;
        }
        clauses.Add(new Clause { Sql = sql, Parameters = parameters });
        this.seq++;
    }


    /// <summary>
    /// Lefts the join.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>SqlBuilder.</returns>
    public SqlBuilder LeftJoin(string sql, object parameters = null)
    {
        this.AddClause("leftjoin", sql, parameters, joiner: "\nLEFT JOIN ", prefix: "\nLEFT JOIN ", postfix: "\n");
        return this;
    }

    /// <summary>
    /// Wheres the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>SqlBuilder.</returns>
    public SqlBuilder Where(string sql, object parameters = null)
    {
        this.AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n");
        return this;
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>SqlBuilder.</returns>
    public SqlBuilder OrderBy(string sql, object parameters = null)
    {
        this.AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: "\n");
        return this;
    }

    /// <summary>
    /// Selects the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>SqlBuilder.</returns>
    public SqlBuilder Select(string sql, object parameters = null)
    {
        this.AddClause("select", sql, parameters, " , ", prefix: "", postfix: "\n");
        return this;
    }

    /// <summary>
    /// Adds the parameters.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>SqlBuilder.</returns>
    public SqlBuilder AddParameters(object parameters)
    {
        this.AddClause("--parameters", "", parameters, "");
        return this;
    }

    /// <summary>
    /// Joins the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>SqlBuilder.</returns>
    public SqlBuilder Join(string sql, object parameters = null)
    {
        this.AddClause("join", sql, parameters, joiner: "\nJOIN ", prefix: "\nJOIN", postfix: "\n");
        return this;
    }
}
#endif