namespace YAF.Classes.Utils
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;

  public static class NameValueCollectionExtensions
  {
    /// <summary>
    /// Flattens a <see cref="NameValueCollection"/> to a simple string <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="collection">
    /// </param>
    /// <returns>
    /// </returns>
    public static IDictionary<string, string> ToSimpleDictionary(this NameValueCollection collection)
    {
      return collection.AllKeys.ToDictionary(key => key, key => collection[key]);
    }    

    /// <summary>
    /// Gets the value as an <see cref="IEnumerable"/> handling splitting the string if needed.
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>Does not return null.</returns>
    public static IEnumerable<string> GetValueList(this NameValueCollection collection, string paramName)
    {
      return collection[paramName] == null ? new List<string>() : collection[paramName].Split(',').AsEnumerable();
    }

    /// <summary>
    /// Gets the first value of <paramref name="paramName"/> in the collection or default (Null).
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static string GetFirstOrDefault(this NameValueCollection collection, string paramName)
    {
      return collection.GetValueList(paramName).FirstOrDefault();
    }
  }
}