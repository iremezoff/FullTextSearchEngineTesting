using System.Collections.Generic;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.CreateQueryStrategies
{
    class PartiaJoinCreateQueryStrategy : ICreateQueryStrategy
    {
        public IEnumerable<QueryItem> CreateQuery(string inputString)
        {
            return new[] { new QueryItem() { Text = inputString, Join = TextJoin.Partial } };
        }
    }
}