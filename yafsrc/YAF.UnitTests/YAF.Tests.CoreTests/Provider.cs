/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/

namespace YAF.Tests.CoreTests
{
    using System.Collections;
    using System.Configuration.Provider;
    using System.Reflection;
    using System.Web.Security;

    /// <summary>
    /// Provider Helper
    /// </summary>
    public static class Provider
    {
        /// <summary>
        /// Adds the membership provider.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="provider">The provider.</param>
        public static void AddMembershipProvider(this ProviderCollection providers, string providerName, MembershipProvider provider)
        {
            GetMembershipHashtable().Add(providerName, provider);
        }

        /// <summary>
        /// Adds the role provider.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="provider">The provider.</param>
        public static void AddRoleProvider(this ProviderCollection providers, string providerName, RoleProvider provider)
        {
            GetRolesHashtable().Add(providerName, provider);
        }

        /// <summary>
        /// Removes the membership provider.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="providerName">Name of the provider.</param>
        public static void RemoveMembershipProvider(this ProviderCollection providers, string providerName)
        {
            GetMembershipHashtable().Remove(providerName);
        }

        /// <summary>
        /// Removes the role provider.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="providerName">Name of the provider.</param>
        public static void RemoveRoleProvider(this ProviderCollection providers, string providerName)
        {
            GetRolesHashtable().Remove(providerName);
        }

        /// <summary>
        /// Gets the membership hash table.
        /// </summary>
        /// <returns>Returns the Membership Hash Table</returns>
        public static Hashtable GetMembershipHashtable()
        {
            var hashtableField = typeof(ProviderCollection).GetField("_Hashtable", BindingFlags.Instance | BindingFlags.NonPublic);
            return hashtableField.GetValue(Membership.Providers) as Hashtable;
        }

        /// <summary>
        /// Gets the roles hash table.
        /// </summary>
        /// <returns>Returns the Roles Hash Table</returns>
        public static Hashtable GetRolesHashtable()
        {
            var hashtableField = typeof(ProviderCollection).GetField("_Hashtable", BindingFlags.Instance | BindingFlags.NonPublic);
            return hashtableField.GetValue(Roles.Providers) as Hashtable;
        }
    }
}
