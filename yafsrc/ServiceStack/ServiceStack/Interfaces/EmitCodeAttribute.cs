// ***********************************************************************
// <copyright file="EmitCodeAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack
{
    /// <summary>
    /// Enum Lang
    /// </summary>
    [Flags]
    public enum Lang
    {
        /// <summary>
        /// The c sharp
        /// </summary>
        CSharp = 1 << 0,
        /// <summary>
        /// The f sharp
        /// </summary>
        FSharp = 1 << 1,
        /// <summary>
        /// The vb
        /// </summary>
        Vb = 1 << 2,
        /// <summary>
        /// The type script
        /// </summary>
        TypeScript = 1 << 3,
        /// <summary>
        /// The dart
        /// </summary>
        Dart = 1 << 4,
        /// <summary>
        /// The swift
        /// </summary>
        Swift = 1 << 5,
        /// <summary>
        /// The java
        /// </summary>
        Java = 1 << 6,
        /// <summary>
        /// The kotlin
        /// </summary>
        Kotlin = 1 << 7,
    }

    /// <summary>
    /// Class EmitCodeAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitCodeAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public Lang Lang { get; set; }
        /// <summary>
        /// Gets or sets the statements.
        /// </summary>
        /// <value>The statements.</value>
        public string[] Statements { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitCodeAttribute"/> class.
        /// </summary>
        /// <param name="lang">The language.</param>
        /// <param name="statement">The statement.</param>
        public EmitCodeAttribute(Lang lang, string statement) : this(lang, new[] { statement }) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitCodeAttribute"/> class.
        /// </summary>
        /// <param name="lang">The language.</param>
        /// <param name="statements">The statements.</param>
        /// <exception cref="System.ArgumentNullException">Statements</exception>
        public EmitCodeAttribute(Lang lang, string[] statements)
        {
            Lang = lang;
            Statements = statements ?? throw new ArgumentNullException(nameof(Statements));
        }
    }

    /// <summary>
    /// Class EmitCSharp.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitCSharp : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitCSharp"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitCSharp(params string[] statements) : base(Lang.CSharp, statements) { }
    }
    /// <summary>
    /// Class EmitFSharp.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitFSharp : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitFSharp"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitFSharp(params string[] statements) : base(Lang.FSharp, statements) { }
    }
    /// <summary>
    /// Class EmitVb.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitVb : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitVb"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitVb(params string[] statements) : base(Lang.Vb, statements) { }
    }
    /// <summary>
    /// Class EmitTypeScript.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitTypeScript : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitTypeScript"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitTypeScript(params string[] statements) : base(Lang.TypeScript, statements) { }
    }
    /// <summary>
    /// Class EmitDart.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitDart : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitDart"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitDart(params string[] statements) : base(Lang.Dart, statements) { }
    }
    /// <summary>
    /// Class EmitSwift.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitSwift : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitSwift"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitSwift(params string[] statements) : base(Lang.Swift, statements) { }
    }
    /// <summary>
    /// Class EmitJava.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitJava : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitJava"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitJava(params string[] statements) : base(Lang.Java, statements) { }
    }
    /// <summary>
    /// Class EmitKotlin.
    /// Implements the <see cref="ServiceStack.EmitCodeAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.EmitCodeAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EmitKotlin : EmitCodeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitKotlin"/> class.
        /// </summary>
        /// <param name="statements">The statements.</param>
        public EmitKotlin(params string[] statements) : base(Lang.Kotlin, statements) { }
    }
}