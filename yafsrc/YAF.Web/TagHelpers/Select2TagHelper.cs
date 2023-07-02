// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace YAF.Web.TagHelpers;

using System.Collections;
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using TagHelperOutputExtensions = Microsoft.AspNetCore.Mvc.TagHelpers.TagHelperOutputExtensions;

/// <summary>
/// <see cref="ITagHelper"/> implementation targeting &lt;select&gt; elements with <c>asp-for</c> and/or
/// <c>asp-items</c> attribute(s).
/// </summary>
[HtmlTargetElement("select", Attributes = ForAttributeName)]
[HtmlTargetElement("select", Attributes = ItemsAttributeName)]
[HtmlTargetElement("select", Attributes = PlaceHolderAttributeName)]
[HtmlTargetElement("select", Attributes = IconItemAttributeName)]
public class Select2TagHelper : TagHelper
{
    private const string ForAttributeName = "asp-for";

    private const string ItemsAttributeName = "asp-list";

    private const string PlaceHolderAttributeName = "placeholder";

    private const string IconItemAttributeName = "icon-item";

    private bool _allowMultiple;

    private ICollection<string> _currentValues;

    private IModelMetadataProvider _metadataProvider;

    private const string IdAttributeDotReplacement = "_";

    /// <summary>
    /// Creates a new <see cref="Select2TagHelper"/>.
    /// </summary>
    /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
    /// <param name="metadataProvider">The metadata Provider</param>
    public Select2TagHelper(IHtmlGenerator generator, IModelMetadataProvider metadataProvider)
    {
        this.Generator = generator;
        this._metadataProvider = metadataProvider;
    }

    /// <inheritdoc />
    public override int Order => -1000;

    /// <summary>
    /// Gets the <see cref="IHtmlGenerator"/> used to generate the <see cref="Select2TagHelper"/>'s output.
    /// </summary>
    protected IHtmlGenerator Generator { get; }

    /// <summary>
    /// Gets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// A collection of <see cref="SelectListItem"/> objects used to populate the &lt;select&gt; element with
    /// &lt;optgroup&gt; and &lt;option&gt; elements.
    /// </summary>
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
    /// <remarks>
    /// Passed through to the generated HTML in all cases. Also used to determine whether <see cref="For"/> is
    /// valid with an empty <see cref="ModelExpression.Name"/>.
    /// </remarks>
    public string Name { get; set; }

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (this.For is null)
        {
            // Informs contained elements that they're running within a targeted <select/> element.
            context.Items[typeof(Select2TagHelper)] = null;
            return;
        }

        // Base allowMultiple on the instance or declared type of the expression to avoid a
        // "SelectExpressionNotEnumerable" InvalidOperationException during generation.
        // Metadata.IsEnumerableType is similar but does not take runtime type into account.
        var realModelType = this.For.ModelExplorer.ModelType;
        this._allowMultiple = typeof(string) != realModelType &&
                              typeof(IEnumerable).IsAssignableFrom(realModelType);
        this._currentValues = this.Generator.GetCurrentValues(this.ViewContext, this.For.ModelExplorer, this.For.Name, this._allowMultiple);

