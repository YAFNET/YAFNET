// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="RewriteProcessing.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace YAF.UrlRewriter;

/// <summary>
/// Processing flag. Tells the rewriter how to continue processing (or not).
/// </summary>
public enum RewriteProcessing
{
    /// <summary>
    /// Continue processing at the next rule.
    /// </summary>
    ContinueProcessing,

    /// <summary>
    /// Halt processing.
    /// </summary>
    StopProcessing,

    /// <summary>
    /// Restart processing at the first rule.
    /// </summary>
    RestartProcessing
}