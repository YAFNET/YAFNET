using System;
using System.Collections.Generic;
using System.Reflection;
namespace ServiceStack.OrmLite.Dapper
{
    using System.Dynamic;
    using System.Linq.Expressions;

    public static partial class SqlMapper
    {
        private sealed class DapperRowMetaObject : System.Dynamic.DynamicMetaObject
        {
            private static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();
            private static readonly MethodInfo setValueMethod = typeof(DapperRow).GetMethod("SetValue", new[] { typeof(string), typeof(object) });

            public DapperRowMetaObject(
                Expression expression,
                BindingRestrictions restrictions
                )
                : base(expression, restrictions)
            {
            }

            public DapperRowMetaObject(
                Expression expression,
                BindingRestrictions restrictions,
                object value
                )
                : base(expression, restrictions, value)
            {
            }

            private DynamicMetaObject CallMethod(
                MethodInfo method,
                Expression[] parameters
                )
            {
                var callMethod = new System.Dynamic.DynamicMetaObject(
                    System.Linq.Expressions.Expression.Call(
                        System.Linq.Expressions.Expression.Convert(Expression, LimitType),
                        method,
                        parameters),
                    System.Dynamic.BindingRestrictions.GetTypeRestriction(Expression, LimitType)
                    );
                return callMethod;
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                var parameters = new Expression[]
                                     {
                                         System.Linq.Expressions.Expression.Constant(binder.Name)
                                     };

                var callMethod = CallMethod(getValueMethod, parameters);

                return callMethod;
            }

            // Needed for Visual basic dynamic support
            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                var parameters = new Expression[]
                                     {
                                         System.Linq.Expressions.Expression.Constant(binder.Name)
                                     };

                var callMethod = CallMethod(getValueMethod, parameters);

                return callMethod;
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                var parameters = new[]
                                     {
                                         System.Linq.Expressions.Expression.Constant(binder.Name),
                                         value.Expression,
                                     };

                var callMethod = CallMethod(setValueMethod, parameters);

                return callMethod;
            }
        }
    }
}
