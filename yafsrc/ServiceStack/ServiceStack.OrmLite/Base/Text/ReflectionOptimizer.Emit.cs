// ***********************************************************************
// <copyright file="ReflectionOptimizer.Emit.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if NETFX || NET9_0_OR_GREATER

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class EmitReflectionOptimizer. This class cannot be inherited.
/// Implements the <see cref="ReflectionOptimizer" />
/// </summary>
/// <seealso cref="ReflectionOptimizer" />
public sealed class EmitReflectionOptimizer : ReflectionOptimizer
{
    /// <summary>
    /// The provider
    /// </summary>
    private static EmitReflectionOptimizer provider;
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static EmitReflectionOptimizer Provider => provider ??= new EmitReflectionOptimizer();
    /// <summary>
    /// Prevents a default instance of the <see cref="EmitReflectionOptimizer"/> class from being created.
    /// </summary>
    private EmitReflectionOptimizer() { }

    /// <summary>
    /// Uses the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    public override Type UseType(Type type)
    {
        if (type.IsInterface || type.IsAbstract)
        {
            return DynamicProxy.GetInstanceFor(type).GetType();
        }

        return type;
    }

    /// <summary>
    /// Creates the dynamic get method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="memberInfo">The member information.</param>
    /// <returns>DynamicMethod.</returns>
    static internal DynamicMethod CreateDynamicGetMethod<T>(MemberInfo memberInfo)
    {
        var memberType = memberInfo is FieldInfo ? "Field" : "Property";
        var name = $"_Get{memberType}[T]_{memberInfo.Name}_";
        var returnType = typeof(object);

        return !memberInfo.DeclaringType.IsInterface
            ? new DynamicMethod(name, returnType, [typeof(T)], memberInfo.DeclaringType, true)
            : new DynamicMethod(name, returnType, [typeof(T)], memberInfo.Module, true);
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public override GetMemberDelegate CreateGetter(PropertyInfo propertyInfo)
    {
        var getter = CreateDynamicGetMethod(propertyInfo);

        var gen = getter.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);

        gen.Emit(
            propertyInfo.DeclaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass,
            propertyInfo.DeclaringType);

        var mi = propertyInfo.GetGetMethod(true);
        if (mi == null)
        {
            return null;
        }

        gen.Emit(mi.IsFinal ? OpCodes.Call : OpCodes.Callvirt, mi);

        if (propertyInfo.PropertyType.IsValueType)
        {
            gen.Emit(OpCodes.Box, propertyInfo.PropertyType);
        }

        gen.Emit(OpCodes.Ret);

        return (GetMemberDelegate)getter.CreateDelegate(typeof(GetMemberDelegate));
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public override GetMemberDelegate<T> CreateGetter<T>(PropertyInfo propertyInfo)
    {
        var getter = CreateDynamicGetMethod<T>(propertyInfo);

        var gen = getter.GetILGenerator();
        var mi = propertyInfo.GetGetMethod(true);
        if (mi == null)
        {
            return null;
        }

        if (typeof(T).IsValueType)
        {
            gen.Emit(OpCodes.Ldarga_S, 0);

            if (typeof(T) != propertyInfo.DeclaringType)
            {
                gen.Emit(OpCodes.Unbox, propertyInfo.DeclaringType);
            }
        }
        else
        {
            gen.Emit(OpCodes.Ldarg_0);

            if (typeof(T) != propertyInfo.DeclaringType)
            {
                gen.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            }
        }

        gen.Emit(mi.IsFinal ? OpCodes.Call : OpCodes.Callvirt, mi);

        if (propertyInfo.PropertyType.IsValueType)
        {
            gen.Emit(OpCodes.Box, propertyInfo.PropertyType);
        }

        gen.Emit(OpCodes.Isinst, typeof(object));

        gen.Emit(OpCodes.Ret);

        return (GetMemberDelegate<T>)getter.CreateDelegate(typeof(GetMemberDelegate<T>));
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public override SetMemberDelegate CreateSetter(PropertyInfo propertyInfo)
    {
        var mi = propertyInfo.GetSetMethod(true);
        if (mi == null)
        {
            return null;
        }

        var setter = CreateDynamicSetMethod(propertyInfo);

        var gen = setter.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);

        gen.Emit(
            propertyInfo.DeclaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass,
            propertyInfo.DeclaringType);

        gen.Emit(OpCodes.Ldarg_1);

        gen.Emit(
            propertyInfo.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass,
            propertyInfo.PropertyType);

        gen.EmitCall(mi.IsFinal ? OpCodes.Call : OpCodes.Callvirt, mi, null);

        gen.Emit(OpCodes.Ret);

        return (SetMemberDelegate)setter.CreateDelegate(typeof(SetMemberDelegate));
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public override SetMemberDelegate<T> CreateSetter<T>(PropertyInfo propertyInfo)
    {
        return ExpressionReflectionOptimizer.Provider.CreateSetter<T>(propertyInfo);
    }


    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate.</returns>
    public override GetMemberDelegate CreateGetter(FieldInfo fieldInfo)
    {
        var getter = CreateDynamicGetMethod(fieldInfo);

        var gen = getter.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0);

        gen.Emit(fieldInfo.DeclaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, fieldInfo.DeclaringType);

        gen.Emit(OpCodes.Ldfld, fieldInfo);

        if (fieldInfo.FieldType.IsValueType)
        {
            gen.Emit(OpCodes.Box, fieldInfo.FieldType);
        }

        gen.Emit(OpCodes.Ret);

        return (GetMemberDelegate)getter.CreateDelegate(typeof(GetMemberDelegate));
    }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
    public override GetMemberDelegate<T> CreateGetter<T>(FieldInfo fieldInfo)
    {
        var getter = CreateDynamicGetMethod<T>(fieldInfo);

        var gen = getter.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0);

        gen.Emit(OpCodes.Ldfld, fieldInfo);

        if (fieldInfo.FieldType.IsValueType)
        {
            gen.Emit(OpCodes.Box, fieldInfo.FieldType);
        }

        gen.Emit(OpCodes.Ret);

        return (GetMemberDelegate<T>)getter.CreateDelegate(typeof(GetMemberDelegate<T>));
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate.</returns>
    public override SetMemberDelegate CreateSetter(FieldInfo fieldInfo)
    {
        var setter = CreateDynamicSetMethod(fieldInfo);

        var gen = setter.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);

        gen.Emit(fieldInfo.DeclaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, fieldInfo.DeclaringType);

        gen.Emit(OpCodes.Ldarg_1);

        gen.Emit(fieldInfo.FieldType.IsClass
                ? OpCodes.Castclass
                : OpCodes.Unbox_Any,
            fieldInfo.FieldType);

        gen.Emit(OpCodes.Stfld, fieldInfo);
        gen.Emit(OpCodes.Ret);

        return (SetMemberDelegate)setter.CreateDelegate(typeof(SetMemberDelegate));
    }

