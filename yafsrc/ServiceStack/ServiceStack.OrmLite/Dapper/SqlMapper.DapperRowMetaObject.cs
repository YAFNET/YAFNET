// ***********************************************************************
// <copyright file="SqlMapper.DapperRowMetaObject.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Class DapperRow. This class cannot be inherited.
    /// Implements the <see cref="object" />
    /// Implements the <see cref="System.Dynamic.IDynamicMetaObjectProvider" />
    /// </summary>
    /// <seealso cref="object" />
    /// <seealso cref="System.Dynamic.IDynamicMetaObjectProvider" />
    private sealed partial class DapperRow : System.Dynamic.IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
        System.Dynamic.DynamicMetaObject System.Dynamic.IDynamicMetaObjectProvider.GetMetaObject(
            System.Linq.Expressions.Expression parameter)
        {
            return new DapperRowMetaObject(parameter, System.Dynamic.BindingRestrictions.Empty, this);
        }
    }

    /// <summary>
    /// Class DapperRowMetaObject. This class cannot be inherited.
    /// Implements the <see cref="System.Dynamic.DynamicMetaObject" />
    /// </summary>
    /// <seealso cref="System.Dynamic.DynamicMetaObject" />
    private sealed class DapperRowMetaObject : System.Dynamic.DynamicMetaObject
    {
        /// <summary>
        /// The get value method
        /// </summary>
        private static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();
        /// <summary>
        /// The set value method
        /// </summary>
        private static readonly MethodInfo setValueMethod = typeof(DapperRow).GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });

        /// <summary>
        /// Initializes a new instance of the <see cref="DapperRowMetaObject"/> class.
        /// </summary>
        /// <param name="expression">The expression representing this <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.</param>
        /// <param name="restrictions">The set of binding restrictions under which the binding is valid.</param>
        public DapperRowMetaObject(
            System.Linq.Expressions.Expression expression,
            System.Dynamic.BindingRestrictions restrictions
        )
            : base(expression, restrictions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DapperRowMetaObject"/> class.
        /// </summary>
        /// <param name="expression">The expression representing this <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.</param>
        /// <param name="restrictions">The set of binding restrictions under which the binding is valid.</param>
        /// <param name="value">The runtime value represented by the <see cref="T:System.Dynamic.DynamicMetaObject" />.</param>
        public DapperRowMetaObject(
            System.Linq.Expressions.Expression expression,
            System.Dynamic.BindingRestrictions restrictions,
            object value
        )
            : base(expression, restrictions, value)
        {
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Dynamic.DynamicMetaObject.</returns>
        private System.Dynamic.DynamicMetaObject CallMethod(
            MethodInfo method,
            System.Linq.Expressions.Expression[] parameters
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

        /// <summary>
        /// Performs the binding of the dynamic get member operation.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.GetMemberBinder" /> that represents the details of the dynamic operation.</param>
        /// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
        public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder)
        {
            var parameters = new System.Linq.Expressions.Expression[]
                                 {
                                     System.Linq.Expressions.Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        // Needed for Visual basic dynamic support
        /// <summary>
        /// Performs the binding of the dynamic invoke member operation.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.InvokeMemberBinder" /> that represents the details of the dynamic operation.</param>
        /// <param name="args">An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - arguments to the invoke member operation.</param>
        /// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
        public override System.Dynamic.DynamicMetaObject BindInvokeMember(System.Dynamic.InvokeMemberBinder binder, System.Dynamic.DynamicMetaObject[] args)
        {
            var parameters = new System.Linq.Expressions.Expression[]
                                 {
                                     System.Linq.Expressions.Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        /// <summary>
        /// Performs the binding of the dynamic set member operation.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.SetMemberBinder" /> that represents the details of the dynamic operation.</param>
        /// <param name="value">The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the value for the set member operation.</param>
        /// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
        public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value)
        {
            var parameters = new System.Linq.Expressions.Expression[]
                                 {
                                     System.Linq.Expressions.Expression.Constant(binder.Name),
                                     value.Expression,
                                 };

            var callMethod = CallMethod(setValueMethod, parameters);

            return callMethod;
        }

        /// <summary>
        /// The s nix keys
        /// </summary>
        static readonly string[] s_nixKeys = Array.Empty<string>();
        /// <summary>
        /// Gets the dynamic member names.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (HasValue && Value is IDictionary<string, object> lookup) return lookup.Keys;
            return s_nixKeys;
        }
    }
}