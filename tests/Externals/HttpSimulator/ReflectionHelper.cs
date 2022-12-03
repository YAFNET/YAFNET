/* httpcontext-simulator 
 * a simulator used to simulate http context during integration testing
 *
 * Copyright (C) Phil Haack 
 * http://code.google.com/p/httpcontext-simulator/
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

namespace HttpSimulator
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Helper class to simplify common reflection tasks.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="fieldName">
        /// Name of the member. 
        /// </param>
        /// <param name="type">
        /// Type of the member. 
        /// </param>
        /// ///
        public static T GetStaticFieldValue<T>(string fieldName, Type type)
        {
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }

            return default;
        }

        /// <summary>
        /// The instantiate.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <param name="constructorArgumentTypes">
        /// The constructor argument types.
        /// </param>
        /// <param name="constructorParameterValues">
        /// The constructor parameter values.
        /// </param>
        /// <returns>
        /// The instantiate.
        /// </returns>
        public static object Instantiate(
            string typeName, Type[] constructorArgumentTypes, params object[] constructorParameterValues)
        {
            return Instantiate(Type.GetType(typeName, true), constructorArgumentTypes, constructorParameterValues);
        }

        /// <summary>
        /// The instantiate.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="constructorArgumentTypes">
        /// The constructor argument types.
        /// </param>
        /// <param name="constructorParameterValues">
        /// The constructor parameter values.
        /// </param>
        /// <returns>
        /// The instantiate.
        /// </returns>
        public static object Instantiate(
            Type type, Type[] constructorArgumentTypes, params object[] constructorParameterValues)
        {
            var constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgumentTypes, null);
            return constructor.Invoke(constructorParameterValues);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="memberName">
        /// Name of the member. 
        /// </param>
        /// <param name="source">
        /// The object that contains the member. 
        /// </param>
        /// <param name="value">
        /// The value to set the member to. 
        /// </param>
        public static void SetPrivateInstanceFieldValue(string memberName, object source, object value)
        {
            var field = source.GetType().GetField(
                memberName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException($"Could not find the private instance field '{memberName}'");
            }

            field.SetValue(source, value);
        }
    }
}