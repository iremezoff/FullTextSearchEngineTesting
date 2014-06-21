using FullTextSearchEngineTesting.CreateQueryStrategies;

namespace FullTextSearchEngineTesting.Cases.ManyTimesSameQueries
{
    class ManyTimesSameQueriesFullJoinTestCase : ManyTimesSameQueriesTestCase
    {
        public ManyTimesSameQueriesFullJoinTestCase()
            : base("Молодёжная", new FullJoinCreateQueryStrategy())
        {
        }
    }
}