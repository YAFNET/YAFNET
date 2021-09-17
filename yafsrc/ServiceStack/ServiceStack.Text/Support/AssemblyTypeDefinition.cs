// ***********************************************************************
// <copyright file="AssemblyTypeDefinition.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Common.Support
{
    /// <summary>
    /// Class AssemblyTypeDefinition.
    /// </summary>
    internal class AssemblyTypeDefinition
    {
        /// <summary>
        /// The type definition seperator
        /// </summary>
        private const char TypeDefinitionSeperator = ',';
        /// <summary>
        /// The type name index
        /// </summary>
        private const int TypeNameIndex = 0;
        /// <summary>
        /// The assembly name index
        /// </summary>
        private const int AssemblyNameIndex = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyTypeDefinition"/> class.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public AssemblyTypeDefinition(string typeDefinition)
        {
            if (string.IsNullOrEmpty(typeDefinition))
            {
                throw new ArgumentNullException();
            }
            var parts = typeDefinition.Split(TypeDefinitionSeperator);
            TypeName = parts[TypeNameIndex].Trim();
            AssemblyName = parts.Length > AssemblyNameIndex ? parts[AssemblyNameIndex].Trim() : null;
        }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName { get; set; }
    }
}