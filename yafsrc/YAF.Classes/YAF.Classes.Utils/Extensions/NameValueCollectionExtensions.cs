namespace YAF.Classes.Utils
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;

  using YAF.Classes.Pattern;

  public static class NameValueCollectionExtensions
  {
    /// <summary>
    /// Flattens a <see cref="NameValueCollection"/> to a simple string <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="collection">
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IDictionary<string, string> ToSimpleDictionary([NotNull] this NameValueCollection collection)
    {
      CodeContracts.ArgumentNotNull(collection, "collection");

      return collection.AllKeys.ToDictionary(key => key, key => collection[key]);
    }

    /// <summary>
    /// Gets the value as an <see cref="IEnumerable"/> handling splitting the string if needed.
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>Does not return null.</returns>
    public static IEnumerable<string> GetValueList([NotNull] this NameValueCollection collection, [NotNull] string paramName)
    {
      CodeContracts.ArgumentNotNull(collection, "collection");
      CodeContracts.ArgumentNotNull(paramName, "paramName");

      return collection[paramName] == null ? new List<string>() : collection[paramName].Split(',').AsEnumerable();
    }

    /// <summary>
    /// Gets the first value of <paramref name="paramName"/> in the collection or default (Null).
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static string GetFirstOrDefault([NotNull] this NameValueCollection collection, [NotNull] string paramName)
    {
      CodeContracts.ArgumentNotNull(collection, "collection");
      CodeContracts.ArgumentNotNull(paramName, "paramName");

      return collection.GetValueList(paramName).FirstOrDefault();
    }
  }
}