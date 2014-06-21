using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullTextSearchEngineTesting.InfoData;
using Nest;

namespace FullTextSearchEngineTesting.Engines
{
    class ElasticSearchEngine : ISearchEngine
    {
        private ConnectionSettings _config;

        public ElasticSearchEngine()
        {
            _config = new ConnectionSettings(new Uri("http://localhost:9200"));
            _config.SetDefaultIndex("fias2");
        }
        public void Dispose()
        { }

        private static string Prepare(QueryItem item)
        {
            return item.Join == TextJoin.Partial
                ? item.Text + "*"
                : item.Text;
        }

        public Result Run(IEnumerable<QueryItem> searchQuery)
        {
            string queryStr = string.Join(" AND ", searchQuery.Select(Prepare));

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var client = new ElasticClient(_config);
            var response =
                client.Search<SearchResult>(
                    d => d.Type("addr").Query(q => q.QueryString(qd => qd.OnField(t => t.Fullname).Query(queryStr))));
            foreach (var topDoc in response.Documents)
            {

            }
            sw.Stop();

            return new Result() { ElapsedTime = sw.ElapsedMilliseconds / 1000m, TotalFound = response.Total };
        }
    }
}