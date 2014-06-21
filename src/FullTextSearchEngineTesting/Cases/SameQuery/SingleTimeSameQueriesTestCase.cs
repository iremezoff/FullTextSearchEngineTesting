using FullTextSearchEngineTesting.CreateQueryStrategies;
using FullTextSearchEngineTesting.InfoData;
using FullTextSearchEngineTesting.Strategies;

namespace FullTextSearchEngineTesting.Cases.SameQuery
{
    internal abstract class SingleTimeSameQueriesTestCase : ITestCase
    {
        private readonly ICreateQueryStrategy _createQueryStrategy;

        protected int Times = 1;
        protected string InputString;

        protected SingleTimeSameQueriesTestCase(string inputString, ICreateQueryStrategy createQueryStrategy)
        {
            InputString = inputString;
            _createQueryStrategy = createQueryStrategy;
        }

        public string Name
        {
            get
            {

                string name = Times + " запросов, ";

                if (_createQueryStrategy.GetType().Name.Contains("FullJoin"))
                    name += "полное вхождение";
                else
                {
                    name += "частичное вхождение";
                }

                return name;
            }
        }

        public virtual CaseResult Perform(ISearchEngine engine, IStrategy strategy)
        {
            return strategy.Run(engine, i => _createQueryStrategy.CreateQuery(InputString), Times);
        }
    }
}