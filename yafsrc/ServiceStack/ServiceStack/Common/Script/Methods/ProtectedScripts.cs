// ***********************************************************************
// <copyright file="ProtectedScripts.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using ServiceStack.IO;
using ServiceStack.Text;

namespace ServiceStack.Script;

// ReSharper disable InconsistentNaming
/// <summary>
/// Class ProtectedScripts.
/// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
/// </summary>
/// <seealso cref="ServiceStack.Script.ScriptMethods" />
public class ProtectedScripts : ScriptMethods
{
    /// <summary>
    /// The instance
    /// </summary>
    public static readonly ProtectedScripts Instance = new();

    /// <summary>
    /// Resolves the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public object resolve(ScriptScopeContext scope, object type)
    {
        if (type == null)
            return null;
        var t = type as Type ?? (type is string s
                                     ? @typeof(s)
                                     : throw new NotSupportedException($"{nameof(resolve)} requires a Type or Type Name, received '{type.GetType().Name}'"));

        var instance = scope.Context.Container.Resolve(t);
        return instance;
    }

    /// <summary>
    /// Defaults the specified type name.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>System.Object.</returns>
    public object @default(string typeName)
    {
        var type = assertTypeOf(typeName);
        return type.GetDefaultValue();
    }

    /// <summary>
    /// News the specified type name.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>System.Object.</returns>
    public object @new(string typeName)
    {
        var type = @typeof(typeName);
        return type != null
                   ? createInstance(type)
                   : null;
    }

    /// <summary>
    /// News the specified type name.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="constructorArgs">The constructor arguments.</param>
    /// <returns>System.Object.</returns>
    public object @new(string typeName, List<object> constructorArgs)
    {
        var type = @typeof(typeName);
        return type != null
                   ? createInstance(type, constructorArgs)
                   : null;
    }

    /// <summary>
    /// Sets the specified instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object set(object instance, Dictionary<string, object> args)
    {
        args.PopulateInstance(instance);
        return instance;
    }

    /// <summary>
    /// Types the generic types.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>Type[].</returns>
    private Type[] typeGenericTypes(string typeName)
    {
        return typeGenericTypes(typeGenericArgs(typeName));
    }

    /// <summary>
    /// Types the generic types.
    /// </summary>
    /// <param name="genericArgs">The generic arguments.</param>
    /// <returns>Type[].</returns>
    private Type[] typeGenericTypes(List<string> genericArgs)
    {
        var genericTypes = new List<Type>();
        foreach (var genericArg in genericArgs)
        {
            var genericType = @typeof(genericArg);
            genericTypes.Add(genericType);
        }

        return genericTypes.ToArray();
    }

    /// <summary>
    /// Types the generic arguments.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    private static List<string> typeGenericArgs(string typeName)
    {
        var argList = typeName.RightPart('<');
        argList = argList.Substring(0, argList.Length - 1);
        var splitArgs = StringUtils.SplitGenericArgs(argList);
        return splitArgs;
    }

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public object createInstance(Type type) => AssertCanCreateType(type).CreateInstance();

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="constructorArgs">The constructor arguments.</param>
    /// <returns>System.Object.</returns>
    public object createInstance(Type type, List<object> constructorArgs)
    {
        var key = callKey(AssertCanCreateType(type), "<new>", constructorArgs);

        var activator = (ObjectActivator)Context.Cache.GetOrAdd(key, k =>
            {

                var args = constructorArgs;
                var argTypes = args?.Select(x => x?.GetType()).ToArray() ?? TypeConstants.EmptyTypeArray;

                var ctorInfo = ResolveConstructor(type, argTypes);
                return ctorInfo.GetActivator();
            });

        return activator(constructorArgs?.ToArray() ?? TypeConstants.EmptyObjectArray);
    }