    /// <summary>
    /// The dynamic get method arguments
    /// </summary>
    readonly static Type[] DynamicGetMethodArgs = [typeof(object)];

    /// <summary>
    /// Creates the dynamic get method.
    /// </summary>
    /// <param name="memberInfo">The member information.</param>
    /// <returns>DynamicMethod.</returns>
    static internal DynamicMethod CreateDynamicGetMethod(MemberInfo memberInfo)
    {
        var memberType = memberInfo is FieldInfo ? "Field" : "Property";
        var name = $"_Get{memberType}_{memberInfo.Name}_";
        var returnType = typeof(object);

        return !memberInfo.DeclaringType.IsInterface
            ? new DynamicMethod(name, returnType, DynamicGetMethodArgs, memberInfo.DeclaringType, true)
            : new DynamicMethod(name, returnType, DynamicGetMethodArgs, memberInfo.Module, true);
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
    public override SetMemberDelegate<T> CreateSetter<T>(FieldInfo fieldInfo)
    {
        return ExpressionReflectionOptimizer.Provider.CreateSetter<T>(fieldInfo);
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
            var isDynamic = assembly is AssemblyBuilder
                            || string.IsNullOrEmpty(assembly.Location);
            return isDynamic;
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
            var dm = new DynamicMethod("MyCtor", type, Type.EmptyTypes, typeof(ReflectionExtensions).Module, true);
            var ilgen = dm.GetILGenerator();
            ilgen.Emit(OpCodes.Nop);
            ilgen.Emit(OpCodes.Newobj, emptyCtor);
            ilgen.Emit(OpCodes.Ret);

            return (EmptyCtorDelegate)dm.CreateDelegate(typeof(EmptyCtorDelegate));
        }

        //Anonymous types don't have empty constructors
        return () => RuntimeHelpers.GetUninitializedObject(type);
    }

