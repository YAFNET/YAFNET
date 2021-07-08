namespace ServiceStack.OrmLite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using ServiceStack.Text;
    public abstract partial class SqlExpression<T> : IHasUntypedSqlExpression, IHasDialectProvider
    {
        public const string TrueLiteral = "(1=1)";
        public const string FalseLiteral = "(1=0)";

        private Expression<Func<T, bool>> underlyingExpression;
        private List<string> orderByProperties = new();
        private string selectExpression = string.Empty;
        private string fromExpression;

        private string orderBy = string.Empty;
        public HashSet<string> OnlyFields { get; protected set; }

        public List<string> UpdateFields { get; set; }
        public List<string> InsertFields { get; set; }

        protected ModelDefinition modelDef;
        public string TableAlias { get; set; }
        public IOrmLiteDialectProvider DialectProvider { get; set; }
        public List<IDbDataParameter> Params { get; set; }
        public Func<string, string> SqlFilter { get; set; }
        public static Action<SqlExpression<T>> SelectFilter { get; set; }
        public int? Rows { get; set; }
        public int? Offset { get; set; }
        public bool PrefixFieldWithTableName { get; set; }
        public bool UseSelectPropertiesAsAliases { get; set; }
        public bool WhereStatementWithoutWhereString { get; set; }

        protected bool CustomSelect { get; set; }
        protected bool useFieldName = false;
        protected bool selectDistinct = false;
        protected bool visitedExpressionIsTableColumn = false;
        protected bool skipParameterizationForThisExpression = false;
        private bool hasEnsureConditions = false;
        private bool inSqlMethodCall = false;

        protected string Sep { get; private set; } = string.Empty;

        protected SqlExpression(IOrmLiteDialectProvider dialectProvider)
        {
            this.UpdateFields = new List<string>();
            this.InsertFields = new List<string>();

            this.modelDef = typeof(T).GetModelDefinition();
            this.PrefixFieldWithTableName = OrmLiteConfig.IncludeTablePrefixes;
            this.WhereStatementWithoutWhereString = false;

            this.DialectProvider = dialectProvider;
            this.Params = new List<IDbDataParameter>();
            this.tableDefs.Add(this.modelDef);

            var initFilter = OrmLiteConfig.SqlExpressionInitFilter;
            if (initFilter != null)
            {
                initFilter(this.GetUntyped());
            }
        }

        public SqlExpression<T> Clone()
        {
            return this.CopyTo(this.DialectProvider.SqlExpression<T>());
        }

        protected virtual SqlExpression<T> CopyTo(SqlExpression<T> to)
        {
            to.modelDef = this.modelDef;
            to.tableDefs = this.tableDefs;

            to.UpdateFields = this.UpdateFields;
            to.InsertFields = this.InsertFields;

            to.selectExpression = this.selectExpression;
            to.OnlyFields = this.OnlyFields != null ? new HashSet<string>(this.OnlyFields, StringComparer.OrdinalIgnoreCase) : null;

            to.TableAlias = this.TableAlias;
            to.fromExpression = this.fromExpression;
            to.WhereExpression = this.WhereExpression;
            to.GroupByExpression = this.GroupByExpression;
            to.HavingExpression = this.HavingExpression;
            to.orderBy = this.orderBy;
            to.orderByProperties = this.orderByProperties;

            to.Offset = this.Offset;
            to.Rows = this.Rows;

            to.CustomSelect = this.CustomSelect;
            to.PrefixFieldWithTableName = this.PrefixFieldWithTableName;
            to.useFieldName = this.useFieldName;
            to.selectDistinct = this.selectDistinct;
            to.WhereStatementWithoutWhereString = this.WhereStatementWithoutWhereString;
            to.visitedExpressionIsTableColumn = this.visitedExpressionIsTableColumn;
            to.skipParameterizationForThisExpression = this.skipParameterizationForThisExpression;
            to.UseSelectPropertiesAsAliases = this.UseSelectPropertiesAsAliases;
            to.hasEnsureConditions = this.hasEnsureConditions;

            to.Params = new List<IDbDataParameter>(this.Params);

            to.underlyingExpression = this.underlyingExpression;
            to.SqlFilter = this.SqlFilter;

            return to;
        }

        /// <summary>
        /// Generate a unique SHA1 hash of expression with param values for caching
        /// </summary>
        public string ComputeHash(bool includeParams = true)
        {
            var uniqueExpr = this.Dump(includeParams);

            // fastest up to 500 chars https://wintermute79.wordpress.com/2014/10/10/c-sha-1-benchmark/
            using var sha1 = new System.Security.Cryptography.SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(uniqueExpr));
            var hexFormat = hash.ToHex();

            return hexFormat;
        }

        /// <summary>
        /// Dump internal state of this SqlExpression into a string
        /// </summary>
        /// <param name="includeParams"></param>
        /// <returns></returns>
        public string Dump(bool includeParams)
        {
            var sb = StringBuilderCache.Allocate();

            sb.Append('<').Append(this.ModelDef.Name);
            foreach (var tableDef in this.tableDefs)
            {
                sb.Append(',').Append(tableDef);
            }

            sb.Append('>').AppendLine();

            if (!this.UpdateFields.IsEmpty())
                sb.AppendLine(this.UpdateFields.Join(","));
            if (!this.InsertFields.IsEmpty())
                sb.AppendLine(this.InsertFields.Join(","));

            if (!string.IsNullOrEmpty(this.selectExpression))
                sb.AppendLine(this.selectExpression);
            if (!this.OnlyFields.IsEmpty())
                sb.AppendLine(this.OnlyFields.Join(","));

            if (!string.IsNullOrEmpty(this.TableAlias))
                sb.AppendLine(this.TableAlias);
            if (!string.IsNullOrEmpty(this.fromExpression))
                sb.AppendLine(this.fromExpression);

            if (!string.IsNullOrEmpty(this.WhereExpression))
                sb.AppendLine(this.WhereExpression);

            if (!string.IsNullOrEmpty(this.GroupByExpression))
                sb.AppendLine(this.GroupByExpression);

            if (!string.IsNullOrEmpty(this.HavingExpression))
                sb.AppendLine(this.HavingExpression);

            if (!string.IsNullOrEmpty(this.orderBy))
                sb.AppendLine(this.orderBy);
            if (!this.orderByProperties.IsEmpty())
                sb.AppendLine(this.orderByProperties.Join(","));

            if (this.Offset != null || this.Rows != null)
                sb.Append(this.Offset ?? 0).Append(',').Append(this.Rows ?? 0).AppendLine();

            sb.Append("FLAGS:");
            sb.Append(this.CustomSelect ? "1" : "0");
            sb.Append(this.PrefixFieldWithTableName ? "1" : "0");
            sb.Append(this.useFieldName ? "1" : "0");
            sb.Append(this.selectDistinct ? "1" : "0");
            sb.Append(this.WhereStatementWithoutWhereString ? "1" : "0");
            sb.Append(this.visitedExpressionIsTableColumn ? "1" : "0");
            sb.Append(this.skipParameterizationForThisExpression ? "1" : "0");
            sb.Append(this.UseSelectPropertiesAsAliases ? "1" : "0");
            sb.Append(this.hasEnsureConditions ? "1" : "0");
            sb.AppendLine();

            if (includeParams)
            {
                sb.Append("PARAMS:").Append(this.Params.Count).AppendLine();
                if (this.Params.Count > 0)
                {
                    foreach (var p in this.Params)
                    {
                        sb.Append(p.ParameterName).Append('=');
                        sb.AppendLine(p.Value.ConvertTo<string>());
                    }
                }
            }

            var uniqueExpr = StringBuilderCache.ReturnAndFree(sb);
            return uniqueExpr;
        }

        /// <summary>
        /// Clear select expression. All properties will be selected.
        /// </summary>
        public virtual SqlExpression<T> Select()
        {
            return this.Select(string.Empty);
        }

        internal SqlExpression<T> SelectIfDistinct(string selectExpression) => this.selectDistinct ? this.SelectDistinct(selectExpression) : this.Select(selectExpression);

        /// <summary>
        /// set the specified selectExpression.
        /// </summary>
        /// <param name='selectExpression'>
        /// raw Select expression: "SomeField1, SomeField2 from SomeTable"
        /// </param>
        public virtual SqlExpression<T> Select(string selectExpression)
        {
            selectExpression?.SqlVerifyFragment();

            return this.UnsafeSelect(selectExpression);
        }

        /// <summary>
        /// set the specified DISTINCT selectExpression.
        /// </summary>
        /// <param name='selectExpression'>
        /// raw Select expression: "SomeField1, SomeField2 from SomeTable"
        /// </param>
        public virtual SqlExpression<T> SelectDistinct(string selectExpression)
        {
            selectExpression?.SqlVerifyFragment();

            return this.UnsafeSelect(selectExpression, true);
        }

        public virtual SqlExpression<T> UnsafeSelect(string rawSelect) => this.UnsafeSelect(rawSelect, false);

        public virtual SqlExpression<T> UnsafeSelect(string rawSelect, bool distinct)
        {
            if (string.IsNullOrEmpty(rawSelect))
            {
                this.BuildSelectExpression(string.Empty, distinct);
            }
            else
            {
                this.selectExpression = "SELECT " + (distinct ? "DISTINCT " : string.Empty) + rawSelect;
                this.CustomSelect = true;
                this.OnlyFields = null;
            }

            return this;
        }

        /// <summary>
        /// Set the specified selectExpression using matching fields.
        /// </summary>
        /// <param name='fields'>
        /// Matching Fields: "SomeField1, SomeField2"
        /// </param>
        public virtual SqlExpression<T> Select(string[] fields) => this.Select(fields, false);

        /// <summary>
        /// Set the specified DISTINCT selectExpression using matching fields.
        /// </summary>
        /// <param name='fields'>
        /// Matching Fields: "SomeField1, SomeField2"
        /// </param>
        public virtual SqlExpression<T> SelectDistinct(string[] fields) => this.Select(fields, true);

        internal virtual SqlExpression<T> Select(string[] fields, bool distinct)
        {
            if (fields == null || fields.Length == 0)
                return this.Select(string.Empty);

            this.useFieldName = true;

            var allTableDefs = this.GetAllTables();

            var fieldsList = new List<string>();
            var sb = StringBuilderCache.Allocate();
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field))
                    continue;

                if (field.EndsWith(".*"))
                {
                    var tableName = field.Substring(0, field.Length - 2);
                    var tableDef = allTableDefs.FirstOrDefault(x => string.Equals(x.Name, tableName, StringComparison.OrdinalIgnoreCase));
                    if (tableDef != null)
                    {
                        foreach (var fieldDef in tableDef.FieldDefinitionsArray)
                        {
                            var qualifiedField = this.GetQuotedColumnName(tableDef, fieldDef.Name);
                            if (fieldDef.CustomSelect != null)
                                qualifiedField += " AS " + fieldDef.Name;

                            if (sb.Length > 0)
                                sb.Append(", ");

                            sb.Append(qualifiedField);
                            fieldsList.Add(fieldDef.Name);
                        }
                    }
                }
                else
                {
                    fieldsList.Add(field); // Could be non-matching referenced property

                    var match = this.FirstMatchingField(field);
                    if (match == null)
                        continue;

                    var fieldDef = match.Item2;
                    var qualifiedName = this.GetQuotedColumnName(match.Item1, fieldDef.Name);
                    if (fieldDef.CustomSelect != null)
                        qualifiedName += " AS " + fieldDef.Name;

                    if (sb.Length > 0)
                        sb.Append(", ");

                    sb.Append(qualifiedName);
                }
            }

            this.UnsafeSelect(StringBuilderCache.ReturnAndFree(sb), distinct);
            this.OnlyFields = new HashSet<string>(fieldsList, StringComparer.OrdinalIgnoreCase);

            return this;
        }

        private SqlExpression<T> InternalSelect(Expression fields, bool distinct = false)
        {
            this.Reset(this.Sep = string.Empty);

            this.CustomSelect = true;
            var selectSql = this.Visit(fields);
            if (!IsSqlClass(selectSql))
            {
                selectSql = this.ConvertToParam(selectSql);
            }

            this.BuildSelectExpression(selectSql.ToString(), distinct);
            return this;
        }

        /// <summary>
        /// Fields to be selected.
        /// </summary>
        /// <param name='fields'>
        /// x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}
        /// </param>
        public virtual SqlExpression<T> Select(Expression<Func<T, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1>(Expression<Func<Table1, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> Select<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12, object>> fields)
        {
            return this.InternalSelect(fields);
        }

        public virtual SqlExpression<T> SelectDistinct(Expression<Func<T, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1>(Expression<Func<Table1, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12>(Expression<Func<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Table8, Table9, Table10, Table11, Table12, object>> fields)
        {
            return this.InternalSelect(fields, true);
        }

        public virtual SqlExpression<T> SelectDistinct()
        {
            this.selectDistinct = true;
            return this;
        }

        public virtual SqlExpression<T> From(string tables)
        {
            tables?.SqlVerifyFragment();

            return this.UnsafeFrom(tables);
        }

        public virtual SqlExpression<T> IncludeTablePrefix()
        {
            this.PrefixFieldWithTableName = true;
            return this;
        }

        public virtual SqlExpression<T> SetTableAlias(string tableAlias)
        {
            this.PrefixFieldWithTableName = tableAlias != null;
            this.TableAlias = tableAlias;
            return this;
        }

        public virtual SqlExpression<T> UnsafeFrom(string rawFrom)
        {
            if (string.IsNullOrEmpty(rawFrom))
            {
                this.FromExpression = null;
            }
            else
            {
                var singleTable = rawFrom.ToLower().IndexOfAny("join", ",") == -1;
                this.FromExpression = singleTable
                    ? " \nFROM " + this.DialectProvider.GetQuotedTableName(rawFrom)
                    : " \nFROM " + rawFrom;
            }

            return this;
        }

        public virtual SqlExpression<T> Where()
        {
            this.underlyingExpression = null; // Where() clears the expression

            this.WhereExpression = null;
            return this;
        }

        private string FormatFilter(string sqlFilter, params object[] filterParams)
        {
            if (string.IsNullOrEmpty(sqlFilter))
                return null;

            for (var i = 0; i < filterParams.Length; i++)
            {
                var pLiteral = "{" + i + "}";
                var filterParam = filterParams[i];

                if (filterParam is SqlInValues sqlParams)
                {
                    if (sqlParams.Count > 0)
                    {
                        var sqlIn = this.CreateInParamSql(sqlParams.GetValues());
                        sqlFilter = sqlFilter.Replace(pLiteral, sqlIn);
                    }
                    else
                    {
                        sqlFilter = sqlFilter.Replace(pLiteral, SqlInValues.EmptyIn);
                    }
                }
                else
                {
                    var p = this.AddParam(filterParam);
                    sqlFilter = sqlFilter.Replace(pLiteral, p.ParameterName);
                }
            }

            return sqlFilter;
        }

        private string CreateInParamSql(IEnumerable values)
        {
            var sbParams = StringBuilderCache.Allocate();
            foreach (var item in values)
            {
                var p = this.AddParam(item);

                if (sbParams.Length > 0)
                    sbParams.Append(",");

                sbParams.Append(p.ParameterName);
            }

            var sqlIn = StringBuilderCache.ReturnAndFree(sbParams);
            return sqlIn;
        }

        public virtual SqlExpression<T> UnsafeWhere(string rawSql, params object[] filterParams)
        {
            return this.AppendToWhere("AND", this.FormatFilter(rawSql, filterParams));
        }

        public virtual SqlExpression<T> Where(string sqlFilter, params object[] filterParams)
        {
            return this.AppendToWhere("AND", this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
        }

        public virtual SqlExpression<T> UnsafeAnd(string rawSql, params object[] filterParams)
        {
            return this.AppendToWhere("AND", this.FormatFilter(rawSql, filterParams));
        }

        public virtual SqlExpression<T> And(string sqlFilter, params object[] filterParams)
        {
            return this.AppendToWhere("AND", this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
        }

        public virtual SqlExpression<T> UnsafeOr(string rawSql, params object[] filterParams)
        {
            return this.AppendToWhere("OR", this.FormatFilter(rawSql, filterParams));
        }

        public virtual SqlExpression<T> Or(string sqlFilter, params object[] filterParams)
        {
            return this.AppendToWhere("OR", this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
        }

        public virtual SqlExpression<T> AddCondition(string condition, string sqlFilter, params object[] filterParams)
        {
            return this.AppendToWhere(condition, this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams));
        }

        public virtual SqlExpression<T> Where(Expression<Func<T, bool>> predicate) => this.AppendToWhere("AND", predicate);
        public virtual SqlExpression<T> Where(Expression<Func<T, bool>> predicate, params object[] filterParams) => this.AppendToWhere("AND", predicate, filterParams);

        public virtual SqlExpression<T> And(Expression<Func<T, bool>> predicate) => this.AppendToWhere("AND", predicate);
        public virtual SqlExpression<T> And(Expression<Func<T, bool>> predicate, params object[] filterParams) => this.AppendToWhere("AND", predicate, filterParams);

        public virtual SqlExpression<T> Or(Expression<Func<T, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or(Expression<Func<T, bool>> predicate, params object[] filterParams) => this.AppendToWhere("OR", predicate, filterParams);

        private LambdaExpression originalLambda;

        void Reset(string sep = " ", bool useFieldName = true)
        {
            this.Sep = sep;
            this.useFieldName = useFieldName;
            this.originalLambda = null;
        }

        protected SqlExpression<T> AppendToWhere(string condition, Expression predicate, object[] filterParams)
        {
            if (predicate == null)
                return this;

            this.Reset();

            var newExpr = WhereExpressionToString(this.Visit(predicate));
            var formatExpr = this.FormatFilter(newExpr, filterParams);
            return this.AppendToWhere(condition, formatExpr);
        }

        protected SqlExpression<T> AppendToWhere(string condition, Expression predicate)
        {
            if (predicate == null)
                return this;

            this.Reset();

            var newExpr = WhereExpressionToString(this.Visit(predicate));
            return this.AppendToWhere(condition, newExpr);
        }

        private static string WhereExpressionToString(object expression)
        {
            if (expression is bool b)
                return b ? TrueLiteral : FalseLiteral;
            return expression.ToString();
        }

        protected SqlExpression<T> AppendToWhere(string condition, string sqlExpression)
        {
            var addExpression = string.IsNullOrEmpty(this.WhereExpression)
                ? (this.WhereStatementWithoutWhereString ? string.Empty : "WHERE ") + sqlExpression
                : " " + condition + " " + sqlExpression;

            if (!this.hasEnsureConditions)
            {
                this.WhereExpression += addExpression;
            }
            else
            {
                if (this.WhereExpression[this.WhereExpression.Length - 1] != ')')
                    throw new NotSupportedException("Invalid whereExpression Expression with Ensure Conditions");

                // insert before normal WHERE parens: {EnsureConditions} AND (1+1)
                if (this.WhereExpression.EndsWith(TrueLiteral, StringComparison.Ordinal))
                {
                    // insert before ^1+1)
                    this.WhereExpression = this.WhereExpression.Substring(0, this.WhereExpression.Length - (TrueLiteral.Length - 1))
                                           + sqlExpression + ")";
                }
                else
                {
                    // insert before ^)
                    this.WhereExpression = this.WhereExpression.Substring(0, this.WhereExpression.Length - 1)
                                           + addExpression + ")";
                }
            }

            return this;
        }

        public virtual SqlExpression<T> Ensure(Expression<Func<T, bool>> predicate) => this.AppendToEnsure(predicate);
        public virtual SqlExpression<T> Ensure<Target>(Expression<Func<Target, bool>> predicate) => this.AppendToEnsure(predicate);
        public virtual SqlExpression<T> Ensure<Source, Target>(Expression<Func<Source, Target, bool>> predicate) => this.AppendToEnsure(predicate);
        public virtual SqlExpression<T> Ensure<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate) => this.AppendToEnsure(predicate);
        public virtual SqlExpression<T> Ensure<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate) => this.AppendToEnsure(predicate);
        public virtual SqlExpression<T> Ensure<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate) => this.AppendToEnsure(predicate);

        protected SqlExpression<T> AppendToEnsure(Expression predicate)
        {
            if (predicate == null)
                return this;

            this.Reset();

            var newExpr = WhereExpressionToString(this.Visit(predicate));
            return this.Ensure(newExpr);
        }

        /// <summary>
        /// Add a WHERE Condition to always be applied, irrespective of other WHERE conditions
        /// </summary>
        public SqlExpression<T> Ensure(string sqlFilter, params object[] filterParams)
        {
            var condition = this.FormatFilter(sqlFilter, filterParams);
            if (string.IsNullOrEmpty(this.WhereExpression))
            {
                this.WhereExpression = "WHERE " + condition
                                                + " AND " + TrueLiteral; // allow subsequent WHERE conditions to be inserted before parens
            }
            else
            {
                if (!this.hasEnsureConditions)
                {
                    var existingExpr = this.WhereExpression.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase)
                        ? this.WhereExpression.Substring("WHERE ".Length)
                        : this.WhereExpression;

                    this.WhereExpression = "WHERE " + condition + " AND (" + existingExpr + ")";
                }
                else
                {
                    if (!this.WhereExpression.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
                        throw new NotSupportedException("Invalid whereExpression Expression with Ensure Conditions");

                    this.WhereExpression = "WHERE " + condition + " AND " + this.WhereExpression.Substring("WHERE ".Length);
                }
            }

            this.hasEnsureConditions = true;
            return this;
        }

        private string ListExpression(Expression expr, string strExpr)
        {
            if (expr is LambdaExpression lambdaExpr)
            {
                if (lambdaExpr.Parameters.Count == 1 && lambdaExpr.Body is MemberExpression me)
                {
                    var tableDef = lambdaExpr.Parameters[0].Type.GetModelMetadata();
                    var fieldDef = tableDef?.GetFieldDefinition(me.Member.Name);
                    if (fieldDef != null)
                        return this.DialectProvider.GetQuotedColumnName(tableDef, me.Member.Name);
                }
            }

            return strExpr;
        }

        public virtual SqlExpression<T> GroupBy()
        {
            return this.GroupBy(string.Empty);
        }

        public virtual SqlExpression<T> GroupBy(string groupBy)
        {
            return this.UnsafeGroupBy(groupBy.SqlVerifyFragment());
        }

        public virtual SqlExpression<T> UnsafeGroupBy(string groupBy)
        {
            if (!string.IsNullOrEmpty(groupBy))
                this.GroupByExpression = "GROUP BY " + groupBy;
            return this;
        }

        private SqlExpression<T> InternalGroupBy(Expression expr)
        {
            this.Reset(this.Sep = string.Empty);

            var groupByExpr = this.Visit(expr);
            if (IsSqlClass(groupByExpr))
            {
                StripAliases(groupByExpr as SelectList); // No "AS ColumnAlias" in GROUP BY, just the column names/expressions

                return this.GroupBy(groupByExpr.ToString());
            }

            if (groupByExpr is string strExpr)
            {
                return this.GroupBy(this.ListExpression(expr, strExpr));
            }

            return this;
        }

        public virtual SqlExpression<T> GroupBy<Table>(Expression<Func<Table, object>> keySelector)
        {
            return this.InternalGroupBy(keySelector);
        }

        public virtual SqlExpression<T> GroupBy<Table1, Table2>(Expression<Func<Table1, Table2, object>> keySelector)
        {
            return this.InternalGroupBy(keySelector);
        }

        public virtual SqlExpression<T> GroupBy<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> keySelector)
        {
            return this.InternalGroupBy(keySelector);
        }

        public virtual SqlExpression<T> GroupBy<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> keySelector)
        {
            return this.InternalGroupBy(keySelector);
        }

        public virtual SqlExpression<T> GroupBy(Expression<Func<T, object>> keySelector)
        {
            return this.InternalGroupBy(keySelector);
        }

        public virtual SqlExpression<T> Having()
        {
            return this.Having(string.Empty);
        }

        public virtual SqlExpression<T> Having(string sqlFilter, params object[] filterParams)
        {
            this.HavingExpression = this.FormatFilter(sqlFilter.SqlVerifyFragment(), filterParams);

            if (this.HavingExpression != null) this.HavingExpression = "HAVING " + this.HavingExpression;

            return this;
        }

        public virtual SqlExpression<T> UnsafeHaving(string sqlFilter, params object[] filterParams)
        {
            this.HavingExpression = this.FormatFilter(sqlFilter, filterParams);

            if (this.HavingExpression != null) this.HavingExpression = "HAVING " + this.HavingExpression;

            return this;
        }

        protected SqlExpression<T> AppendHaving(Expression predicate)
        {
            if (predicate != null)
            {
                this.Reset();

                this.HavingExpression = WhereExpressionToString(this.Visit(predicate));
                if (!string.IsNullOrEmpty(this.HavingExpression)) this.HavingExpression = "HAVING " + this.HavingExpression;
            }
            else this.HavingExpression = string.Empty;

            return this;
        }

        public virtual SqlExpression<T> Having(Expression<Func<T, bool>> predicate) => this.AppendHaving(predicate);
        public virtual SqlExpression<T> Having<Table>(Expression<Func<Table, bool>> predicate) => this.AppendHaving(predicate);
        public virtual SqlExpression<T> Having<Table1, Table2>(Expression<Func<Table1, Table2, bool>> predicate) => this.AppendHaving(predicate);
        public virtual SqlExpression<T> Having<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, bool>> predicate) => this.AppendHaving(predicate);

        public virtual SqlExpression<T> OrderBy() => this.OrderBy(string.Empty);

        public virtual SqlExpression<T> OrderBy(string orderBy) => this.UnsafeOrderBy(orderBy.SqlVerifyFragment());

        public virtual SqlExpression<T> OrderBy(long columnIndex) => this.UnsafeOrderBy(columnIndex.ToString());

        public virtual SqlExpression<T> UnsafeOrderBy(string orderBy)
        {
            this.orderByProperties.Clear();
            if (!string.IsNullOrEmpty(orderBy))
            {
                this.orderByProperties.Add(orderBy);
            }

            this.BuildOrderByClauseInternal();
            return this;
        }

        public virtual SqlExpression<T> OrderByRandom() => this.OrderBy(this.DialectProvider.SqlRandom);

        public ModelDefinition GetModelDefinition(FieldDefinition fieldDef)
        {
            if (this.modelDef.FieldDefinitions.Any(x => x == fieldDef))
                return this.modelDef;

            return this.tableDefs
                .FirstOrDefault(tableDef => tableDef.FieldDefinitions.Any(x => x == fieldDef));
        }

        private SqlExpression<T> OrderByFields(string orderBySuffix, FieldDefinition[] fields)
        {
            this.orderByProperties.Clear();

            if (fields.Length == 0)
            {
                this.orderBy = null;
                return this;
            }

            this.useFieldName = true;

            var sbOrderBy = StringBuilderCache.Allocate();
            foreach (var field in fields)
            {
                var tableDef = this.GetModelDefinition(field);
                var qualifiedName = this.modelDef != null
                    ? this.GetQuotedColumnName(tableDef, field.Name)
                    : this.DialectProvider.GetQuotedColumnName(field);

                if (sbOrderBy.Length > 0)
                    sbOrderBy.Append(", ");

                sbOrderBy.Append(qualifiedName + orderBySuffix);
            }

            this.orderBy = "ORDER BY " + StringBuilderCache.ReturnAndFree(sbOrderBy);
            return this;
        }

        static class OrderBySuffix
        {
            public const string Asc = "";
            public const string Desc = " DESC";
        }

        public virtual SqlExpression<T> OrderByFields(params FieldDefinition[] fields) => this.OrderByFields(OrderBySuffix.Asc, fields);

        public virtual SqlExpression<T> OrderByFieldsDescending(params FieldDefinition[] fields) => this.OrderByFields(OrderBySuffix.Desc, fields);

        private SqlExpression<T> OrderByFields(string orderBySuffix, string[] fieldNames)
        {
            this.orderByProperties.Clear();

            if (fieldNames.Length == 0)
            {
                this.orderBy = null;
                return this;
            }

            this.useFieldName = true;

            var sbOrderBy = StringBuilderCache.Allocate();
            foreach (var fieldName in fieldNames)
            {
                var reverse = fieldName.StartsWith("-");
                var useSuffix = reverse
                    ? orderBySuffix == OrderBySuffix.Asc ? OrderBySuffix.Desc : OrderBySuffix.Asc
                    : orderBySuffix;
                var useName = reverse ? fieldName.Substring(1) : fieldName;

                var field = this.FirstMatchingField(useName);
                var qualifiedName = field != null
                    ? this.GetQuotedColumnName(field.Item1, field.Item2.Name)
                    : string.Equals("Random", useName, StringComparison.OrdinalIgnoreCase)
                        ? this.DialectProvider.SqlRandom
                        : throw new ArgumentException("Could not find field " + useName);

                if (sbOrderBy.Length > 0)
                    sbOrderBy.Append(", ");

                sbOrderBy.Append(qualifiedName + useSuffix);
            }

            this.orderBy = "ORDER BY " + StringBuilderCache.ReturnAndFree(sbOrderBy);
            return this;
        }

        public virtual SqlExpression<T> OrderByFields(params string[] fieldNames) => this.OrderByFields(string.Empty, fieldNames);

        public virtual SqlExpression<T> OrderByFieldsDescending(params string[] fieldNames) => this.OrderByFields(" DESC", fieldNames);

        public virtual SqlExpression<T> OrderBy(Expression<Func<T, object>> keySelector) => this.OrderByInternal(keySelector);

        public virtual SqlExpression<T> OrderBy<Table>(Expression<Func<Table, object>> fields) => this.OrderByInternal(fields);
        public virtual SqlExpression<T> OrderBy<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields) => this.OrderByInternal(fields);
        public virtual SqlExpression<T> OrderBy<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields) => this.OrderByInternal(fields);
        public virtual SqlExpression<T> OrderBy<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields) => this.OrderByInternal(fields);
        public virtual SqlExpression<T> OrderBy<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields) => this.OrderByInternal(fields);

        private SqlExpression<T> OrderByInternal(Expression expr)
        {
            this.Reset(this.Sep = string.Empty);

            this.orderByProperties.Clear();
            var orderBySql = this.Visit(expr);
            if (IsSqlClass(orderBySql))
            {
                var fields = orderBySql.ToString();
                this.orderByProperties.Add(fields);
                this.BuildOrderByClauseInternal();
            }
            else if (orderBySql is string strExpr)
            {
                return this.GroupBy(this.ListExpression(expr, strExpr));
            }

            return this;
        }

        public static bool IsSqlClass(object obj)
        {
            return obj != null &&
                   (obj is PartialSqlString ||
                    obj is SelectList);
        }

        public virtual SqlExpression<T> ThenBy(string orderBy)
        {
            orderBy.SqlVerifyFragment();
            this.orderByProperties.Add(orderBy);
            this.BuildOrderByClauseInternal();
            return this;
        }

        public virtual SqlExpression<T> ThenBy(Expression<Func<T, object>> keySelector) => this.ThenByInternal(keySelector);
        public virtual SqlExpression<T> ThenBy<Table>(Expression<Func<Table, object>> fields) => this.ThenByInternal(fields);
        public virtual SqlExpression<T> ThenBy<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields) => this.ThenByInternal(fields);
        public virtual SqlExpression<T> ThenBy<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields) => this.ThenByInternal(fields);
        public virtual SqlExpression<T> ThenBy<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields) => this.ThenByInternal(fields);
        public virtual SqlExpression<T> ThenBy<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields) => this.ThenByInternal(fields);

        private SqlExpression<T> ThenByInternal(Expression keySelector)
        {
            this.Reset(this.Sep = string.Empty);

            var orderBySql = this.Visit(keySelector);
            if (IsSqlClass(orderBySql))
            {
                var fields = orderBySql.ToString();
                this.orderByProperties.Add(fields);
                this.BuildOrderByClauseInternal();
            }

            return this;
        }

        public virtual SqlExpression<T> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            return this.OrderByDescendingInternal(keySelector);
        }

        public virtual SqlExpression<T> OrderByDescending<Table>(Expression<Func<Table, object>> keySelector) => this.OrderByDescendingInternal(keySelector);
        public virtual SqlExpression<T> OrderByDescending<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields) => this.OrderByDescendingInternal(fields);
        public virtual SqlExpression<T> OrderByDescending<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields) => this.OrderByDescendingInternal(fields);
        public virtual SqlExpression<T> OrderByDescending<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields) => this.OrderByDescendingInternal(fields);
        public virtual SqlExpression<T> OrderByDescending<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields) => this.OrderByDescendingInternal(fields);

        private SqlExpression<T> OrderByDescendingInternal(Expression keySelector)
        {
            this.Reset(this.Sep = string.Empty);

            this.orderByProperties.Clear();
            var orderBySql = this.Visit(keySelector);
            if (IsSqlClass(orderBySql))
            {
                var fields = orderBySql.ToString();
                fields.ParseTokens()
                    .Each(x => this.orderByProperties.Add(x + " DESC"));
                this.BuildOrderByClauseInternal();
            }

            return this;
        }

        public virtual SqlExpression<T> OrderByDescending(string orderBy) => this.UnsafeOrderByDescending(orderBy.SqlVerifyFragment());

        public virtual SqlExpression<T> OrderByDescending(long columnIndex) => this.UnsafeOrderByDescending(columnIndex.ToString());

        private SqlExpression<T> UnsafeOrderByDescending(string orderBy)
        {
            this.orderByProperties.Clear();
            this.orderByProperties.Add(orderBy + " DESC");
            this.BuildOrderByClauseInternal();
            return this;
        }

        public virtual SqlExpression<T> ThenByDescending(string orderBy)
        {
            orderBy.SqlVerifyFragment();
            this.orderByProperties.Add(orderBy + " DESC");
            this.BuildOrderByClauseInternal();
            return this;
        }

        public virtual SqlExpression<T> ThenByDescending(Expression<Func<T, object>> keySelector) => this.ThenByDescendingInternal(keySelector);
        public virtual SqlExpression<T> ThenByDescending<Table>(Expression<Func<Table, object>> fields) => this.ThenByDescendingInternal(fields);
        public virtual SqlExpression<T> ThenByDescending<Table1, Table2>(Expression<Func<Table1, Table2, object>> fields) => this.ThenByDescendingInternal(fields);
        public virtual SqlExpression<T> ThenByDescending<Table1, Table2, Table3>(Expression<Func<Table1, Table2, Table3, object>> fields) => this.ThenByDescendingInternal(fields);
        public virtual SqlExpression<T> ThenByDescending<Table1, Table2, Table3, Table4>(Expression<Func<Table1, Table2, Table3, Table4, object>> fields) => this.ThenByDescendingInternal(fields);
        public virtual SqlExpression<T> ThenByDescending<Table1, Table2, Table3, Table4, Table5>(Expression<Func<Table1, Table2, Table3, Table4, Table5, object>> fields) => this.ThenByDescendingInternal(fields);

        private SqlExpression<T> ThenByDescendingInternal(Expression keySelector)
        {
            this.Reset(this.Sep = string.Empty);

            var orderBySql = this.Visit(keySelector);
            if (IsSqlClass(orderBySql))
            {
                var fields = orderBySql.ToString();
                fields.ParseTokens()
                    .Each(x => this.orderByProperties.Add(x + " DESC"));
                this.BuildOrderByClauseInternal();
            }

            return this;
        }

        private void BuildOrderByClauseInternal()
        {
            if (this.orderByProperties.Count > 0)
            {
                var sb = StringBuilderCache.Allocate();
                foreach (var prop in this.orderByProperties)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");

                    sb.Append(prop);
                }

                this.orderBy = "ORDER BY " + StringBuilderCache.ReturnAndFree(sb);
            }
            else
            {
                this.orderBy = null;
            }
        }

        /// <summary>
        /// Offset of the first row to return. The offset of the initial row is 0
        /// </summary>
        public virtual SqlExpression<T> Skip(int? skip = null)
        {
            this.Offset = skip;
            return this;
        }

        /// <summary>
        /// Number of rows returned by a SELECT statement
        /// </summary>
        public virtual SqlExpression<T> Take(int? take = null)
        {
            this.Rows = take;
            return this;
        }

        /// <summary>
        /// Set the specified offset and rows for SQL Limit clause.
        /// </summary>
        /// <param name='skip'>
        /// Offset of the first row to return. The offset of the initial row is 0
        /// </param>
        /// <param name='rows'>
        /// Number of rows returned by a SELECT statement
        /// </param>
        public virtual SqlExpression<T> Limit(int skip, int rows)
        {
            this.Offset = skip;
            this.Rows = rows;
            return this;
        }

        /// <summary>
        /// Set the specified offset and rows for SQL Limit clause where they exist.
        /// </summary>
        /// <param name='skip'>
        /// Offset of the first row to return. The offset of the initial row is 0
        /// </param>
        /// <param name='rows'>
        /// Number of rows returned by a SELECT statement
        /// </param>
        public virtual SqlExpression<T> Limit(int? skip, int? rows)
        {
            this.Offset = skip;
            this.Rows = rows;
            return this;
        }

        /// <summary>
        /// Set the specified rows for Sql Limit clause.
        /// </summary>
        /// <param name='rows'>
        /// Number of rows returned by a SELECT statement
        /// </param>
        public virtual SqlExpression<T> Limit(int rows)
        {
            this.Offset = null;
            this.Rows = rows;
            return this;
        }

        /// <summary>
        /// Clear Sql Limit clause
        /// </summary>
        public virtual SqlExpression<T> Limit()
        {
            this.Offset = null;
            this.Rows = null;
            return this;
        }

        /// <summary>
        /// Clear Offset and Limit clauses. Alias for Limit()
        /// </summary>
        /// <returns></returns>
        public virtual SqlExpression<T> ClearLimits()
        {
            return this.Limit();
        }

        /// <summary>
        /// Fields to be updated.
        /// </summary>
        /// <param name='updatefields'>
        /// List&lt;string&gt; containing Names of properties to be updated
        /// </param>
        public virtual SqlExpression<T> Update(List<string> updateFields)
        {
            this.UpdateFields = updateFields;
            return this;
        }

        /// <summary>
        /// Fields to be updated.
        /// </summary>
        /// <param name='updatefields'>
        /// IEnumerable&lt;string&gt; containing Names of properties to be updated
        /// </param>
        public virtual SqlExpression<T> Update(IEnumerable<string> updateFields)
        {
            this.UpdateFields = new List<string>(updateFields);
            return this;
        }

        /// <summary>
        /// Fields to be updated.
        /// </summary>
        /// <param name='fields'>
        /// x=> x.SomeProperty1 or x=> new { x.SomeProperty1, x.SomeProperty2 }
        /// </param>
        public virtual SqlExpression<T> Update(Expression<Func<T, object>> fields)
        {
            this.Reset(this.Sep = string.Empty, this.useFieldName = false);
            this.UpdateFields = fields.GetFieldNames().ToList();
            return this;
        }

        /// <summary>
        /// Clear UpdateFields list ( all fields will be updated)
        /// </summary>
        public virtual SqlExpression<T> Update()
        {
            this.UpdateFields = new List<string>();
            return this;
        }

        /// <summary>
        /// Fields to be inserted.
        /// </summary>
        /// <param name='fields'>
        /// x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}
        /// </param>
        /// <typeparam name='TKey'>
        /// objectWithProperties
        /// </typeparam>
        public virtual SqlExpression<T> Insert<TKey>(Expression<Func<T, TKey>> fields)
        {
            this.Reset(this.Sep = string.Empty, this.useFieldName = false);
            var fieldList = this.Visit(fields);
            this.InsertFields = fieldList.ToString().Split(',').Select(f => f.Trim()).ToList();
            return this;
        }

        /// <summary>
        /// fields to be inserted.
        /// </summary>
        /// <param name='insertFields'>
        /// IList&lt;string&gt; containing Names of properties to be inserted
        /// </param>
        public virtual SqlExpression<T> Insert(List<string> insertFields)
        {
            this.InsertFields = insertFields;
            return this;
        }

        /// <summary>
        /// Clear InsertFields list ( all fields will be inserted)
        /// </summary>
        public virtual SqlExpression<T> Insert()
        {
            this.InsertFields = new List<string>();
            return this;
        }

        public virtual SqlExpression<T> WithSqlFilter(Func<string, string> sqlFilter)
        {
            this.SqlFilter = sqlFilter;
            return this;
        }

        public string SqlTable(ModelDefinition modelDef)
        {
            return this.DialectProvider.GetQuotedTableName(modelDef);
        }

        public string SqlColumn(string columnName)
        {
            return this.DialectProvider.GetQuotedColumnName(columnName);
        }

        public virtual IDbDataParameter AddParam(object value)
        {
            var paramName = this.Params.Count.ToString();
            var paramValue = value;

            var parameter = this.CreateParam(paramName, paramValue);
            this.DialectProvider.InitQueryParam(parameter);
            this.Params.Add(parameter);
            return parameter;
        }

        public string ConvertToParam(object value)
        {
            var p = this.AddParam(value);
            return p.ParameterName;
        }

        public virtual void CopyParamsTo(IDbCommand dbCmd)
        {
            try
            {
                foreach (var sqlParam in this.Params)
                {
                    dbCmd.Parameters.Add(sqlParam);
                }
            }
            catch (Exception)
            {
                // SQL Server + PostgreSql doesn't allow re-using db params in multiple queries
                foreach (var sqlParam in this.Params)
                {
                    var p = dbCmd.CreateParameter();
                    p.PopulateWith(sqlParam);
                    dbCmd.Parameters.Add(p);
                }
            }
        }

        public virtual string ToDeleteRowStatement()
        {
            string sql;
            var hasTableJoin = this.tableDefs.Count > 1;
            if (hasTableJoin)
            {
                var clone = this.Clone();
                var pk = this.DialectProvider.GetQuotedColumnName(this.modelDef, this.modelDef.PrimaryKey);
                clone.Select(pk);
                var subSql = clone.ToSelectStatement(QueryType.Select);
                sql = $"DELETE FROM {this.DialectProvider.GetQuotedTableName(this.modelDef)} WHERE {pk} IN ({subSql})";
            }
            else
            {
                sql = $"DELETE FROM {this.DialectProvider.GetQuotedTableName(this.modelDef)} {this.WhereExpression}";
            }

            return this.SqlFilter != null
                ? this.SqlFilter(sql)
                : sql;
        }

        public virtual void PrepareUpdateStatement(IDbCommand dbCmd, T item, bool excludeDefaults = false)
        {
            this.CopyParamsTo(dbCmd);

            var setFields = StringBuilderCache.Allocate();

            foreach (var fieldDef in this.modelDef.FieldDefinitions)
            {
                if (fieldDef.ShouldSkipUpdate())
                    continue;
                if (fieldDef.IsRowVersion)
                    continue;
                if (this.UpdateFields.Count > 0
                    && !this.UpdateFields.Contains(fieldDef.Name)) continue; // added

                var value = fieldDef.GetValue(item);
                if (excludeDefaults
                    && (value == null || !fieldDef.IsNullable && value.Equals(value.GetType().GetDefaultValue())))
                    continue;

                if (setFields.Length > 0)
                    setFields.Append(", ");

                setFields
                    .Append(this.DialectProvider.GetQuotedColumnName(fieldDef.FieldName))
                    .Append("=")
                    .Append(this.DialectProvider.GetUpdateParam(dbCmd, value, fieldDef));
            }

            if (setFields.Length == 0)
                throw new ArgumentException($"No non-null or non-default values were provided for type: {typeof(T).Name}");

            var sql = $"UPDATE {this.DialectProvider.GetQuotedTableName(this.modelDef)} " +
                      $"SET {StringBuilderCache.ReturnAndFree(setFields)} {this.WhereExpression}";

            dbCmd.CommandText = this.SqlFilter != null
                ? this.SqlFilter(sql)
                : sql;
        }

        public virtual void PrepareUpdateStatement(IDbCommand dbCmd, Dictionary<string, object> updateFields)
        {
            this.CopyParamsTo(dbCmd);

            var setFields = StringBuilderCache.Allocate();

            foreach (var entry in updateFields)
            {
                var fieldDef = this.ModelDef.AssertFieldDefinition(entry.Key);
                if (fieldDef.ShouldSkipUpdate())
                    continue;
                if (fieldDef.IsRowVersion)
                    continue;

                if (this.UpdateFields.Count > 0
                    && !this.UpdateFields.Contains(fieldDef.Name)) // added
                    continue;

                var value = entry.Value;
                if (value == null && !fieldDef.IsNullable)
                    continue;

                if (setFields.Length > 0)
                    setFields.Append(", ");

                setFields
                    .Append(this.DialectProvider.GetQuotedColumnName(fieldDef.FieldName))
                    .Append("=")
                    .Append(this.DialectProvider.GetUpdateParam(dbCmd, value, fieldDef));
            }

            if (setFields.Length == 0)
                throw new ArgumentException($"No non-null or non-default values were provided for type: {typeof(T).Name}");

            var sql = $"UPDATE {this.DialectProvider.GetQuotedTableName(this.modelDef)} " +
                      $"SET {StringBuilderCache.ReturnAndFree(setFields)} {this.WhereExpression}";

            dbCmd.CommandText = this.SqlFilter != null
                ? this.SqlFilter(sql)
                : sql;
        }

        public virtual string ToSelectStatement() => ToSelectStatement(QueryType.Select);

        public virtual string ToSelectStatement(QueryType forType)
        {
            SelectFilter?.Invoke(this);
            OrmLiteConfig.SqlExpressionSelectFilter?.Invoke(this.GetUntyped());

            var sql = this.DialectProvider
                .ToSelectStatement(forType, modelDef, this.SelectExpression, this.BodyExpression, this.OrderByExpression, offset: Offset, rows: Rows);

            return this.SqlFilter != null
                ? this.SqlFilter(sql)
                : sql;
        }

        /// <summary>
        /// Merge params into an encapsulated SQL Statement with embedded param values
        /// </summary>
        public virtual string ToMergedParamsSelectStatement()
        {
            var sql = this.ToSelectStatement(QueryType.Select);
            var mergedSql = this.DialectProvider.MergeParamsIntoSql(sql, this.Params);
            return mergedSql;
        }

        public virtual string ToCountStatement()
        {
            SelectFilter?.Invoke(this);
            OrmLiteConfig.SqlExpressionSelectFilter?.Invoke(this.GetUntyped());

            var sql = "SELECT COUNT(*)" + this.BodyExpression;

            return this.SqlFilter != null
                ? this.SqlFilter(sql)
                : sql;
        }

        public string SelectExpression
        {
            get
            {
                if (string.IsNullOrEmpty(this.selectExpression)) this.BuildSelectExpression(string.Empty, false);
                return this.selectExpression;
            }

            set => this.selectExpression = value;
        }

        public string FromExpression
        {
            get => string.IsNullOrEmpty(this.fromExpression)
                ? " \nFROM " + this.DialectProvider.GetQuotedTableName(this.modelDef) + (this.TableAlias != null ? " " + this.DialectProvider.GetQuotedName(this.TableAlias) : string.Empty)
                : this.fromExpression;
            set => this.fromExpression = value;
        }

        public string BodyExpression =>
            this.FromExpression
            + (string.IsNullOrEmpty(this.WhereExpression) ? string.Empty : "\n" + this.WhereExpression)
            + (string.IsNullOrEmpty(this.GroupByExpression) ? string.Empty : "\n" + this.GroupByExpression)
            + (string.IsNullOrEmpty(this.HavingExpression) ? string.Empty : "\n" + this.HavingExpression);

        public string WhereExpression { get; set; }

        public string GroupByExpression { get; set; } = string.Empty;

        public string HavingExpression { get; set; }

        public string OrderByExpression
        {
            get => string.IsNullOrEmpty(this.orderBy) ? string.Empty : "\n" + this.orderBy;
            set => this.orderBy = value;
        }

        public ModelDefinition ModelDef
        {
            get => this.modelDef;
            protected set => this.modelDef = value;
        }

        protected internal bool UseFieldName
        {
            get => this.useFieldName;
            set => this.useFieldName = value;
        }

        public virtual object Visit(Expression exp)
        {
            this.visitedExpressionIsTableColumn = false;

            if (exp == null)
                return string.Empty;

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return this.VisitLambda(exp as LambdaExpression);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess(exp as MemberExpression);
                case ExpressionType.Constant:
                    return this.VisitConstant(exp as ConstantExpression);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                    // return "(" + VisitBinary(exp as BinaryExpression) + ")";
                    return this.VisitBinary(exp as BinaryExpression);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary(exp as UnaryExpression);
                case ExpressionType.Parameter:
                    return this.VisitParameter(exp as ParameterExpression);
                case ExpressionType.Call:
                    return this.VisitMethodCall(exp as MethodCallExpression);
                case ExpressionType.New:
                    return this.VisitNew(exp as NewExpression);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray(exp as NewArrayExpression);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit(exp as MemberInitExpression);
                case ExpressionType.Index:
                    return this.VisitIndexExpression(exp as IndexExpression);
                case ExpressionType.Conditional:
                    return this.VisitConditional(exp as ConditionalExpression);
                default:
                    return exp.ToString();
            }
        }

        protected internal virtual object VisitJoin(Expression exp)
        {
            this.skipParameterizationForThisExpression = true;
            var visitedExpression = this.Visit(exp);
            this.skipParameterizationForThisExpression = false;
            return visitedExpression;
        }

        protected virtual object VisitLambda(LambdaExpression lambda)
        {
            if (this.originalLambda == null) this.originalLambda = lambda;

            if (lambda.Body.NodeType == ExpressionType.MemberAccess && this.Sep == " ")
            {
                MemberExpression m = lambda.Body as MemberExpression;

                if (m.Expression != null)
                {
                    var r = this.VisitMemberAccess(m);
                    if (!(r is PartialSqlString))
                        return r;

                    if (m.Expression.Type.IsNullableType())
                        return r.ToString();

                    return $"{r}={this.GetQuotedTrueValue()}";
                }

            }
            else if (lambda.Body.NodeType == ExpressionType.Conditional && this.Sep == " ")
            {
                ConditionalExpression c = lambda.Body as ConditionalExpression;

                var r = this.VisitConditional(c);
                if (!(r is PartialSqlString))
                    return r;

                return $"{r}={this.GetQuotedTrueValue()}";
            }

            return this.Visit(lambda.Body);
        }

        public virtual object GetValue(object value, Type type)
        {
            if (this.skipParameterizationForThisExpression)
                return this.DialectProvider.GetQuotedValue(value, type);

            var paramValue = this.DialectProvider.GetParamValue(value, type);
            return paramValue ?? "null";
        }

        protected virtual object VisitBinary(BinaryExpression b)
        {
            object originalLeft = null, originalRight = null, left, right;
            var operand = this.BindOperant(b.NodeType);   // sep= " " ??
            if (operand == "AND" || operand == "OR")
            {
                if (this.IsBooleanComparison(b.Left))
                {
                    left = this.VisitMemberAccess((MemberExpression)b.Left);
                    if (left is PartialSqlString)
                        left = new PartialSqlString($"{left}={this.GetQuotedTrueValue()}");
                }
                else if (b.Left is ConditionalExpression)
                {
                    left = this.VisitConditional((ConditionalExpression)b.Left);
                    if (left is PartialSqlString)
                        left = new PartialSqlString($"{left}={this.GetQuotedTrueValue()}");
                }
                else left = this.Visit(b.Left);

                if (this.IsBooleanComparison(b.Right))
                {
                    right = this.VisitMemberAccess((MemberExpression)b.Right);
                    if (right is PartialSqlString)
                        right = new PartialSqlString($"{right}={this.GetQuotedTrueValue()}");
                }
                else if (b.Right is ConditionalExpression)
                {
                    right = this.VisitConditional((ConditionalExpression)b.Right);
                    if (right is PartialSqlString)
                        right = new PartialSqlString($"{right}={this.GetQuotedTrueValue()}");
                }
                else right = this.Visit(b.Right);

                if (!(left is PartialSqlString) && !(right is PartialSqlString))
                {
                    var result = CachedExpressionCompiler.Evaluate(this.PreEvaluateBinary(b, left, right));
                    return result;
                }

                if (!(left is PartialSqlString))
                    left = (bool)left ? this.GetTrueExpression() : this.GetFalseExpression();
                if (!(right is PartialSqlString))
                    right = (bool)right ? this.GetTrueExpression() : this.GetFalseExpression();
            }
            else if ((operand == "=" || operand == "<>") && b.Left is MethodCallExpression && ((MethodCallExpression)b.Left).Method.Name == "CompareString")
            {
                // Handle VB.NET converting (x => x.Name == "Foo") into (x => CompareString(x.Name, "Foo", False)
                var methodExpr = (MethodCallExpression)b.Left;
                var args = this.VisitExpressionList(methodExpr.Arguments);
                right = this.GetValue(args[1], typeof(string));
                this.ConvertToPlaceholderAndParameter(ref right);
                return new PartialSqlString($"({args[0]} {operand} {right})");
            }
            else
            {
                originalLeft = left = this.Visit(b.Left);
                originalRight = right = this.Visit(b.Right);

                // Handle "expr = true/false", including with the constant on the left
                if (operand == "=" || operand == "<>")
                {
                    if (left is bool)
                    {
                        Swap(ref left, ref right); // Should be safe to swap for equality/inequality checks
                    }

                    if (right is bool &&
                        (left == null || left.ToString().Equals("null", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (operand == "=")
                            return false; // "null == true/false" becomes "false"
                        if (operand == "<>")
                            return true; // "null != true/false" becomes "true"
                    }

                    if (right is bool && !this.IsFieldName(left) && !(b.Left is ConditionalExpression))
                    {
                        // Don't change anything when "expr" is a column name or ConditionalExpression - then we really want "ColName = 1" or (Case When 1=0 Then 1 Else 0 End = 1)
                        if (operand == "=")
                            return (bool)right ? left : this.GetNotValue(left); // "expr == true" becomes "expr", "expr == false" becomes "not (expr)"
                        if (operand == "<>")
                            return (bool)right ? this.GetNotValue(left) : left; // "expr != true" becomes "not (expr)", "expr != false" becomes "expr"
                    }
                }

                var leftEnum = left as EnumMemberAccess;
                var rightEnum = right as EnumMemberAccess;

                var rightNeedsCoercing = leftEnum != null && rightEnum == null;
                var leftNeedsCoercing = rightEnum != null && leftEnum == null;

                if (rightNeedsCoercing)
                {
                    var rightPartialSql = right as PartialSqlString;
                    if (rightPartialSql == null)
                    {
                        right = this.GetValue(right, leftEnum.EnumType);
                    }
                }
                else if (leftNeedsCoercing)
                {
                    var leftPartialSql = left as PartialSqlString;
                    if (leftPartialSql == null)
                    {
                        left = this.DialectProvider.GetQuotedValue(left, rightEnum.EnumType);
                    }
                }
                else if (!(left is PartialSqlString) && !(right is PartialSqlString))
                {
                    var evaluatedValue = CachedExpressionCompiler.Evaluate(this.PreEvaluateBinary(b, left, right));
                    var result = this.VisitConstant(Expression.Constant(evaluatedValue));
                    return result;
                }
                else if (!(left is PartialSqlString))
                {
                    left = this.DialectProvider.GetQuotedValue(left, left?.GetType());
                }
                else if (!(right is PartialSqlString))
                {
                    right = this.GetValue(right, right?.GetType());
                }
            }

            if (left.ToString().Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                Swap(ref left, ref right); // "null is x" will not work, so swap the operands
            }

            var separator = this.Sep;
            if (right.ToString().Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                if (operand == "=")
                    operand = "is";
                else if (operand == "<>")
                    operand = "is not";

                separator = " ";
            }

            if (operand == "+" && b.Left.Type == typeof(string) && b.Right.Type == typeof(string))
                return this.BuildConcatExpression(new List<object> { left, right });

            this.VisitFilter(operand, originalLeft, originalRight, ref left, ref right);

            switch (operand)
            {
                case "MOD":
                case "COALESCE":
                    return new PartialSqlString($"{operand}({left},{right})");
                default:
                    return new PartialSqlString("(" + left + separator + operand + separator + right + ")");
            }
        }

        private BinaryExpression PreEvaluateBinary(BinaryExpression b, object left, object right)
        {
            var visitedBinaryExp = b;

            if (this.IsParameterAccess(b.Left) || this.IsParameterAccess(b.Right))
            {
                var eLeft = !this.IsParameterAccess(b.Left) ? b.Left : Expression.Constant(left, b.Left.Type);
                var eRight = !this.IsParameterAccess(b.Right) ? b.Right : Expression.Constant(right, b.Right.Type);
                if (b.NodeType == ExpressionType.Coalesce)
                    visitedBinaryExp = Expression.Coalesce(eLeft, eRight, b.Conversion);
                else
                    visitedBinaryExp = Expression.MakeBinary(b.NodeType, eLeft, eRight, b.IsLiftedToNull, b.Method);
            }

            return visitedBinaryExp;
        }

        /// <summary>
        /// Determines whether the expression is the parameter inside MemberExpression which should be compared with TrueExpression.
        /// </summary>
        /// <returns>Returns true if the specified expression is the parameter inside MemberExpression which should be compared with TrueExpression;
        /// otherwise, false.</returns>
        protected virtual bool IsBooleanComparison(Expression e)
        {
            if (!(e is MemberExpression)) return false;

            var m = (MemberExpression)e;

            if (m.Member.DeclaringType.IsNullableType() &&
                m.Member.Name == "HasValue") // nameof(Nullable<bool>.HasValue)
                return false;

            return this.IsParameterAccess(m);
        }

        /// <summary>
        /// Determines whether the expression is the parameter.
        /// </summary>
        /// <returns>Returns true if the specified expression is parameter;
        /// otherwise, false.</returns>
        protected virtual bool IsParameterAccess(Expression e)
        {
            return this.CheckExpressionForTypes(e, new[] { ExpressionType.Parameter });
        }

        /// <summary>
        /// Determines whether the expression is a Parameter or Convert Expression.
        /// </summary>
        /// <returns>Returns true if the specified expression is parameter or convert;
        /// otherwise, false.</returns>
        protected virtual bool IsParameterOrConvertAccess(Expression e)
        {
            return this.CheckExpressionForTypes(e, new[] { ExpressionType.Parameter, ExpressionType.Convert });
        }

        /// <summary>
        /// Check whether the expression is a constant expression to determine
        /// whether we should use the expression value instead of Column Name
        /// </summary>
        protected virtual bool IsConstantExpression(Expression e)
        {
            return this.CheckExpressionForTypes(e, new[] { ExpressionType.Constant });
        }

        protected bool CheckExpressionForTypes(Expression e, ExpressionType[] types)
        {
            while (e != null)
            {
                if (types.Contains(e.NodeType))
                {
                    var subUnaryExpr = e as UnaryExpression;
                    var isSubExprAccess = subUnaryExpr?.Operand is IndexExpression;
                    if (!isSubExprAccess)
                        return true;
                }

                if (e is BinaryExpression binaryExpr)
                {
                    if (this.CheckExpressionForTypes(binaryExpr.Left, types))
                        return true;

                    if (this.CheckExpressionForTypes(binaryExpr.Right, types))
                        return true;
                }

                if (e is MethodCallExpression methodCallExpr)
                {
                    for (var i = 0; i < methodCallExpr.Arguments.Count; i++)
                    {
                        if (this.CheckExpressionForTypes(methodCallExpr.Arguments[i], types))
                            return true;
                    }

                    if (this.CheckExpressionForTypes(methodCallExpr.Object, types))
                        return true;
                }

                if (e is UnaryExpression unaryExpr)
                {
                    if (this.CheckExpressionForTypes(unaryExpr.Operand, types))
                        return true;
                }

                if (e is ConditionalExpression condExpr)
                {
                    if (this.CheckExpressionForTypes(condExpr.Test, types))
                        return true;

                    if (this.CheckExpressionForTypes(condExpr.IfTrue, types))
                        return true;

                    if (this.CheckExpressionForTypes(condExpr.IfFalse, types))
                        return true;
                }

                var memberExpr = e as MemberExpression;
                e = memberExpr?.Expression;
            }

            return false;
        }

        private static void Swap(ref object left, ref object right)
        {
            var temp = right;
            right = left;
            left = temp;
        }

        protected virtual void VisitFilter(string operand, object originalLeft, object originalRight, ref object left, ref object right)
        {
            if (this.skipParameterizationForThisExpression || this.visitedExpressionIsTableColumn)
                return;

            if (originalLeft is EnumMemberAccess && originalRight is EnumMemberAccess)
                return;

            if (operand == "AND" || operand == "OR" || operand == "is" || operand == "is not")
                return;

            if (!(right is PartialSqlString))
            {
                this.ConvertToPlaceholderAndParameter(ref right);
            }
        }

        protected virtual void ConvertToPlaceholderAndParameter(ref object right)
        {
            var parameter = this.AddParam(right);

            right = parameter.ParameterName;
        }

        protected virtual object VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null)
            {
                if (m.Member.DeclaringType.IsNullableType())
                {
                    if (m.Member.Name == nameof(Nullable<bool>.Value))
                        return this.Visit(m.Expression);
                    if (m.Member.Name == nameof(Nullable<bool>.HasValue))
                    {
                        var doesNotEqualNull = Expression.MakeBinary(ExpressionType.NotEqual, m.Expression, Expression.Constant(null));
                        return this.Visit(doesNotEqualNull); // Nullable<T>.HasValue is equivalent to "!= null"
                    }

                    throw new ArgumentException($"Expression '{m}' accesses unsupported property '{m.Member}' of Nullable<T>");
                }

                if (m.Member.DeclaringType == typeof(string) &&
                    m.Member.Name == nameof(string.Length))
                {
                    return this.VisitLengthStringProperty(m);
                }

                if (this.IsParameterOrConvertAccess(m))
                {
                    // We don't want to use the Column Name for constant expressions unless we're in a Sql. method call
                    if (this.inSqlMethodCall || !this.IsConstantExpression(m))
                        return this.GetMemberExpression(m);
                }
            }

            return CachedExpressionCompiler.Evaluate(m);
        }

        protected virtual object GetMemberExpression(MemberExpression m)
        {
            var propertyInfo = m.Member as PropertyInfo;

            var modelType = m.Expression.Type;
            if (m.Expression.NodeType == ExpressionType.Convert)
            {
                if (m.Expression is UnaryExpression unaryExpr)
                {
                    modelType = unaryExpr.Operand.Type;
                }
            }

            this.OnVisitMemberType(modelType);

            var tableDef = modelType.GetModelDefinition();

            var tableAlias = this.GetTableAlias(m);
            var columnName = tableAlias == null
                ? this.GetQuotedColumnName(tableDef, m.Member.Name)
                : this.GetQuotedColumnName(tableDef, tableAlias, m.Member.Name);

            if (propertyInfo != null && propertyInfo.PropertyType.IsEnum)
                return new EnumMemberAccess(columnName, propertyInfo.PropertyType);

            return new PartialSqlString(columnName);
        }

        protected virtual string GetTableAlias(MemberExpression m)
        {
            if (this.originalLambda == null)
                return null;

            if (m.Expression is ParameterExpression pe)
            {
                var tableParamName = this.originalLambda.Parameters.Count > 0 && this.originalLambda.Parameters[0].Type == this.ModelDef.ModelType
                    ? this.originalLambda.Parameters[0].Name
                    : null;

                if (pe.Type == this.ModelDef.ModelType && pe.Name == tableParamName)
                    return this.TableAlias;

                var joinType = this.joinAlias?.ModelDef?.ModelType;
                var joinParamName = this.originalLambda.Parameters.Count > 1 && this.originalLambda.Parameters[1].Type == joinType
                    ? this.originalLambda.Parameters[1].Name
                    : null;

                if (pe.Type == joinType && pe.Name == joinParamName)
                    return this.joinAlias.Alias;
            }

            return null;
        }

        protected virtual void OnVisitMemberType(Type modelType)
        {
            var tableDef = modelType.GetModelDefinition();
            if (tableDef != null) this.visitedExpressionIsTableColumn = true;
        }

        protected virtual object VisitMemberInit(MemberInitExpression exp)
        {
            return CachedExpressionCompiler.Evaluate(exp);
        }

        protected virtual object VisitNew(NewExpression nex)
        {
            var isAnonType = nex.Type.Name.StartsWith("<>");
            if (isAnonType)
            {
                var exprs = this.VisitExpressionList(nex.Arguments);

                for (var i = 0; i < exprs.Count; ++i)
                {
                    exprs[i] = this.SetAnonTypePropertyNamesForSelectExpression(exprs[i], nex.Arguments[i], nex.Members[i]);
                }

                return new SelectList(exprs);
            }

            return CachedExpressionCompiler.Evaluate(nex);
        }

        bool IsLambdaArg(Expression expr)
        {
            if (expr is ParameterExpression pe)
                return this.IsLambdaArg(pe.Name);
            if (expr is UnaryExpression ue && ue.Operand is ParameterExpression uepe)
                return this.IsLambdaArg(uepe.Name);
            return false;
        }

        bool IsLambdaArg(string name)
        {
            var args = this.originalLambda?.Parameters;
            if (args != null)
            {
                foreach (var x in args)
                {
                    if (x.Name == name)
                        return true;
                }
            }

            return false;
        }

        private object SetAnonTypePropertyNamesForSelectExpression(object expr, Expression arg, MemberInfo member)
        {
            // When selecting a column use the anon type property name, rather than the table property name, as the returned column name
            if (arg is MemberExpression propExpr && this.IsLambdaArg(propExpr.Expression))
            {
                if (this.UseSelectPropertiesAsAliases || // Use anon property alias when explicitly requested
                    propExpr.Member.Name != member.Name || // or when names don't match
                    propExpr.Expression.Type != this.ModelDef.ModelType)
                {
                    return new SelectItemExpression(this.DialectProvider, expr.ToString(), member.Name);
                }
/*|| // or when selecting a field from a different table
                    member.Name != ModelDef.FieldDefinitions.First(x => x.Name == member.Name).FieldName)*/  // or when name and alias don't match

                return expr;
            }

            // When selecting an entire table use the anon type property name as a prefix for the returned column name
            // to allow the caller to distinguish properties with the same names from different tables
            var selectList = arg is ParameterExpression paramExpr && paramExpr.Name != member.Name
                ? expr as SelectList
                : null;
            if (selectList != null)
            {
                foreach (var item in selectList.Items)
                {
                    if (item is SelectItem selectItem)
                    {
                        if (!string.IsNullOrEmpty(selectItem.Alias))
                        {
                            selectItem.Alias = member.Name + selectItem.Alias;
                        }
                        else
                        {
                            if (item is SelectItemColumn columnItem)
                            {
                                columnItem.Alias = member.Name + columnItem.ColumnName;
                            }
                        }
                    }
                }
            }

            var methodCallExpr = arg as MethodCallExpression;
            var mi = methodCallExpr?.Method;
            var declareType = mi?.DeclaringType;

            if (declareType != null && declareType.Name == nameof(Sql))
            {
                if (mi.Name == nameof(Sql.TableAlias) || mi.Name == nameof(Sql.JoinAlias))
                {
                    if (expr is PartialSqlString ps && ps.Text.IndexOf(',') >= 0)
                        return ps;                                               // new { buyer = Sql.TableAlias(b, "buyer")
                    return new PartialSqlString(expr + " AS " + member.Name);    // new { BuyerName = Sql.TableAlias(b.Name, "buyer") }
                }

                if (mi.Name != nameof(Sql.Desc) && mi.Name != nameof(Sql.Asc) && mi.Name != nameof(Sql.As) && mi.Name != nameof(Sql.AllFields))
                    return new PartialSqlString(expr + " AS " + member.Name);    // new { Alias = Sql.Count("*") }
            }

            if (expr is string s && s == Sql.EOT) // new { t1 = Sql.EOT, t2 = "0 EOT" }
                return new PartialSqlString(s);

            if (arg is ConditionalExpression ce ||                           // new { Alias = x.Value > 1 ? 1 : x.Value }
                arg is BinaryExpression be ||                           // new { Alias = x.First + " " + x.Last }
                arg is MemberExpression me ||                           // new { Alias = DateTime.UtcNow }
                arg is ConstantExpression ct)
            {
                // new { Alias = 1 }
                IOrmLiteConverter converter;
                var strExpr = !(expr is PartialSqlString) && (converter = this.DialectProvider.GetConverterBestMatch(expr.GetType())) != null
                    ? converter.ToQuotedString(expr.GetType(), expr)
                    : expr.ToString();

                return new PartialSqlString(OrmLiteUtils.UnquotedColumnName(strExpr) != member.Name
                    ? strExpr + " AS " + member.Name
                    : strExpr);
            }

            var usePropertyAlias = this.UseSelectPropertiesAsAliases ||
                                   expr is PartialSqlString p && Equals(p, PartialSqlString.Null); // new { Alias = (DateTime?)null }
            return usePropertyAlias
                ? new SelectItemExpression(this.DialectProvider, expr.ToString(), member.Name)
                : expr;
        }

        private static void StripAliases(SelectList selectList)
        {
            if (selectList == null)
                return;

            foreach (var item in selectList.Items)
            {
                if (item is SelectItem selectItem)
                {
                    selectItem.Alias = null;
                }
                else if (item is PartialSqlString p)
                {
                    if (p.Text.IndexOf(' ') >= 0)
                    {
                        var right = p.Text.RightPart(' ');
                        if (right.StartsWithIgnoreCase("AS "))
                        {
                            p.Text = p.Text.LeftPart(' ');
                        }
                    }
                }
            }
        }

        private class SelectList
        {
            public readonly IEnumerable<object> Items;

            public SelectList(IEnumerable<object> items)
            {
                this.Items = items;
            }

            public override string ToString()
            {
                return this.Items.ToSelectString();
            }
        }

        protected virtual object VisitParameter(ParameterExpression p)
        {
            var paramModelDef = p.Type.GetModelDefinition();
            if (paramModelDef != null)
            {
                var tablePrefix = paramModelDef == this.ModelDef && this.TableAlias != null
                    ? this.TableAlias
                    : paramModelDef.ModelName;
                return new SelectList(this.DialectProvider.GetColumnNames(paramModelDef, tablePrefix));
            }

            return p.Name;
        }

        protected virtual object VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
                return PartialSqlString.Null;

            return c.Value;
        }

        protected virtual object VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    var o = this.Visit(u.Operand);
                    return this.GetNotValue(o);
                case ExpressionType.Convert:
                    if (u.Method != null)
                    {
                        var e = u.Operand;
                        if (this.IsParameterAccess(e))
                            return this.Visit(e);

                        return CachedExpressionCompiler.Evaluate(u);
                    }

                    break;
            }
            return this.Visit(u.Operand);
        }

        protected virtual object VisitIndexExpression(IndexExpression e)
        {
            var arg = e.Arguments[0];
            var oIndex = arg is ConstantExpression constant
                ? constant.Value
                : CachedExpressionCompiler.Evaluate(arg);

            var index = (int)Convert.ChangeType(oIndex, typeof(int));
            var oCollection = CachedExpressionCompiler.Evaluate(e.Object);

            if (oCollection is List<object> list)
                return list[index];

            throw new NotImplementedException("Unknown Expression: " + e);
        }

        protected virtual object VisitConditional(ConditionalExpression e)
        {
            var test = this.IsBooleanComparison(e.Test)
                ? new PartialSqlString($"{this.VisitMemberAccess((MemberExpression)e.Test)}={this.GetQuotedTrueValue()}")
                : this.Visit(e.Test);

            if (test is bool)
            {
                if ((bool)test)
                {
                    var ifTrue = this.Visit(e.IfTrue);
                    if (!IsSqlClass(ifTrue))
                    {
                        if (this.Sep == " ")
                            ifTrue = new PartialSqlString(this.ConvertToParam(ifTrue));
                    }
                    else if (e.IfTrue.Type == typeof(bool))
                    {
                        var isBooleanComparison = this.IsBooleanComparison(e.IfTrue);
                        if (!isBooleanComparison)
                        {
                            if (this.Sep == " ")
                                ifTrue = ifTrue.ToString();
                            else
                                ifTrue = new PartialSqlString($"(CASE WHEN {ifTrue} THEN {1} ELSE {0} END)");
                        }
                    }

                    return ifTrue;
                }

                var ifFalse = this.Visit(e.IfFalse);
                if (!IsSqlClass(ifFalse))
                {
                    if (this.Sep == " ")
                        ifFalse = new PartialSqlString(this.ConvertToParam(ifFalse));
                }
                else if (e.IfFalse.Type == typeof(bool))
                {
                    var isBooleanComparison = this.IsBooleanComparison(e.IfFalse);
                    if (!isBooleanComparison)
                    {
                        if (this.Sep == " ")
                            ifFalse = ifFalse.ToString();
                        else
                            ifFalse = new PartialSqlString($"(CASE WHEN {ifFalse} THEN {1} ELSE {0} END)");
                    }
                }

                return ifFalse;
            }
            else
            {
                var ifTrue = this.Visit(e.IfTrue);
                if (!IsSqlClass(ifTrue))
                    ifTrue = this.ConvertToParam(ifTrue);
                else if (e.IfTrue.Type == typeof(bool))
                {
                    var isBooleanComparison = this.IsBooleanComparison(e.IfTrue);
                    if (!isBooleanComparison)
                    {
                        ifTrue = $"(CASE WHEN {ifTrue} THEN {this.GetQuotedTrueValue()} ELSE {this.GetQuotedFalseValue()} END)";
                    }
                }

                var ifFalse = this.Visit(e.IfFalse);
                if (!IsSqlClass(ifFalse))
                    ifFalse = this.ConvertToParam(ifFalse);
                else if (e.IfFalse.Type == typeof(bool))
                {
                    var isBooleanComparison = this.IsBooleanComparison(e.IfFalse);
                    if (!isBooleanComparison)
                    {
                        ifFalse = $"(CASE WHEN {ifFalse} THEN {this.GetQuotedTrueValue()} ELSE {this.GetQuotedFalseValue()} END)";
                    }
                }

                return new PartialSqlString($"(CASE WHEN {test} THEN {ifTrue} ELSE {ifFalse} END)");
            }
        }

        private object GetNotValue(object o)
        {
            if (!(o is PartialSqlString))
                return !(bool)o;

            if (this.IsFieldName(o))
                return new PartialSqlString(o + "=" + this.GetQuotedFalseValue());

            return new PartialSqlString("NOT (" + o + ")");
        }

        protected virtual bool IsColumnAccess(MethodCallExpression m)
        {
            if (m.Object == null)
            {
                foreach (var arg in m.Arguments)
                {
                    if (!(arg is LambdaExpression) && this.IsParameterAccess(arg))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (m.Object is MethodCallExpression methCallExp)
                return this.IsColumnAccess(methCallExp);

            if (m.Object is ConditionalExpression condExp)
                return this.IsParameterAccess(condExp);

            if (m.Object is UnaryExpression unaryExp)
                return this.IsParameterAccess(unaryExp);

            var exp = m.Object as MemberExpression;
            return this.IsParameterAccess(exp)
                   && this.IsJoinedTable(exp.Expression.Type);
        }


        protected virtual object VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Sql))
            {
                var hold = this.inSqlMethodCall;
                this.inSqlMethodCall = true;
                var ret = this.VisitSqlMethodCall(m);
                this.inSqlMethodCall = hold;
                return ret;
            }

            if (this.IsStaticArrayMethod(m))
                return this.VisitStaticArrayMethodCall(m);

            if (IsEnumerableMethod(m))
                return this.VisitEnumerableMethodCall(m);

            if (this.IsStaticStringMethod(m))
                return this.VisitStaticStringMethodCall(m);

            if (this.IsColumnAccess(m))
                return this.VisitColumnAccessMethod(m);

            return this.EvaluateExpression(m);
        }

        private object EvaluateExpression(Expression m)
        {
            try
            {
                return CachedExpressionCompiler.Evaluate(m);
            }
            catch (InvalidOperationException)
            {
                if (this.originalLambda == null)
                    throw;

                // Can't use expression.Compile() if lambda expression contains captured parameters.
                // Fallback invokes expression with default parameters from original lambda expression
                var lambda = Expression.Lambda(m, this.originalLambda.Parameters).Compile();

                var exprParams = new object[this.originalLambda.Parameters.Count];
                for (var i = 0; i < this.originalLambda.Parameters.Count; i++)
                {
                    var p = this.originalLambda.Parameters[i];
                    exprParams[i] = p.Type.CreateInstance();
                }

                var ret = lambda.DynamicInvoke(exprParams);
                return ret;
            }
        }

        protected virtual List<object> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            var list = new List<object>();
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var e = original[i];
                if (e.NodeType == ExpressionType.NewArrayInit ||
                    e.NodeType == ExpressionType.NewArrayBounds)
                {
                    list.AddRange(this.VisitNewArrayFromExpressionList(e as NewArrayExpression));
                }
                else
                {
                    list.Add(this.Visit(e));
                }
            }

            return list;
        }

        protected virtual List<object> VisitInSqlExpressionList(ReadOnlyCollection<Expression> original)
        {
            var list = new List<object>();
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var e = original[i];
                if (e.NodeType == ExpressionType.NewArrayInit ||
                    e.NodeType == ExpressionType.NewArrayBounds)
                {
                    list.AddRange(this.VisitNewArrayFromExpressionList(e as NewArrayExpression));
                }
                else if (e.NodeType == ExpressionType.MemberAccess)
                {
                    list.Add(this.VisitMemberAccess(e as MemberExpression));
                }
                else
                {
                    list.Add(this.Visit(e));
                }
            }

            return list;
        }

        protected virtual object VisitNewArray(NewArrayExpression na)
        {
            var exprs = this.VisitExpressionList(na.Expressions);
            var sb = StringBuilderCache.Allocate();
            foreach (var e in exprs)
            {
                sb.Append(sb.Length > 0 ? "," + e : e);
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }

        protected virtual List<object> VisitNewArrayFromExpressionList(NewArrayExpression na)
        {
            var exprs = this.VisitExpressionList(na.Expressions);
            return exprs;
        }

        protected virtual string BindOperant(ExpressionType e)
        {
            switch (e)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "MOD";
                case ExpressionType.Coalesce:
                    return "COALESCE";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.RightShift:
                    return ">>";
                default:
                    return e.ToString();
            }
        }

        protected virtual string GetQuotedColumnName(ModelDefinition tableDef, string memberName) => // Always call if no tableAlias to exec overrides
            this.GetQuotedColumnName(tableDef, null, memberName);

        protected virtual string GetQuotedColumnName(ModelDefinition tableDef, string tableAlias, string memberName)
        {
            if (this.useFieldName)
            {
                var fd = tableDef.FieldDefinitions.FirstOrDefault(x => x.Name == memberName);
                var fieldName = fd != null
                    ? fd.FieldName
                    : memberName;

                if (tableDef.ModelType.IsInterface && this.ModelDef.ModelType.HasInterface(tableDef.ModelType))
                {
                    tableDef = this.ModelDef;
                }

                if (fd?.CustomSelect != null)
                    return fd.CustomSelect;

                var includePrefix = this.PrefixFieldWithTableName && !tableDef.ModelType.IsInterface;
                return includePrefix
                    ? tableAlias == null
                        ? this.DialectProvider.GetQuotedColumnName(tableDef, fieldName)
                        : this.DialectProvider.GetQuotedColumnName(tableDef, tableAlias, fieldName)
                    : this.DialectProvider.GetQuotedColumnName(fieldName);
            }

            return memberName;
        }

        protected string RemoveQuoteFromAlias(string exp)
        {
            if ((exp.StartsWith("\"") || exp.StartsWith("`") || exp.StartsWith("'"))
                &&
                (exp.EndsWith("\"") || exp.EndsWith("`") || exp.EndsWith("'")))
            {
                exp = exp.Remove(0, 1);
                exp = exp.Remove(exp.Length - 1, 1);
            }

            return exp;
        }

        protected virtual bool IsFieldName(object quotedExp)
        {
            var fieldExpr = quotedExp.ToString().StripTablePrefixes();
            var unquotedExpr = fieldExpr.StripDbQuotes();

            var isTableField = this.modelDef.FieldDefinitionsArray
                .Any(x => this.GetColumnName(x.FieldName) == unquotedExpr);
            if (isTableField)
                return true;

            var isJoinedField = this.tableDefs.Any(t => t.FieldDefinitionsArray
                .Any(x => this.GetColumnName(x.FieldName) == unquotedExpr));

            return isJoinedField;
        }

        protected string GetColumnName(string fieldName)
        {
            return this.DialectProvider.NamingStrategy.GetColumnName(fieldName);
        }

        protected object GetTrueExpression()
        {
            return new PartialSqlString($"({this.GetQuotedTrueValue()}={this.GetQuotedTrueValue()})");
        }

        protected object GetFalseExpression()
        {
            return new PartialSqlString($"({this.GetQuotedTrueValue()}={this.GetQuotedFalseValue()})");
        }

        private string quotedTrue;
        protected object GetQuotedTrueValue()
        {
            return new PartialSqlString(this.quotedTrue ??= this.DialectProvider.GetQuotedValue(true, typeof(bool)));
        }

        private string quotedFalse;
        protected object GetQuotedFalseValue()
        {
            return new PartialSqlString(this.quotedFalse ??= this.DialectProvider.GetQuotedValue(false, typeof(bool)));
        }

        private void BuildSelectExpression(string fields, bool distinct)
        {
            this.OnlyFields = null;
            this.selectDistinct = distinct;

            this.selectExpression = $"SELECT {(this.selectDistinct ? "DISTINCT " : string.Empty)}" +
                                    (string.IsNullOrEmpty(fields)
                                        ? this.DialectProvider.GetColumnNames(this.modelDef, this.PrefixFieldWithTableName ? this.TableAlias ?? this.ModelDef.ModelName : null).ToSelectString()
                                        : fields);
        }

        public IList<string> GetAllFields()
        {
            return this.modelDef.FieldDefinitions.ConvertAll(r => r.Name);
        }

        protected virtual bool IsStaticArrayMethod(MethodCallExpression m)
        {
            return m.Object == null
                   && m.Method.Name == "Contains"
                   && m.Arguments.Count == 2;
        }

        protected virtual object VisitStaticArrayMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    List<object> args = this.VisitExpressionList(m.Arguments);
                    object quotedColName = args.Last();

                    Expression memberExpr = m.Arguments[0];
                    if (memberExpr.NodeType == ExpressionType.MemberAccess)
                        memberExpr = m.Arguments[0] as MemberExpression;

                    return this.ToInPartialString(memberExpr, quotedColName);

                default:
                    throw new NotSupportedException();
            }
        }

        private static bool IsEnumerableMethod(MethodCallExpression m)
        {
            return m.Object != null
                && m.Object.Type.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>))
                && m.Object.Type != typeof(string)
                && m.Method.Name == "Contains"
                && m.Arguments.Count == 1;
        }

        protected virtual object VisitEnumerableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    List<object> args = this.VisitExpressionList(m.Arguments);
                    object quotedColName = args[0];
                    return this.ToInPartialString(m.Object, quotedColName);

                default:
                    throw new NotSupportedException();
            }
        }

        private object ToInPartialString(Expression memberExpr, object quotedColName)
        {
            var result = this.EvaluateExpression(memberExpr);

            var inArgs = Sql.Flatten(result as IEnumerable);

            var sqlIn = inArgs.Count > 0
                ? this.CreateInParamSql(inArgs)
                : "NULL";

            var statement = $"{quotedColName} IN ({sqlIn})";
            return new PartialSqlString(statement);
        }

        protected virtual bool IsStaticStringMethod(MethodCallExpression m)
        {
            return m.Object == null
                   && (m.Method.Name == nameof(string.Concat) || m.Method.Name == nameof(string.Compare));
        }

        protected virtual object VisitStaticStringMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case nameof(string.Concat):
                    return this.BuildConcatExpression(this.VisitExpressionList(m.Arguments));
                case nameof(string.Compare):
                    return this.BuildCompareExpression(this.VisitExpressionList(m.Arguments));

                default:
                    throw new NotSupportedException();
            }
        }

        private object VisitLengthStringProperty(MemberExpression m)
        {
            var sql = this.Visit(m.Expression);
            if (!IsSqlClass(sql))
            {
                if (sql == null)
                    return 0;

                sql = ((string)sql).Length;
                return sql;
            }

            return this.ToLengthPartialString(sql);
        }

        protected virtual PartialSqlString ToLengthPartialString(object arg)
        {
            return new PartialSqlString($"CHAR_LENGTH({arg})");
        }

        private PartialSqlString BuildConcatExpression(List<object> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                if (!(args[i] is PartialSqlString))
                    args[i] = this.ConvertToParam(args[i]);
            }

            return this.ToConcatPartialString(args);
        }

        private PartialSqlString BuildCompareExpression(List<object> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                if (!(args[i] is PartialSqlString))
                    args[i] = this.ConvertToParam(args[i]);
            }

            return this.ToComparePartialString(args);
        }

        protected PartialSqlString ToConcatPartialString(List<object> args)
        {
            return new PartialSqlString(this.DialectProvider.SqlConcat(args));
        }

        protected virtual PartialSqlString ToComparePartialString(List<object> args)
        {
            return new PartialSqlString($"(CASE WHEN {args[0]} = {args[1]} THEN 0 WHEN {args[0]} > {args[1]} THEN 1 ELSE -1 END)");
        }

        protected virtual object VisitSqlMethodCall(MethodCallExpression m)
        {
            List<object> args = this.VisitInSqlExpressionList(m.Arguments);
            object quotedColName = args[0];
            args.RemoveAt(0);

            string statement;

            switch (m.Method.Name)
            {
                case nameof(Sql.In):
                    statement = this.ConvertInExpressionToSql(m, quotedColName);
                    break;
                case nameof(Sql.Asc):
                    statement = $"{quotedColName} ASC";
                    break;
                case nameof(Sql.Desc):
                    statement = $"{quotedColName} DESC";
                    break;
                case nameof(Sql.As):
                    statement = $"{quotedColName} AS {this.DialectProvider.GetQuotedColumnName(this.RemoveQuoteFromAlias(args[0].ToString()))}";
                    break;
                case nameof(Sql.Cast):
                    statement = this.DialectProvider.SqlCast(quotedColName, args[0].ToString());
                    break;
                case nameof(Sql.Sum):
                case nameof(Sql.Count):
                case nameof(Sql.Min):
                case nameof(Sql.Max):
                case nameof(Sql.Avg):
                    statement = $"{m.Method.Name}({quotedColName}{(args.Count == 1 ? $",{args[0]}" : string.Empty)})";
                    break;
                case nameof(Sql.CountDistinct):
                    statement = $"COUNT(DISTINCT {quotedColName})";
                    break;
                case nameof(Sql.AllFields):
                    var argDef = m.Arguments[0].Type.GetModelMetadata();
                    statement = this.DialectProvider.GetQuotedTableName(argDef) + ".*";
                    break;
                case nameof(Sql.JoinAlias):
                case nameof(Sql.TableAlias):
                    if (quotedColName is SelectList && m.Arguments.Count == 2 && m.Arguments[0] is ParameterExpression p)
                    {
                        var paramModelDef = p.Type.GetModelDefinition();
                        var alias = this.Visit(m.Arguments[1]).ToString();
                        statement = new SelectList(this.DialectProvider.GetColumnNames(paramModelDef, alias)).ToString();
                    }
                    else
                    {
                        //statement = args[0] + "." + quotedColName.ToString().LastRightPart('.');
                        statement = this.DialectProvider.GetQuotedName(args[0].ToString()) + "." + quotedColName.ToString().LastRightPart('.');
                    }
                    break;
                case nameof(Sql.Custom):
                    statement = quotedColName.ToString();
                    break;
                default:
                    throw new NotSupportedException();
            }

            return new PartialSqlString(statement);
        }

        protected string ConvertInExpressionToSql(MethodCallExpression m, object quotedColName)
        {
            var argValue = this.EvaluateExpression(m.Arguments[1]);

            if (argValue == null)
                return FalseLiteral; // "column IN (NULL)" is always false

            if (argValue is IEnumerable enumerableArg)
            {
                var inArgs = Sql.Flatten(enumerableArg);
                if (inArgs.Count == 0)
                    return FalseLiteral; // "column IN ([])" is always false

                string sqlIn = this.CreateInParamSql(inArgs);
                return $"{quotedColName} IN ({sqlIn})";
            }

            if (argValue is ISqlExpression exprArg)
            {
                var subSelect = exprArg.ToSelectStatement(QueryType.Select);
                var renameParams = new List<Tuple<string, string>>();
                foreach (var p in exprArg.Params)
                {
                    var oldName = p.ParameterName;
                    var newName = this.DialectProvider.GetParam(this.Params.Count.ToString());
                    if (oldName != newName)
                    {
                        var pClone = this.DialectProvider.CreateParam().PopulateWith(p);
                        renameParams.Add(Tuple.Create(oldName, newName));
                        pClone.ParameterName = newName;
                        this.Params.Add(pClone);
                    }
                    else
                    {
                        this.Params.Add(p);
                    }
                }

                // regex replace doesn't work when param is at end of string "AND a = :0"
                var lastChar = subSelect[subSelect.Length - 1];
                if (!(char.IsWhiteSpace(lastChar) || lastChar == ')'))
                    subSelect += " ";

                for (var i = renameParams.Count - 1; i >= 0; i--)
                {
                    // Replace complete db params [@1] and not partial tokens [@1]0
                    var paramsRegex = new Regex(renameParams[i].Item1 + "([^\\d])");
                    subSelect = paramsRegex.Replace(subSelect, renameParams[i].Item2 + "$1");
                }

                return this.CreateInSubQuerySql(quotedColName, subSelect);
            }

            throw new NotSupportedException($"In({argValue.GetType()})");
        }

        protected virtual string CreateInSubQuerySql(object quotedColName, string subSelect)
        {
            return $"{quotedColName} IN ({subSelect})";
        }

        protected virtual object VisitColumnAccessMethod(MethodCallExpression m)
        {
            List<object> args = this.VisitExpressionList(m.Arguments);
            var quotedColName = this.Visit(m.Object);
            if (!IsSqlClass(quotedColName))
                quotedColName = this.ConvertToParam(quotedColName);

            var statement = string.Empty;

            var arg = args.Count > 0 ? args[0] : null;
            var wildcardArg = arg != null ? this.DialectProvider.EscapeWildcards(arg.ToString()) : string.Empty;
            var escapeSuffix = wildcardArg.IndexOf('^') >= 0 ? " escape '^'" : string.Empty;
            switch (m.Method.Name)
            {
                case "Trim":
                    statement = $"ltrim(rtrim({quotedColName}))";
                    break;
                case "LTrim":
                    statement = $"ltrim({quotedColName})";
                    break;
                case "RTrim":
                    statement = $"rtrim({quotedColName})";
                    break;
                case "ToUpper":
                    statement = $"upper({quotedColName})";
                    break;
                case "ToLower":
                    statement = $"lower({quotedColName})";
                    break;
                case "Equals":
                    var argType = arg?.GetType();
                    var converter = argType != null && argType != typeof(string)
                        ? this.DialectProvider.GetConverterBestMatch(argType)
                        : null;
                    statement = converter != null
                        ? $"{quotedColName}={this.ConvertToParam(converter.ToDbValue(argType, arg))}"
                        : $"{quotedColName}={this.ConvertToParam(arg)}";
                    break;
                case "StartsWith":
                    statement = !OrmLiteConfig.StripUpperInLike
                        ? $"upper({quotedColName}) like {this.ConvertToParam(wildcardArg.ToUpper() + "%")}{escapeSuffix}"
                        : $"{quotedColName} like {this.ConvertToParam(wildcardArg + "%")}{escapeSuffix}";
                    break;
                case "EndsWith":
                    statement = !OrmLiteConfig.StripUpperInLike
                        ? $"upper({quotedColName}) like {this.ConvertToParam("%" + wildcardArg.ToUpper())}{escapeSuffix}"
                        : $"{quotedColName} like {this.ConvertToParam("%" + wildcardArg)}{escapeSuffix}";
                    break;
                case "Contains":
                    statement = !OrmLiteConfig.StripUpperInLike
                        ? $"upper({quotedColName}) like {this.ConvertToParam("%" + wildcardArg.ToUpper() + "%")}{escapeSuffix}"
                        : $"{quotedColName} like {this.ConvertToParam("%" + wildcardArg + "%")}{escapeSuffix}";
                    break;
                case "Substring":
                    var startIndex = int.Parse(args[0].ToString()) + 1;
                    statement = args.Count == 2
                        ? this.GetSubstringSql(quotedColName, startIndex, int.Parse(args[1].ToString()))
                        : this.GetSubstringSql(quotedColName, startIndex);
                    break;
                case "ToString":
                    statement = m.Object?.Type == typeof(string)
                        ? $"({quotedColName})"
                        : this.ToCast(quotedColName.ToString());
                    break;
                default:
                    throw new NotSupportedException();
            }
            return new PartialSqlString(statement);
        }

        protected virtual string ToCast(string quotedColName)
        {
            return $"cast({quotedColName} as varchar(1000))";
        }

        public virtual string GetSubstringSql(object quotedColumn, int startIndex, int? length = null)
        {
            return length != null
                ? $"substring({quotedColumn} from {startIndex} for {length.Value})"
                : $"substring({quotedColumn} from {startIndex})";
        }

        public IDbDataParameter CreateParam(string name,
            object value = null,
            ParameterDirection direction = ParameterDirection.Input,
            DbType? dbType = null,
            DataRowVersion sourceVersion = DataRowVersion.Default)
        {
            var p = this.DialectProvider.CreateParam();
            p.ParameterName = this.DialectProvider.GetParam(name);
            p.Direction = direction;

            if (!this.DialectProvider.IsMySqlConnector())
            {
                // throws NotSupportedException
                p.SourceVersion = sourceVersion;
            }

            this.DialectProvider.ConfigureParam(p, value, dbType);

            return p;
        }

        public IUntypedSqlExpression GetUntyped()
        {
            return new UntypedSqlExpressionProxy<T>(this);
        }
    }

    public interface ISqlExpression
    {
        List<IDbDataParameter> Params { get; }

        string ToSelectStatement();
        string ToSelectStatement(QueryType forType);
        string SelectInto<TModel>();
        string SelectInto<TModel>(QueryType forType);
    }

    public enum QueryType
    {
        Select,
        Single,
        Scalar,
    }

    public interface IHasDialectProvider
    {
        IOrmLiteDialectProvider DialectProvider { get; }
    }

    public class PartialSqlString
    {
        public static PartialSqlString Null = new("null");

        public PartialSqlString(string text)
        {
            this.Text = text;
        }

        public string Text { get; internal set; }
        public override string ToString() => this.Text;

        protected bool Equals(PartialSqlString other) => this.Text == other.Text;
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((PartialSqlString)obj);
        }

        public override int GetHashCode() => this.Text != null ? this.Text.GetHashCode() : 0;
    }

    public class EnumMemberAccess : PartialSqlString
    {
        public EnumMemberAccess(string text, Type enumType)
            : base(text)
        {
            if (!enumType.IsEnum) throw new ArgumentException("Type not valid", nameof(enumType));

            this.EnumType = enumType;
        }

        public Type EnumType { get; private set; }
    }

    public abstract class SelectItem
    {
        protected SelectItem(IOrmLiteDialectProvider dialectProvider, string alias)
        {
            this.DialectProvider = dialectProvider ?? throw new ArgumentNullException(nameof(dialectProvider));

            this.Alias = alias;
        }

        /// <summary>
        /// Unquoted alias for the column or expression being selected.
        /// </summary>
        public string Alias { get; set; }

        protected IOrmLiteDialectProvider DialectProvider { get; set; }

        public abstract override string ToString();
    }

    public class SelectItemExpression : SelectItem
    {
        public SelectItemExpression(IOrmLiteDialectProvider dialectProvider, string selectExpression, string alias)
            : base(dialectProvider, alias)
        {
            if (string.IsNullOrEmpty(selectExpression))
                throw new ArgumentNullException(nameof(selectExpression));
            if (string.IsNullOrEmpty(alias))
                throw new ArgumentNullException(nameof(alias));

            this.SelectExpression = selectExpression;
            this.Alias = alias;
        }

        /// <summary>
        /// The SQL expression being selected, including any necessary quoting.
        /// </summary>
        public string SelectExpression { get; set; }

        public override string ToString()
        {
            var text = this.SelectExpression;
            if (!string.IsNullOrEmpty(this.Alias)) // Note that even though Alias must be non-empty in the constructor it may be set to null/empty later
                return text + " AS " + this.DialectProvider.GetQuotedName(this.Alias);

            return text;
        }
    }

    public class SelectItemColumn : SelectItem
    {
        public SelectItemColumn(IOrmLiteDialectProvider dialectProvider, string columnName, string columnAlias = null, string quotedTableAlias = null)
            : base(dialectProvider, columnAlias)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException(nameof(columnName));

            this.ColumnName = columnName;
            this.QuotedTableAlias = quotedTableAlias;
        }

        /// <summary>
        /// Unquoted column name being selected.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Table name or alias used to prefix the column name, if any. Already quoted.
        /// </summary>
        public string QuotedTableAlias { get; set; }

        public override string ToString()
        {
            var text = this.DialectProvider.GetQuotedColumnName(this.ColumnName);

            if (!string.IsNullOrEmpty(this.QuotedTableAlias))
                text = this.QuotedTableAlias + "." + text;
            if (!string.IsNullOrEmpty(this.Alias))
                text += " AS " + this.DialectProvider.GetQuotedName(this.Alias);

            return text;
        }
    }

    public class OrmLiteDataParameter : IDbDataParameter
    {
        public DbType DbType { get; set; }
        public ParameterDirection Direction { get; set; }
        public bool IsNullable { get; set; }
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public int Size { get; set; }
    }

    public static class DbDataParameterExtensions
    {
        public static IDbDataParameter CreateParam(this IDbConnection db,
            string name,
            object value = null,
            Type fieldType = null,
            DbType? dbType = null,
            byte? precision = null,
            byte? scale = null,
            int? size = null)
        {
            return db.GetDialectProvider().CreateParam(name, value, fieldType, dbType, precision, scale, size);
        }

        public static IDbDataParameter CreateParam(this IOrmLiteDialectProvider dialectProvider,
            string name,
            object value = null,
            Type fieldType = null,
            DbType? dbType = null,
            byte? precision = null,
            byte? scale = null,
            int? size = null)
        {
            var p = dialectProvider.CreateParam();

            p.ParameterName = dialectProvider.GetParam(name);

            dialectProvider.ConfigureParam(p, value, dbType);

            if (precision != null)
                p.Precision = precision.Value;
            if (scale != null)
                p.Scale = scale.Value;
            if (size != null)
                p.Size = size.Value;

            return p;
        }

        internal static void ConfigureParam(this IOrmLiteDialectProvider dialectProvider, IDbDataParameter p, object value, DbType? dbType)
        {
            if (value != null)
            {
                dialectProvider.InitDbParam(p, value.GetType());
                p.Value = dialectProvider.GetParamValue(value, value.GetType());
            }
            else
            {
                p.Value = DBNull.Value;
            }

            // Can't check DbType in PostgreSQL before p.Value is assinged
            if (p.Value is string strValue && strValue.Length > p.Size)
            {
                var stringConverter = dialectProvider.GetStringConverter();
                p.Size = strValue.Length > stringConverter.StringLength
                    ? strValue.Length
                    : stringConverter.StringLength;
            }

            if (dbType != null)
                p.DbType = dbType.Value;
        }

        public static IDbDataParameter AddQueryParam(this IOrmLiteDialectProvider dialectProvider,
            IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef) => dialectProvider.AddParam(dbCmd, value, fieldDef, paramFilter: dialectProvider.InitQueryParam);

        public static IDbDataParameter AddUpdateParam(this IOrmLiteDialectProvider dialectProvider,
            IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef) => dialectProvider.AddParam(dbCmd, value, fieldDef, paramFilter: dialectProvider.InitUpdateParam);

        public static IDbDataParameter AddParam(this IOrmLiteDialectProvider dialectProvider,
            IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef, Action<IDbDataParameter> paramFilter)
        {
            var paramName = dbCmd.Parameters.Count.ToString();
            var parameter = dialectProvider.CreateParam(paramName, value, fieldDef?.ColumnType);

            paramFilter?.Invoke(parameter);

            if (fieldDef != null)
                dialectProvider.SetParameter(fieldDef, parameter);

            dbCmd.Parameters.Add(parameter);

            return parameter;
        }

        public static string GetInsertParam(this IOrmLiteDialectProvider dialectProvider,
            IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef)
        {
            var p = dialectProvider.AddUpdateParam(dbCmd, value, fieldDef);
            return fieldDef.CustomInsert != null
                ? string.Format(fieldDef.CustomInsert, p.ParameterName)
                : p.ParameterName;
        }

        public static string GetUpdateParam(this IOrmLiteDialectProvider dialectProvider,
            IDbCommand dbCmd,
            object value,
            FieldDefinition fieldDef)
        {
            var p = dialectProvider.AddUpdateParam(dbCmd, value, fieldDef);

            return fieldDef.CustomUpdate != null
                ? string.Format(fieldDef.CustomUpdate, p.ParameterName)
                : p.ParameterName;
        }
    }
}