    /// <summary>
    /// Asserts the type of the can create.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    private Type AssertCanCreateType(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (!type.IsPublic && !Context.AllowScriptingOfAllTypes)
            throw new NotSupportedException(
                "Can only create instances of non public Types when AllowScriptingOfAllTypes=true");
        return type;
    }

    /// <summary>
    /// Resolves the constructor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="argTypes">The argument types.</param>
    /// <returns>ConstructorInfo.</returns>
    private ConstructorInfo ResolveConstructor(Type type, Type[] argTypes)
    {
        var argsCount = argTypes.Length;
        var ctors = type.GetConstructors()
            .Where(x => x.GetParameters().Length == argsCount).ToArray();

        if (ctors.Length == 0)
        {
            var argTypesList = string.Join(",", argTypes.Select(x => x?.Name ?? "null"));
            throw new NotSupportedException(
                $"Constructor {typeQualifiedName(type)}({argTypesList}) does not exist");
        }

        ConstructorInfo targetCtor = null;
        if (ctors.Length > 1)
        {
            var candidates = 0;
            foreach (var ctor in ctors)
            {
                var match = true;

                var ctorParams = ctor.GetParameters();
                for (var i = 0; i < argTypes.Length; i++)
                {
                    var argType = argTypes[i];
                    if (argType == null)
                        continue;

                    match = ctorParams[i].ParameterType == argType;
                    if (!match)
                        break;
                }

                if (match)
                {
                    targetCtor = ctor;
                    candidates++;
                }
            }

            if (targetCtor == null || candidates != 1)
            {
                var argTypesList = string.Join(",", argTypes.Select(x => x?.Name ?? "null"));
                throw new NotSupportedException(
                    $"Could not resolve ambiguous constructor {typeQualifiedName(type)}({argTypesList})");
            }
        }
        else
        {
            targetCtor = ctors[0];
        }

        return targetCtor;
    }

    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>Type.</returns>
    public Type getType(object instance) => instance?.GetType();

    /// <summary>
    /// Types the name of the qualified.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.String.</returns>
    public string typeQualifiedName(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        var sb = StringBuilderCache.Allocate();
        sb.Append(type.Namespace).Append('.');

        if (type.GenericTypeArguments.Length > 0)
        {
            sb.Append(type.Name.LeftPart('`'))
                .Append('<');

            var i = 0;
            foreach (var arg in type.GenericTypeArguments)
            {
                if (i++ > 0)
                    sb.Append(',');

                sb.Append(typeQualifiedName(arg));
            }
            sb.Append('>');
        }
        else
        {
            sb.Append(type.Name);
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Types the not found error message.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>System.String.</returns>
    public static string TypeNotFoundErrorMessage(string typeName) => $"Could not resolve Type '{typeName}'. " +
                                                                      "Use ScriptContext.ScriptAssemblies or ScriptContext.AllowScriptingOfAllTypes + ScriptNamespaces to increase Type resolution";

    /// <summary>
    /// Asserts the type of.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Type.</returns>
    public Type assertTypeOf(string name)
    {
        var type = @typeof(name);
        if (type == null)
            throw new NotSupportedException(TypeNotFoundErrorMessage(name));
        return type;
    }

    /// <summary>
    /// Returns Type from type name syntax of .NET's typeof()
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>Type.</returns>
    public Type @typeof(string typeName)
    {
        typeName = typeName?.Trim();

        if (string.IsNullOrEmpty(typeName))
            return null;

        var key = "type:" + typeName;

        Type cookType(Type type, List<string> genericArgs, bool isArray, bool isNullable)
        {
            if (type.IsGenericType)
            {
                var isGenericDefinition = genericArgs != null && genericArgs.All(x => x == "");
                if (!isGenericDefinition)
                {
                    var genericTypes = typeGenericTypes(genericArgs);
                    type = type.MakeGenericType(genericTypes);
                }
            }

            if (isArray)
            {
                type = type.MakeArrayType();
            }

            return isNullable
                       ? typeof(Nullable<>).MakeGenericType(type)
                       : type;
        }

        Type onlyTypeOf(string _typeName)
        {
            var isArray = _typeName.EndsWith("[]");
            if (isArray)
            {
                _typeName = _typeName.Substring(0, _typeName.Length - 2);
            }

            var isGeneric = _typeName.IndexOf('<') >= 0;
            List<string> genericArgs = null;

            if (isGeneric)
            {
                genericArgs = typeGenericArgs(_typeName);
                _typeName = _typeName.LeftPart('<') + '`' + Math.Max(genericArgs.Count, 1);
            }
            var isNullable = _typeName.EndsWith("?");
            if (isNullable)
                _typeName = _typeName.Substring(0, _typeName.Length - 1);

            if (_typeName.IndexOf('.') >= 0)
            {
                if (Context.ScriptTypeQualifiedNameMap.TryGetValue(_typeName, out var type))
                    return cookType(type, genericArgs, isArray, isNullable);

                if (Context.AllowScriptingOfAllTypes)
                {
                    type = AssemblyUtils.FindType(_typeName);
                    if (type != null)
                        return cookType(type, genericArgs, isArray, isNullable);
                }
            }
            else
            {
                var ret = _typeName switch
                    {
                        "int" => !isArray ? typeof(int) : typeof(int[]),
                        "long" => !isArray ? typeof(long) : typeof(long[]),
                        "bool" => !isArray ? typeof(bool) : typeof(bool[]),
                        "char" => !isArray ? typeof(char) : typeof(char[]),
                        "double" => !isArray ? typeof(double) : typeof(double[]),
                        "float" => !isArray ? typeof(float) : typeof(float[]),
                        "decimal" => !isArray ? typeof(decimal) : typeof(decimal[]),
                        "byte" => !isArray ? typeof(byte) : typeof(byte[]),
                        "sbyte" => !isArray ? typeof(sbyte) : typeof(sbyte[]),
                        "uint" => !isArray ? typeof(uint) : typeof(uint[]),
                        "ulong" => !isArray ? typeof(ulong) : typeof(ulong[]),
                        "object" => !isArray ? typeof(object) : typeof(object[]),
                        "short" => !isArray ? typeof(short) : typeof(short[]),
                        "ushort" => !isArray ? typeof(ushort) : typeof(ushort[]),
                        "string" => !isArray ? typeof(string) : typeof(string[]),
                        "Guid" => !isArray ? typeof(Guid) : typeof(Guid[]),
                        "TimeSpan" => !isArray ? typeof(TimeSpan) : typeof(TimeSpan[]),
                        "DateTime" => !isArray ? typeof(DateTime) : typeof(DateTime[]),
                        "DateTimeOffset" => !isArray ? typeof(DateTimeOffset) : typeof(DateTimeOffset[]),
                        _ => null,
                    };
                if (ret != null)
                {
                    return isNullable
                               ? typeof(Nullable<>).MakeGenericType(ret)
                               : ret;
                }

                if (Context.ScriptTypeNameMap.TryGetValue(_typeName, out var type))
                    return cookType(type, genericArgs, isArray, isNullable);
            }

            foreach (var ns in Context.ScriptNamespaces)
            {
                var lookupType = ns + "." + _typeName;
                if (Context.ScriptTypeQualifiedNameMap.TryGetValue(lookupType, out var type))
                    return cookType(type, genericArgs, isArray, isNullable);

                if (Context.AllowScriptingOfAllTypes)
                {
                    type = AssemblyUtils.FindType(lookupType);
                    if (type != null)
                        return cookType(type, genericArgs, isArray, isNullable);
                }
            }

            return null;
        }

        var resolvedType = (Type)Context.Cache.GetOrAdd(key, k =>
            {

                var type = onlyTypeOf(typeName);
                if (type != null)
                    return type;

                var parts = typeName.Split('.');
                if (parts.Length > 1)
                {
                    var nameBuilder = "";
                    for (var i = 0; i < parts.Length; i++)
                    {
                        try
                        {
                            if (i > 0)
                                nameBuilder += '.';

                            nameBuilder += parts[i];
                            var parentType = onlyTypeOf(nameBuilder);
                            if (parentType != null)
                            {
                                var nestedTypeName = parts[++i];
                                var nestedType = parentType.GetNestedType(nestedTypeName);
                                i++;
                                while (i < parts.Length)
                                {
                                    nestedTypeName = parts[i++];
                                    nestedType = nestedType.GetNestedType(nestedTypeName);
                                }
                                return nestedType;
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                return null;
            });

        return resolvedType;
    }

    /// <summary>
    /// Calls the specified instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.Object.</returns>
    public object call(object instance, string name) => call(instance, name, null);

    /// <summary>
    /// Calls the key.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.String.</returns>
    internal string callKey(Type type, string name, List<object> args)
    {
        var sb = StringBuilderCache.Allocate()
            .Append("call:")
            .Append(type.Namespace)
            .Append('.')
            .Append(type.Name)
            .Append('.')
            .Append(name);

        if (type.GenericTypeArguments.Length > 0)
        {
            sb.Append('<');
            for (var i = 0; i < type.GenericTypeArguments.Length; i++)
            {
                if (i > 0)
                    sb.Append(',');
                var genericArg = type.GenericTypeArguments[i];
                sb.Append(typeQualifiedName(genericArg));
            }
            sb.Append('>');
        }

        appendArgTypes(sb, args);
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Arguments the types string.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>System.String.</returns>
    static string argTypesString(List<object> args)
    {
        var sb = StringBuilderCache.Allocate();
        appendArgTypes(sb, args);
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Appends the argument types.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="args">The arguments.</param>
    private static void appendArgTypes(StringBuilder sb, List<object> args)
    {
        sb.Append('(');

        if (args != null)
        {
            for (var i = 0; i < args.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                var argType = args[i]?.GetType();
                sb.Append(argType == null ? "null" : argType.Namespace + '.' + argType.Name);
            }
        }

        sb.Append(')');
    }

    /// <summary>
    /// Calls the specified instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="name">The name.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object call(object instance, string name, List<object> args)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var type = instance.GetType();

        var key = callKey(type, name, args);

        var invoker = (Delegate)Context.Cache.GetOrAdd(key, k =>
            {
                var argTypes = args?.Select(x => x?.GetType()).ToArray();
                var targetMethod = ResolveMethod(type, name, argTypes, argTypes?.Length ?? 0, out var fn);
                if (targetMethod != null && targetMethod.IsStatic)
                    throw new NotSupportedException($"Cannot call static method {instance.GetType().Name}.{targetMethod.Name}");

                return fn ?? targetMethod.GetInvokerDelegate();
            });

        if (invoker is MethodInvoker methodInvoker)
        {
            var ret = methodInvoker(instance, args?.ToArray() ?? TypeConstants.EmptyObjectArray);
            return ret;
        }
        if (invoker is ActionInvoker actionInvoker)
        {
            actionInvoker(instance, args?.ToArray() ?? TypeConstants.EmptyObjectArray);
            return IgnoreResult.Value;
        }

        throw new NotSupportedException($"Cannot call {invoker.GetType().Name} methods");
    }

    /// <summary>
    /// Resolves the method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="argTypes">The argument types.</param>
    /// <param name="argsCount">The arguments count.</param>
    /// <param name="invokerDelegate">The invoker delegate.</param>
    /// <returns>MethodInfo.</returns>
    private MethodInfo ResolveMethod(Type type, string methodName, Type[] argTypes, int? argsCount, out Delegate invokerDelegate)
    {
        invokerDelegate = null;
        var isGeneric = methodName.IndexOf('<') >= 0;
        var name = isGeneric ? methodName.LeftPart('<') : methodName;

        var genericArgs = isGeneric
                              ? typeGenericArgs(methodName)
                              : TypeConstants.EmptyStringList;
        var genericArgsCount = genericArgs.Count;

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(x => x.Name == name && (argsCount == null || x.GetParameters().Length == argsCount.Value)
                                       && (genericArgs.Count == 0 && !x.IsGenericMethod || x.IsGenericMethod && x.GetGenericArguments().Length == genericArgsCount))
            .ToArray();

        MethodInfo targetMethod = null;
        if (methods.Length == 0)
        {
            if ((argTypes?.Length ?? 0) == 0)
            {
                var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                if (prop != null)
                {
                    targetMethod = prop.GetMethod;
                    if (targetMethod == null)
                    {
                        throw new NotSupportedException(
                            $"Property {typeQualifiedName(type)}.{name} does not have a getter");
                    }
                }
                else
                {
                    var field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    if (field != null)
                    {
                        if (field.IsStatic)
                        {
                            invokerDelegate = (StaticMethodInvoker)((args) => field.GetValue(null));
                            return null;
                        }
                        else
                        {
                            invokerDelegate = (MethodInvoker)((instance, args) => field.GetValue(instance));
                            return null;
                        }
                    }
                }
            }

            if (targetMethod == null)
            {
                throw new NotSupportedException(
                    $"Method {typeQualifiedName(type)}.{name} does not exist");
            }
        }

        if (targetMethod == null)
        {
            if (methods.Length > 1)
            {
                var candidates = 0;
                foreach (var method in methods)
                {
                    var match = true;

                    var methodParams = method.GetParameters();
                    if (argTypes != null)
                    {
                        for (var i = 0; i < argTypes.Length; i++)
                        {
                            var argType = argTypes[i];
                            if (argType == null)
                                continue;

                            match = methodParams[i].ParameterType == argType;
                            if (!match)
                                break;
                        }
                    }

                    if (match)
                    {
                        targetMethod = method;
                        candidates++;
                    }
                }

                if (targetMethod == null || candidates != 1)
                {
                    var argTypesList = argTypes != null ? string.Join(",", argTypes.Select(x => x?.Name ?? "null")) : "";
                    throw new NotSupportedException(
                        $"Could not resolve ambiguous method {typeQualifiedName(type)}.{name}({argTypesList})");
                }
            }
            else
            {
                targetMethod = methods[0];
            }
        }

        if (targetMethod.IsGenericMethod)
        {
            var genericTypes = typeGenericTypes(methodName);
            targetMethod = targetMethod.MakeGenericMethod(genericTypes);
        }

        if (targetMethod == null)
            throw new NotSupportedException(MethodNotExists($"{type.Name}.{name}"));

        return targetMethod;
    }

    /// <summary>
    /// Qualified Constructor Name Examples:
    /// - Type()
    /// - Type(string)
    /// - GenericType&lt;string&lt;(System.Int32)
    /// - Namespace.Type()
    /// </summary>
    /// <param name="qualifiedConstructorName">Name of the qualified constructor.</param>
    /// <returns>ObjectActivator.</returns>
    public ObjectActivator Constructor(string qualifiedConstructorName)
    {
        if (qualifiedConstructorName.IndexOf('(') == -1)
            throw new NotSupportedException($"Invalid Constructor Name '{qualifiedConstructorName}', " +
                                            "format: <type>(<arg-types>), e.g. Uri(String), see: https://sharpscript.net/docs/script-net");

        var name = qualifiedConstructorName;

        var activator = (ObjectActivator)Context.Cache.GetOrAdd(nameof(Constructor) + ":" + name, k =>
            {
                var argList = name.LastRightPart('(');
                argList = argList?.Substring(0, argList.Length - 1);
                var argTypes = typeGenericTypes(StringUtils.SplitGenericArgs(argList));

                name = name.LastLeftPart('(');

                var type = assertTypeOf(name);

                var ctor = ResolveConstructor(type, argTypes);

                return ctor.GetActivator();
            });

        return activator;
    }

    /// <summary>
    /// Shorter Alias for Constructor
    /// </summary>
    /// <param name="qualifiedMethodName">Name of the qualified method.</param>
    /// <returns>Delegate.</returns>
    public Delegate C(string qualifiedMethodName) => Constructor(qualifiedMethodName);

    /// <summary>
    /// Shorter Alias for Function
    /// </summary>
    /// <param name="qualifiedMethodName">Name of the qualified method.</param>
    /// <returns>Delegate.</returns>
    public Delegate F(string qualifiedMethodName) => Function(qualifiedMethodName);

    /// <summary>
    /// Shorter Alias for Function(name,args)
    /// </summary>
    /// <param name="qualifiedMethodName">Name of the qualified method.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>Delegate.</returns>
    public Delegate F(string qualifiedMethodName, List<object> args) => Function(qualifiedMethodName, args);

    /// <summary>
    /// Qualified Method Name Examples:
    /// - Console.WriteLine(string)
    /// - Type.StaticMethod
    /// - Type.InstanceMethod
    /// - GenericType&lt;string&lt;.Method
    /// - GenericType&lt;string&lt;.GenericMethod&lt;System.Int32&lt;
    /// - Namespace.Type.Method
    /// </summary>
    /// <param name="qualifiedMethodName">Name of the qualified method.</param>
    /// <returns>Delegate.</returns>
    public Delegate Function(string qualifiedMethodName)
    {
        if (qualifiedMethodName.IndexOf('.') == -1)
            throw new NotSupportedException($"Invalid Function Name '{qualifiedMethodName}', " +
                                            "format: <type>.<method>(<arg-types>), e.g. Console.WriteLine(string), see: https://sharpscript.net/docs/script-net");

        var invoker = (Delegate)Context.Cache.GetOrAdd(nameof(Function) + ":" + qualifiedMethodName, k =>
            ResolveFunction(qualifiedMethodName));

        return invoker;
    }

    /// <summary>
    /// Resolve Function from qualified type name, when args Type list are unspecified fallback to use args to resolve ambiguous methods
    /// Qualified Method Name Examples:
    /// - Console.WriteLine ['string']
    /// - Type.StaticMethod
    /// - Type.InstanceMethod
    /// - GenericType&lt;string&lt;.Method
    /// - GenericType&lt;string&lt;.GenericMethod&lt;System.Int32&lt;
    /// - Namespace.Type.Method
    /// </summary>
    /// <param name="qualifiedMethodName">Name of the qualified method.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>Delegate.</returns>
    public Delegate Function(string qualifiedMethodName, List<object> args)
    {
        if (qualifiedMethodName.IndexOf('.') == -1)
            throw new NotSupportedException($"Invalid Function Name '{qualifiedMethodName}', " +
                                            "format: <type>.<method>(<arg-types>), e.g. Console.WriteLine(string), see: https://sharpscript.net/docs/script-net");

        var key = nameof(Function) + ":" + qualifiedMethodName + argTypesString(args);
        var invoker = (Delegate)Context.Cache.GetOrAdd(key, k =>
            ResolveFunction(qualifiedMethodName, args?.Select(x => x?.GetType()).ToArray()));

        return invoker;
    }

    /// <summary>
    /// Resolves the function.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="argTypes">The argument types.</param>
    /// <returns>Delegate.</returns>
    private Delegate ResolveFunction(string name, Type[] argTypes = null)
    {
        var hasArgsList = name.IndexOf('(') >= 0;
        var argList = hasArgsList
                          ? name.LastRightPart('(')
                          : null;
        argList = argList?.Substring(0, argList.Length - 1);

        name = name.LastLeftPart('(');

        var lastGenericPos = name.LastIndexOf('>');
        var lastSepPos = name.LastIndexOf('.');

        int pos;
        if (lastSepPos > lastGenericPos)
        {
            pos = lastSepPos;
        }
        else
        {
            var genericPos = name.IndexOf('<');
            pos = genericPos >= 0
                      ? name.LastIndexOf('.', genericPos)
                      : name.LastIndexOf('.');

            if (pos == -1)
                pos = name.IndexOf(">.", StringComparison.Ordinal) + 1;
        }

        if (pos == -1)
            throw new NotSupportedException($"Could not parse Function Name '{name}', " +
                                            "format: <type>.<method>(<arg-types>), e.g. Console.WriteLine(string)");


        var typeName = name.Substring(0, pos);
        var methodName = name.Substring(pos + 1);

        if (hasArgsList)
        {
            var splitArgs = StringUtils.SplitGenericArgs(argList);
            argTypes = typeGenericTypes(splitArgs);
            for (var i = 0; i < argTypes.Length; i++)
            {
                if (argTypes[i] == null)
                    throw new NotSupportedException($"Could not resolve Argument Type '{splitArgs[i]}' for '{name}'");
            }
        }

        var type = assertTypeOf(typeName);

        var method = ResolveMethod(type, methodName, argTypes, argTypes?.Length, out var fn);
        return fn ?? method.GetInvokerDelegate();
    }

    /// <summary>
    /// Methods the not exists.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>System.String.</returns>
    static string MethodNotExists(string methodName) => $"Method {methodName} does not exist";

    /// <summary>
    /// VFSs the memory.
    /// </summary>
    /// <returns>MemoryVirtualFiles.</returns>
    public MemoryVirtualFiles vfsMemory() => new();

    /// <summary>
    /// VFSs the file system.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <returns>FileSystemVirtualFiles.</returns>
    public FileSystemVirtualFiles vfsFileSystem(string dirPath) => new(dirPath);

    /// <summary>
    /// Oses the paths.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public string osPaths(string path) => Env.IsWindows
                                              ? path.Replace('/', '\\')
                                              : path.Replace('\\', '/');

    /// <summary>
    /// Resolves the file.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualFile.</returns>
    public IVirtualFile resolveFile(ScriptScopeContext scope, string virtualPath) =>
        ResolveFile(scope.Context.VirtualFiles, scope.PageResult.VirtualPath, virtualPath);

    /// <summary>
    /// Resolves the file.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualFile.</returns>
    public IVirtualFile ResolveFile(string filterName, ScriptScopeContext scope, string virtualPath)
    {
        var file = ResolveFile(scope.Context.VirtualFiles, scope.PageResult.VirtualPath, virtualPath);
        if (file == null)
            throw new FileNotFoundException($"{filterName} '{virtualPath}' in page '{scope.Page.VirtualPath}' was not found");

        return file;
    }

    /// <summary>
    /// Resolves the file.
    /// </summary>
    /// <param name="virtualFiles">The virtual files.</param>
    /// <param name="fromVirtualPath">From virtual path.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualFile.</returns>
    public IVirtualFile ResolveFile(IVirtualPathProvider virtualFiles, string fromVirtualPath, string virtualPath)
    {
        IVirtualFile file;

        var pathMapKey = nameof(ResolveFile) + ">" + fromVirtualPath;
        var pathMapping = Context.GetPathMapping(pathMapKey, virtualPath);
        if (pathMapping != null)
        {
            file = virtualFiles.GetFile(pathMapping);
            if (file != null)
                return file;
            Context.RemovePathMapping(pathMapKey, pathMapping);
        }

        var tryExactMatch = virtualPath.IndexOf('/') >= 0; //if nested path specified, look for an exact match first
        if (tryExactMatch)
        {
            file = virtualFiles.GetFile(virtualPath);
            if (file != null)
            {
                Context.SetPathMapping(pathMapKey, virtualPath, virtualPath);
                return file;
            }
        }

        var parentPath = fromVirtualPath.IndexOf('/') >= 0
                             ? fromVirtualPath.LastLeftPart('/')
                             : "";

        do
        {
            var seekPath = parentPath.CombineWith(virtualPath);
            file = virtualFiles.GetFile(seekPath);
            if (file != null)
            {
                Context.SetPathMapping(pathMapKey, virtualPath, seekPath);
                return file;
            }

            if (parentPath == "")
                break;

            parentPath = parentPath.IndexOf('/') >= 0
                             ? parentPath.LastLeftPart('/')
                             : "";
        } while (true);

        return null;
    }

    /// <summary>
    /// Includes the file.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="virtualPath">The virtual path.</param>
    public async Task includeFile(ScriptScopeContext scope, string virtualPath)
    {
        var file = ResolveFile(nameof(includeFile), scope, virtualPath);
        using var reader = file.OpenRead();
        await reader.CopyToAsync(scope.OutputStream).ConfigAwait();
    }

    /// <summary>
    /// Ifs the debug include script.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="virtualPath">The virtual path.</param>
    public async Task ifDebugIncludeScript(ScriptScopeContext scope, string virtualPath)
    {
        if (scope.Context.DebugMode)
        {
            await scope.OutputStream.WriteAsync("<script>").ConfigAwait();
            await includeFile(scope, virtualPath).ConfigAwait();
            await scope.OutputStream.WriteAsync("</script>").ConfigAwait();
        }
    }

    /// <summary>
    /// Gets the virtual files.
    /// </summary>
    /// <value>The virtual files.</value>
    IVirtualPathProvider VirtualFiles => Context.VirtualFiles;

    // Old Aliases for Backwards compatibility
    /// <summary>
    /// VFSs all files.
    /// </summary>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    [Alias("allFiles")]
    public IEnumerable<IVirtualFile> vfsAllFiles() => allFiles(VirtualFiles);
    /// <summary>
    /// VFSs all root files.
    /// </summary>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    [Alias("allRootFiles")]
    public IEnumerable<IVirtualFile> vfsAllRootFiles() => allRootFiles(VirtualFiles);
    /// <summary>
    /// VFSs all root directories.
    /// </summary>
    /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
    [Alias("allRootDirectories")]
    public IEnumerable<IVirtualDirectory> vfsAllRootDirectories() => allRootDirectories(VirtualFiles);
    /// <summary>
    /// VFSs the combine path.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    /// <param name="relativePath">The relative path.</param>
    /// <returns>System.String.</returns>
    [Alias("combinePath")]
    public string vfsCombinePath(string basePath, string relativePath) => combinePath(VirtualFiles, basePath, relativePath);
    /// <summary>
    /// Dirs the files find.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    [Alias("findFilesInDirectory")]
    public IEnumerable<IVirtualFile> dirFilesFind(string dirPath, string globPattern) => findFilesInDirectory(VirtualFiles, dirPath, globPattern);
    /// <summary>
    /// Fileses the find.
    /// </summary>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    [Alias("findFiles")]
    public IEnumerable<IVirtualFile> filesFind(string globPattern) => findFiles(VirtualFiles, globPattern);
    /// <summary>
    /// Files the write.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="contents">The contents.</param>
    /// <returns>System.String.</returns>
    [Alias("writeFile")]
    public string fileWrite(string virtualPath, object contents) => writeFile(VirtualFiles, virtualPath, contents);
    /// <summary>
    /// Files the append.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="contents">The contents.</param>
    /// <returns>System.String.</returns>
    [Alias("appendToFile")]
    public string fileAppend(string virtualPath, object contents) => appendToFile(VirtualFiles, virtualPath, contents);
    /// <summary>
    /// Files the delete.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    [Alias("deleteFile")]
    public string fileDelete(string virtualPath) => deleteFile(VirtualFiles, virtualPath);
    /// <summary>
    /// Dirs the delete.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    [Alias("deleteFile")]
    public string dirDelete(string virtualPath) => deleteFile(VirtualFiles, virtualPath);
    /// <summary>
    /// Files the read all.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    [Alias("fileTextContents")]
    public string fileReadAll(string virtualPath) => fileTextContents(VirtualFiles, virtualPath);
    /// <summary>
    /// Files the read all bytes.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.Byte[].</returns>
    [Alias("fileBytesContent")]
    public byte[] fileReadAllBytes(string virtualPath) => fileBytesContent(VirtualFiles, virtualPath);

    /// <summary>
    /// Alls the files.
    /// </summary>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> allFiles() => allFiles(VirtualFiles);
    /// <summary>
    /// Alls the files.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> allFiles(IVirtualPathProvider vfs) => vfs.GetAllFiles();

    /// <summary>
    /// Alls the root files.
    /// </summary>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> allRootFiles() => allRootFiles(VirtualFiles);
    /// <summary>
    /// Alls the root files.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> allRootFiles(IVirtualPathProvider vfs) => vfs.GetRootFiles();
    /// <summary>
    /// Alls the root directories.
    /// </summary>
    /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
    public IEnumerable<IVirtualDirectory> allRootDirectories() => allRootDirectories(VirtualFiles);
    /// <summary>
    /// Alls the root directories.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
    public IEnumerable<IVirtualDirectory> allRootDirectories(IVirtualPathProvider vfs) => vfs.GetRootDirectories();
    /// <summary>
    /// Combines the path.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    /// <param name="relativePath">The relative path.</param>
    /// <returns>System.String.</returns>
    public string combinePath(string basePath, string relativePath) => combinePath(VirtualFiles, basePath, relativePath);
    /// <summary>
    /// Combines the path.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="basePath">The base path.</param>
    /// <param name="relativePath">The relative path.</param>
    /// <returns>System.String.</returns>
    public string combinePath(IVirtualPathProvider vfs, string basePath, string relativePath) => vfs.CombineVirtualPath(basePath, relativePath);

    /// <summary>
    /// Dirs the specified virtual path.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualDirectory.</returns>
    public IVirtualDirectory dir(string virtualPath) => dir(VirtualFiles, virtualPath);
    /// <summary>
    /// Dirs the specified VFS.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualDirectory.</returns>
    public IVirtualDirectory dir(IVirtualPathProvider vfs, string virtualPath) => vfs.GetDirectory(virtualPath);
    /// <summary>
    /// Dirs the exists.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dirExists(string virtualPath) => VirtualFiles.DirectoryExists(virtualPath);
    /// <summary>
    /// Dirs the exists.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dirExists(IVirtualPathProvider vfs, string virtualPath) => vfs.DirectoryExists(virtualPath);
    /// <summary>
    /// Dirs the file.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>IVirtualFile.</returns>
    public IVirtualFile dirFile(string dirPath, string fileName) => dirFile(VirtualFiles, dirPath, fileName);
    /// <summary>
    /// Dirs the file.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>IVirtualFile.</returns>
    public IVirtualFile dirFile(IVirtualPathProvider vfs, string dirPath, string fileName) => vfs.GetDirectory(dirPath)?.GetFile(fileName);
    /// <summary>
    /// Dirs the files.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> dirFiles(string dirPath) => dirFiles(VirtualFiles, dirPath);
    /// <summary>
    /// Dirs the files.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="dirPath">The dir path.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> dirFiles(IVirtualPathProvider vfs, string dirPath) => vfs.GetDirectory(dirPath)?.GetFiles() ?? new List<IVirtualFile>();
    /// <summary>
    /// Dirs the directory.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="dirName">Name of the dir.</param>
    /// <returns>IVirtualDirectory.</returns>
    public IVirtualDirectory dirDirectory(string dirPath, string dirName) => dirDirectory(VirtualFiles, dirPath, dirName);
    /// <summary>
    /// Dirs the directory.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="dirName">Name of the dir.</param>
    /// <returns>IVirtualDirectory.</returns>
    public IVirtualDirectory dirDirectory(IVirtualPathProvider vfs, string dirPath, string dirName) => vfs.GetDirectory(dirPath)?.GetDirectory(dirName);
    /// <summary>
    /// Dirs the directories.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
    public IEnumerable<IVirtualDirectory> dirDirectories(string dirPath) => dirDirectories(VirtualFiles, dirPath);
    /// <summary>
    /// Dirs the directories.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="dirPath">The dir path.</param>
    /// <returns>IEnumerable&lt;IVirtualDirectory&gt;.</returns>
    public IEnumerable<IVirtualDirectory> dirDirectories(IVirtualPathProvider vfs, string dirPath) => vfs.GetDirectory(dirPath)?.GetDirectories() ?? new List<IVirtualDirectory>();
    /// <summary>
    /// Finds the files in directory.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> findFilesInDirectory(string dirPath, string globPattern) => findFilesInDirectory(VirtualFiles, dirPath, globPattern);
    /// <summary>
    /// Finds the files in directory.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="dirPath">The dir path.</param>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> findFilesInDirectory(IVirtualPathProvider vfs, string dirPath, string globPattern) => vfs.GetDirectory(dirPath)?.GetAllMatchingFiles(globPattern);

    /// <summary>
    /// Finds the files.
    /// </summary>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> findFiles(string globPattern) => findFiles(VirtualFiles, globPattern);

    /// <summary>
    /// Dirs the find files.
    /// </summary>
    /// <param name="dir">The dir.</param>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> dirFindFiles(IVirtualDirectory dir, string globPattern) => dir.GetAllMatchingFiles(globPattern);
    /// <summary>
    /// Dirs the find files.
    /// </summary>
    /// <param name="dir">The dir.</param>
    /// <param name="globPattern">The glob pattern.</param>
    /// <param name="maxDepth">The maximum depth.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> dirFindFiles(IVirtualDirectory dir, string globPattern, int maxDepth) => dir.GetAllMatchingFiles(globPattern, maxDepth);
    /// <summary>
    /// Finds the files.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="globPattern">The glob pattern.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> findFiles(IVirtualPathProvider vfs, string globPattern) => vfs.GetAllMatchingFiles(globPattern);
    /// <summary>
    /// Finds the files.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="globPattern">The glob pattern.</param>
    /// <param name="maxDepth">The maximum depth.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    public IEnumerable<IVirtualFile> findFiles(IVirtualPathProvider vfs, string globPattern, int maxDepth) => vfs.GetAllMatchingFiles(globPattern, maxDepth);

    /// <summary>
    /// Files the exists.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool fileExists(string virtualPath) => fileExists(VirtualFiles, virtualPath);
    /// <summary>
    /// Files the exists.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool fileExists(IVirtualPathProvider vfs, string virtualPath) => vfs.FileExists(virtualPath);
    /// <summary>
    /// Files the specified virtual path.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualFile.</returns>
    public IVirtualFile file(string virtualPath) => file(VirtualFiles, virtualPath);
    /// <summary>
    /// Files the specified VFS.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>IVirtualFile.</returns>
    public IVirtualFile file(IVirtualPathProvider vfs, string virtualPath) => vfs.GetFile(virtualPath);
    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="contents">The contents.</param>
    /// <returns>System.String.</returns>
    public string writeFile(string virtualPath, object contents) => writeFile(VirtualFiles, virtualPath, contents);
    /// <summary>
    /// Writes the file.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="contents">The contents.</param>
    /// <returns>System.String.</returns>
    public string writeFile(IVirtualPathProvider vfs, string virtualPath, object contents)
    {
        vfs.WriteFile(virtualPath, contents);
        return virtualPath;
    }

    /// <summary>
    /// Writes the files.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="files">The files.</param>
    /// <returns>System.Object.</returns>
    public object writeFiles(IVirtualPathProvider vfs, Dictionary<string, object> files)
    {
        vfs.WriteFiles(files);
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Writes the text files.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="textFiles">The text files.</param>
    /// <returns>System.Object.</returns>
    public object writeTextFiles(IVirtualPathProvider vfs, Dictionary<string, string> textFiles)
    {
        vfs.WriteFiles(textFiles);
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Appends to file.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="contents">The contents.</param>
    /// <returns>System.String.</returns>
    public string appendToFile(string virtualPath, object contents) => appendToFile(VirtualFiles, virtualPath, contents);
    /// <summary>
    /// Appends to file.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="contents">The contents.</param>
    /// <returns>System.String.</returns>
    public string appendToFile(IVirtualPathProvider vfs, string virtualPath, object contents)
    {
        vfs.AppendFile(virtualPath, contents);

        return virtualPath;
    }

    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string deleteFile(string virtualPath) => deleteFile(VirtualFiles, virtualPath);
    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string deleteFile(IVirtualPathProvider vfs, string virtualPath)
    {
        vfs.DeleteFile(virtualPath);
        return virtualPath;
    }

    /// <summary>
    /// Deletes the directory.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string deleteDirectory(string virtualPath) => deleteFile(VirtualFiles, virtualPath);
    /// <summary>
    /// Deletes the directory.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string deleteDirectory(IVirtualPathProvider vfs, string virtualPath)
    {
        vfs.DeleteFolder(virtualPath);
        return virtualPath;
    }

    /// <summary>
    /// Files the text contents.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string fileTextContents(string virtualPath) => fileTextContents(VirtualFiles, virtualPath);
    /// <summary>
    /// Files the text contents.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string fileTextContents(IVirtualPathProvider vfs, string virtualPath) => vfs.GetFile(virtualPath)?.ReadAllText();

    /// <summary>
    /// Files the contents.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.Object.</returns>
    public object fileContents(IVirtualPathProvider vfs, string virtualPath) =>
        vfs.GetFile(virtualPath).GetContents();

    // string virtual filePath or IVirtualFile
    /// <summary>
    /// Files the contents.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>System.Object.</returns>
    public object fileContents(object file) => file is null
                                                   ? null
                                                   : file is string path
                                                       ? fileContents(VirtualFiles, path)
                                                       : file is IVirtualFile ifile
                                                           ? ifile.GetContents()
                                                           : throw new NotSupportedException(nameof(fileContents) + " expects string virtualPath or IVirtualFile but was " + file.GetType().Name);

    /// <summary>
    /// Texts the contents.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>System.String.</returns>
    public string textContents(IVirtualFile file) => file?.ReadAllText();
    /// <summary>
    /// Files the content of the bytes.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.Byte[].</returns>
    public byte[] fileBytesContent(string virtualPath) => fileBytesContent(VirtualFiles, virtualPath);
    /// <summary>
    /// Files the content of the bytes.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.Byte[].</returns>
    public byte[] fileBytesContent(IVirtualPathProvider vfs, string virtualPath) => vfs.GetFile(virtualPath)?.ReadAllBytes();
    /// <summary>
    /// Byteses the content.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>System.Byte[].</returns>
    public byte[] bytesContent(IVirtualFile file) => file?.ReadAllBytes();
    /// <summary>
    /// Files the hash.
    /// </summary>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string fileHash(string virtualPath) => fileHash(VirtualFiles, virtualPath);
    /// <summary>
    /// Files the hash.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <returns>System.String.</returns>
    public string fileHash(IVirtualPathProvider vfs, string virtualPath) => vfs.GetFileHash(virtualPath);
    /// <summary>
    /// Files the hash.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>System.String.</returns>
    public string fileHash(IVirtualFile file) => file?.GetFileHash();
    /// <summary>
    /// Files the is binary.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool fileIsBinary(IVirtualFile file) => MimeTypes.IsBinary(MimeTypes.GetMimeType(file.Extension));
    /// <summary>
    /// Files the type of the content.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>System.String.</returns>
    public string fileContentType(IVirtualFile file) => MimeTypes.GetMimeType(file.Extension);


    /// <summary>
    /// Converts the type of the data type to content.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <returns>System.String.</returns>
    private static string ConvertDataTypeToContentType(string dataType)
    {
        switch (dataType)
        {
            case "json":
                return MimeTypes.Json;
            case "jsv":
                return MimeTypes.Jsv;
            case "csv":
                return MimeTypes.Csv;
            case "xml":
                return MimeTypes.Xml;
            case "text":
                return MimeTypes.PlainText;
            case "form":
                return MimeTypes.FormUrlEncoded;
        }

        throw new NotSupportedException($"Unknown dataType '{dataType}'");
    }

    /// <summary>
    /// Converts the data to string.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <returns>System.String.</returns>
    private static string ConvertDataToString(object data, string contentType)
    {
        if (data is string s)
            return s;
        switch (contentType)
        {
            case MimeTypes.PlainText:
                return data.ToString();
            case MimeTypes.Json:
                return data.ToJson();
            case MimeTypes.Csv:
                return data.ToCsv();
            case MimeTypes.Jsv:
                return data.ToJsv();
            case MimeTypes.Xml:
                return data.ToXml();
            case MimeTypes.FormUrlEncoded:
                WriteComplexTypeDelegate holdQsStrategy = QueryStringStrategy.FormUrlEncoded;
                QueryStringSerializer.ComplexTypeStrategy = QueryStringStrategy.FormUrlEncoded;
                var urlEncodedBody = QueryStringSerializer.SerializeToString(data);
                QueryStringSerializer.ComplexTypeStrategy = holdQsStrategy;
                return urlEncodedBody;
        }

        throw new NotSupportedException($"Can not serialize to unknown Content-Type '{contentType}'");
    }

    /// <summary>
    /// Creates the cache key.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string CreateCacheKey(string url, Dictionary<string, object> options = null)
    {
        var sb = StringBuilderCache.Allocate()
            .Append(url);

        if (options != null)
        {
            foreach (var entry in options)
            {
                sb.Append(entry.Key)
                    .Append('=')
                    .Append(entry.Value);
            }
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Scripts the methods.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>List&lt;ScriptMethodInfo&gt;.</returns>
    public List<ScriptMethodInfo> scriptMethods(ScriptScopeContext scope)
    {
        var methods = scope.Context.ScriptMethods.SelectMany(x => ScriptMethodInfo.GetScriptMethods(x.GetType()))
            .OrderBy(x => x.Name)
            .ThenBy(x => x.ParamCount)
            .ToList();
        return methods;
    }

    /// <summary>
    /// Scripts the method names.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> scriptMethodNames(ScriptScopeContext scope) => scriptMethods(scope)
        .Map(x => x.Name);

    /// <summary>
    /// Scripts the method signatures.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> scriptMethodSignatures(ScriptScopeContext scope) => scriptMethods(scope)
        .Map(x => x.Signature);

    /// <summary>
    /// Filters the methods.
    /// </summary>
    /// <param name="methodInfos">The method infos.</param>
    /// <returns>ScriptMethodInfo[].</returns>
    private ScriptMethodInfo[] filterMethods(MethodInfo[] methodInfos) =>
        methodInfos.Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object))
            .Select(ScriptMethodInfo.Create).ToArray();
    /// <summary>
    /// Methodses the specified o.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> methods(object o)
    {
        if (o == null)
            return TypeConstants.EmptyStringList;

        var mis = methodTypes(o);
        return mis.Map(x => x.Name).OrderBy(x => x).ToList();
    }

    /// <summary>
    /// Methods the types.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>ScriptMethodInfo[].</returns>
    public ScriptMethodInfo[] methodTypes(object o)
    {
        if (o == null)
            return TypeConstants<ScriptMethodInfo>.EmptyArray;

        var type = o as Type ?? o.GetType();

        return filterMethods(type.GetInstanceMethods());
    }

    /// <summary>
    /// Statics the methods.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> staticMethods(object o)
    {
        if (o == null)
            return TypeConstants.EmptyStringList;

        var mis = staticMethodTypes(o);
        return mis.Map(x => x.Name).OrderBy(x => x).ToList();
    }

    /// <summary>
    /// Statics the method types.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>ScriptMethodInfo[].</returns>
    public ScriptMethodInfo[] staticMethodTypes(object o)
    {
        if (o == null)
            return TypeConstants<ScriptMethodInfo>.EmptyArray;

        var type = o as Type ?? o.GetType();

        return filterMethods(type.GetMethods(BindingFlags.Static | BindingFlags.Public));
    }

    /// <summary>
    /// Alls the method types.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>ScriptMethodInfo[].</returns>
    public ScriptMethodInfo[] allMethodTypes(object o)
    {
        if (o == null)
            return TypeConstants<ScriptMethodInfo>.EmptyArray;

        var type = o as Type ?? o.GetType();

        return type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(ScriptMethodInfo.Create).ToArray();
    }

    /// <summary>
    /// Alls the member infos.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>MemberInfo[].</returns>
    public MemberInfo[] allMemberInfos(object o)
    {
        if (o == null)
            return TypeConstants<MemberInfo>.EmptyArray;

        var type = o as Type ?? o.GetType();

        return type.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    /// <summary>
    /// All cache names
    /// </summary>
    static readonly string[] AllCacheNames = {
                                                     nameof(ScriptContext.Cache),
                                                     nameof(ScriptContext.CacheMemory),
                                                     nameof(ScriptContext.ExpiringCache),
                                                     nameof(ScriptTemplateUtils.BinderCache),
                                                     nameof(ScriptContext.JsTokenCache),
                                                     nameof(ScriptContext.AssignExpressionCache),
                                                     nameof(ScriptContext.PathMappings),
                                                 };

    /// <summary>
    /// Gets the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache.</param>
    /// <returns>IDictionary.</returns>
    internal IDictionary GetCache(string cacheName)
    {
        switch (cacheName)
        {
            case nameof(ScriptContext.Cache):
                return Context.Cache;
            case nameof(ScriptContext.CacheMemory):
                return Context.CacheMemory;
            case nameof(ScriptContext.ExpiringCache):
                return Context.ExpiringCache;
            case nameof(ScriptTemplateUtils.BinderCache):
                return ScriptTemplateUtils.BinderCache;
            case nameof(ScriptContext.JsTokenCache):
                return Context.JsTokenCache;
            case nameof(ScriptContext.AssignExpressionCache):
                return Context.AssignExpressionCache;
            case nameof(ScriptContext.PathMappings):
                return Context.PathMappings;
        }
        return null;
    }

    /// <summary>
    /// Caches the clear.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="cacheNames">The cache names.</param>
    /// <returns>System.Object.</returns>
    public object cacheClear(ScriptScopeContext scope, object cacheNames)
    {
        IEnumerable<string> caches = cacheNames switch {
            string strName => strName.EqualsIgnoreCase("all") ? AllCacheNames : new[] {strName},
            IEnumerable<string> nameList => nameList,
            _ => throw new NotSupportedException(
                     $"{nameof(cacheClear)} expects a cache name or list of cache names but received: {cacheNames.GetType().Name}")
        };

        int entriesRemoved = 0;
        foreach (var cacheName in caches)
        {
            var cache = GetCache(cacheName);
            if (cache == null)
                throw new NotSupportedException(nameof(cacheClear) + $": Unknown cache '{cacheName}'");

            entriesRemoved += cache.Count;
            cache.Clear();
        }

        return entriesRemoved;
    }

    /// <summary>
    /// Invalidates all caches.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object invalidateAllCaches(ScriptScopeContext scope)
    {
        cacheClear(scope, "all");
        return scope.Context.InvalidateCachesBefore = DateTime.UtcNow;
    }

    /// <summary>
    /// Shes the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>System.String.</returns>
    public string sh(ScriptScopeContext scope, string arguments) => sh(scope, arguments, null);
    /// <summary>
    /// Shes the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public string sh(ScriptScopeContext scope, string arguments, Dictionary<string, object> options)
    {
        if (string.IsNullOrEmpty(arguments))
            return null;

        options ??= new Dictionary<string, object>();

        if (Env.IsWindows)
        {
            options["arguments"] = "/C " + arguments;
            return proc(scope, "cmd.exe", options);
        }
        else
        {
            var escapedArgs = arguments.Replace("\"", "\\\"");
            options["arguments"] = $"-c \"{escapedArgs}\"";
            return proc(scope, "/bin/bash", options);
        }
    }

    /// <summary>
    /// Procs the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>System.String.</returns>
    public string proc(ScriptScopeContext scope, string fileName) => proc(scope, fileName, null);
    /// <summary>
    /// Procs the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public string proc(ScriptScopeContext scope, string fileName, Dictionary<string, object> options)
    {
        var process = new Process
                          {
                              StartInfo =
                                  {
                                      FileName = fileName,
                                      UseShellExecute = false,
                                      CreateNoWindow = true,
                                      RedirectStandardOutput = true,
                                  }
                          };

        if (options.TryGetValue("arguments", out var oArguments))
            process.StartInfo.Arguments = oArguments.AsString();

        if (options.TryGetValue("dir", out var oWorkDir))
            process.StartInfo.WorkingDirectory = oWorkDir.AsString();

        try
        {
            using (process)
            {
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                process.WaitForExit();
                process.Close();

                if (!string.IsNullOrEmpty(error))
                    throw new Exception($"`{fileName} {process.StartInfo.Arguments}` command failed: " + error);

                return output;
            }
        }
        catch (Exception ex)
        {
            throw new StopFilterExecutionException(scope, options, ex);
        }
    }

    /// <summary>
    /// Executables the path.
    /// </summary>
    /// <param name="exeName">Name of the executable.</param>
    /// <returns>System.String.</returns>
    public string exePath(string exeName)
    {
        try
        {
            var p = new Process
                        {
                            StartInfo =
                                {
                                    UseShellExecute = false,
                                    FileName = Env.IsWindows
                                                   ? "where"  //Win 7/Server 2003+
                                                   : "which", //macOS / Linux
                                    Arguments = exeName,
                                    RedirectStandardOutput = true
                                }
                        };
            p.Start();
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (p.ExitCode == 0)
            {
                // just return first match
                var fullPath = output.Substring(0, output.IndexOf(Environment.NewLine, StringComparison.Ordinal));
                if (!string.IsNullOrEmpty(fullPath))
                {
                    return fullPath;
                }
            }
        }
        catch
        {
            // ignored
        }
        return null;
    }

    /// <summary>
    /// Exits the specified exit code.
    /// </summary>
    /// <param name="exitCode">The exit code.</param>
    /// <returns>StopExecution.</returns>
    public StopExecution exit(int exitCode)
    {
        Environment.Exit(exitCode);
        return StopExecution.Value;
    }

    /// <summary>
    /// Inspects the vars.
    /// </summary>
    /// <param name="vars">The vars.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult inspectVars(object vars)
    {
        Inspect.vars(vars);
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Checks the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    private static string check(string target) =>
        string.IsNullOrWhiteSpace(target?.Replace("\"", "")) ? null : target;
    /// <summary>
    /// Winpathes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    private static string winpath(string path) => path?.Replace('/', '\\');
    /// <summary>
    /// Unixpathes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    private static string unixpath(string path) => path?.Replace('\\', '/');

    /// <summary>
    /// Mvs the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>System.String.</returns>
    public string mv(ScriptScopeContext scope, string from, string to)
    {
        _ = check(from) ?? throw new ArgumentNullException(nameof(from));
        _ = check(to) ?? throw new ArgumentNullException(nameof(to));
        return Env.IsWindows
                   ? sh(scope, $"MOVE /Y {winpath(from)} {winpath(to)}")
                   : sh(scope, $"mv -f {unixpath(from)} {unixpath(to)}");
    }
    /// <summary>
    /// Cps the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>System.String.</returns>
    public string cp(ScriptScopeContext scope, string from, string to)
    {
        _ = check(from) ?? throw new ArgumentNullException(nameof(from));
        _ = check(to) ?? throw new ArgumentNullException(nameof(to));
        return Env.IsWindows
                   ? sh(scope, $"COPY /Y {winpath(from)} {winpath(to)}")
                   : sh(scope, $"cp -f {unixpath(from)} {unixpath(to)}");
    }
    /// <summary>
    /// Xcopies the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>System.String.</returns>
    public string xcopy(ScriptScopeContext scope, string from, string to)
    {
        _ = check(from) ?? throw new ArgumentNullException(nameof(from));
        _ = check(to) ?? throw new ArgumentNullException(nameof(to));
        return Env.IsWindows
                   ? sh(scope, $"XCOPY /E /H {winpath(from)} {winpath(to)}")
                   : sh(scope, $"cp -R {unixpath(from)} {unixpath(to)}");
    }
    /// <summary>
    /// Rms the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>System.String.</returns>
    public string rm(ScriptScopeContext scope, string from, string to)
    {
        _ = check(from) ?? throw new ArgumentNullException(nameof(from));
        _ = check(to) ?? throw new ArgumentNullException(nameof(to));
        return Env.IsWindows
                   ? sh(scope, $"DEL /y {winpath(from)} {winpath(to)}")
                   : sh(scope, $"rm -f {unixpath(from)} {unixpath(to)}");
    }
    /// <summary>
    /// Rmdirs the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string rmdir(ScriptScopeContext scope, string target)
    {
        _ = check(target) ?? throw new ArgumentNullException(nameof(target));
        return Env.IsWindows
                   ? sh(scope, $"RMDIR /Q /S {winpath(target)}")
                   : sh(scope, $"rm -rf {unixpath(target)}");
    }
    /// <summary>
    /// Mkdirs the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string mkdir(ScriptScopeContext scope, string target)
    {
        _ = check(target) ?? throw new ArgumentNullException(nameof(target));
        return Env.IsWindows
                   ? sh(scope, $"MKDIR {winpath(target)}")
                   : sh(scope, $"mkdir -p {unixpath(target)}");
    }
    /// <summary>
    /// Cats the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string cat(ScriptScopeContext scope, string target)
    {
        _ = check(target) ?? throw new ArgumentNullException(nameof(target));
        return Env.IsWindows
                   ? sh(scope, $"type {winpath(target)}")
                   : sh(scope, $"cat {unixpath(target)}");
    }
    /// <summary>
    /// Touches the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string touch(ScriptScopeContext scope, string target)
    {
        _ = check(target) ?? throw new ArgumentNullException(nameof(target));
        return Env.IsWindows
                   ? sh(scope, $"CALL >> {winpath(target)}")
                   : sh(scope, $"touch {unixpath(target)}");
    }

    /// <summary>
    /// Files this instance.
    /// </summary>
    /// <returns>FileScripts.</returns>
    public FileScripts File() => new();
    /// <summary>
    /// Directories this instance.
    /// </summary>
    /// <returns>DirectoryScripts.</returns>
    public DirectoryScripts Directory() => new();

    /// <summary>
    /// Hexadecimals the hash.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="s">The s.</param>
    /// <returns>System.String.</returns>
    static string HexHash(HashAlgorithm hash, string s) => HexHash(hash, s.ToUtf8Bytes());
    /// <summary>
    /// Hexadecimals the hash.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="bytes">The bytes.</param>
    /// <returns>System.String.</returns>
    static string HexHash(HashAlgorithm hash, byte[] bytes)
    {
        using var _ = hash;
        return bytes == null || bytes.Length == 0 ? null : _.ComputeHash(bytes).ToHex();
    }
  
    /// <summary>
    /// Sha256s the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string sha256(object target) => target is string s
                                               ? HexHash(SHA256.Create(), s)
                                               : target is byte[] b
                                                   ? HexHash(SHA256.Create(), b)
                                                   : throw new NotSupportedException(target?.GetType().Name);
    /// <summary>
    /// Sha512s the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string sha512(object target) => target is string s
                                               ? HexHash(SHA512.Create(), s)
                                               : target is byte[] b
                                                   ? HexHash(SHA512.Create(), b)
                                                   : throw new NotSupportedException(target?.GetType().Name);

    /// <summary>
    /// Deletes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Delete(string path) => System.IO.File.Exists(path)
                                                   ? File().Delete(path)
                                                   : System.IO.Directory.Exists(path)
                                                       ? Directory().Delete(path)
                                                       : IgnoreResult.Value;
    /// <summary>
    /// Deletes the specified os.
    /// </summary>
    /// <param name="os">The os.</param>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Delete(IOScript os, string path) => os.Delete(path);

    /// <summary>
    /// Existses the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Exists(string path) => System.IO.File.Exists(path) || System.IO.Directory.Exists(path);
    /// <summary>
    /// Existses the specified os.
    /// </summary>
    /// <param name="os">The os.</param>
    /// <param name="path">The path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Exists(IOScript os, string path) => os.Exists(path);

    /// <summary>
    /// Moves the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Move(string from, string to) => System.IO.File.Exists(from)
                                                            ? File().Move(from, to)
                                                            : System.IO.Directory.Exists(from)
                                                                ? Directory().Move(from, to)
                                                                : IgnoreResult.Value;
    /// <summary>
    /// Moves the specified os.
    /// </summary>
    /// <param name="os">The os.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Move(IOScript os, string from, string to) => os.Move(from, to);

    /// <summary>
    /// Copies the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Copy(string from, string to) => System.IO.File.Exists(from)
                                                            ? File().Copy(from, to)
                                                            : System.IO.Directory.Exists(from)
                                                                ? Directory().Copy(from, to)
                                                                : IgnoreResult.Value;
    /// <summary>
    /// Copies the specified os.
    /// </summary>
    /// <param name="os">The os.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Copy(IOScript os, string from, string to) => os.Copy(from, to);

    /// <summary>
    /// Creates the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Create(string from, string to) => File().Copy(from, to);
    /// <summary>
    /// Creates the specified fs.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Create(FileScripts fs, string from, string to) => fs.Copy(from, to);

    /// <summary>
    /// Replaces the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="backup">The backup.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Replace(string from, string to, string backup) => File().Replace(from, to, backup);
    /// <summary>
    /// Replaces the specified fs.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="backup">The backup.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Replace(FileScripts fs, string from, string to, string backup) => fs.Replace(from, to, backup);

    /// <summary>
    /// Reads all bytes.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.Byte[].</returns>
    public byte[] ReadAllBytes(string path) => File().ReadAllBytes(path);
    /// <summary>
    /// Reads all bytes.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <returns>System.Byte[].</returns>
    public byte[] ReadAllBytes(FileScripts fs, string path) => fs.ReadAllBytes(path);
    /// <summary>
    /// Reads all lines.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] ReadAllLines(string path) => File().ReadAllLines(path);
    /// <summary>
    /// Reads all lines.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] ReadAllLines(FileScripts fs, string path) => fs.ReadAllLines(path);
    /// <summary>
    /// Reads all text.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public string ReadAllText(string path) => File().ReadAllText(path);
    /// <summary>
    /// Reads all text.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public string ReadAllText(FileScripts fs, string path) => fs.ReadAllText(path);

    /// <summary>
    /// Writes all bytes.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllBytes(string path, byte[] bytes) => File().WriteAllBytes(path, bytes);
    /// <summary>
    /// Writes all bytes.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllBytes(FileScripts fs, string path, byte[] bytes) => fs.WriteAllBytes(path, bytes);
    /// <summary>
    /// Writes all lines.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="lines">The lines.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllLines(string path, string[] lines) => File().WriteAllLines(path, lines);
    /// <summary>
    /// Writes all lines.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <param name="lines">The lines.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllLines(FileScripts fs, string path, string[] lines) => fs.WriteAllLines(path, lines);
    /// <summary>
    /// Writes all text.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllText(string path, string text) => File().WriteAllText(path, text);
    /// <summary>
    /// Writes all text.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllText(FileScripts fs, string path, string text) => fs.WriteAllText(path, text);

    /// <summary>
    /// Appends all lines.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="lines">The lines.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult AppendAllLines(string path, string[] lines) => File().AppendAllLines(path, lines);
    /// <summary>
    /// Appends all lines.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <param name="lines">The lines.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult AppendAllLines(FileScripts fs, string path, string[] lines) => fs.AppendAllLines(path, lines);
    /// <summary>
    /// Appends all text.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult AppendAllText(string path, string text) => File().AppendAllText(path, text);
    /// <summary>
    /// Appends all text.
    /// </summary>
    /// <param name="fs">The fs.</param>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult AppendAllText(FileScripts fs, string path, string text) => fs.AppendAllText(path, text);

    /// <summary>
    /// Creates the directory.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult CreateDirectory(string path) => Directory().CreateDirectory(path);
    /// <summary>
    /// Creates the directory.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult CreateDirectory(DirectoryScripts ds, string path) => ds.CreateDirectory(path);

    /// <summary>
    /// Gets the directories.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] GetDirectories(string path) => Directory().GetDirectories(path);
    /// <summary>
    /// Gets the directories.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] GetDirectories(DirectoryScripts ds, string path) => ds.GetDirectories(path);
    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] GetFiles(string path) => Directory().GetFiles(path);
    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] GetFiles(DirectoryScripts ds, string path) => ds.GetFiles(path);
    /// <summary>
    /// Gets the logical drives.
    /// </summary>
    /// <returns>System.String[].</returns>
    public string[] GetLogicalDrives() => Directory().GetLogicalDrives();
    /// <summary>
    /// Gets the logical drives.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <returns>System.String[].</returns>
    public string[] GetLogicalDrives(DirectoryScripts ds) => ds.GetLogicalDrives();
    /// <summary>
    /// Gets the current directory.
    /// </summary>
    /// <returns>System.String.</returns>
    public string GetCurrentDirectory() => Directory().GetCurrentDirectory();
    /// <summary>
    /// Gets the current directory.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <returns>System.String.</returns>
    public string GetCurrentDirectory(DirectoryScripts ds) => ds.GetCurrentDirectory();
    /// <summary>
    /// Gets the directory root.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public string GetDirectoryRoot(string path) => Directory().GetDirectoryRoot(path);
    /// <summary>
    /// Gets the directory root.
    /// </summary>
    /// <param name="ds">The ds.</param>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public string GetDirectoryRoot(DirectoryScripts ds, string path) => ds.GetDirectoryRoot(path);
}

/// <summary>
/// Interface IOScript
/// </summary>
public interface IOScript
{
    /// <summary>
    /// Deletes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    IgnoreResult Delete(string path);
    /// <summary>
    /// Existses the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool Exists(string target);
    /// <summary>
    /// Moves the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    IgnoreResult Move(string from, string to);
    /// <summary>
    /// Copies the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    IgnoreResult Copy(string from, string to);
}
/// <summary>
/// Class FileScripts.
/// Implements the <see cref="ServiceStack.Script.IOScript" />
/// </summary>
/// <seealso cref="ServiceStack.Script.IOScript" />
public class FileScripts : IOScript
{
    /// <summary>
    /// Copies the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Copy(string from, string to)
    {
        File.Copy(from, to);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Creates the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Create(string path)
    {
        using var _ = File.Create(path);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Deletes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Delete(string path)
    {
        File.Delete(path);
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Existses the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Exists(string path) => File.Exists(path);
    /// <summary>
    /// Moves the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Move(string from, string to)
    {
        File.Move(from, to);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Replaces the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="backup">The backup.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Replace(string from, string to, string backup)
    {
        File.Replace(from, to, backup);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Reads all bytes.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.Byte[].</returns>
    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
    /// <summary>
    /// Reads all lines.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] ReadAllLines(string path) => File.ReadAllLines(path);
    /// <summary>
    /// Reads all text.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public string ReadAllText(string path) => File.ReadAllText(path);
    /// <summary>
    /// Writes all bytes.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllBytes(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Writes all lines.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="lines">The lines.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllLines(string path, string[] lines)
    {
        File.WriteAllLines(path, lines);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Writes all text.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult WriteAllText(string path, string text)
    {
        File.WriteAllText(path, text);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Appends all lines.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="lines">The lines.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult AppendAllLines(string path, string[] lines)
    {
        File.AppendAllLines(path, lines);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Appends all text.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult AppendAllText(string path, string text)
    {
        File.AppendAllText(path, text);
        return IgnoreResult.Value;
    }
}
/// <summary>
/// Class DirectoryScripts.
/// Implements the <see cref="ServiceStack.Script.IOScript" />
/// </summary>
/// <seealso cref="ServiceStack.Script.IOScript" />
public class DirectoryScripts : IOScript
{
    /// <summary>
    /// Creates the directory.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Deletes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Delete(string path)
    {
        FileSystemVirtualFiles.DeleteDirectoryRecursive(path);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Existses the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Exists(string path) => Directory.Exists(path);
    /// <summary>
    /// Gets the directories.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] GetDirectories(string path) => Directory.GetDirectories(path);
    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] GetFiles(string path) => Directory.GetFiles(path);
    /// <summary>
    /// Gets the logical drives.
    /// </summary>
    /// <returns>System.String[].</returns>
    public string[] GetLogicalDrives() => Directory.GetLogicalDrives();
    /// <summary>
    /// Gets the file system entries.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String[].</returns>
    public string[] GetFileSystemEntries(string path) => Directory.GetFileSystemEntries(path);
    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>DirectoryInfo.</returns>
    public DirectoryInfo GetParent(string path) => Directory.GetParent(path);
    /// <summary>
    /// Gets the current directory.
    /// </summary>
    /// <returns>System.String.</returns>
    public string GetCurrentDirectory() => Directory.GetCurrentDirectory();
    /// <summary>
    /// Gets the directory root.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public string GetDirectoryRoot(string path) => Directory.GetDirectoryRoot(path);
    /// <summary>
    /// Moves the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Move(string from, string to)
    {
        Directory.Move(from, to);
        return IgnoreResult.Value;
    }
    /// <summary>
    /// Copies the specified from.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult Copy(string from, string to)
    {
        CopyAllTo(from, to);
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Copies all to.
    /// </summary>
    /// <param name="src">The source.</param>
    /// <param name="dst">The DST.</param>
    /// <param name="excludePaths">The exclude paths.</param>
    public static void CopyAllTo(string src, string dst, string[] excludePaths = null)
    {
        var d = Path.DirectorySeparatorChar;

        foreach (string dirPath in Directory.GetDirectories(src, "*.*", SearchOption.AllDirectories))
        {
            if (!excludePaths.IsEmpty() && excludePaths?.Any(x => dirPath.StartsWith(x)) == true)
                continue;

            try { Directory.CreateDirectory(dirPath.Replace(src, dst)); }
            catch
            {
                // ignored
            }
        }

        foreach (string newPath in Directory.GetFiles(src, "*.*", SearchOption.AllDirectories))
        {
            if (!excludePaths.IsEmpty() && excludePaths?.Any(x => newPath.StartsWith(x)) == true)
                continue;
            try
            {
                File.Copy(newPath, newPath.Replace(src, dst), overwrite: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}