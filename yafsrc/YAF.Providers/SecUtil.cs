//------------------------------------------------------------------------------
// <copyright file="SecurityUtil.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 * SecurityUtil class
 *
 * Copyright (c) 1999 Microsoft Corporation
 */

namespace YAF.Providers {
    using  System;
    using System.Globalization;
    using System.Web.Hosting;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Configuration.Provider;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Xml;

    internal static class SecUtility {

        internal const int Infinite = Int32.MaxValue;
        internal static string GetDefaultAppName()
        {
            try {
                string appName = HostingEnvironment.ApplicationVirtualPath;
                if (String.IsNullOrEmpty(appName)) {

                    appName = System.Diagnostics.Process.GetCurrentProcess().
                                     MainModule.ModuleName;

                    int indexOfDot = appName.IndexOf('.');
                    if (indexOfDot != -1) {
                        appName = appName.Remove(indexOfDot);
                    }
                }

                if (String.IsNullOrEmpty(appName)) {
                    return "/";
                }
                else {
                    return appName;
                }
            }
            catch {
                return "/";
            }
        }

        // We don't trim the param before checking with password parameters
        internal static bool ValidatePasswordParameter(ref string param, int maxSize) {
            if (param == null) {
                return false;
            }

            if (param.Length < 1) {
                return false;
            }

            if (maxSize > 0 && (param.Length > maxSize) ) {
                return false;
            }

            return true;
        }

        internal static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize) {
            if (param == null) {
                return !checkForNull;
            }

            param = param.Trim();
            if ((checkIfEmpty && param.Length < 1) ||
                 (maxSize > 0 && param.Length > maxSize) ||
                 (checkForCommas && param.Contains(","))) {
                return false;
            }

            return true;
        }

        // We don't trim the param before checking with password parameters
        internal static void CheckPasswordParameter(ref string param, int maxSize, string paramName) {
            if (param == null) {
                throw new ArgumentNullException(paramName);
            }

            if (param.Length < 1) {
                throw new ArgumentException(SR.GetString(SR.Parameter_can_not_be_empty, paramName), paramName);
            }

            if (maxSize > 0 && param.Length > maxSize) {
                throw new ArgumentException(SR.GetString(SR.Parameter_too_long, paramName, maxSize.ToString(CultureInfo.InvariantCulture)), paramName);
            }
        }

