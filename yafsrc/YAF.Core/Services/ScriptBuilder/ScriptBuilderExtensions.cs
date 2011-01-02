namespace YAF.Core
{
  #region Using

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// the script builder extensions.
  /// </summary>
  public static class ScriptBuilderExtensions
  {
    #region Public Methods

    /// <summary>
    /// Creates a function statement. AddFunction is you want the function statement inserted into the Builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <param name="addFunction">
    /// The add function.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptFunctionContext CreateFunction([NotNull] this IScriptBuilder scriptBuilder, bool addFunction)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      var newFunction = new ScriptFunctionContext(scriptBuilder, new JavaScriptFunction());

      if (addFunction)
      {
        scriptBuilder.Statements.Add(newFunction.ScriptStatement);
      }

      return newFunction;
    }

    /// <summary>
    /// Creates a function statement and adds it to the builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptFunctionContext CreateFunction([NotNull] this IScriptBuilder scriptBuilder)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      return scriptBuilder.CreateFunction(true);
    }

    /// <summary>
    /// Creates a statement and optionally adds it to the builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <param name="addStatement">
    /// The add statement.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext CreateStatement(
      [NotNull] this IScriptBuilder scriptBuilder, bool addStatement)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      var newStatement = new ScriptStatementContext(scriptBuilder, new JavaScriptStatement());

      if (addStatement)
      {
        scriptBuilder.Statements.Add(newStatement.ScriptStatement);
      }

      return newStatement;
    }

    /// <summary>
    /// Creates a statement and adds it to the builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext CreateStatement([NotNull] this IScriptBuilder scriptBuilder)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      return scriptBuilder.CreateStatement(true);
    }

    #endregion
  }
}