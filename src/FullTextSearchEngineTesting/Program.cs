using System;
using System.Diagnostics;
using FullTextSearchEngineTesting.Cases.ManyDifferentFPPQuery;
using FullTextSearchEngineTesting.Cases.ManyTimesDifferentQueries;
using FullTextSearchEngineTesting.Cases.ManyTimesDifferentRandomQueries;
using FullTextSearchEngineTesting.Cases.ManyTimesRealQuery;
using FullTextSearchEngineTesting.Cases.ManyTimesSameQueries;
using FullTextSearchEngineTesting.Cases.SameQuery;
using FullTextSearchEngineTesting.Engines;
using FullTextSearchEngineTesting.Strategies;

namespace FullTextSearchEngineTesting
{
    class Program
    {
        static void Main()
        {
            var engines = new ISearchEngine[]
                {
                    new LuceneSearchEngine(),
                    new MssqlFullTextSearchEngine(),
                    new MssqlLikeSearchEngine(),
                    new SphinxSearchEngine(),
                    new ElasticSearchEngine()
                };

            var cases = new ITestCase[]
                {
                    new SingleTimeSameQueriesFullJoinTestCase(),
                    new SingleTimeSameQueriesPartialJoinTestCase(),
                    new ManyTimesSameQueriesFullJoinTestCase(),
                    new ManyTimesSameQueriesPartialJoinTestCase(),
                    new ManyTimesDifferentQueriesFullJoinTestCase(),
                    new ManyTimesDifferentQueriesPartialJoinTestCase(),
                    new ManyTimesDifferentQueriesFPPTestCase(),
                    new ManyTimesDifferentRandomQueriesTestCase(),
                    new ManyTimesRealQueriesTestCase()
                };

            var strategies = new IStrategy[]
                {
                    new ParallelStrategy(), new SerialStrategy()
                };

            for (int i = 0; i < engines.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i, engines[i].GetType().Name);
            }

            int choise;

            while (!int.TryParse(Console.ReadLine(), out choise) || choise < 0 ||
                   choise > engines.Length - 1) { }

            var engine = engines[choise];

            for (int i = 0; i < cases.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i, cases[i].Name);
            }

            while (!int.TryParse(Console.ReadLine(), out choise) || choise < 0 ||
                  choise > cases.Length - 1) { }

            var testCase = cases[choise];

            for (int i = 0; i < strategies.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i, strategies[i].GetType().Name);
            }

            while (!int.TryParse(Console.ReadLine(), out choise) || choise < 0 ||
                 choise > cases.Length - 1) { }

            var strategy = strategies[choise];

            var sw = new Stopwatch();
            sw.Start();
            var result = testCase.Perform(engine, strategy);
            sw.Stop();

            Console.WriteLine(testCase.Name);
            Console.WriteLine("MinElapsed: " + result.MinElapsed);
            Console.WriteLine("MaxElapsed: " + result.MaxElapsed);
            Console.WriteLine("AvgElapsed: " + result.AvgElasped);
            Console.WriteLine("AvgElapsed without first: " + result.AvgWithountFirstElasped);
            Console.WriteLine("MaxElapsed without first: " + result.MaxElapsedWithoutFirst);
            Console.WriteLine("First elapsed: " + result.FirstElapsed);
            Console.WriteLine("Last elapsed: " + result.LastElapsed);
            Console.WriteLine("Results for min elapsed: " + result.MinResults);
            Console.WriteLine("Results for max elapsed: " + result.MaxResults);
            Console.WriteLine("Results for max elapsed without first: " + result.MaxResultsWithoutFirst);
            Console.WriteLine("Elapsed time for test: " + result.CaseElapsedTime);

            Console.Read();
        }
    }
}
