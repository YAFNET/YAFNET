namespace YAF.Classes.Interfaces
{
  using System.Data;

  /// <summary>
  /// The i style transform.
  /// </summary>
  public interface IStyleTransform
  {
    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    void DecodeStyleByTable(ref DataTable dt);

    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    void DecodeStyleByTable(ref DataTable dt, bool colorOnly);

    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    /// <param name="styleColumns">
    /// The style Columns.
    /// </param>
    /// <param name="colorOnly">
    /// The styleColumns can contain param array to handle several style columns.
    /// </param>
    void DecodeStyleByTable(ref DataTable dt, bool colorOnly, params string[] styleColumns);

    /// <summary>
    /// The decode style by row.
    /// </summary>
    /// <param name="dr">
    /// The dr.
    /// </param>
    void DecodeStyleByRow(ref DataRow dr);

    /// <summary>
    /// The decode style by row.
    /// </summary>
    /// <param name="dr">
    /// The dr.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    void DecodeStyleByRow(ref DataRow dr, bool colorOnly);

    /// <summary>
    /// The decode style by string.
    /// </summary>
    /// <param name="styleStr">
    /// The style str.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    /// <returns>
    /// The decode style by string.
    /// </returns>
    string DecodeStyleByString(string styleStr, bool colorOnly);
  }
}