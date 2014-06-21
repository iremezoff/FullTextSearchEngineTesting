using System.Collections.Generic;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.Cases.ManyTimesDifferentQueries
{
    class ManyTimesDifferentQueriesFullJoinTestCase : ManyTimesDifferentQueryTestCase
    {
        protected override IEnumerable<QueryItem> CreateQuery(int item)
        {
            return new[] { new QueryItem() { Text = Queries[item], Join = TextJoin.Full } };
        }
    }
}