// ***********************************************************************
// <copyright file="MySqlStringConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.MySql.Converters
{
    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class MySqlStringConverter.
    /// Implements the <see cref="StringConverter" />
    /// </summary>
    /// <seealso cref="StringConverter" />
    public class MySqlStringConverter : StringConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlStringConverter"/> class.
        /// </summary>
        public MySqlStringConverter() : base(255) {}

        //https://stackoverflow.com/a/37721151/85785
        /// <summary>
        /// Gets the maximum length of the variable character.
        /// </summary>
        /// <value>The maximum length of the variable character.</value>
        public override int MaxVarCharLength => UseUnicode ? 16383 : 21844;

        /// <summary>
        /// Gets the maximum column definition.
        /// </summary>
        /// <value>The maximum column definition.</value>
        public override string MaxColumnDefinition => "LONGTEXT";
    }

    /// <summary>
    /// Class MySqlCharArrayConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.CharArrayConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.CharArrayConverter" />
    public class MySqlCharArrayConverter : CharArrayConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlCharArrayConverter"/> class.
        /// </summary>
        public MySqlCharArrayConverter() : base(255) { }

        /// <summary>
        /// Gets the maximum column definition.
        /// </summary>
        /// <value>The maximum column definition.</value>
        public override string MaxColumnDefinition => "LONGTEXT";
    }

    /// <summary>
    /// Class MySql55StringConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
    public class MySql55StringConverter : StringConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySql55StringConverter"/> class.
        /// </summary>
        public MySql55StringConverter() : base(255) {}

        //https://stackoverflow.com/a/37721151/85785
        /// <summary>
        /// Gets the maximum length of the variable character.
        /// </summary>
        /// <value>The maximum length of the variable character.</value>
        public override int MaxVarCharLength => UseUnicode ? 16383 : 21844;

        /// <summary>
        /// Gets the maximum column definition.
        /// </summary>
        /// <value>The maximum column definition.</value>
        public override string MaxColumnDefinition => "LONGTEXT";
    }

    /// <summary>
    /// Class MySql55CharArrayConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.CharArrayConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.CharArrayConverter" />
    public class MySql55CharArrayConverter : CharArrayConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySql55CharArrayConverter"/> class.
        /// </summary>
        public MySql55CharArrayConverter() : base(255) { }

        /// <summary>
        /// Gets the maximum column definition.
        /// </summary>
        /// <value>The maximum column definition.</value>
        public override string MaxColumnDefinition => "LONGTEXT";
    }
}