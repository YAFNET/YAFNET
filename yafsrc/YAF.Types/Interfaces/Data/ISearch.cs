namespace YAF.Types.Interfaces.Data
{
    using System.Collections.Generic;

    using YAF.Types.Models;

    public interface ISearch
    {
        IEnumerable<SearchResult> Execute(ISearchContext context);
    }
}