    /// <summary>
    /// The dynamic set method arguments
    /// </summary>
    readonly static Type[] DynamicSetMethodArgs = [typeof(object), typeof(object)];

    /// <summary>
    /// Creates the dynamic set method.
    /// </summary>
    /// <param name="memberInfo">The member information.</param>
    /// <returns>DynamicMethod.</returns>
    static internal DynamicMethod CreateDynamicSetMethod(MemberInfo memberInfo)
    {
        var memberType = memberInfo is FieldInfo ? "Field" : "Property";
        var name = $"_Set{memberType}_{memberInfo.Name}_";
        var returnType = typeof(void);

        return !memberInfo.DeclaringType.IsInterface
            ? new DynamicMethod(name, returnType, DynamicSetMethodArgs, memberInfo.DeclaringType, true)
            : new DynamicMethod(name, returnType, DynamicSetMethodArgs, memberInfo.Module, true);
    }
}


/// <summary>
/// Class DynamicProxy.
/// </summary>
public static class DynamicProxy
{
    /// <summary>
    /// Gets the instance for.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public static T GetInstanceFor<T>()
    {
        return (T)GetInstanceFor(typeof(T));
    }

    /// <summary>
    /// The module builder
    /// </summary>
    readonly static ModuleBuilder ModuleBuilder;
    /// <summary>
    /// The dynamic assembly
    /// </summary>
    readonly static AssemblyBuilder DynamicAssembly;
    /// <summary>
    /// The empty types
    /// </summary>
    readonly static Type[] EmptyTypes = Type.EmptyTypes;

    /// <summary>
    /// Gets the instance for.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <returns>System.Object.</returns>
    public static object GetInstanceFor(Type targetType)
    {
        lock (DynamicAssembly)
        {
            var constructedType = DynamicAssembly.GetType(ProxyName(targetType)) ?? GetConstructedType(targetType);
            var instance = Activator.CreateInstance(constructedType);
            return instance;
        }
    }

    /// <summary>
    /// Proxies the name.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <returns>System.String.</returns>
    static string ProxyName(Type targetType)
    {
        return targetType.Name + "Proxy";
    }

    /// <summary>
    /// Initializes static members of the <see cref="DynamicProxy"/> class.
    /// </summary>
    static DynamicProxy()
    {
        var assemblyName = new AssemblyName("DynImpl");
#if NET9_0_OR_GREATER
        DynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
            DynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
#endif
        ModuleBuilder = DynamicAssembly.DefineDynamicModule("DynImplModule");
    }

    /// <summary>
    /// Gets the type of the constructed.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <returns>Type.</returns>
    static Type GetConstructedType(Type targetType)
    {
        var typeBuilder = ModuleBuilder.DefineType(targetType.Name + "Proxy", TypeAttributes.Public);

        var ctorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            []);
        var ilGenerator = ctorBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ret);

        IncludeType(targetType, typeBuilder);

        foreach (var face in targetType.GetInterfaces())
        {
            IncludeType(face, typeBuilder);
        }

