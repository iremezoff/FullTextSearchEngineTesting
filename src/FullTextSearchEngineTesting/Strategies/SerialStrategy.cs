using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.Strategies
{
    class SerialStrategy : IStrategy
    {
        public CaseResult Run(ISearchEngine engine, Func<int, IEnumerable<QueryItem>> queryGenerator, int times)
        {
            var results = new Result[times];
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < times; i++)
            {
                results[i] = engine.Run(queryGenerator(i));
            }
            sw.Stop();
            if (results.Length == 1)
            {
                results = new[] { results[0], results[0] };
            }
            return new CaseResult()
            {
                MinElapsed = results.Min().ElapsedTime,
                MaxElapsed = results.Max().ElapsedTime,
                FirstElapsed = results.First().ElapsedTime,
                MaxElapsedWithoutFirst = results.Skip(1).Max().ElapsedTime,
                LastElapsed = results.Last().ElapsedTime,
                AvgElasped = results.Average(r => r.ElapsedTime),
                AvgWithountFirstElasped = results.Skip(1).Average(r => r.ElapsedTime),
                MinResults = results.Min().TotalFound,
                MaxResults = results.Max().TotalFound,
                MaxResultsWithoutFirst = results.Skip(1).Max().TotalFound,
                CaseElapsedTime = sw.ElapsedMilliseconds / 1000m
            };
        }
    }
}