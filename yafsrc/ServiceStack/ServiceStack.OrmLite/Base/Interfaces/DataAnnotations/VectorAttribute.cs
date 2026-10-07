// ***********************************************************************
// <copyright file="VectorAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Store a float[] property in a vector column of an RDBMS with vector support, to find the rows most similar to an
/// embedding, e.g:
/// <para>[Vector(1536)] public float[] Embedding { get; set; }</para>
/// Add [Index] to also create a vector index, where the RDBMS has one.
/// </summary>
/// <param name="dimensions">How many values each vector has.</param>
[AttributeUsage(AttributeTargets.Property)]
public class VectorAttribute(int dimensions) : AttributeBase
{
    /// <summary>
    /// How many values each vector has, which is the size of the embeddings of the model that creates them
    /// </summary>
    /// <value>The dimensions.</value>
    public int Dimensions { get; } = dimensions;

    /// <summary>
    /// The distance its vector index is created for, which is the one queries should order by to use it
    /// </summary>
    /// <value>The distance.</value>
    public VectorDistance Distance { get; set; } = VectorDistance.Cosine;

    /// <summary>
    /// The precision of each value, where Half stores them in half the space, e.g. PostgreSQL's halfvec
    /// </summary>
    /// <value>The precision.</value>
    public VectorPrecision Precision { get; set; } = VectorPrecision.Single;

    /// <summary>
    /// The type of the vector index of an indexed [Vector], where the RDBMS has it
    /// </summary>
    /// <value>The type of the index.</value>
    public VectorIndexType IndexType { get; set; } = VectorIndexType.Hnsw;

    /// <summary>
    /// The maximum connections of each vector of an HNSW index (PostgreSQL's m, MariaDB's M), or 0 for the RDBMS's
    /// default. More connections find similar vectors more accurately, in a larger index that's slower to build.
    /// </summary>
    /// <value>The maximum connections.</value>
    public int M { get; set; }

    /// <summary>
    /// How many candidates are compared when an HNSW index is built (PostgreSQL's ef_construction), or 0 for the
    /// RDBMS's default. More are more accurate and slower to build.
    /// </summary>
    /// <value>The candidates compared when building.</value>
    public int EfConstruction { get; set; }

    /// <summary>
    /// How many lists an IVFFlat index divides vectors into (PostgreSQL's lists), or 0 for the RDBMS's default,
    /// e.g. rows / 1000 for up to a million rows
    /// </summary>
    /// <value>The lists.</value>
    public int Lists { get; set; }
}

/// <summary>
/// The precision of the values of a vector
/// </summary>
public enum VectorPrecision
{
    /// <summary>
    /// 32-bit floats
    /// </summary>
    Single,

    /// <summary>
    /// 16-bit floats, which take half the space with less precision
    /// </summary>
    Half
}

/// <summary>
/// The type of a vector index
/// </summary>
public enum VectorIndexType
{
    /// <summary>
    /// A graph of similar vectors, which is accurate and fast to search, and slower to build
    /// </summary>
    Hnsw,

    /// <summary>
    /// Lists of similar vectors, which are faster to build and use less memory, and less accurate to search.
    /// PostgreSQL's pgvector only.
    /// </summary>
    IvfFlat
}

/// <summary>
/// How the similarity of 2 vectors is measured, where smaller is more similar
/// </summary>
public enum VectorDistance
{
    /// <summary>
    /// The angle between 2 vectors, ignoring their length. The usual choice for text embeddings.
    /// </summary>
    Cosine,

    /// <summary>
    /// The straight-line (Euclidean) distance between 2 vectors
    /// </summary>
    L2,

    /// <summary>
    /// The inner (dot) product of 2 vectors negated, so that smaller is more similar. For normalized vectors.
    /// </summary>
    NegativeInnerProduct
}
