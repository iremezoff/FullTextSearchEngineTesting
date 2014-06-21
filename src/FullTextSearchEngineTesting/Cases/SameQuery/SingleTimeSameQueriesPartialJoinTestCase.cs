using FullTextSearchEngineTesting.Cases.ManyTimesSameQueries;
using FullTextSearchEngineTesting.CreateQueryStrategies;

namespace FullTextSearchEngineTesting.Cases.SameQuery
{
    class SingleTimeSameQueriesPartialJoinTestCase : ManyTimesSameQueriesTestCase
    {
        public SingleTimeSameQueriesPartialJoinTestCase()
            : base("�����", new PartiaJoinCreateQueryStrategy())
        {
        }
    }
}