// ***********************************************************************
// <copyright file="IAppSettings.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace ServiceStack.Configuration
{
    /// <summary>
    /// Interface IAppSettings
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        Dictionary<string, string> GetAll();

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        List<string> GetAllKeys();

        /// <summary>
        /// Existses the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Exists(string key);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        string GetString(string name);

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        IList<string> GetList(string key);

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IDictionary&lt;System.String, System.String&gt;.</returns>
        IDictionary<string, string> GetDictionary(string key);

        /// <summary>
        /// Gets the key value pairs.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        List<KeyValuePair<string, string>> GetKeyValuePairs(string key);

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        T Get<T>(string name);

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>T.</returns>
        T Get<T>(string name, T defaultValue);
    }

    /// <summary>
    /// Interface IRuntimeAppSettings
    /// </summary>
    public interface IRuntimeAppSettings
    {
        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>T.</returns>
        T Get<T>(Web.IRequest request, string name, T defaultValue);
    }
}