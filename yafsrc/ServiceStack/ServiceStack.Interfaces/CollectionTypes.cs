// ***********************************************************************
// <copyright file="CollectionTypes.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /*
    * Useful collection DTO's that provide pretty Xml output for collection types, e.g.
    * 
    * ArrayOfIntId Ids { get; set; }		
    * ... =>
    * 
    * <Ids>
    *   <Id>1</Id>
    *   <Id>2</Id>
    *   <Id>3</Id>
    * <Ids>
    */

    /// <summary>
    /// Class ArrayOfString.
    /// Implements the <see cref="string" />
    /// </summary>
    /// <seealso cref="string" />
    [CollectionDataContract(ItemName = "String")]
    public partial class ArrayOfString : List<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfString"/> class.
        /// </summary>
        public ArrayOfString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfString"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfString(IEnumerable<string> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfString"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfString(params string[] args) : base(args) { }
    }

    /// <summary>
    /// Class ArrayOfStringId.
    /// Implements the <see cref="string" />
    /// </summary>
    /// <seealso cref="string" />
    [CollectionDataContract(ItemName = "Id")]
    public partial class ArrayOfStringId : List<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfStringId"/> class.
        /// </summary>
        public ArrayOfStringId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfStringId"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfStringId(IEnumerable<string> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfStringId"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfStringId(params string[] args) : base(args) { }
    }

    /// <summary>
    /// Class ArrayOfGuid.
    /// Implements the <see cref="Guid" />
    /// </summary>
    /// <seealso cref="Guid" />
    [CollectionDataContract(ItemName = "Guid")]
    public partial class ArrayOfGuid : List<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfGuid"/> class.
        /// </summary>
        public ArrayOfGuid()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfGuid"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfGuid(IEnumerable<Guid> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfGuid"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfGuid(params Guid[] args) : base(args) { }
    }

    /// <summary>
    /// Class ArrayOfGuidId.
    /// Implements the <see cref="Guid" />
    /// </summary>
    /// <seealso cref="Guid" />
    [CollectionDataContract(ItemName = "Id")]
    public partial class ArrayOfGuidId : List<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfGuidId"/> class.
        /// </summary>
        public ArrayOfGuidId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfGuidId"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfGuidId(IEnumerable<Guid> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfGuidId"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfGuidId(params Guid[] args) : base(args) { }
    }

    /// <summary>
    /// Class ArrayOfLong.
    /// Implements the <see cref="long" />
    /// </summary>
    /// <seealso cref="long" />
    [CollectionDataContract(ItemName = "Long")]
    public partial class ArrayOfLong : List<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfLong"/> class.
        /// </summary>
        public ArrayOfLong()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfLong"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfLong(IEnumerable<long> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfLong"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfLong(params long[] args) : base(args) { }
    }

    /// <summary>
    /// Class ArrayOfLongId.
    /// Implements the <see cref="long" />
    /// </summary>
    /// <seealso cref="long" />
    [CollectionDataContract(ItemName = "Id")]
    public partial class ArrayOfLongId : List<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfLongId"/> class.
        /// </summary>
        public ArrayOfLongId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfLongId"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfLongId(IEnumerable<long> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfLongId"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfLongId(params long[] args) : base(args) { }
    }

    /// <summary>
    /// Class ArrayOfInt.
    /// Implements the <see cref="int" />
    /// </summary>
    /// <seealso cref="int" />
    [CollectionDataContract(ItemName = "Int")]
    public partial class ArrayOfInt : List<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfInt"/> class.
        /// </summary>
        public ArrayOfInt()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfInt"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfInt(IEnumerable<int> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfInt"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfInt(params int[] args) : base(args) { }
    }

    /// <summary>
    /// Class ArrayOfIntId.
    /// Implements the <see cref="int" />
    /// </summary>
    /// <seealso cref="int" />
    [CollectionDataContract(ItemName = "Id")]
    public partial class ArrayOfIntId : List<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfIntId"/> class.
        /// </summary>
        public ArrayOfIntId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfIntId"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ArrayOfIntId(IEnumerable<int> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfIntId"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArrayOfIntId(params int[] args) : base(args) { }
    }

}