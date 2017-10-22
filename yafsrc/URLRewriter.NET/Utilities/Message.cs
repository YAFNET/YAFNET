// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Utilities
{
    /// <summary>
    /// Message ids
    /// </summary>
    internal enum Message
    {
        AttributeNotAllowed,
        ElementNotAllowed,
        ElementNoAttributes,
        ElementNoElements,
        MappedValuesNotAllowed,
        ValueOfProcessingAttribute,
        AttributeRequired,
        FullTypeNameRequiresAssemblyName,
        AssemblyNameRequired,
        TypeNameRequired,
        MapAlreadyDefined,
        InvalidTypeSpecified,
        InputIsNotHex,
        AddressesNotOfSameType,
        ProductName,
        StoppingBecauseOfRule,
        RestartingBecauseOfRule,
        ResultNotFound,
        CallingErrorHandler,
        RewritingXtoY,
        RedirectingXtoY,
        TooManyRestarts,
        InvalidHttpStatusCode,
        StartedProcessing,
        AttributeCannotBeBlank,
        InvalidBooleanAttribute,
        InvalidIntegerAttribute,
        MissingConfigFileSection,
        MappingNotFound,
        TransformFunctionNotFound
    }
}
