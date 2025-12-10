// ***********************************************************************
// <copyright file="ReflectionOptimizer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class ReflectionOptimizer.
/// </summary>
public abstract class ReflectionOptimizer
{
    /// <summary>
    /// The instance
    /// </summary>
    public static ReflectionOptimizer Instance { get; set; } =
        EmitReflectionOptimizer.Provider;

    /// <summary>
    /// Uses the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    public abstract Type UseType(Type type);

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public abstract GetMemberDelegate CreateGetter(PropertyInfo propertyInfo);
    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public abstract GetMemberDelegate<T> CreateGetter<T>(PropertyInfo propertyInfo);
    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public abstract SetMemberDelegate CreateSetter(PropertyInfo propertyInfo);
    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public abstract SetMemberDelegate<T> CreateSetter<T>(PropertyInfo propertyInfo);

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public abstract GetMemberDelegate CreateGetter(FieldInfo fieldInfo);
    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public abstract GetMemberDelegate<T> CreateGetter<T>(FieldInfo fieldInfo);
    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public abstract SetMemberDelegate CreateSetter(FieldInfo fieldInfo);
    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public abstract SetMemberDelegate<T> CreateSetter<T>(FieldInfo fieldInfo);

    /// <summary>
    /// Creates the setter reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberRefDelegate&lt;T&gt;.</returns>
    public abstract SetMemberRefDelegate<T> CreateSetterRef<T>(FieldInfo fieldInfo);

    /// <summary>
    /// Determines whether the specified assembly is dynamic.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns><c>true</c> if the specified assembly is dynamic; otherwise, <c>false</c>.</returns>
    public abstract bool IsDynamic(Assembly assembly);
    /// <summary>
    /// Creates the constructor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>EmptyCtorDelegate.</returns>
    public abstract EmptyCtorDelegate CreateConstructor(Type type);
}

