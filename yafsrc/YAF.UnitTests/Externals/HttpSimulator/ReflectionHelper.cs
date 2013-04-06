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
    #region

    using System;
    using System.Reflection;

    #endregion

    /// <summary>
    /// Helper class to simplify common reflection tasks.
    /// </summary>
    public sealed class ReflectionHelper
    {
        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ReflectionHelper"/> class from being created.
        /// </summary>
        private ReflectionHelper()
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        /// <returns></returns>
        public static T GetPrivateInstanceFieldValue<T>(string fieldName, object source)
        {
            FieldInfo field = source.GetType().GetField(
                fieldName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(source);
            }

            return default(T);
        }

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
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }

            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="fieldName">
        /// Name of the member. 
        /// </param>
        /// <param name="typeName">
        /// </param>
        public static T GetStaticFieldValue<T>(string fieldName, string typeName)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }

            return default(T);
        }

        /// <summary>
        /// The instantiate.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// The instantiate.
        /// </returns>
        public static object Instantiate(string typeName)
        {
            return Instantiate(typeName, null, null);
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
            ConstructorInfo constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgumentTypes, null);
            return constructor.Invoke(constructorParameterValues);
        }

        /// <summary>
        /// Invokes a non-public static method.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static TReturn InvokeNonPublicMethod<TReturn>(Type type, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, o => o.GetType());

            MethodInfo method = type.GetMethod(
                methodName, BindingFlags.NonPublic | BindingFlags.Static, null, paramTypes, null);
            if (method == null)
            {
                throw new ArgumentException(
                    string.Format("Could not find a method with the name '{0}'", methodName), "method");
            }

            return (TReturn)method.Invoke(null, parameters);
        }

        /// <summary>
        /// The invoke non public method.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        ///   </exception>
        public static TReturn InvokeNonPublicMethod<TReturn>(
            object source, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, o => o.GetType());

            MethodInfo method = source.GetType().GetMethod(
                methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, paramTypes, null);
            if (method == null)
            {
                throw new ArgumentException(
                    string.Format("Could not find a method with the name '{0}'", methodName), "method");
            }

            return (TReturn)method.Invoke(source, parameters);
        }

        /// <summary>
        /// The invoke non public property.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        ///   </exception>
        public static TReturn InvokeNonPublicProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(
                propertyName, BindingFlags.NonPublic | BindingFlags.Instance, null, typeof(TReturn), new Type[0], null);
            if (propertyInfo == null)
            {
                throw new ArgumentException(
                    string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");
            }

            return (TReturn)propertyInfo.GetValue(source, null);
        }

        /// <summary>
        /// The invoke non public property.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>
        /// The invoke non public property.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   </exception>
        public static object InvokeNonPublicProperty(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(
                propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException(
                    string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");
            }

            return propertyInfo.GetValue(source, null);
        }

        /// <summary>
        /// The invoke property.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="TReturn">
        /// </typeparam>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static TReturn InvokeProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException(
                    string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");
            }

            return (TReturn)propertyInfo.GetValue(source, null);
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
            FieldInfo field = source.GetType().GetField(
                memberName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException(
                    string.Format("Could not find the private instance field '{0}'", memberName));
            }

            field.SetValue(source, value);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public static void SetStaticFieldValue<T>(string fieldName, Type type, T value)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", fieldName));
            }

            field.SetValue(null, value);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="value">The value.</param>
        public static void SetStaticFieldValue<T>(string fieldName, string typeName, T value)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", fieldName));
            }

            field.SetValue(null, value);
        }

        #endregion
    }
}