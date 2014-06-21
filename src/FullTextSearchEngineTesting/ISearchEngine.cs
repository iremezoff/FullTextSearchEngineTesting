using System;
using System.Collections.Generic;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting
{
    interface ISearchEngine : IDisposable
    {
        Result Run(IEnumerable<QueryItem> searchQuery);
    }
}
