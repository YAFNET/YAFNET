namespace ServiceStack.OrmLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ServiceStack.Text;

    public delegate string JoinFormatDelegate(IOrmLiteDialectProvider dialect, ModelDefinition tableDef, string joinExpr);

    public class TableOptions
    {
        public string Expression { get; set; }
        public string Alias { get; set; }

        internal JoinFormatDelegate JoinFormat;
        internal ModelDefinition ModelDef;
        internal string ParamName;
    }

    public abstract partial class SqlExpression<T> : ISqlExpression
    {
        protected List<ModelDefinition> tableDefs = new();

        public List<ModelDefinition> GetAllTables()
        {
            var allTableDefs = new List<ModelDefinition> { this.modelDef };
            allTableDefs.AddRange(this.tableDefs);
            return allTableDefs;
        }

        public bool IsJoinedTable(Type type)
        {
            return this.tableDefs.FirstOrDefault(x => x.ModelType == type) != null;
        }

        public SqlExpression<T> Join<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("INNER JOIN", joinExpr);
        }

        public SqlExpression<T> Join<Target>(Expression<Func<T, Target, bool>> joinExpr, TableOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.Expression != null)
                throw new ArgumentException("Can't set both Join Expression and TableOptions Expression");

            return this.InternalJoin("INNER JOIN", joinExpr, options);
        }

        public SqlExpression<T> Join<Target>(Expression<Func<T, Target, bool>> joinExpr, JoinFormatDelegate joinFormat) => this.InternalJoin("INNER JOIN", joinExpr, joinFormat ?? throw new ArgumentNullException(nameof(joinFormat)));

        public SqlExpression<T> Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("INNER JOIN", joinExpr);
        }

        public SqlExpression<T> Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat) => this.InternalJoin("INNER JOIN", joinExpr, joinFormat);

        public SqlExpression<T> Join<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, TableOptions options) => this.InternalJoin("INNER JOIN", joinExpr, options);

        public SqlExpression<T> Join(Type sourceType, Type targetType, Expression joinExpr = null)
        {
            return this.InternalJoin("INNER JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition());
        }

        public SqlExpression<T> LeftJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("LEFT JOIN", joinExpr);
        }

        public SqlExpression<T> LeftJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, JoinFormatDelegate joinFormat) => this.InternalJoin("LEFT JOIN", joinExpr, joinFormat ?? throw new ArgumentNullException(nameof(joinFormat)));

        public SqlExpression<T> LeftJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, TableOptions options) => this.InternalJoin("LEFT JOIN", joinExpr, options ?? throw new ArgumentNullException(nameof(options)));

        public SqlExpression<T> LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("LEFT JOIN", joinExpr);
        }

        public SqlExpression<T> LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat) => this.InternalJoin("LEFT JOIN", joinExpr, joinFormat);

        public SqlExpression<T> LeftJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, TableOptions options) => this.InternalJoin("LEFT JOIN", joinExpr, options);

        public SqlExpression<T> LeftJoin(Type sourceType, Type targetType, Expression joinExpr = null)
        {
            return this.InternalJoin("LEFT JOIN", joinExpr, sourceType.GetModelDefinition(), targetType.GetModelDefinition());
        }

        public SqlExpression<T> RightJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("RIGHT JOIN", joinExpr);
        }

        public SqlExpression<T> RightJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, JoinFormatDelegate joinFormat) => this.InternalJoin("RIGHT JOIN", joinExpr, joinFormat ?? throw new ArgumentNullException(nameof(joinFormat)));

        public SqlExpression<T> RightJoin<Target>(Expression<Func<T, Target, bool>> joinExpr, TableOptions options) => this.InternalJoin("RIGHT JOIN", joinExpr, options ?? throw new ArgumentNullException(nameof(options)));

        public SqlExpression<T> RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("RIGHT JOIN", joinExpr);
        }

        public SqlExpression<T> RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat) => this.InternalJoin("RIGHT JOIN", joinExpr, joinFormat);

        public SqlExpression<T> RightJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr, TableOptions options) => this.InternalJoin("RIGHT JOIN", joinExpr, options);

        public SqlExpression<T> FullJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("FULL JOIN", joinExpr);
        }

        public SqlExpression<T> FullJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("FULL JOIN", joinExpr);
        }

        public SqlExpression<T> CrossJoin<Target>(Expression<Func<T, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("CROSS JOIN", joinExpr);
        }

        public SqlExpression<T> CrossJoin<Source, Target>(Expression<Func<Source, Target, bool>> joinExpr = null)
        {
            return this.InternalJoin("CROSS JOIN", joinExpr);
        }

        protected SqlExpression<T> InternalJoin<Source, Target>(string joinType, Expression<Func<Source, Target, bool>> joinExpr, JoinFormatDelegate joinFormat) => this.InternalJoin(joinType, joinExpr, joinFormat != null ? new TableOptions { JoinFormat = joinFormat } : null);

        protected SqlExpression<T> InternalJoin<Source, Target>(string joinType, Expression<Func<Source, Target, bool>> joinExpr, TableOptions options = null)
        {
            var sourceDef = typeof(Source).GetModelDefinition();
            var targetDef = typeof(Target).GetModelDefinition();

            return this.InternalJoin(joinType, joinExpr, sourceDef, targetDef, options);
        }

        protected SqlExpression<T> InternalJoin<Source, Target>(string joinType, Expression joinExpr)
        {
            var sourceDef = typeof(Source).GetModelDefinition();
            var targetDef = typeof(Target).GetModelDefinition();

            return this.InternalJoin(joinType, joinExpr, sourceDef, targetDef);
        }

        public SqlExpression<T> Join<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr) => this.InternalJoin<Source, Target>("INNER JOIN", joinExpr);
        public SqlExpression<T> LeftJoin<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr) => this.InternalJoin<Source, Target>("LEFT JOIN", joinExpr);
        public SqlExpression<T> RightJoin<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr) => this.InternalJoin<Source, Target>("RIGHT JOIN", joinExpr);
        public SqlExpression<T> FullJoin<Source, Target, T3>(Expression<Func<Source, Target, T3, bool>> joinExpr) => this.InternalJoin<Source, Target>("FULL JOIN", joinExpr);

        public SqlExpression<T> Join<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr) => this.InternalJoin<Source, Target>("INNER JOIN", joinExpr);
        public SqlExpression<T> LeftJoin<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr) => this.InternalJoin<Source, Target>("LEFT JOIN", joinExpr);
        public SqlExpression<T> RightJoin<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr) => this.InternalJoin<Source, Target>("RIGHT JOIN", joinExpr);
        public SqlExpression<T> FullJoin<Source, Target, T3, T4>(Expression<Func<Source, Target, T3, T4, bool>> joinExpr) => this.InternalJoin<Source, Target>("FULL JOIN", joinExpr);

        private string InternalCreateSqlFromExpression(Expression joinExpr, bool isCrossJoin)
        {
            return $"{(isCrossJoin ? "WHERE" : "ON")} {this.VisitJoin(joinExpr)}";
        }

        private string InternalCreateSqlFromDefinitions(ModelDefinition sourceDef, ModelDefinition targetDef, bool isCrossJoin)
        {
            var parentDef = sourceDef;
            var childDef = targetDef;

            var refField = parentDef.GetRefFieldDefIfExists(childDef);
            if (refField == null)
            {
                parentDef = targetDef;
                childDef = sourceDef;
                refField = parentDef.GetRefFieldDefIfExists(childDef);
            }

            if (refField == null)
            {
                if (!isCrossJoin)
                    throw new ArgumentException($"Could not infer relationship between {sourceDef.ModelName} and {targetDef.ModelName}");

                return string.Empty;
            }

            return string.Format("{0}\n({1}.{2} = {3}.{4})",
                isCrossJoin ? "WHERE" : "ON",
                this.DialectProvider.GetQuotedTableName(parentDef),
                this.SqlColumn(parentDef.PrimaryKey.FieldName),
                this.DialectProvider.GetQuotedTableName(childDef),
                this.SqlColumn(refField.FieldName));
        }

        public SqlExpression<T> CustomJoin(string joinString)
        {
            this.PrefixFieldWithTableName = true;
            this.FromExpression += " " + joinString;
            return this;
        }

        private TableOptions joinAlias;

        protected virtual SqlExpression<T> InternalJoin(string joinType, Expression joinExpr, ModelDefinition sourceDef, ModelDefinition targetDef, TableOptions options = null)
        {
            this.PrefixFieldWithTableName = true;

            this.Reset();

            var joinFormat = options?.JoinFormat;
            if (options?.Alias != null)
            {
                // Set joinAlias
                options.ParamName = joinExpr is LambdaExpression l && l.Parameters.Count == 2
                    ? l.Parameters[1].Name
                    : null;
                if (options.ParamName != null)
                {
                    joinFormat = null;
                    options.ModelDef = targetDef;
                    this.joinAlias = options;
                }
            }


            if (!this.tableDefs.Contains(sourceDef)) this.tableDefs.Add(sourceDef);
            if (!this.tableDefs.Contains(targetDef)) this.tableDefs.Add(targetDef);

            var isCrossJoin = "CROSS JOIN" == joinType;

            var sqlExpr = joinExpr != null
                ? this.InternalCreateSqlFromExpression(joinExpr, isCrossJoin)
                : this.InternalCreateSqlFromDefinitions(sourceDef, targetDef, isCrossJoin);

            var joinDef = this.tableDefs.Contains(targetDef) && !this.tableDefs.Contains(sourceDef)
                ? sourceDef
                : targetDef;

            this.FromExpression += joinFormat != null
                ? $" {joinType} {joinFormat(this.DialectProvider, joinDef, sqlExpr)}"
                : this.joinAlias != null
                    ? $" {joinType} {this.SqlTable(joinDef)} {this.DialectProvider.GetQuotedName(this.joinAlias.Alias)} {sqlExpr}"
                    : $" {joinType} {this.SqlTable(joinDef)} {sqlExpr}";


            if (this.joinAlias != null)
            {
                // Unset joinAlias
                this.joinAlias = null;
                if (options != null)
                {
                    options.ParamName = null;
                    options.ModelDef = null;
                }
            }

            return this;
        }

        public string SelectInto<TModel>()
        {
            if (this.CustomSelect && this.OnlyFields == null || typeof(TModel) == typeof(T) && !this.PrefixFieldWithTableName)
            {
                return this.ToSelectStatement(QueryType.Select);
            }

            this.useFieldName = true;

            var sbSelect = StringBuilderCache.Allocate();
            var selectDef = this.modelDef;
            var orderedDefs = this.tableDefs;

            if (typeof(TModel) != typeof(List<object>) &&
                typeof(TModel) != typeof(Dictionary<string, object>) &&
                typeof(TModel) != typeof(object) && // dynamic
                !typeof(TModel).IsValueTuple())
            {
                selectDef = typeof(TModel).GetModelDefinition();
                if (selectDef != this.modelDef && this.tableDefs.Contains(selectDef))
                {
                    orderedDefs = this.tableDefs.ToList(); // clone
                    orderedDefs.Remove(selectDef);
                    orderedDefs.Insert(0, selectDef);
                }
            }

            foreach (var fieldDef in selectDef.FieldDefinitions)
            {
                var found = false;

                if (fieldDef.BelongToModelName != null)
                {
                    var tableDef = orderedDefs.FirstOrDefault(x => x.Name == fieldDef.BelongToModelName);
                    if (tableDef != null)
                    {
                        var matchingField = FindWeakMatch(tableDef, fieldDef);
                        if (matchingField != null)
                        {
                            if (this.OnlyFields == null || this.OnlyFields.Contains(fieldDef.Name))
                            {
                                if (sbSelect.Length > 0)
                                    sbSelect.Append(", ");

                                if (fieldDef.CustomSelect == null)
                                {
                                    if (!fieldDef.IsRowVersion)
                                    {
                                        sbSelect.Append($"{this.GetQuotedColumnName(tableDef, matchingField.Name)} AS {this.SqlColumn(fieldDef.Name)}");
                                    }
                                    else
                                    {
                                        sbSelect.Append(this.DialectProvider.GetRowVersionSelectColumn(fieldDef, this.DialectProvider.GetTableName(tableDef.ModelName)));
                                    }
                                }
                                else
                                {
                                    sbSelect.Append(fieldDef.CustomSelect + " AS " + fieldDef.FieldName);
                                }

                                continue;
                            }
                        }
                    }
                }

                foreach (var tableDef in orderedDefs)
                {
                    foreach (var tableFieldDef in tableDef.FieldDefinitions)
                    {
                        if (tableFieldDef.Name == fieldDef.Name)
                        {
                            if (this.OnlyFields != null && !this.OnlyFields.Contains(fieldDef.Name))
                                continue;

                            if (sbSelect.Length > 0)
                                sbSelect.Append(", ");

                            var tableAlias = tableDef == this.modelDef // Use TableAlias if source modelDef
                                ? this.TableAlias
                                : null;

                            if (fieldDef.CustomSelect == null)
                            {
                                if (!fieldDef.IsRowVersion)
                                {
                                    sbSelect.Append(tableAlias == null
                                        ? this.GetQuotedColumnName(tableDef, tableFieldDef.Name)
                                        : this.GetQuotedColumnName(tableDef, tableAlias, tableFieldDef.Name));

                                    if (tableFieldDef.RequiresAlias)
                                        sbSelect.Append(" AS ").Append(this.SqlColumn(fieldDef.Name));
                                }
                                else
                                {
                                    sbSelect.Append(this.DialectProvider.GetRowVersionSelectColumn(fieldDef, this.DialectProvider.GetTableName(tableAlias ?? tableDef.ModelName, tableDef.Schema)));
                                }
                            }
                            else
                            {
                                sbSelect.Append(tableFieldDef.CustomSelect).Append(" AS ").Append(tableFieldDef.FieldName);
                            }

                            found = true;
                            break;
                        }
                    }

                    if (found)
                        break;
                }

                if (!found)
                {
                    // Add support for auto mapping `{Table}{Field}` convention
                    foreach (var tableDef in orderedDefs)
                    {
                        var matchingField = FindWeakMatch(tableDef, fieldDef);
                        if (matchingField != null)
                        {
                            if (this.OnlyFields != null && !this.OnlyFields.Contains(fieldDef.Name))
                                continue;

                            if (sbSelect.Length > 0)
                                sbSelect.Append(", ");

                            var tableAlias = tableDef == this.modelDef // Use TableAlias if source modelDef
                                ? this.TableAlias
                                : null;

                            sbSelect.Append($"{this.DialectProvider.GetQuotedColumnName(tableDef, tableAlias, matchingField)} as {this.SqlColumn(fieldDef.Name)}");

                            break;
                        }
                    }
                }
            }

            var select = StringBuilderCache.ReturnAndFree(sbSelect);

            var columns = select.Length > 0 ? select : "*";
            this.SelectExpression = "SELECT " + (this.selectDistinct ? "DISTINCT " : string.Empty) + columns;

            return this.ToSelectStatement(QueryType.Select);
        }

        private static FieldDefinition FindWeakMatch(ModelDefinition tableDef, FieldDefinition fieldDef)
        {
            return tableDef.FieldDefinitions
                .FirstOrDefault(x =>
                    string.Compare(tableDef.Name + x.Name, fieldDef.Name, StringComparison.OrdinalIgnoreCase) == 0
                    || string.Compare(tableDef.ModelName + x.FieldName, fieldDef.Name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public virtual SqlExpression<T> Where<Target>(Expression<Func<Target, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<Source, Target>(Expression<Func<Source, Target, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<Target>(Expression<Func<Target, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<Source, Target>(Expression<Func<Source, Target, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> And<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> predicate) => this.AppendToWhere("AND", predicate);

        public virtual SqlExpression<T> Or<Target>(Expression<Func<Target, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<Source, Target>(Expression<Func<Source, Target, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public virtual SqlExpression<T> Or<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> predicate) => this.AppendToWhere("OR", predicate);

        public Tuple<ModelDefinition, FieldDefinition> FirstMatchingField(string fieldName)
        {
            foreach (var tableDef in this.tableDefs)
            {
                var firstField = tableDef.FieldDefinitions.FirstOrDefault(
                    x => string.Compare(x.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(
                        x.FieldName,
                        fieldName,
                        StringComparison.OrdinalIgnoreCase) == 0);

                if (firstField != null)
                {
                    return Tuple.Create(tableDef, firstField);
                }
            }

            // Fallback to fully qualified '{Table}{Field}' property convention
            foreach (var tableDef in this.tableDefs)
            {
                var firstField = tableDef.FieldDefinitions.FirstOrDefault(
                    x => string.Compare(tableDef.Name + x.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0 ||
                         string.Compare(
                             tableDef.ModelName + x.FieldName,
                             fieldName,
                             StringComparison.OrdinalIgnoreCase) == 0);

                if (firstField != null)
                {
                    return Tuple.Create(tableDef, firstField);
                }
            }

            return null;
        }
    }
}