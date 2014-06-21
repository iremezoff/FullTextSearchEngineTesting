using FullTextSearchEngineTesting.InfoData;
using FullTextSearchEngineTesting.Strategies;

namespace FullTextSearchEngineTesting
{
    interface ITestCase
    {
        string Name { get; }
        CaseResult Perform(ISearchEngine engine, IStrategy strategy);
    }
}