// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace YAF.Web.TagHelpers;

using System.Collections;
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using TagHelperOutputExtensions = Microsoft.AspNetCore.Mvc.TagHelpers.TagHelperOutputExtensions;

/// <summary>
/// <see cref="ITagHelper" /> implementation targeting &lt;select&gt; elements with <c>asp-for</c> and/or
/// <c>asp-items</c> attribute(s).
/// </summary>
[HtmlTargetElement("select", Attributes = ForAttributeName)]
[HtmlTargetElement("select", Attributes = ItemsAttributeName)]
[HtmlTargetElement("select", Attributes = PlaceHolderAttributeName)]
[HtmlTargetElement("select", Attributes = IconItemAttributeName)]
public class ChoicesTagHelper : TagHelper
{
    /// <summary>
    /// For attribute name
    /// </summary>
    private const string ForAttributeName = "asp-for";

    /// <summary>
    /// The items attribute name
    /// </summary>
    private const string ItemsAttributeName = "asp-list";

    /// <summary>
    /// The placeholder attribute name
    /// </summary>
    private const string PlaceHolderAttributeName = "placeholder";

    /// <summary>
    /// The icon item attribute name
    /// </summary>
    private const string IconItemAttributeName = "icon-item";

    /// <summary>
    /// The allow multiple
    /// </summary>
    private bool _allowMultiple;

    /// <summary>
    /// The current values
    /// </summary>
    private ICollection<string> _currentValues;

    /// <summary>
    /// The identifier attribute dot replacement
    /// </summary>
    private const string IdAttributeDotReplacement = "_";

    /// <summary>
    /// Creates a new <see cref="ChoicesTagHelper" />.
    /// </summary>
    /// <param name="generator">The <see cref="IHtmlGenerator" />.</param>
    /// <param name="metadataProvider">The metadata Provider</param>
    public ChoicesTagHelper(IHtmlGenerator generator, IModelMetadataProvider metadataProvider)
    {
        this.Generator = generator;
    }

    /// <inheritdoc />
    public override int Order => -1000;

    /// <summary>
    /// Gets the <see cref="IHtmlGenerator" /> used to generate the <see cref="ChoicesTagHelper" />'s output.
    /// </summary>
    /// <value>The generator.</value>
    protected IHtmlGenerator Generator { get; }

    /// <summary>
    /// Gets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext" /> of the executing view.
    /// </summary>
    /// <value>The view context.</value>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    /// <value>For.</value>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// A collection of <see cref="SelectListItem" /> objects used to populate the &lt;select&gt; element with
    /// &lt;optgroup&gt; and &lt;option&gt; elements.
    /// </summary>
    /// <value>The items.</value>
    [HtmlAttributeName(ItemsAttributeName)]
    public IEnumerable<SelectListItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the icon item.
    /// </summary>
    /// <value>The icon item.</value>
    [HtmlAttributeName(IconItemAttributeName)]
    public string IconItem { get; set; }

    /// <summary>
    /// The name of the &lt;input&gt; element.
    /// </summary>
    /// <value>The name.</value>
    /// <remarks>Passed through to the generated HTML in all cases. Also used to determine whether <see cref="For" /> is
    /// valid with an empty <see cref="ModelExpression.Name" />.</remarks>
    public string Name { get; set; }

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (this.For is null)
        {
            // Informs contained elements that they're running within a targeted <select/> element.
            context.Items[typeof(ChoicesTagHelper)] = null;
            return;
        }

        // Base allowMultiple on the instance or declared type of the expression to avoid a
        // "SelectExpressionNotEnumerable" InvalidOperationException during generation.
        // Metadata.IsEnumerableType is similar but does not take runtime type into account.
        var realModelType = this.For.ModelExplorer.ModelType;
        this._allowMultiple = typeof(string) != realModelType &&
                              typeof(IEnumerable).IsAssignableFrom(realModelType);
        this._currentValues = this.Generator.GetCurrentValues(this.ViewContext, this.For.ModelExplorer, this.For.Name, this._allowMultiple);

