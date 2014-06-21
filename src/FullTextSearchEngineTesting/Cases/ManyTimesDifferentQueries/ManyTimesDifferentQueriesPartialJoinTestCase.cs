using System.Collections.Generic;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.Cases.ManyTimesDifferentQueries
{
    class ManyTimesDifferentQueriesPartialJoinTestCase : ManyTimesDifferentQueryTestCase
    {
        protected override IEnumerable<QueryItem> CreateQuery(int item)
        {
            return new[]
            {new QueryItem() {Text = Queries[item].Substring(0, Queries[item].Length/2), Join = TextJoin.Partial}};
        }
    }
}