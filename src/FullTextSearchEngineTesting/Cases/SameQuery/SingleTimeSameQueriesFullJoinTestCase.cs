using FullTextSearchEngineTesting.CreateQueryStrategies;

namespace FullTextSearchEngineTesting.Cases.SameQuery
{
    class SingleTimeSameQueriesFullJoinTestCase : SingleTimeSameQueriesTestCase
    {
        public SingleTimeSameQueriesFullJoinTestCase()
            : base("�������������", new FullJoinCreateQueryStrategy())
        {
        }
    }
}