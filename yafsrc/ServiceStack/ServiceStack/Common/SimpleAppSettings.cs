// ***********************************************************************
// <copyright file="SimpleAppSettings.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Configuration;

namespace ServiceStack;

using ServiceStack.Text;

/// <summary>
/// Class SimpleAppSettings.
/// Implements the <see cref="ServiceStack.Configuration.IAppSettings" />
/// </summary>
/// <seealso cref="ServiceStack.Configuration.IAppSettings" />
public class SimpleAppSettings : IAppSettings
{
    /// <summary>
    /// The settings
    /// </summary>
    private readonly Dictionary<string, string> settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleAppSettings"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public SimpleAppSettings(Dictionary<string, string> settings = null)
    {
        this.settings = settings ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
    public Dictionary<string, string> GetAll() => settings;

    /// <summary>
    /// Gets all keys.
    /// </summary>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> GetAllKeys() => settings.Keys.ToList();

    /// <summary>
    /// Existses the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Exists(string key) => settings.ContainsKey(key);

    /// <summary>
    /// Sets the specified key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Set<T>(string key, T value)
    {
        var textValue = value as string ?? value.ToJsv();

        settings[key] = textValue;
    }

    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public string GetString(string key) => settings.TryGetValue(key, out string value)
                                               ? value
                                               : null;

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>IList&lt;System.String&gt;.</returns>
    public IList<string> GetList(string key) => GetString(key).FromJsv<List<string>>();

    /// <summary>
    /// Gets the dictionary.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>IDictionary&lt;System.String, System.String&gt;.</returns>
    public IDictionary<string, string> GetDictionary(string key) => GetString(key).FromJsv<Dictionary<string, string>>();
    /// <summary>
    /// Gets the key value pairs.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
    public List<KeyValuePair<string, string>> GetKeyValuePairs(string key) => GetString(key).FromJsv<List<KeyValuePair<string, string>>>();

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key.</param>
    /// <returns>T.</returns>
    public T Get<T>(string key) => GetString(key).FromJsv<T>();

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>T.</returns>
    public T Get<T>(string key, T defaultValue)
    {
        var value = GetString(key);
        return value != null ? value.FromJsv<T>() : defaultValue;
    }
}