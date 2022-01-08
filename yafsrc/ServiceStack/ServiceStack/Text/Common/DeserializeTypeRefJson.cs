// ***********************************************************************
// <copyright file="DeserializeTypeRefJson.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using ServiceStack.Text.Json;

namespace ServiceStack.Text.Common
{
    /// <summary>
    /// Class DeserializeTypeRefJson.
    /// </summary>
    internal static class DeserializeTypeRefJson
    {
        /// <summary>
        /// The serializer
        /// </summary>
        private static readonly JsonTypeSerializer Serializer = (JsonTypeSerializer)JsonTypeSerializer.Instance;

        /// <summary>
        /// Strings to type.
        /// </summary>
        /// <param name="strType">Type of the string.</param>
        /// <param name="typeConfig">The type configuration.</param>
        /// <param name="ctorFn">The ctor function.</param>
        /// <param name="typeAccessors">The type accessors.</param>
        /// <returns>System.Object.</returns>
        internal static object StringToType(ReadOnlySpan<char> strType,
            TypeConfig typeConfig,
            EmptyCtorDelegate ctorFn,
            KeyValuePair<string, TypeAccessor>[] typeAccessors)
        {
            var index = 0;
            var type = typeConfig.Type;

            if (strType.IsEmpty)
                return null;

            var buffer = strType;
            var strTypeLength = strType.Length;

            //if (!Serializer.EatMapStartChar(strType, ref index))
            for (; index < strTypeLength; index++) { if (!JsonUtils.IsWhiteSpace(buffer[index])) break; } //Whitespace inline
            if (buffer[index] != JsWriter.MapStartChar)
                throw DeserializeTypeRef.CreateSerializationError(type, strType.ToString());

            index++;
            if (JsonTypeSerializer.IsEmptyMap(strType, index))
                return ctorFn();

            var config = JsConfig.GetConfig();
            var typeAttr = config.TypeAttrMemory;

            object instance = null;
            var textCase = typeConfig.TextCase.GetValueOrDefault(config.TextCase);
            var lenient = config.PropertyConvention == PropertyConvention.Lenient || textCase == TextCase.SnakeCase;

            for (; index < strTypeLength; index++) { if (!JsonUtils.IsWhiteSpace(buffer[index])) break; } //Whitespace inline

            while (index < strTypeLength)
            {
                var propertyName = JsonTypeSerializer.UnescapeJsString(strType, JsonUtils.QuoteChar, true, ref index);

                //Serializer.EatMapKeySeperator(strType, ref index);
                for (; index < strTypeLength; index++) { if (!JsonUtils.IsWhiteSpace(buffer[index])) break; } //Whitespace inline
                if (strTypeLength != index) index++;

                var propertyValueStr = Serializer.EatValue(strType, ref index);
                var possibleTypeInfo = propertyValueStr != null && propertyValueStr.Length > 1;

                switch (instance)
                {
                    //if we already have an instance don't check type info, because then we will have a half deserialized object
                    //we could throw here or just use the existing instance.
                    case null when possibleTypeInfo && propertyName.Equals(typeAttr.Span, StringComparison.OrdinalIgnoreCase):
                    {
                        var explicitTypeName = Serializer.ParseString(propertyValueStr);
                        var explicitType = config.TypeFinder(explicitTypeName);

                        if (explicitType == null || explicitType.IsInterface || explicitType.IsAbstract)
                        {
                            Tracer.Instance.WriteWarning("Could not find type: " + propertyValueStr.ToString());
                        }
                        else if (!type.IsAssignableFrom(explicitType))
                        {
                            Tracer.Instance.WriteWarning("Could not assign type: " + propertyValueStr.ToString());
                        }
                        else
                        {
                            JsWriter.AssertAllowedRuntimeType(explicitType);
                            instance = explicitType.CreateInstance();
                        }

                        if (instance != null)
                        {
                            //If __type info doesn't match, ignore it.
                            if (!type.IsInstanceOfType(instance))
                            {
                                instance = null;
                            }
                            else
                            {
                                var derivedType = instance.GetType();
                                if (derivedType != type)
                                {
                                    var map = DeserializeTypeRef.GetCachedTypeAccessors(derivedType, Serializer);
                                    if (map != null)
                                        typeAccessors = map;
                                }
                            }
                        }

                        Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
                        continue;
                    }
                    case null:
                        instance = ctorFn();
                        break;
                }

                var typeAccessor = typeAccessors.Get(propertyName, lenient);

                var propType = possibleTypeInfo && propertyValueStr[0] == '_' ? TypeAccessor.ExtractType(Serializer, propertyValueStr) : null;
                if (propType != null)
                {
                    try
                    {
                        if (typeAccessor != null)
                        {
                            //var parseFn = Serializer.GetParseFn(propType);
                            var parseFn = JsonReader.GetParseStringSpanFn(propType);

                            var propertyValue = parseFn(propertyValueStr);
                            if (typeConfig.OnDeserializing != null)
                                propertyValue = typeConfig.OnDeserializing(instance, propertyName.ToString(), propertyValue);
                            typeAccessor.SetProperty(instance, propertyValue);
                        }

                        //Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
                        for (; index < strTypeLength; index++) { if (!JsonUtils.IsWhiteSpace(buffer[index])) break; } //Whitespace inline
                        if (index != strTypeLength)
                        {
                            var success = buffer[index] == JsWriter.ItemSeperator || buffer[index] == JsWriter.MapEndChar;
                            index++;
                            if (success)
                                for (; index < strTypeLength; index++) { if (!JsonUtils.IsWhiteSpace(buffer[index])) break; } //Whitespace inline
                        }

                        continue;
                    }
                    catch (Exception e)
                    {
                        config.OnDeserializationError?.Invoke(instance, propType, propertyName.ToString(), propertyValueStr.ToString(), e);
                        if (config.ThrowOnError) throw DeserializeTypeRef.GetSerializationException(propertyName.ToString(), propertyValueStr.ToString(), propType, e);
                        else Tracer.Instance.WriteWarning("WARN: failed to set dynamic property {0} with: {1}", propertyName.ToString(), propertyValueStr.ToString());
                    }
                }

                if (typeAccessor?.GetProperty != null && typeAccessor.SetProperty != null)
                {
                    try
                    {
                        var propertyValue = typeAccessor.GetProperty(propertyValueStr);
                        if (typeConfig.OnDeserializing != null)
                            propertyValue = typeConfig.OnDeserializing(instance, propertyName.ToString(), propertyValue);
                        typeAccessor.SetProperty(instance, propertyValue);
                    }
                    catch (NotSupportedException) { throw; }
                    catch (Exception e)
                    {
                        config.OnDeserializationError?.Invoke(instance, propType ?? typeAccessor.PropertyType, propertyName.ToString(), propertyValueStr.ToString(), e);
                        if (config.ThrowOnError) throw DeserializeTypeRef.GetSerializationException(propertyName.ToString(), propertyValueStr.ToString(), typeAccessor.PropertyType, e);
                        else Tracer.Instance.WriteWarning("WARN: failed to set property {0} with: {1}", propertyName.ToString(), propertyValueStr.ToString());
                    }
                }
                else
                {
                    // the property is not known by the DTO
                    typeConfig.OnDeserializing?.Invoke(instance, propertyName.ToString(), propertyValueStr.ToString());
                }

                //Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
                for (; index < strTypeLength; index++) { if (!JsonUtils.IsWhiteSpace(buffer[index])) break; } //Whitespace inline
                if (index != strType.Length)
                {
                    var success = buffer[index] == JsWriter.ItemSeperator || buffer[index] == JsWriter.MapEndChar;
                    index++;
                    if (success)
                        for (; index < strTypeLength; index++) { if (!JsonUtils.IsWhiteSpace(buffer[index])) break; } //Whitespace inline
                }

            }

            return instance;
        }
    }
}