#if NET9_0_OR_GREATER
        return typeBuilder.CreateTypeInfo().AsType();
#else
            return typeBuilder.CreateType();
#endif
    }

    /// <summary>
    /// Includes the type.
    /// </summary>
    /// <param name="typeOfT">The type of t.</param>
    /// <param name="typeBuilder">The type builder.</param>
    static void IncludeType(Type typeOfT, TypeBuilder typeBuilder)
    {
        var methodInfos = typeOfT.GetMethods();
        foreach (var methodInfo in methodInfos)
        {
            if (methodInfo.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                continue; // we always add a set for a get.
            }

            if (methodInfo.Name.StartsWith("get_", StringComparison.Ordinal))
            {
                BindProperty(typeBuilder, methodInfo);
            }
            else
            {
                BindMethod(typeBuilder, methodInfo);
            }
        }

        typeBuilder.AddInterfaceImplementation(typeOfT);
    }

    /// <summary>
    /// Binds the method.
    /// </summary>
    /// <param name="typeBuilder">The type builder.</param>
    /// <param name="methodInfo">The method information.</param>
    static void BindMethod(TypeBuilder typeBuilder, MethodInfo methodInfo)
    {
        var methodBuilder = typeBuilder.DefineMethod(
            methodInfo.Name,
            MethodAttributes.Public | MethodAttributes.Virtual,
            methodInfo.ReturnType,
            [.. methodInfo.GetParameters().Select(p => p.GetType())]
        );
        var methodILGen = methodBuilder.GetILGenerator();
        if (methodInfo.ReturnType == typeof(void))
        {
            methodILGen.Emit(OpCodes.Ret);
        }
        else
        {
            if (methodInfo.ReturnType.IsValueType || methodInfo.ReturnType.IsEnum)
            {
                var getMethod = typeof(Activator).GetMethod("CreateInstance", [typeof(Type)]);
                var lb = methodILGen.DeclareLocal(methodInfo.ReturnType);
                methodILGen.Emit(OpCodes.Ldtoken, lb.LocalType);
                methodILGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                methodILGen.Emit(OpCodes.Callvirt, getMethod);
                methodILGen.Emit(OpCodes.Unbox_Any, lb.LocalType);
            }
            else
            {
                methodILGen.Emit(OpCodes.Ldnull);
            }
            methodILGen.Emit(OpCodes.Ret);
        }
        typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
    }

    /// <summary>
    /// Binds the property.
    /// </summary>
    /// <param name="typeBuilder">The type builder.</param>
    /// <param name="methodInfo">The method information.</param>
    public static void BindProperty(TypeBuilder typeBuilder, MethodInfo methodInfo)
    {
        // Backing Field
        var propertyName = methodInfo.Name.Replace("get_", "");
        var propertyType = methodInfo.ReturnType;
        var backingField = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

        //Getter
        var backingGet = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public |
                                                                         MethodAttributes.SpecialName | MethodAttributes.Virtual |
                                                                         MethodAttributes.HideBySig, propertyType, EmptyTypes);
        var getIl = backingGet.GetILGenerator();

        getIl.Emit(OpCodes.Ldarg_0);
        getIl.Emit(OpCodes.Ldfld, backingField);
        getIl.Emit(OpCodes.Ret);


        //Setter
        var backingSet = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public |
                                                                         MethodAttributes.SpecialName | MethodAttributes.Virtual |
                                                                         MethodAttributes.HideBySig, null, [propertyType]);

        var setIl = backingSet.GetILGenerator();

        setIl.Emit(OpCodes.Ldarg_0);
        setIl.Emit(OpCodes.Ldarg_1);
        setIl.Emit(OpCodes.Stfld, backingField);
        setIl.Emit(OpCodes.Ret);

        // Property
        var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
        propertyBuilder.SetGetMethod(backingGet);
        propertyBuilder.SetSetMethod(backingSet);
    }
}

#endif