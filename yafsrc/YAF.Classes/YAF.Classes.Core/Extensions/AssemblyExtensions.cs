namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;

  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// The assembly extensions.
  /// </summary>
  public static class AssemblyExtensions
  {
    #region Public Methods

    /// <summary>
    /// The find modules.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    /// <param name="moduleNamespace">
    /// The module namespace.
    /// </param>
    /// <param name="moduleBaseInterface">
    /// The module base interface.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IList<Type> FindModules(
      [NotNull] this IEnumerable<Assembly> assemblies, 
      [NotNull] string moduleNamespace, 
      [NotNull] string moduleBaseInterface)
    {
      CodeContracts.ArgumentNotNull(assemblies, "assemblies");
      CodeContracts.ArgumentNotNull(moduleNamespace, "moduleNamespace");
      CodeContracts.ArgumentNotNull(moduleBaseInterface, "moduleBaseInterface");

      var moduleClassTypes = new List<Type>();
      var baseInterface = Type.GetType(moduleBaseInterface);

      // get classes...
      foreach (
        var types in
          assemblies.OfType<Assembly>().Select(
            a =>
            a.GetExportedTypes().Where(t => t.Namespace != null && !t.IsAbstract && t.Namespace.Equals(moduleNamespace))
              .ToList()))
      {
        moduleClassTypes.AddRange(types.Where(modClass => modClass.GetInterfaces().Any(i => i.Equals(baseInterface))));
      }

      return moduleClassTypes;
    }

    #endregion
  }
}