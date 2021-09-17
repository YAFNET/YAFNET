// ***********************************************************************
// <copyright file="HtmlScriptBlocks.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /* Usages:

    {{#ul {class:'nav'}}} <li>item</li> {{/ul}}
    {{#ul {each:items, class:'nav'}}} <li>{{it}}</li> {{/ul}}
    {{#ul {each:numbers, it:'num', class:'nav'}}} <li>{{num}}</li> {{/ul}}

    {{#ul {if:hasAccess, each:items, where:'Age > 27',
           class:['nav', !disclaimerAccepted ? 'blur' : ''],
           id:`ul-${id}`, selected:true} }}
        {{#li {class: {alt:isOdd(index), active:Name==highlight} }}
            {{Name}}
        {{/li}}
    {{else}}
        <div>no items</div>
    {{/ul}}

    // Equivalent to:

    {{#if hasAccess}}
        {{ items | where => it.Age > 27 | assignTo: items }}
        {{#if !isEmpty(items)}}
            <ul {{ ['nav', !disclaimerAccepted ? 'blur' : ''] | htmlClass }} id="menu-{{id}}">
            {{#each items}}
                <li {{ {alt:isOdd(index), active:Name==highlight} | htmlClass }}>{{Name}}</li>
            {{/each}}
            </ul>
        {{else}}
            <div>no items</div>
        {{/if}}
    {{/if}}

    // Razor:

    @{
        var persons = (items as IEnumerable<Person>)?.Where(x => x.Age > 27);
    }
    @if (hasAccess)
    {
        if (persons?.Any() == true)
        {
            <ul id="menu-@id" class="nav @(!disclaimerAccepted ? "hide" : "")">
                @{
                    var index = 0;
                }
                @foreach (var person in persons)
                {
                    <li class="@(index++ % 2 == 1 ? "alt " : "" )@(person.Name == activeName ? "active" : "")">
                        @person.Name
                    </li>
                }
            </ul>
        }
        else
        {
            <div>no items</div>
        }
    }
    */
    /// <summary>
    /// Class ScriptUlBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptUlBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "ul";
    }
    /// <summary>
    /// Class ScriptOlBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptOlBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "ol";
    }
    /// <summary>
    /// Class ScriptLiBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptLiBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "li";
    }
    /// <summary>
    /// Class ScriptDivBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptDivBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "div";
    }
    /// <summary>
    /// Class ScriptPBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptPBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "p";
    }
    /// <summary>
    /// Class ScriptFormBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptFormBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "form";
    }
    /// <summary>
    /// Class ScriptInputBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptInputBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "input";
    }
    /// <summary>
    /// Class ScriptSelectBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptSelectBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "select";
    }
    /// <summary>
    /// Class ScriptOptionBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptOptionBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "option";
    }
    /// <summary>
    /// Class ScriptTextAreaBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptTextAreaBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "textarea";
    }
    /// <summary>
    /// Class ScriptButtonBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptButtonBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "button";
    }
    /// <summary>
    /// Class ScriptTableBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptTableBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "table";
    }
    /// <summary>
    /// Class ScriptTrBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptTrBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "tr";
    }
    /// <summary>
    /// Class ScriptTdBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptTdBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "td";
    }
    /// <summary>
    /// Class ScriptTHeadBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptTHeadBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "thead";
    }
    /// <summary>
    /// Class ScriptTBodyBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptTBodyBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "tbody";
    }
    /// <summary>
    /// Class ScriptTFootBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptTFootBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "tfoot";
    }
    /// <summary>
    /// Class ScriptDlBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptDlBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "dl";
    }
    /// <summary>
    /// Class ScriptDtBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptDtBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "dt";
    }
    /// <summary>
    /// Class ScriptDdBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptDdBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "dd";
    }

    //Don't emit new line on in-line elements
    /// <summary>
    /// Class ScriptSpanBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptSpanBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "span";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptABlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptABlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "a";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptImgBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptImgBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "img";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptEmBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptEmBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "em";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptBBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptBBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "b";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptIBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptIBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "i";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptStrongBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptStrongBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "strong";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptScriptBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptScriptBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "script";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptStyleBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptStyleBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "style";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptLinkBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptLinkBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "link";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }
    /// <summary>
    /// Class ScriptMetaBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptHtmlBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptHtmlBlock" />
    public class ScriptMetaBlock : ScriptHtmlBlock
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public override string Tag => "meta";
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public override string Suffix => "";
    }

    /// <summary>
    /// Class ScriptHtmlBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptBlock" />
    public abstract class ScriptHtmlBlock : ScriptBlock
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Tag;
        /// <summary>
        /// Parse Body using Specified Language. Uses host language if unspecified.
        /// </summary>
        /// <value>The body.</value>
        public override ScriptLanguage Body => ScriptTemplate.Language;

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public abstract string Tag { get; }

        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public virtual string Suffix { get; } = Environment.NewLine;

        /// <summary>
        /// Write as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="System.NotSupportedException">'where' should be a string expression but instead found '{oWhere.GetType().Name}'</exception>
        public override async Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
        {
            var htmlAttrs = await block.Argument.GetJsExpressionAndEvaluateAsync(scope) as Dictionary<string, object>;
            var hasEach = false;
            IEnumerable each = null;
            var binding = "it";
            var hasExplicitBinding = false;
            JsToken where = null;

            if (htmlAttrs != null)
            {
                if (htmlAttrs.TryGetValue("if", out var oIf))
                {
                    if (Script.DefaultScripts.isFalsy(oIf))
                        return;
                    htmlAttrs.Remove("if");
                }

                if (htmlAttrs.TryGetValue(nameof(where), out var oWhere))
                {
                    if (oWhere is not string whereExpr)
                        throw new NotSupportedException($"'where' should be a string expression but instead found '{oWhere.GetType().Name}'");

                    where = whereExpr.GetCachedJsExpression(scope);
                    htmlAttrs.Remove(nameof(where));
                }

                if (htmlAttrs.TryGetValue(nameof(each), out var oEach))
                {
                    hasEach = true;
                    htmlAttrs.Remove(nameof(each));
                }
                each = oEach as IEnumerable;

                if (htmlAttrs.TryGetValue("it", out var oIt) && oIt is string it)
                {
                    binding = it;
                    hasExplicitBinding = true;
                    htmlAttrs.Remove("it");
                }

                if (htmlAttrs.TryGetValue("class", out var oClass))
                {
                    var cls = scope.Context.HtmlMethods.htmlClassList(oClass);
                    if (string.IsNullOrEmpty(cls))
                        htmlAttrs.Remove("class");
                    else
                        htmlAttrs["class"] = cls;
                }
            }

            var attrString = scope.Context.HtmlMethods.htmlAttrsList(htmlAttrs);

            if (HtmlScripts.VoidElements.Contains(Tag)) //e.g. img, input, br, etc
            {
                await scope.OutputStream.WriteAsync($"<{Tag}{attrString}>{Suffix}", token).ConfigAwait();
            }
            else
            {
                if (hasEach)
                {
                    var hasElements = each != null && each.GetEnumerator().MoveNext();
                    if (hasElements)
                    {
                        await scope.OutputStream.WriteAsync($"<{Tag}{attrString}>{Suffix}", token).ConfigAwait();

                        var index = 0;
                        var whereIndex = 0;
                        foreach (var element in each)
                        {
                            // Add all properties into scope if called without explicit in argument
                            var scopeArgs = !hasExplicitBinding && CanExportScopeArgs(element)
                                ? element.ToObjectDictionary()
                                : new Dictionary<string, object>();

                            scopeArgs[binding] = element;
                            scopeArgs[nameof(index)] = AssertWithinMaxQuota(whereIndex++);
                            var itemScope = scope.ScopeWithParams(scopeArgs);

                            if (where != null)
                            {
                                var result = await @where.EvaluateToBoolAsync(itemScope);
                                if (!result)
                                    continue;
                            }

                            itemScope.ScopedParams[nameof(index)] = AssertWithinMaxQuota(index++);

                            await WriteBodyAsync(itemScope, block, token).ConfigAwait();
                        }

                        await scope.OutputStream.WriteAsync($"</{Tag}>{Suffix}", token).ConfigAwait();
                    }
                    else
                    {
                        await WriteElseAsync(scope, block.ElseBlocks, token).ConfigAwait();
                    }
                }
                else
                {
                    await scope.OutputStream.WriteAsync($"<{Tag}{attrString}>{Suffix}", token).ConfigAwait();
                    await WriteBodyAsync(scope, block, token).ConfigAwait();
                    await scope.OutputStream.WriteAsync($"</{Tag}>{Suffix}", token).ConfigAwait();
                }
            }
        }
    }

    /// <summary>
    /// Class HtmlScriptBlocks.
    /// Implements the <see cref="ServiceStack.Script.IScriptPlugin" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.IScriptPlugin" />
    public class HtmlScriptBlocks : IScriptPlugin
    {
        /// <summary>
        /// Usages: {{#ul {each:items, class:'nav'} }} <li>{{it}}</li> {{/ul}}
        /// </summary>
        /// <param name="context">The context.</param>

        public void Register(ScriptContext context)
        {
            context.ScriptBlocks.AddRange(new ScriptBlock[] {
                new ScriptUlBlock(),
                new ScriptOlBlock(),
                new ScriptLiBlock(),
                new ScriptDivBlock(),
                new ScriptPBlock(),
                new ScriptFormBlock(),
                new ScriptInputBlock(),
                new ScriptSelectBlock(),
                new ScriptOptionBlock(),
                new ScriptTextAreaBlock(),
                new ScriptButtonBlock(),
                new ScriptTableBlock(),
                new ScriptTrBlock(),
                new ScriptTdBlock(),
                new ScriptTHeadBlock(),
                new ScriptTBodyBlock(),
                new ScriptTFootBlock(),
                new ScriptDlBlock(),
                new ScriptDtBlock(),
                new ScriptDdBlock(),
                new ScriptSpanBlock(),
                new ScriptABlock(),
                new ScriptImgBlock(),
                new ScriptEmBlock(),
                new ScriptBBlock(),
                new ScriptIBlock(),
                new ScriptStrongBlock(),
                new ScriptScriptBlock(),
                new ScriptStyleBlock(),
                new ScriptLinkBlock(),
                new ScriptMetaBlock(),
            });
        }
    }

}