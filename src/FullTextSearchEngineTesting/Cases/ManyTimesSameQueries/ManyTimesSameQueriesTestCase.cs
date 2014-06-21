using FullTextSearchEngineTesting.Cases.SameQuery;
using FullTextSearchEngineTesting.CreateQueryStrategies;

namespace FullTextSearchEngineTesting.Cases.ManyTimesSameQueries
{
    abstract class ManyTimesSameQueriesTestCase : SingleTimeSameQueriesTestCase
    {
        protected ManyTimesSameQueriesTestCase(string inputString, ICreateQueryStrategy createQueryStrategy)
            : base(inputString, createQueryStrategy)
        {
            Times = 10;
            InputString = inputString;
        }
    }
}