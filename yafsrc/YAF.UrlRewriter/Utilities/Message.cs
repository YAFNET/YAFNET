// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="Message.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Utilities
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
