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
    /// The create function.
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
    /// The create function.
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
    /// The create statement.
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
    /// The create statement.
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