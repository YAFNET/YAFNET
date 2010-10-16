namespace YAF.Classes.Utils
{
  using System.Web;
  using System.Web.UI;

  public static class HtmlWriterExtensions
  {
    /// <summary>
    /// The render anchor.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    /// <param name="innerText">
    /// The inner text.
    /// </param>
    public static void RenderAnchor(this HtmlTextWriter writer, string href, string cssClass, string innerText)
    {
      writer.WriteBeginTag("a");
      writer.WriteAttribute("href", href);
      if (cssClass.IsSet())
      {
        writer.WriteAttribute("class", cssClass);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
      writer.Write(innerText);
      writer.WriteEndTag("a");
    }

    /// <summary>
    /// The render img tag.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="src">
    /// The src.
    /// </param>
    /// <param name="alt">
    /// The alt.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    public static void RenderImgTag(this HtmlTextWriter writer, string src, string alt, string title)
    {
      RenderImgTag(writer, src, alt, title, null);
    }

    /// <summary>
    /// The render img tag.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="src">
    /// The src.
    /// </param>
    /// <param name="alt">
    /// The alt.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    public static void RenderImgTag(this HtmlTextWriter writer, string src, string alt, string title, string cssClass)
    {
      // this will output the start of the img element - <img
      writer.WriteBeginTag("img");

      writer.WriteAttribute("src", src);
      writer.WriteAttribute("alt", alt);

      if (title.IsSet())
      {
        writer.WriteAttribute("title", title);
      }

      if (cssClass.IsSet())
      {
        writer.WriteAttribute("class", cssClass);
      }

      writer.Write(HtmlTextWriter.SelfClosingTagEnd);
    }

    #region Render Anchor Begin Functions

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href)
    {
      RenderAnchorBegin(writer, href, null, null);
    }

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href, string cssClass)
    {
      RenderAnchorBegin(writer, href, cssClass, null, null, null);
    }

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href, string cssClass, string title)
    {
      RenderAnchorBegin(writer, href, cssClass, title, null, null);
    }

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="onclick">
    /// The onclick.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href, string cssClass, string title, string onclick, string id)
    {
      writer.WriteBeginTag("a");
      writer.WriteAttribute("href", href);
      if (cssClass.IsSet())
      {
        writer.WriteAttribute("class", cssClass);
      }

      if (title.IsSet())
      {
        writer.WriteAttribute("title", HttpContext.Current.Server.HtmlEncode(title));
      }

      if (onclick.IsSet())
      {
        writer.WriteAttribute("onclick", onclick);
      }

      if (id.IsSet())
      {
        writer.WriteAttribute("id", id);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
    }

    #endregion    

    /// <summary>
    /// The write begin td.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="classId">
    /// The class id.
    /// </param>
    public static void WriteBeginTD(this HtmlTextWriter writer, string classId)
    {
      writer.WriteBeginTag("td");
      if (classId.IsSet())
      {
        writer.WriteAttribute("class", classId);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
    }

    /// <summary>
    /// The write begin td.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public static void WriteBeginTD(this HtmlTextWriter writer)
    {
      WriteBeginTD(writer, null);
    }

    /// <summary>
    /// The write end td.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public static void WriteEndTD(this HtmlTextWriter writer)
    {
      writer.WriteEndTag("td");
    }
  }
}