        // Whether (not being highly unlikely) we generate anything, could update contained <option/>
        // elements. Provide selected values for <option/> tag helpers.
        var currentValues = this._currentValues is null ? null : new CurrentValues(this._currentValues);
        context.Items[typeof(ChoicesTagHelper)] = currentValues;
    }

    /// <inheritdoc />
    /// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        // Pass through attribute that is also a well-known HTML attribute. Must be done prior to any copying
        // from a TagBuilder.
        if (this.Name != null)
        {
            TagHelperOutputExtensions.CopyHtmlAttribute(output, nameof(this.Name), context);
        }

        // Ensure GenerateSelect() _never_ looks anything up in ViewData.
        var items = this.Items ?? [];

        if (this.For is null)
        {
            var options = this.GenerateGroupsAndOptions(optionLabel: null, selectList: items);
            output.PostContent.AppendHtml(options);
            return;
        }

        // Ensure Generator does not throw due to empty "fullName" if user provided a name attribute.
        IDictionary<string, object> htmlAttributes = null;
        if (string.IsNullOrEmpty(this.For.Name) &&
            string.IsNullOrEmpty(this.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix) &&
            !string.IsNullOrEmpty(this.Name))
        {
            htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                                 {
                                     { "name", this.Name }
                                 };
        }

        var tagBuilder = this.GenerateSelect(
            this.ViewContext,
            this.For.ModelExplorer,
            optionLabel: null,
            expression: this.For.Name,
            selectList: items,
            currentValues: this._currentValues,
            allowMultiple: this._allowMultiple,
            htmlAttributes: htmlAttributes);

        if (tagBuilder == null)
        {
            return;
        }

        TagHelperOutputExtensions.MergeAttributes(output, tagBuilder);
        if (tagBuilder.HasInnerHtml)
        {
            output.PostContent.AppendHtml(tagBuilder.InnerHtml);
        }
    }

    /// <summary>
    /// Generates the select.
    /// </summary>
    /// <param name="viewContext">The view context.</param>
    /// <param name="modelExplorer">The model explorer.</param>
    /// <param name="optionLabel">The option label.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="selectList">The select list.</param>
    /// <param name="allowMultiple">if set to <c>true</c> [allow multiple].</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns>TagBuilder.</returns>
    /// <exception cref="System.ArgumentNullException">viewContext</exception>
    public TagBuilder GenerateSelect(
        ViewContext viewContext,
        ModelExplorer modelExplorer,
        string optionLabel,
        string expression,
        IEnumerable<SelectListItem> selectList,
        bool allowMultiple,
        object htmlAttributes)
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        var currentValues = this.Generator.GetCurrentValues(viewContext, modelExplorer, expression, allowMultiple);
        return this.GenerateSelect(
            viewContext,
            modelExplorer,
            optionLabel,
            expression,
            selectList,
            currentValues,
            allowMultiple,
            htmlAttributes);
    }

    /// <summary>
    /// Generates the select.
    /// </summary>
    /// <param name="viewContext">The view context.</param>
    /// <param name="modelExplorer">The model explorer.</param>
    /// <param name="optionLabel">The option label.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="selectList">The select list.</param>
    /// <param name="currentValues">The current values.</param>
    /// <param name="allowMultiple">if set to <c>true</c> [allow multiple].</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns>TagBuilder.</returns>
    /// <exception cref="System.ArgumentNullException">viewContext</exception>
    /// <exception cref="System.ArgumentNullException">FormatHtmlGenerator_FieldNameCannotBeNullOrEmpty - expression</exception>
    public virtual TagBuilder GenerateSelect(
        ViewContext viewContext,
        ModelExplorer modelExplorer,
        string optionLabel,
        string expression,
        IEnumerable<SelectListItem> selectList,
        ICollection<string> currentValues,
        bool allowMultiple,
        object htmlAttributes)
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        var fullName = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
        var htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);
        if (!IsFullNameValid(fullName, htmlAttributeDictionary))
        {
            throw new ArgumentException(
                $"The name of an HTML field cannot be null or empty. Instead use methods {typeof(IHtmlHelper).FullName}.{nameof(IHtmlHelper.Editor)} or {typeof(IHtmlHelper<>).FullName}.{nameof(IHtmlHelper<object>.EditorFor)} with a non-empty htmlFieldName argument value.",
                nameof(expression));
        }

        // If we got a null selectList, try to use ViewData to get the list of items.
        selectList ??= GetSelectListItems(viewContext, expression);

        // Convert each ListItem to an <option> tag and wrap them with <optgroup> if requested.
        var listItemBuilder = this.GenerateGroupsAndOptions(optionLabel, selectList, currentValues);

        var tagBuilder = new TagBuilder(HtmlTag.Select);
        tagBuilder.InnerHtml.SetHtmlContent(listItemBuilder);
        tagBuilder.MergeAttributes(htmlAttributeDictionary);
        NameAndIdProvider.GenerateId(viewContext, tagBuilder, fullName, IdAttributeDotReplacement);
        if (!string.IsNullOrEmpty(fullName))
        {
            tagBuilder.MergeAttribute("name", fullName, replaceExisting: true);
        }

        if (allowMultiple)
        {
            tagBuilder.MergeAttribute("multiple", "multiple");
        }

        // If there are any errors for a named field, we add the css attribute.
        if (viewContext.ViewData.ModelState.TryGetValue(fullName, out var entry) && entry.Errors.Count > 0)
        {
            tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generates the groups and options.
    /// </summary>
    /// <param name="optionLabel">The option label.</param>
    /// <param name="selectList">The select list.</param>
    /// <returns>IHtmlContent.</returns>
    public IHtmlContent GenerateGroupsAndOptions(string optionLabel, IEnumerable<SelectListItem> selectList)
    {
        return this.GenerateGroupsAndOptions(optionLabel, selectList, currentValues: null);
    }

    /// <summary>
    /// Generates the groups and options.
    /// </summary>
    /// <param name="optionLabel">The option label.</param>
    /// <param name="selectList">The select list.</param>
    /// <param name="currentValues">The current values.</param>
    /// <returns>IHtmlContent.</returns>
    private IHtmlContent GenerateGroupsAndOptions(
        string optionLabel,
        IEnumerable<SelectListItem> selectList,
        ICollection<string> currentValues)
    {
        if (selectList is not IList<SelectListItem> itemsList)
        {
            itemsList = [.. selectList];
        }

        var count = itemsList.Count;
        if (optionLabel != null)
        {
            count++;
        }

        // Short-circuit work below if there's nothing to add.
        if (count == 0)
        {
            return HtmlString.Empty;
        }

        var listItemBuilder = new HtmlContentBuilder(count);

        // Make optionLabel the first item that gets rendered.
        if (optionLabel != null)
        {
            listItemBuilder.AppendLine(
                this.GenerateOption(
                new SelectListItem()
                    {
                        Text = optionLabel,
                        Value = string.Empty,
                        Selected = false
                    },
                currentValues: null));
        }

        // Group items in the SelectList if requested.
        // The worst case complexity of this algorithm is O(number of groups*n).
        // If there aren't any groups, it is O(n) where n is number of items in the list.
        var optionGenerated = new bool[itemsList.Count];
        for (var i = 0; i < itemsList.Count; i++)
        {
            if (optionGenerated[i])
            {
                continue;
            }

            var item = itemsList[i];
            var optGroup = item.Group;
            if (optGroup != null)
            {
                var groupBuilder = new TagBuilder(HtmlTag.OptionGroup);
                if (optGroup.Name != null)
                {
                    groupBuilder.MergeAttribute("label", optGroup.Name);
                }

                if (optGroup.Disabled)
                {
                    groupBuilder.MergeAttribute("disabled", "disabled");
                }

                groupBuilder.InnerHtml.AppendLine();

                for (var j = i; j < itemsList.Count; j++)
                {
                    var groupItem = itemsList[j];

                    if (!optionGenerated[j] &&
                        ReferenceEquals(optGroup, groupItem.Group))
                    {
                        groupBuilder.InnerHtml.AppendLine(this.GenerateOption(groupItem, currentValues));
                        optionGenerated[j] = true;
                    }
                }

                listItemBuilder.AppendLine(groupBuilder);
            }
            else
            {
                listItemBuilder.AppendLine(this.GenerateOption(item, currentValues));
                optionGenerated[i] = true;
            }
        }

        return listItemBuilder;
    }

    /// <summary>
    /// Generates the option.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="text">The text.</param>
    /// <param name="selected">if set to <c>true</c> [selected].</param>
    /// <param name="iconItem">The icon item.</param>
    /// <returns>TagBuilder.</returns>
    private static TagBuilder GenerateOption(SelectListItem item, string text, bool selected, string iconItem)
    {
        var tagBuilder = new TagBuilder(HtmlTag.Option);
        tagBuilder.InnerHtml.AppendHtml(text);

        if (item.Value != null)
        {
            tagBuilder.Attributes["value"] = item.Value;
        }

        if (selected)
        {
            tagBuilder.Attributes["selected"] = "selected";
        }

        if (item.Disabled)
        {
            tagBuilder.Attributes["disabled"] = "disabled";
        }

        tagBuilder.Attributes["data-custom-properties"] = $$"""{ "label": "<i class='fas fa-{{iconItem}} text-secondary'></i>&nbsp;{{item.Text}}" }""";

        return tagBuilder;
    }

    /// <summary>
    /// Generates the option.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="currentValues">The current values.</param>
    /// <returns>IHtmlContent.</returns>
    private IHtmlContent GenerateOption(SelectListItem item, ICollection<string> currentValues)
    {
        var selected = item.Selected;
        if (currentValues != null)
        {
            var value = item.Value ?? item.Text;
            selected = currentValues.Contains(value);
        }

        var tagBuilder = GenerateOption(item, item.Text, selected, this.IconItem);
        return tagBuilder;
    }

    /// <summary>
    /// Determines whether [is full name valid] [the specified full name].
    /// </summary>
    /// <param name="fullName">The full name.</param>
    /// <param name="htmlAttributeDictionary">The HTML attribute dictionary.</param>
    /// <returns><c>true</c> if [is full name valid] [the specified full name]; otherwise, <c>false</c>.</returns>
    private static bool IsFullNameValid(string fullName, IDictionary<string, object> htmlAttributeDictionary)
    {
        return IsFullNameValid(fullName, htmlAttributeDictionary, fallbackAttributeName: "name");
    }

    /// <summary>
    /// Determines whether [is full name valid] [the specified full name].
    /// </summary>
    /// <param name="fullName">The full name.</param>
    /// <param name="htmlAttributeDictionary">The HTML attribute dictionary.</param>
    /// <param name="fallbackAttributeName">Name of the fallback attribute.</param>
    /// <returns><c>true</c> if [is full name valid] [the specified full name]; otherwise, <c>false</c>.</returns>
    private static bool IsFullNameValid(
        string fullName,
        IDictionary<string, object> htmlAttributeDictionary,
        string fallbackAttributeName)
    {
        if (fullName.IsSet())
        {
            return true;
        }

        // fullName==null is normally an error because name="" is not valid in HTML 5.
        if (htmlAttributeDictionary is null)
        {
            return false;
        }

        // Check if user has provided an explicit name attribute.
        // Generalized a bit because other attributes e.g. data-valmsg-for refer to element names.
        htmlAttributeDictionary.TryGetValue(fallbackAttributeName, out var attributeObject);
        var attributeString = Convert.ToString(attributeObject, CultureInfo.InvariantCulture);
        if (string.IsNullOrEmpty(attributeString))
        {
            return false;
        }

        return true;
    }

    // Only need a dictionary if htmlAttributes is non-null. TagBuilder.MergeAttributes() is fine with null.
    /// <summary>
    /// Gets the HTML attribute dictionary or null.
    /// </summary>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
    private static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
    {
        IDictionary<string, object> htmlAttributeDictionary = null;
        if (htmlAttributes != null)
        {
            htmlAttributeDictionary = htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }

        return htmlAttributeDictionary;
    }

    /// <summary>
    /// Gets the select list items.
    /// </summary>
    /// <param name="viewContext">The view context.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;SelectListItem&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">viewContext</exception>
    /// <exception cref="System.InvalidOperationException">FormatHtmlHelper_MissingSelectData</exception>
    /// <exception cref="System.InvalidOperationException">FormatHtmlHelper_WrongSelectDataType</exception>
    private static IEnumerable<SelectListItem> GetSelectListItems(
        ViewContext viewContext,
        string expression)
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        // Method is called only if user did not pass a select list in. They must provide select list items in the
        // ViewData dictionary and definitely not as the Model. (Even if the Model datatype were correct, a
        // <select> element generated for a collection of SelectListItems would be useless.)
        var value = viewContext.ViewData.Eval(expression);

        // First check whether above evaluation was successful and did not match ViewData.Model.
        if (value is null || value == viewContext.ViewData.Model)
        {
            throw new InvalidOperationException("FormatHtmlHelper_MissingSelectData");

            ////throw new InvalidOperationException(Resources.FormatHtmlHelper_MissingSelectData(
            ////    $"IEnumerable<{nameof(GdsSelectListItem)}>",
            ////    expression));
        }

        // Second check the Eval() call returned a collection of SelectListItems.
        if (value is not IEnumerable<SelectListItem> selectList)
        {
            throw new InvalidOperationException("FormatHtmlHelper_WrongSelectDataType");

            ////throw new InvalidOperationException(Resources.FormatHtmlHelper_WrongSelectDataType(
            ////    expression,
            ////    value.GetType().FullName,
            ////    $"IEnumerable<{nameof(GdsSelectListItem)}>"));
        }

        return selectList;
    }
}

/// <summary>
/// Class CurrentValues.
/// </summary>
internal class CurrentValues
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentValues" /> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public CurrentValues(ICollection<string> values)
    {
        Debug.Assert(values != null);
        this.Values = values;
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <value>The values.</value>
    public ICollection<string> Values { get; }

    /// <summary>
    /// Gets or sets the values and encoded values.
    /// </summary>
    /// <value>The values and encoded values.</value>
    public ICollection<string> ValuesAndEncodedValues { get; set; }
}