/// <summary>
/// Class RuntimeReflectionOptimizer. This class cannot be inherited.
/// Implements the <see cref="ReflectionOptimizer" />
/// </summary>
/// <seealso cref="ReflectionOptimizer" />
public sealed class RuntimeReflectionOptimizer : ReflectionOptimizer
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static RuntimeReflectionOptimizer Provider => field ??= new RuntimeReflectionOptimizer();
    /// <summary>
    /// Prevents a default instance of the <see cref="RuntimeReflectionOptimizer" /> class from being created.
    /// </summary>
    private RuntimeReflectionOptimizer() { }

    /// <summary>
    /// Uses the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    public override Type UseType(Type type)
    {
        return type;
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public override GetMemberDelegate CreateGetter(PropertyInfo propertyInfo)
    {
        var getMethodInfo = propertyInfo.GetGetMethod(true);
        if (getMethodInfo == null)
        {
            return null;
        }

        return o => propertyInfo.GetGetMethod(true).Invoke(o, TypeConstants.EmptyObjectArray);
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public override GetMemberDelegate<T> CreateGetter<T>(PropertyInfo propertyInfo)
    {
        var getMethodInfo = propertyInfo.GetGetMethod(true);
        if (getMethodInfo == null)
        {
            return null;
        }

        return o => propertyInfo.GetGetMethod(true).Invoke(o, TypeConstants.EmptyObjectArray);
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public override SetMemberDelegate CreateSetter(PropertyInfo propertyInfo)
    {
        var propertySetMethod = propertyInfo.GetSetMethod(true);
        if (propertySetMethod == null)
        {
            return null;
        }

        return (o, convertedValue) =>
            propertySetMethod.Invoke(o, [convertedValue]);
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public override SetMemberDelegate<T> CreateSetter<T>(PropertyInfo propertyInfo)
    {
        var propertySetMethod = propertyInfo.GetSetMethod(true);
        if (propertySetMethod == null)
        {
            return null;
        }

        return (o, convertedValue) =>
            propertySetMethod.Invoke(o, [convertedValue]);
    }


    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public override GetMemberDelegate CreateGetter(FieldInfo fieldInfo)
    {
        return fieldInfo.GetValue;
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public override GetMemberDelegate<T> CreateGetter<T>(FieldInfo fieldInfo)
    {
        return x => fieldInfo.GetValue(x);
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public override SetMemberDelegate CreateSetter(FieldInfo fieldInfo)
    {
        return fieldInfo.SetValue;
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public override SetMemberDelegate<T> CreateSetter<T>(FieldInfo fieldInfo)
    {
        return (o, x) => fieldInfo.SetValue(o, x);
    }

    /// <summary>
    /// Creates the setter reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberRefDelegate&lt;T&gt;.</returns>
    public override SetMemberRefDelegate<T> CreateSetterRef<T>(FieldInfo fieldInfo)
    {
        return ExpressionReflectionOptimizer.Provider.CreateSetterRef<T>(fieldInfo);
    }

    /// <summary>
    /// Determines whether the specified assembly is dynamic.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns><c>true</c> if the specified assembly is dynamic; otherwise, <c>false</c>.</returns>
    public override bool IsDynamic(Assembly assembly)
    {
        try
        {
            var isDyanmic = string.IsNullOrEmpty(assembly.Location);
            return isDyanmic;
        }
        catch (NotSupportedException)
        {
            //Ignore assembly.Location not supported in a dynamic assembly.
            return true;
        }
    }

    /// <summary>
    /// Creates the constructor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>EmptyCtorDelegate.</returns>
    public override EmptyCtorDelegate CreateConstructor(Type type)
    {
        var emptyCtor = type.GetConstructor(Type.EmptyTypes);
        if (emptyCtor != null)
        {
            return () => Activator.CreateInstance(type);
        }

        //Anonymous types don't have empty constructors
        return () => RuntimeHelpers.GetUninitializedObject(type);
    }
}

/// <summary>
/// Class ExpressionReflectionOptimizer. This class cannot be inherited.
/// Implements the <see cref="ReflectionOptimizer" />
/// </summary>
/// <seealso cref="ReflectionOptimizer" />
public sealed class ExpressionReflectionOptimizer : ReflectionOptimizer
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static ExpressionReflectionOptimizer Provider => field ??= new ExpressionReflectionOptimizer();
    /// <summary>
    /// Prevents a default instance of the <see cref="ExpressionReflectionOptimizer" /> class from being created.
    /// </summary>
    private ExpressionReflectionOptimizer() { }

    /// <summary>
    /// Uses the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    public override Type UseType(Type type)
    {
        return type;
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public override GetMemberDelegate CreateGetter(PropertyInfo propertyInfo)
    {
        var lambda = GetExpressionLambda(propertyInfo);
        var propertyGetFn = lambda.Compile();
        return propertyGetFn;
    }

    /// <summary>
    /// Gets the expression lambda.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>Expression&lt;GetMemberDelegate&gt;.</returns>
    public static Expression<GetMemberDelegate> GetExpressionLambda(PropertyInfo propertyInfo)
    {
        var getMethodInfo = propertyInfo.GetGetMethod(true);
        if (getMethodInfo == null)
        {
            return null;
        }

        var oInstanceParam = Expression.Parameter(typeof(object), "oInstanceParam");
        var instanceParam = Expression.Convert(oInstanceParam, propertyInfo.ReflectedType); //propertyInfo.DeclaringType doesn't work on Proxy types

        var exprCallPropertyGetFn = Expression.Call(instanceParam, getMethodInfo);
        var oExprCallPropertyGetFn = Expression.Convert(exprCallPropertyGetFn, typeof(object));

        return Expression.Lambda<GetMemberDelegate>
        (
            oExprCallPropertyGetFn,
            oInstanceParam
        );
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public override GetMemberDelegate<T> CreateGetter<T>(PropertyInfo propertyInfo)
    {
        var expr = GetExpressionLambda<T>(propertyInfo);
        return expr.Compile();
    }

    /// <summary>
    /// Gets the expression lambda.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>Expression&lt;GetMemberDelegate&lt;T&gt;&gt;.</returns>
    public static Expression<GetMemberDelegate<T>> GetExpressionLambda<T>(PropertyInfo propertyInfo)
    {
        var instance = Expression.Parameter(typeof(T), "i");
        var property = typeof(T) != propertyInfo.DeclaringType
                           ? Expression.Property(Expression.TypeAs(instance, propertyInfo.DeclaringType), propertyInfo)
                           : Expression.Property(instance, propertyInfo);
        var convertProperty = Expression.TypeAs(property, typeof(object));
        return Expression.Lambda<GetMemberDelegate<T>>(convertProperty, instance);
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public override SetMemberDelegate CreateSetter(PropertyInfo propertyInfo)
    {
        var propertySetMethod = propertyInfo.GetSetMethod(true);
        if (propertySetMethod == null)
        {
            return null;
        }

        try
        {
            var declaringType = propertyInfo.ReflectedType;

            var instance = Expression.Parameter(typeof(object), "i");
            var argument = Expression.Parameter(typeof(object), "a");

            var instanceParam = declaringType.IsValueType && !declaringType.IsNullableType()
                                    ? Expression.Unbox(instance, declaringType)
                                    : Expression.Convert(instance, declaringType);

            var valueParam = Expression.Convert(argument, propertyInfo.PropertyType);

            var setterCall = Expression.Call(instanceParam, propertySetMethod, valueParam);

            return Expression.Lambda<SetMemberDelegate>(setterCall, instance, argument).Compile();
        }
        catch //fallback for Android
        {
            return (o, convertedValue) =>
                propertySetMethod.Invoke(o, [convertedValue]);
        }
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public override SetMemberDelegate<T> CreateSetter<T>(PropertyInfo propertyInfo)
    {
        try
        {
            var lambda = SetExpressionLambda<T>(propertyInfo);
            return lambda?.Compile();
        }
        catch //fallback for Android
        {
            var mi = propertyInfo.GetSetMethod(true);
            return (o, convertedValue) =>
                mi.Invoke(o, [convertedValue]);
        }
    }

    /// <summary>
    /// Sets the expression lambda.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>Expression&lt;SetMemberDelegate&lt;T&gt;&gt;.</returns>
    public static Expression<SetMemberDelegate<T>> SetExpressionLambda<T>(PropertyInfo propertyInfo)
    {
        var mi = propertyInfo.GetSetMethod(true);
        if (mi == null)
        {
            return null;
        }

        var instance = Expression.Parameter(typeof(T), "i");
        var argument = Expression.Parameter(typeof(object), "a");

        var instanceType = typeof(T) != propertyInfo.DeclaringType
                               ? (Expression)Expression.TypeAs(instance, propertyInfo.DeclaringType)
                               : instance;

        var setterCall = Expression.Call(
            instanceType,
            mi,
            Expression.Convert(argument, propertyInfo.PropertyType));

        return Expression.Lambda<SetMemberDelegate<T>>
        (
            setterCall, instance, argument
        );
    }


    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public override GetMemberDelegate CreateGetter(FieldInfo fieldInfo)
    {
        var fieldDeclaringType = fieldInfo.DeclaringType;

        var oInstanceParam = Expression.Parameter(typeof(object), "source");
        var instanceParam = GetCastOrConvertExpression(oInstanceParam, fieldDeclaringType);

        var exprCallFieldGetFn = Expression.Field(instanceParam, fieldInfo);
        var oExprCallFieldGetFn = Expression.Convert(exprCallFieldGetFn, typeof(object));

        var fieldGetterFn = Expression.Lambda<GetMemberDelegate>
            (
                oExprCallFieldGetFn,
                oInstanceParam
            )
            .Compile();

        return fieldGetterFn;
    }

    /// <summary>
    /// Gets the cast or convert expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <returns>Expression.</returns>
    private static Expression GetCastOrConvertExpression(Expression expression, Type targetType)
    {
        Expression result;
        var expressionType = expression.Type;

        if (targetType.IsAssignableFrom(expressionType))
        {
            result = expression;
        }
        else
        {
            // Check if we can use the as operator for casting or if we must use the convert method
            if (targetType.IsValueType && !targetType.IsNullableType())
            {
                result = Expression.Convert(expression, targetType);
            }
            else
            {
                result = Expression.TypeAs(expression, targetType);
            }
        }

        return result;
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public override GetMemberDelegate<T> CreateGetter<T>(FieldInfo fieldInfo)
    {
        var instance = Expression.Parameter(typeof(T), "i");
        var field = typeof(T) != fieldInfo.DeclaringType
                        ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                        : Expression.Field(instance, fieldInfo);
        var convertField = Expression.TypeAs(field, typeof(object));
        return Expression.Lambda<GetMemberDelegate<T>>(convertField, instance).Compile();
    }

    /// <summary>
    /// The set field method
    /// </summary>
    private readonly static MethodInfo setFieldMethod = typeof(ExpressionReflectionOptimizer).GetStaticMethod(nameof(SetField));
    /// <summary>
    /// Sets the field.
    /// </summary>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="field">The field.</param>
    /// <param name="newValue">The new value.</param>
    static internal void SetField<TValue>(ref TValue field, TValue newValue)
    {
        field = newValue;
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public override SetMemberDelegate CreateSetter(FieldInfo fieldInfo)
    {
        var declaringType = fieldInfo.DeclaringType;

        var sourceParameter = Expression.Parameter(typeof(object), "source");
        var valueParameter = Expression.Parameter(typeof(object), "value");

        var sourceExpression = declaringType.IsValueType && !declaringType.IsNullableType()
                                   ? Expression.Unbox(sourceParameter, declaringType)
                                   : GetCastOrConvertExpression(sourceParameter, declaringType);

        var fieldExpression = Expression.Field(sourceExpression, fieldInfo);

        var valueExpression = GetCastOrConvertExpression(valueParameter, fieldExpression.Type);

        var genericSetFieldMethodInfo = setFieldMethod.MakeGenericMethod(fieldExpression.Type);

        var setFieldMethodCallExpression = Expression.Call(
            null, genericSetFieldMethodInfo, fieldExpression, valueExpression);

        var setterFn = Expression.Lambda<SetMemberDelegate>(
            setFieldMethodCallExpression, sourceParameter, valueParameter).Compile();

        return setterFn;
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public override SetMemberDelegate<T> CreateSetter<T>(FieldInfo fieldInfo)
    {
        var instance = Expression.Parameter(typeof(T), "i");
        var argument = Expression.Parameter(typeof(object), "a");

        var field = typeof(T) != fieldInfo.DeclaringType
                        ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                        : Expression.Field(instance, fieldInfo);

        var setterCall = Expression.Assign(
            field,
            Expression.Convert(argument, fieldInfo.FieldType));

        return Expression.Lambda<SetMemberDelegate<T>>
        (
            setterCall, instance, argument
        ).Compile();
    }

    /// <summary>
    /// Creates the setter reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberRefDelegate&lt;T&gt;.</returns>
    public override SetMemberRefDelegate<T> CreateSetterRef<T>(FieldInfo fieldInfo)
    {
        var instance = Expression.Parameter(typeof(T).MakeByRefType(), "i");
        var argument = Expression.Parameter(typeof(object), "a");

        var field = typeof(T) != fieldInfo.DeclaringType
                        ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                        : Expression.Field(instance, fieldInfo);

        var setterCall = Expression.Assign(
            field,
            Expression.Convert(argument, fieldInfo.FieldType));

        return Expression.Lambda<SetMemberRefDelegate<T>>
        (
            setterCall, instance, argument
        ).Compile();
    }

    /// <summary>
    /// Determines whether the specified assembly is dynamic.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns><c>true</c> if the specified assembly is dynamic; otherwise, <c>false</c>.</returns>
    public override bool IsDynamic(Assembly assembly)
    {
        return RuntimeReflectionOptimizer.Provider.IsDynamic(assembly);
    }

    /// <summary>
    /// Creates the constructor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>EmptyCtorDelegate.</returns>
    public override EmptyCtorDelegate CreateConstructor(Type type)
    {
        return RuntimeReflectionOptimizer.Provider.CreateConstructor(type);
    }
}