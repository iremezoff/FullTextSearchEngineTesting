using System.Collections.Generic;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.CreateQueryStrategies
{
    interface ICreateQueryStrategy
    {
        IEnumerable<QueryItem> CreateQuery(string inputString);
    }
}