        // Whether or not (not being highly unlikely) we generate anything, could update contained <option/>
        // elements. Provide selected values for <option/> tag helpers.
        var currentValues = this._currentValues is null ? null : new CurrentValues(this._currentValues);
        context.Items[typeof(Select2TagHelper)] = currentValues;
    }

    /// <inheritdoc />
    /// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (output is null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        // Pass through attribute that is also a well-known HTML attribute. Must be done prior to any copying
        // from a TagBuilder.
        if (this.Name != null)
        {
            TagHelperOutputExtensions.CopyHtmlAttribute(output, nameof(this.Name), context);
        }

        // Ensure GenerateSelect() _never_ looks anything up in ViewData.
        var items = this.Items ?? Enumerable.Empty<SelectListItem>();

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
                                     { "name", this.Name },
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

        if (tagBuilder != null)
        {
            TagHelperOutputExtensions.MergeAttributes(output, tagBuilder);
            if (tagBuilder.HasInnerHtml)
            {
                output.PostContent.AppendHtml(tagBuilder.InnerHtml);
            }
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
        if (viewContext is null)
        {
            throw new ArgumentNullException(nameof(viewContext));
        }

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
        if (viewContext is null)
        {
            throw new ArgumentNullException(nameof(viewContext));
        }

        var fullName = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
        var htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);
        if (!IsFullNameValid(fullName, htmlAttributeDictionary))
        {
            throw new ArgumentNullException("FormatHtmlGenerator_FieldNameCannotBeNullOrEmpty", nameof(expression));

            ////throw new ArgumentException(
            ////    Resources.FormatHtmlGenerator_FieldNameCannotBeNullOrEmpty(
            ////        typeof(IHtmlHelper).FullName,
            ////        nameof(IHtmlHelper.Editor),
            ////        typeof(IHtmlHelper<>).FullName,
            ////        nameof(IHtmlHelper<object>.EditorFor),
            ////        "htmlFieldName"),
            ////    nameof(expression));
        }

        // If we got a null selectList, try to use ViewData to get the list of items.
        selectList ??= GetSelectListItems(viewContext, expression);

        // Convert each ListItem to an <option> tag and wrap them with <optgroup> if requested.
        var listItemBuilder = this.GenerateGroupsAndOptions(optionLabel, selectList, currentValues);

        var tagBuilder = new TagBuilder("select");
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
        if (viewContext.ViewData.ModelState.TryGetValue(fullName, out var entry))
        {
            if (entry.Errors.Count > 0)
            {
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }
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

    private IHtmlContent GenerateGroupsAndOptions(
        string optionLabel,
        IEnumerable<SelectListItem> selectList,
        ICollection<string> currentValues)
    {
        if (selectList is not IList<SelectListItem> itemsList)
        {
            itemsList = selectList.ToList();
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
                        Selected = false,
                    },
                currentValues: null));
        }

        // Group items in the SelectList if requested.
        // The worst case complexity of this algorithm is O(number of groups*n).
        // If there aren't any groups, it is O(n) where n is number of items in the list.
        var optionGenerated = new bool[itemsList.Count];
        for (var i = 0; i < itemsList.Count; i++)
        {
            if (!optionGenerated[i])
            {
                var item = itemsList[i];
                var optGroup = item.Group;
                if (optGroup != null)
                {
                    var groupBuilder = new TagBuilder("optgroup");
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
        }

        return listItemBuilder;
    }

    private static TagBuilder GenerateOption(SelectListItem item, string text, bool selected, string iconItem)
    {
        var tagBuilder = new TagBuilder("option");
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

        tagBuilder.Attributes["data-content"] = $"<span class='select2-image-select-icon'><i class='fas fa-{iconItem} fa-fw text-secondary'></i>&nbsp;{item.Text}</span>";

        return tagBuilder;
    }

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

    private static bool IsFullNameValid(string fullName, IDictionary<string, object> htmlAttributeDictionary)
    {
        return IsFullNameValid(fullName, htmlAttributeDictionary, fallbackAttributeName: "name");
    }

    private static bool IsFullNameValid(
        string fullName,
        IDictionary<string, object> htmlAttributeDictionary,
        string fallbackAttributeName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
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
        }

        return true;
    }

    // Only need a dictionary if htmlAttributes is non-null. TagBuilder.MergeAttributes() is fine with null.
    private static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
    {
        IDictionary<string, object> htmlAttributeDictionary = null;
        if (htmlAttributes != null)
        {
            htmlAttributeDictionary = htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }

        return htmlAttributeDictionary;
    }

    private static IEnumerable<SelectListItem> GetSelectListItems(
        ViewContext viewContext,
        string expression)
    {
        if (viewContext is null)
        {
            throw new ArgumentNullException(nameof(viewContext));
        }

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

internal class CurrentValues
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentValues"/> class.
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