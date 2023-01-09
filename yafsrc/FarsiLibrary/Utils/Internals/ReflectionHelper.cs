using System;
using System.Reflection;

namespace FarsiLibrary.Utils.Internals;

internal static class ReflectionHelper
{
    /// <summary>
    /// Find and returns a FieldInfo by its name, on 
    /// the specified type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static FieldInfo GetField(Type type, string fieldName)
    {
        return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
    }

    /// <summary>
    /// Sets a value to a field of the owner object
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    public static void SetField(object owner, string fieldName, object value)
    {
        if (owner == null)
            throw new ArgumentNullException(nameof(owner), "owner should point to a object. works on instance fields only");

        var type = owner.GetType();
        var fieldinfo = GetField(type, fieldName);

        if (fieldinfo == null)
        {
            throw new ArgumentNullException(fieldName, "fieldName can not be found on the type");
        }

        fieldinfo.SetValue(owner, value);
    }

    /// <summary>
    /// Find and returns a FieldInfo by its name, on 
    /// the specified type.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static TResult GetField<TResult>(object owner, string fieldName)
    {
        return (TResult)GetField(owner, fieldName);
    }

    /// <summary>
    /// returns value of a field in the owner object.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static object GetField(object owner, string fieldName)
    {
        if (owner == null)
            throw new ArgumentNullException(nameof(owner), "owner should point to a object. works on instance fields only");

        var type = owner.GetType();
        var fieldinfo = GetField(type, fieldName);

        if (fieldinfo == null)
            throw new ArgumentNullException(fieldName, "fieldName can not be found on the type");

        return fieldinfo.GetValue(owner);
    }

    /// <summary>
    /// Returns ProprtyInfo of a 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="owner"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    public static PropertyInfo GetProperty(Type type, object owner, string propName)
    {
        return type.GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
    }

    /// <summary>
    /// Returns value of a property in the owner object.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    public static object GetProperty(object owner, string propName)
    {
        if (owner == null)
        {
            throw new ArgumentNullException(nameof(owner), "owner should point to a object. works on instance fields only");
        }

        var type = owner.GetType();
        var propInfo = GetProperty(type, owner, propName);

        if (propInfo == null)
        {
            throw new ArgumentNullException(propName, "propName can not be found on the type");
        }

        return propInfo.GetValue(owner, null);
    }

    /// <summary>
    /// Returns value of a property in the owner object.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="owner"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    public static TResult GetProperty<TResult>(object owner, string propName)
    {
        return (TResult)GetProperty(owner, propName);
    }

    public static void InvokeMethod(object owner, string methodName, params object[] param)
    {
        var type = owner.GetType();
        var mi = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        mi.Invoke(owner, param);
    }

    public static void InvokeStaticMethod(Type ownerType, string methodName, params object[] param)
    {
        var mi = ownerType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        mi.Invoke(null, param);
    }
}