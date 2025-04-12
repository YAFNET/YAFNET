// ***********************************************************************
// <copyright file="ReflectionExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text.Support;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Delegate EmptyCtorFactoryDelegate
/// </summary>
/// <param name="type">The type.</param>
/// <returns>EmptyCtorDelegate.</returns>
public delegate EmptyCtorDelegate EmptyCtorFactoryDelegate(Type type);

/// <summary>
/// Delegate EmptyCtorDelegate
/// </summary>
/// <returns>System.Object.</returns>
public delegate object EmptyCtorDelegate();

/// <summary>
/// Class ReflectionExtensions.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the type code.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>TypeCode.</returns>
    public static TypeCode GetTypeCode(this Type type)
    {
        return Type.GetTypeCode(type);
    }

    /// <summary>
    /// Determines whether [is instance of] [the specified this or base type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="thisOrBaseType">Type of the this or base.</param>
    /// <returns><c>true</c> if [is instance of] [the specified this or base type]; otherwise, <c>false</c>.</returns>
    public static bool IsInstanceOf(this Type type, Type thisOrBaseType)
    {
        while (type != null)
        {
            if (type == thisOrBaseType)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Determines whether [has generic type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if [has generic type] [the specified type]; otherwise, <c>false</c>.</returns>
    public static bool HasGenericType(this Type type)
    {
        while (type != null)
        {
            if (type.IsGenericType)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Firsts the type of the generic.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    public static Type FirstGenericType(this Type type)
    {
        while (type != null)
        {
            if (type.IsGenericType)
            {
                return type;
            }

            type = type.BaseType;
        }

        return null;
    }

    /// <summary>
    /// Gets the type with generic type definition of any.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericTypeDefinitions">The generic type definitions.</param>
    /// <returns>Type.</returns>
    public static Type GetTypeWithGenericTypeDefinitionOfAny(this Type type, params Type[] genericTypeDefinitions)
    {
        foreach (var genericTypeDefinition in genericTypeDefinitions)
        {
            var genericType = type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition);
            if (genericType == null && type == genericTypeDefinition)
            {
                genericType = type;
            }

            if (genericType != null)
            {
                return genericType;
            }
        }

        return null;
    }

    /// <summary>
    /// Determines whether [is or has generic interface type of] [the specified generic type definition].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericTypeDefinition">The generic type definition.</param>
    /// <returns><c>true</c> if [is or has generic interface type of] [the specified generic type definition]; otherwise, <c>false</c>.</returns>
    public static bool IsOrHasGenericInterfaceTypeOf(this Type type, Type genericTypeDefinition)
    {
        return type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition) != null
               || type == genericTypeDefinition;
    }

    /// <summary>
    /// Gets the type with generic type definition of.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericTypeDefinition">The generic type definition.</param>
    /// <returns>Type.</returns>
    public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
    {
        var t = Array.Find(type
            .GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition);

        if (t != null)
        {
            return t;
        }

        var genericType = type.FirstGenericType();
        if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
        {
            return genericType;
        }

        return null;
    }

    /// <summary>
    /// Determines whether the specified interface type has interface.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="interfaceType">Type of the interface.</param>
    /// <returns><c>true</c> if the specified interface type has interface; otherwise, <c>false</c>.</returns>
    public static bool HasInterface(this Type type, Type interfaceType)
    {
        return type.IsAssignableTo(interfaceType);
    }

    /// <summary>
    /// Determines whether [is nullable type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.</returns>
    public static bool IsNullableType(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Gets the underlying type code.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>TypeCode.</returns>
    public static TypeCode GetUnderlyingTypeCode(this Type type)
    {
        return GetTypeCode(Nullable.GetUnderlyingType(type) ?? type);
    }

    /// <summary>
    /// Determines whether [is numeric type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if [is numeric type] [the specified type]; otherwise, <c>false</c>.</returns>
    public static bool IsNumericType(this Type type)
    {
        while (true)
        {
            if (type == null)
            {
                return false;
            }

            if (type.IsEnum) //TypeCode can be TypeCode.Int32
            {
                return JsConfig.TreatEnumAsInteger || type.IsEnumFlags();
            }

            switch (GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                case TypeCode.Object:
                    if (type.IsNullableType())
                    {
                        type = Nullable.GetUnderlyingType(type);
                        continue;
                    }

                    if (type.IsEnum)
                    {
                        return JsConfig.TreatEnumAsInteger || type.IsEnumFlags();
                    }

                    return false;
            }

            return false;
        }
    }

    /// <summary>
    /// Determines whether [is integer type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if [is integer type] [the specified type]; otherwise, <c>false</c>.</returns>
    public static bool IsIntegerType(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        return GetTypeCode(type) switch
        {
            TypeCode.Byte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 => true,
            TypeCode.Object => type.IsNullableType() && IsNumericType(Nullable.GetUnderlyingType(type)),
            _ => false
        };
    }

    /// <summary>
    /// Determines whether [is real number type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if [is real number type] [the specified type]; otherwise, <c>false</c>.</returns>
    public static bool IsRealNumberType(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        return GetTypeCode(type) switch {
            TypeCode.Decimal => true,
            TypeCode.Double => true,
            TypeCode.Single => true,
            TypeCode.Object => type.IsNullableType() && IsNumericType(Nullable.GetUnderlyingType(type)),
            _ => false
        };
    }

    /// <summary>
    /// Gets the type with generic interface of.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericInterfaceType">Type of the generic interface.</param>
    /// <returns>Type.</returns>
    public static Type GetTypeWithGenericInterfaceOf(this Type type, Type genericInterfaceType)
    {
        var t =
            Array.Find(type.GetInterfaces(),
                t => t.IsGenericType && t.GetGenericTypeDefinition() == genericInterfaceType);

        if (t != null)
        {
            return t;
        }

        if (!type.IsGenericType)
        {
            return null;
        }

        var genericType = type.FirstGenericType();
        return genericType.GetGenericTypeDefinition() == genericInterfaceType
                   ? genericType
                   : null;
    }

    /// <summary>
    /// Determines whether [has any type definitions of] [the specified these generic types].
    /// </summary>
    /// <param name="genericType">Type of the generic.</param>
    /// <param name="theseGenericTypes">The these generic types.</param>
    /// <returns><c>true</c> if [has any type definitions of] [the specified these generic types]; otherwise, <c>false</c>.</returns>
    public static bool HasAnyTypeDefinitionsOf(this Type genericType, params Type[] theseGenericTypes)
    {
        if (!genericType.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = genericType.GetGenericTypeDefinition();

        return theseGenericTypes.Exists(thisGenericType => genericTypeDefinition == thisGenericType);
    }

    /// <summary>
    /// Gets the generic arguments if both have same generic definition type and arguments.
    /// </summary>
    /// <param name="assignableFromType">Type of the assignable from.</param>
    /// <param name="typeA">The type a.</param>
    /// <param name="typeB">The type b.</param>
    /// <returns>Type[].</returns>
    public static Type[] GetGenericArgumentsIfBothHaveSameGenericDefinitionTypeAndArguments(
        this Type assignableFromType, Type typeA, Type typeB)
    {
        var typeAInterface = typeA.GetTypeWithGenericInterfaceOf(assignableFromType);
        if (typeAInterface == null)
        {
            return null;
        }

        var typeBInterface = typeB.GetTypeWithGenericInterfaceOf(assignableFromType);
        if (typeBInterface == null)
        {
            return null;
        }

        var typeAGenericArgs = typeAInterface.GetGenericArguments();
        var typeBGenericArgs = typeBInterface.GetGenericArguments();

        if (typeAGenericArgs.Length != typeBGenericArgs.Length)
        {
            return null;
        }

        return typeBGenericArgs.Where((t, i) => typeAGenericArgs[i] != t).Any() ? null : typeAGenericArgs;
    }

    /// <summary>
    /// Gets the generic arguments if both have convertible generic definition type and arguments.
    /// </summary>
    /// <param name="assignableFromType">Type of the assignable from.</param>
    /// <param name="typeA">The type a.</param>
    /// <param name="typeB">The type b.</param>
    /// <returns>TypePair.</returns>
    public static TypePair GetGenericArgumentsIfBothHaveConvertibleGenericDefinitionTypeAndArguments(
        this Type assignableFromType, Type typeA, Type typeB)
    {
        var typeAInterface = typeA.GetTypeWithGenericInterfaceOf(assignableFromType);
        if (typeAInterface == null)
        {
            return null;
        }

        var typeBInterface = typeB.GetTypeWithGenericInterfaceOf(assignableFromType);
        if (typeBInterface == null)
        {
            return null;
        }

        var typeAGenericArgs = typeAInterface.GetGenericArguments();
        var typeBGenericArgs = typeBInterface.GetGenericArguments();

        if (typeAGenericArgs.Length != typeBGenericArgs.Length)
        {
            return null;
        }

        return typeBGenericArgs.Where((t, i) => !AreAllStringOrValueTypes(typeAGenericArgs[i], t)).Any() ? null : new TypePair(typeAGenericArgs, typeBGenericArgs);
    }

    /// <summary>
    /// Ares all string or value types.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool AreAllStringOrValueTypes(params Type[] types)
    {
        return Array.TrueForAll(types,type => type == typeof(string) || type.IsValueType);
    }

    /// <summary>
    /// The constructor methods
    /// </summary>
    private static Dictionary<Type, EmptyCtorDelegate> ConstructorMethods = [];

    /// <summary>
    /// The type names map
    /// </summary>
    private static Dictionary<string, EmptyCtorDelegate> TypeNamesMap = [];

    /// <summary>
    /// Gets the constructor method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>EmptyCtorDelegate.</returns>
    public static EmptyCtorDelegate GetConstructorMethod(Type type)
    {
        if (ConstructorMethods.TryGetValue(type, out var emptyCtorFn))
        {
            return emptyCtorFn;
        }

        emptyCtorFn = GetConstructorMethodToCache(type);

        Dictionary<Type, EmptyCtorDelegate> snapshot, newCache;
        do
        {
            snapshot = ConstructorMethods;
            newCache = new Dictionary<Type, EmptyCtorDelegate>(ConstructorMethods) {
                           [type] = emptyCtorFn
                       };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ConstructorMethods, newCache, snapshot), snapshot));

        return emptyCtorFn;
    }

    /// <summary>
    /// Gets the constructor method.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>EmptyCtorDelegate.</returns>
    public static EmptyCtorDelegate GetConstructorMethod(string typeName)
    {
        if (TypeNamesMap.TryGetValue(typeName, out var emptyCtorFn))
        {
            return emptyCtorFn;
        }

        var type = JsConfig.TypeFinder(typeName);
        if (type == null)
        {
            return null;
        }

        emptyCtorFn = GetConstructorMethodToCache(type);

        Dictionary<string, EmptyCtorDelegate> snapshot, newCache;
        do
        {
            snapshot = TypeNamesMap;
            newCache = new Dictionary<string, EmptyCtorDelegate>(TypeNamesMap)
                           {
                               [typeName] = emptyCtorFn
                           };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref TypeNamesMap, newCache, snapshot), snapshot));

        return emptyCtorFn;
    }

    /// <summary>
    /// Gets the constructor method to cache.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>EmptyCtorDelegate.</returns>
    public static EmptyCtorDelegate GetConstructorMethodToCache(Type type)
    {
        if (type == typeof(string))
        {
            return () => string.Empty;
        }

        if (type.IsInterface)
        {
            if (type.HasGenericType())
            {
                var genericType = type.GetTypeWithGenericTypeDefinitionOfAny(
                    typeof(IDictionary<,>));

                if (genericType != null)
                {
                    var keyType = genericType.GetGenericArguments()[0];
                    var valueType = genericType.GetGenericArguments()[1];
                    return GetConstructorMethodToCache(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
                }

                genericType = type.GetTypeWithGenericTypeDefinitionOfAny(
                    typeof(IEnumerable<>),
                    typeof(ICollection<>),
                    typeof(IList<>));

                if (genericType != null)
                {
                    var elementType = genericType.GetGenericArguments()[0];
                    return GetConstructorMethodToCache(typeof(List<>).MakeGenericType(elementType));
                }
            }
        }
        else if (type.IsArray)
        {
            return () => Array.CreateInstance(type.GetElementType(), 0);
        }
        else if (type.IsGenericTypeDefinition)
        {
            var genericArgs = type.GetGenericArguments();
            var typeArgs = new Type[genericArgs.Length];
            for (var i = 0; i < genericArgs.Length; i++)
            {
                typeArgs[i] = typeof(object);
            }

            var realizedType = type.MakeGenericType(typeArgs);

            return realizedType.CreateInstance;
        }

        return ReflectionOptimizer.Instance.CreateConstructor(type);
    }

    /// <summary>
    /// Class TypeMeta.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static class TypeMeta<T>
    {
        /// <summary>
        /// The empty ctor function
        /// </summary>
        public readonly static EmptyCtorDelegate EmptyCtorFn;

        /// <summary>
        /// Initializes static members of the <see cref="TypeMeta{T}" /> class.
        /// </summary>
        static TypeMeta()
        {
            EmptyCtorFn = GetConstructorMethodToCache(typeof(T));
        }
    }

    /// <summary>
    /// Creates a new instance of type.
    /// First looks at JsConfig.ModelFactory before falling back to CreateInstance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type.</param>
    /// <returns>T.</returns>
    public static T New<T>(this Type type)
    {
        var factoryFn = JsConfig.ModelFactory(type)
                        ?? GetConstructorMethod(type);
        return (T)factoryFn();
    }

    /// <summary>
    /// Creates a new instance of type.
    /// First looks at JsConfig.ModelFactory before falling back to CreateInstance
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public static object New(this Type type)
    {
        var factoryFn = JsConfig.ModelFactory(type)
                        ?? GetConstructorMethod(type);
        return factoryFn();
    }

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object CreateInstance<T>()
    {
        return TypeMeta<T>.EmptyCtorFn();
    }

    /// <summary>
    /// Creates a new instance from the default constructor of type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object CreateInstance(this Type type)
    {
        if (type == null)
        {
            return null;
        }

        var ctorFn = GetConstructorMethod(type);
        return ctorFn();
    }

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type.</param>
    /// <returns>T.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CreateInstance<T>(this Type type)
    {
        if (type == null)
        {
            return default;
        }

        var ctorFn = GetConstructorMethod(type);
        return (T)ctorFn();
    }

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object CreateInstance(string typeName)
    {
        if (typeName == null)
        {
            return null;
        }

        var ctorFn = GetConstructorMethod(typeName);
        return ctorFn();
    }

    /// <summary>
    /// Gets the module.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Module.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Module GetModule(this Type type)
    {
        return type?.Module;
    }

    /// <summary>
    /// Gets all properties.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>PropertyInfo[].</returns>
    public static PropertyInfo[] GetAllProperties(this Type type)
    {
        if (type.IsInterface)
        {
            var propertyInfos = new List<PropertyInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);

            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();
                foreach (var subInterface in subType.GetInterfaces().Where(subInterface => !considered.Contains(subInterface)))
                {
                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                var typeProperties = subType.GetTypesProperties();

                var newPropertyInfos = typeProperties
                    .Where(x => !propertyInfos.Contains(x));

                propertyInfos.InsertRange(0, newPropertyInfos);
            }

            return [.. propertyInfos];
        }

        return [.. type.GetTypesProperties().Where(t => t.GetIndexParameters().Length == 0)];
    }

    /// <summary>
    /// Gets the public properties.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>PropertyInfo[].</returns>
    public static PropertyInfo[] GetPublicProperties(this Type type)
    {
        if (type.IsInterface)
        {
            var propertyInfos = new List<PropertyInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);

            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();
                foreach (var subInterface in subType.GetInterfaces().Where(subInterface => !considered.Contains(subInterface)))
                {
                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                var typeProperties = subType.GetTypesPublicProperties();

                var newPropertyInfos = typeProperties
                    .Where(x => !propertyInfos.Contains(x));

                propertyInfos.InsertRange(0, newPropertyInfos);
            }

            return [.. propertyInfos];
        }

        return [.. type.GetTypesPublicProperties().Where(t => t.GetIndexParameters().Length == 0)];
    }

    /// <summary>
    /// The data member
    /// </summary>
    public const string DataMember = "DataMemberAttribute";

    /// <summary>
    /// The ignore attributes named
    /// </summary>
    static internal string[] IgnoreAttributesNamed = [
        "IgnoreDataMemberAttribute",
        "JsonIgnoreAttribute"
    ];

    /// <summary>
    /// Resets this instance.
    /// </summary>
    static internal void Reset()
    {
        IgnoreAttributesNamed = [
            "IgnoreDataMemberAttribute",
            "JsonIgnoreAttribute"
        ];

        try
        {
            JsConfig<Type>.SerializeFn = x => x?.ToString();
            JsConfig<MethodInfo>.SerializeFn = x => x?.ToString();
            JsConfig<PropertyInfo>.SerializeFn = x => x?.ToString();
            JsConfig<FieldInfo>.SerializeFn = x => x?.ToString();
            JsConfig<MemberInfo>.SerializeFn = x => x?.ToString();
            JsConfig<ParameterInfo>.SerializeFn = x => x?.ToString();
        }
        catch (Exception e)
        {
            Tracer.Instance.WriteError("ReflectionExtensions JsConfig<Type>", e);
        }
    }

    /// <summary>
    /// Gets the serializable properties.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>PropertyInfo[].</returns>
    public static PropertyInfo[] GetSerializableProperties(this Type type)
    {
        var properties = type.IsDto()
                             ? type.GetAllProperties()
                             : type.GetPublicProperties();
        return properties.OnlySerializableProperties(type);
    }

    /// <summary>
    /// Called when [serializable properties].
    /// </summary>
    /// <param name="properties">The properties.</param>
    /// <param name="type">The type.</param>
    /// <returns>PropertyInfo[].</returns>
    public static PropertyInfo[] OnlySerializableProperties(this PropertyInfo[] properties, Type type = null)
    {
        var isDto = type.IsDto();
        var readableProperties = properties.Where(x => x.GetGetMethod(isDto) != null);

        if (isDto)
        {
            return [.. readableProperties.Where(attr =>
                attr.HasAttribute<DataMemberAttribute>())];
        }

        // else return those properties that are not decorated with IgnoreDataMember
        return [.. readableProperties
            .Where(prop => prop.AllAttributes()
                .All(attr =>
                    {
                        var name = attr.GetType().Name;
                        return !IgnoreAttributesNamed.Contains(name);
                    }))
            .Where(prop => !(JsConfig.ExcludeTypes.Contains(prop.PropertyType) ||
                             JsConfig.ExcludeTypeNames.Contains(prop.PropertyType.FullName)))];
    }

    /// <summary>
    /// Gets the on deserializing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Func&lt;System.Object, System.String, System.Object, System.Object&gt;.</returns>
    public static Func<object, string, object, object> GetOnDeserializing<T>()
    {
        var method = typeof(T).GetMethodInfo("OnDeserializing");
        if (method == null || method.ReturnType != typeof(object))
        {
            return null;
        }

        var obj = (Func<T, string, object, object>)method.CreateDelegate(typeof(Func<T, string, object, object>));
        return (instance, memberName, value) => obj((T)instance, memberName, value);
    }

    /// <summary>
    /// Gets the serializable fields.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>FieldInfo[].</returns>
    public static FieldInfo[] GetSerializableFields(this Type type)
    {
        if (type.IsDto())
        {
            return [.. type.GetAllFields().Where(f =>
                f.HasAttribute<DataMemberAttribute>())];
        }

        var config = JsConfig.GetConfig();

        if (!config.IncludePublicFields)
        {
            return TypeConstants.EmptyFieldInfoArray;
        }

        var publicFields = type.GetPublicFields();

        // else return those properties that are not decorated with IgnoreDataMember
        return [.. publicFields
            .Where(prop => prop.AllAttributes()
                .All(attr => !IgnoreAttributesNamed.Contains(attr.GetType().Name)))
            .Where(prop => !config.ExcludeTypes.Contains(prop.FieldType))];
    }

    /// <summary>
    /// Gets the data contract.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>DataContractAttribute.</returns>
    public static DataContractAttribute GetDataContract(this Type type)
    {
        var dataContract = type.FirstAttribute<DataContractAttribute>();

        if (dataContract == null && Env.IsMono)
        {
            return PclExport.Instance.GetWeakDataContract(type);
        }

        return dataContract;
    }

    /// <summary>
    /// Gets the data member.
    /// </summary>
    /// <param name="pi">The pi.</param>
    /// <returns>DataMemberAttribute.</returns>
    public static DataMemberAttribute GetDataMember(this PropertyInfo pi)
    {
        var dataMember = pi.AllAttributes(typeof(DataMemberAttribute))
                             .FirstOrDefault() as DataMemberAttribute;

        if (dataMember == null && Env.IsMono)
        {
            return PclExport.Instance.GetWeakDataMember(pi);
        }

        return dataMember;
    }

    /// <summary>
    /// Gets the data member.
    /// </summary>
    /// <param name="pi">The pi.</param>
    /// <returns>DataMemberAttribute.</returns>
    public static DataMemberAttribute GetDataMember(this FieldInfo pi)
    {
        var dataMember = pi.AllAttributes(typeof(DataMemberAttribute))
                             .FirstOrDefault() as DataMemberAttribute;

        if (dataMember == null && Env.IsMono)
        {
            return PclExport.Instance.GetWeakDataMember(pi);
        }

        return dataMember;
    }

    /// <summary>
    /// Gets the name of the data member.
    /// </summary>
    /// <param name="pi">The pi.</param>
    /// <returns>System.String.</returns>
    public static string GetDataMemberName(this PropertyInfo pi)
    {
        var attr = pi.GetDataMember();
        return attr?.Name;
    }
}