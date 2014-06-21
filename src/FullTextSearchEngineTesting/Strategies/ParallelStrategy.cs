using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.Strategies
{
    class ParallelStrategy:IStrategy
    {
        private object _locker = new object();
        public CaseResult Run(ISearchEngine engine, Func<int, IEnumerable<QueryItem>> queryGenerator, int times)
        {
            int firstThreadIndex = -1;
            int lastThreadIndex = times;
            var results = new Result[times];
            var threads = new Thread[times];

            int threadCounts = 0;

            for (int i = 0; i < times; i++)
            {
                threads[i] = new Thread(p =>
                {
                    int num = (int)p;
                    Interlocked.CompareExchange(ref firstThreadIndex, num, -1);

                    var query = queryGenerator(num);
                    results[num] = engine.Run(query);
                    Interlocked.Increment(ref threadCounts);

                    Interlocked.CompareExchange(ref lastThreadIndex, num, threadCounts);
                });
            }

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < times; i++)
            {
                threads[i].Start(i);
            }

            for (int i = 0; i < times; i++)
            {
                threads[i].Join();
            }
            sw.Stop();

            var firstQueryResult = results.ElementAt(firstThreadIndex);
            var lastQueryResult = results.ElementAt(lastThreadIndex);

            return new CaseResult()
            {
                MinElapsed = results.Min().ElapsedTime,
                MaxElapsed = results.Max().ElapsedTime,
                FirstElapsed = firstQueryResult.ElapsedTime,
                MaxElapsedWithoutFirst = results.Except(new[] { firstQueryResult }).Max().ElapsedTime,
                LastElapsed = lastQueryResult.ElapsedTime,
                AvgElasped = results.Average(r => r.ElapsedTime),
                AvgWithountFirstElasped = results.Except(new[] { firstQueryResult }).Average(r => r.ElapsedTime),
                MinResults = results.Min().TotalFound,
                MaxResults = results.Max().TotalFound,
                MaxResultsWithoutFirst = results.Except(new[] { firstQueryResult }).Max().TotalFound,
                CaseElapsedTime = sw.ElapsedMilliseconds / 1000m
            };
        }
    }
}
