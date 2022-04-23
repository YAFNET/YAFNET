namespace YAF.UrlRewriter.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

using YAF.UrlRewriter.Conditions;

/// <summary>
/// Extension methods for Lists.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Returns true provided all the conditions in the list are met for the given context.
    /// </summary>
    /// <param name="conditions">The list of conditions</param>
    /// <param name="context">The rewrite context</param>
    /// <returns>True if all the conditions are met</returns>
    public static bool IsMatch(this IList<IRewriteCondition> conditions, IRewriteContext context)
    {
        if (conditions == null)
        {
            throw new ArgumentNullException(nameof(conditions));
        }

        // Ensure all the conditions are met, i.e. return false if any are not met.
        return conditions.All(condition => condition.IsMatch(context));
    }
}