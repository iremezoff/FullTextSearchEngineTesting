using FullTextSearchEngineTesting.CreateQueryStrategies;

namespace FullTextSearchEngineTesting.Cases.ManyTimesSameQueries
{
    class ManyTimesSameQueriesPartialJoinTestCase : ManyTimesSameQueriesTestCase
    {
        public ManyTimesSameQueriesPartialJoinTestCase()
            : base("��������", new PartiaJoinCreateQueryStrategy())
        {
        }
    }
}