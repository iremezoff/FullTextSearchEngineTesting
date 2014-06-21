using System;
using System.Collections.Generic;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.Strategies
{
    interface IStrategy
    {
        CaseResult Run(ISearchEngine engine, Func<int, IEnumerable<QueryItem>> queryGenerator, int times);
    }
}