        internal static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName) {
            if (param == null) {
                if (checkForNull) {
                    throw new ArgumentNullException(paramName);
                }

                return;
            }

            param = param.Trim();
            if (checkIfEmpty && param.Length < 1) {
                throw new ArgumentException(SR.GetString(SR.Parameter_can_not_be_empty, paramName), paramName);
            }

            if (maxSize > 0 && param.Length > maxSize) {
                throw new ArgumentException(SR.GetString(SR.Parameter_too_long, paramName, maxSize.ToString(CultureInfo.InvariantCulture)), paramName);
            }

            if (checkForCommas && param.Contains(",")) {
                throw new ArgumentException(SR.GetString(SR.Parameter_can_not_contain_comma, paramName), paramName);
            }
        }

        internal static void CheckArrayParameter(ref string[] param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName) {
            if (param == null) {
                throw new ArgumentNullException(paramName);
            }

            if (param.Length < 1) {
                throw new ArgumentException(SR.GetString(SR.Parameter_array_empty, paramName), paramName);
            }

            Hashtable values = new Hashtable(param.Length);
            for (int i = param.Length - 1; i >= 0; i--) {
                SecUtility.CheckParameter(ref param[i], checkForNull, checkIfEmpty, checkForCommas, maxSize,
                    paramName + "[ " + i.ToString(CultureInfo.InvariantCulture) + " ]");
                if (values.Contains(param[i])) {
                    throw new ArgumentException(SR.GetString(SR.Parameter_duplicate_array_element, paramName), paramName);
                }
                else {
                    values.Add(param[i], param[i]);
                }
            }
        }

        internal static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue) {
            string sValue = config[valueName];
            if (sValue == null) {
                return defaultValue;
            }

            bool result;
            if (bool.TryParse(sValue, out result)) {
                return result;
            }
            else {
                throw new ProviderException(SR.GetString(SR.Value_must_be_boolean, valueName));
            }
        }

        internal static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed) {
            string sValue = config[valueName];

            if (sValue == null) {
                return defaultValue;
            }

            int iValue;
            if (!Int32.TryParse(sValue, out iValue)) {
                if (zeroAllowed) {
                    throw new ProviderException(SR.GetString(SR.Value_must_be_non_negative_integer, valueName));
                }

                throw new ProviderException(SR.GetString(SR.Value_must_be_positive_integer, valueName));
            }

            if (zeroAllowed && iValue < 0) {
                throw new ProviderException(SR.GetString(SR.Value_must_be_non_negative_integer, valueName));
            }

            if (!zeroAllowed && iValue <= 0) {
                throw new ProviderException(SR.GetString(SR.Value_must_be_positive_integer, valueName));
            }

            if (maxValueAllowed > 0 && iValue > maxValueAllowed) {
                throw new ProviderException(SR.GetString(SR.Value_too_big, valueName, maxValueAllowed.ToString(CultureInfo.InvariantCulture)));
            }

            return iValue;
        }

        private static bool IsDirectorySeparatorChar(char ch) {
            return (ch == '\\' || ch == '/');
        }

        internal static bool IsAbsolutePhysicalPath(string path) {
            if (path == null || path.Length < 3)
                return false;

            // e.g c:\foo
            if (path[1] == ':' && IsDirectorySeparatorChar(path[2]))
                return true;

            // e.g \\server\share\foo or //server/share/foo
            return IsUncSharePath(path);
        }

        internal static bool IsUncSharePath(string path) {
            // e.g \\server\share\foo or //server/share/foo
            if (path.Length > 2 && IsDirectorySeparatorChar(path[0]) && IsDirectorySeparatorChar(path[1]))
                return true;
            return false;

        }

        internal static void CheckSchemaVersion(ProviderBase provider, SqlConnection connection, string[] features, string version, ref int schemaVersionCheck) {
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }

            if (features == null) {
                throw new ArgumentNullException("features");
            }

            if (version == null) {
                throw new ArgumentNullException("version");
            }

            if (schemaVersionCheck == -1) {
                throw new ProviderException(SR.GetString(SR.Provider_Schema_Version_Not_Match, provider.ToString(), version));
            }
            else if (schemaVersionCheck == 0) {
                lock (provider) {
                    if (schemaVersionCheck == -1) {
                        throw new ProviderException(SR.GetString(SR.Provider_Schema_Version_Not_Match, provider.ToString(), version));
                    }
                    else if (schemaVersionCheck == 0) {
                        int iStatus = 0;
                        SqlCommand cmd = null;
                        SqlParameter p = null;

                        foreach (string feature in features) {
                            cmd = new SqlCommand("dbo.aspnet_CheckSchemaVersion", connection);

                            cmd.CommandType = CommandType.StoredProcedure;

                            p = new SqlParameter("@Feature", feature);
                            cmd.Parameters.Add(p);

                            p = new SqlParameter("@CompatibleSchemaVersion", version);
                            cmd.Parameters.Add(p);

                            p = new SqlParameter("@ReturnValue", SqlDbType.Int);
                            p.Direction = ParameterDirection.ReturnValue;
                            cmd.Parameters.Add(p);

                            cmd.ExecuteNonQuery();

                            iStatus = ((p.Value != null) ? ((int)p.Value) : -1);
                            if (iStatus != 0) {
                                schemaVersionCheck = -1;

                                throw new ProviderException(SR.GetString(SR.Provider_Schema_Version_Not_Match, provider.ToString(), version));
                            }
                        }

                        schemaVersionCheck = 1;
                    }
                }
            }
        }

        internal static XmlNode GetAndRemoveBooleanAttribute(XmlNode node, string attrib, ref bool val) {
            return GetAndRemoveBooleanAttributeInternal(node, attrib, false /*fRequired*/, ref val);
        }

        // input.Xml cursor must be at a true/false XML attribute
        private static XmlNode GetAndRemoveBooleanAttributeInternal(XmlNode node, string attrib, bool fRequired, ref bool val) {
            XmlNode a = GetAndRemoveAttribute(node, attrib, fRequired);
            if (a != null) {
                if (a.Value == "true") {
                    val = true;
                }
                else if (a.Value == "false") {
                    val = false;
                }
                else {
                    throw new ConfigurationErrorsException(
                                    SR.GetString(SR.Invalid_boolean_attribute, a.Name),
                                    a);
                }
            }

            return a;
        }

        private static XmlNode GetAndRemoveAttribute(XmlNode node, string attrib, bool fRequired) {
            XmlNode a = node.Attributes.RemoveNamedItem(attrib);

            // If the attribute is required and was not present, throw
            if (fRequired && a == null) {
                throw new ConfigurationErrorsException(
                    SR.GetString(SR.Missing_required_attribute, attrib, node.Name),
                    node);
            }

            return a;
        }

        internal static XmlNode GetAndRemoveNonEmptyStringAttribute(XmlNode node, string attrib, ref string val) {
            return GetAndRemoveNonEmptyStringAttributeInternal(node, attrib, false /*fRequired*/, ref val);
        }

        private static XmlNode GetAndRemoveNonEmptyStringAttributeInternal(XmlNode node, string attrib, bool fRequired, ref string val) {
            XmlNode a = GetAndRemoveStringAttributeInternal(node, attrib, fRequired, ref val);
            if (a != null && val.Length == 0) {
                throw new ConfigurationErrorsException(
                    SR.GetString(SR.Empty_attribute, attrib),
                    a);
            }

            return a;
        }

        private static XmlNode GetAndRemoveStringAttributeInternal(XmlNode node, string attrib, bool fRequired, ref string val) {
            XmlNode a = GetAndRemoveAttribute(node, attrib, fRequired);
            if (a != null) {
                val = a.Value;
            }

            return a;
        }

        internal static void CheckForUnrecognizedAttributes(XmlNode node) {
            if (node.Attributes.Count != 0) {
                throw new ConfigurationErrorsException(
                                SR.GetString(SR.Config_base_unrecognized_attribute, node.Attributes[0].Name),
                                node.Attributes[0]);
            }
        }

        internal static void CheckForNonCommentChildNodes(XmlNode node) {
            foreach (XmlNode childNode in node.ChildNodes) {
                if (childNode.NodeType != XmlNodeType.Comment) {
                    throw new ConfigurationErrorsException(
                                    SR.GetString(SR.Config_base_no_child_nodes),
                                    childNode);
                }
            }
        }

        internal static XmlNode GetAndRemoveStringAttribute(XmlNode node, string attrib, ref string val) {
            return GetAndRemoveStringAttributeInternal(node, attrib, false /*fRequired*/, ref val);
        }

        internal static void CheckForbiddenAttribute(XmlNode node, string attrib) {
            XmlAttribute attr = node.Attributes[attrib];
            if (attr != null) {
                throw new ConfigurationErrorsException(
                                SR.GetString(SR.Config_base_unrecognized_attribute, attrib),
                                attr);
            }
        }

        // Returns whether the virtual path is relative.  Note that this returns true for
        // app relative paths (e.g. "~/sub/foo.aspx")
        internal static bool IsRelativeUrl(string virtualPath) {
            // If it has a protocol, it's not relative
            if (virtualPath.IndexOf(":", StringComparison.Ordinal) != -1)
                return false;

            return !IsRooted(virtualPath);
        }

        internal static bool IsRooted(String basepath) {
            return (String.IsNullOrEmpty(basepath) || basepath[0] == '/' || basepath[0] == '\\');
        }

        internal static void GetAndRemoveStringAttribute(NameValueCollection config, string attrib, string providerName, ref string val) {
            val = config.Get(attrib);
            config.Remove(attrib);
        }

        internal static void CheckUnrecognizedAttributes(NameValueCollection config, string providerName) {
            if (config.Count > 0) {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ConfigurationErrorsException(
                                    SR.GetString(SR.Unexpected_provider_attribute, attribUnrecognized, providerName));
            }
        }

        internal static string GetStringFromBool(bool flag) {
            return flag ? "true" : "false";
        }
        internal static void GetAndRemovePositiveOrInfiniteAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            GetPositiveOrInfiniteAttribute(config, attrib, providerName, ref val);
            config.Remove(attrib);
        }

        internal static void GetPositiveOrInfiniteAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            string s = config.Get(attrib);
            int t;

            if (s == null)
            {
                return;
            }

            if (s == "Infinite")
            {
                t = Infinite;
            }
            else
            {
                try
                {
                    t = Convert.ToInt32(s, CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    if (e is ArgumentException || e is FormatException || e is OverflowException)
                    {
                        throw new ConfigurationErrorsException(
                            SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));
                    }
                    else
                    {
                        throw;
                    }

                }

                if (t < 0)
                {
                    throw new ConfigurationErrorsException(
                        SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));

                }
            }

            val = t;
        }

        internal static void GetAndRemovePositiveAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            GetPositiveAttribute(config, attrib, providerName, ref val);
            config.Remove(attrib);
        }

        internal static void GetPositiveAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            string s = config.Get(attrib);
            int t;

            if (s == null)
            {
                return;
            }

            try
            {
                t = Convert.ToInt32(s, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is FormatException || e is OverflowException)
                {
                    throw new ConfigurationErrorsException(
                        SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));
                }
                else
                {
                    throw;
                }

            }

            if (t < 0)
            {
                throw new ConfigurationErrorsException(
                    SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));

            }

            val = t;
        }
    }
}
