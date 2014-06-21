using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dapper;
using FullTextSearchEngineTesting.InfoData;
using MySql.Data.MySqlClient;

namespace FullTextSearchEngineTesting.Engines
{
    class SphinxSearchEngine : ISearchEngine
    {
        public void Dispose()
        {
        }

        private static string Prepare(QueryItem item)
        {
            return item.Join == TextJoin.Partial
                ? item.Text + "*"
                : item.Text;
        }
        public Result Run(IEnumerable<QueryItem> searchQuery)
        {
            string queryStr = string.Format("@fullname {0}", string.Join(" ", searchQuery.Select(Prepare)));

            Stopwatch sw = new Stopwatch();

            sw.Start();
            using (var myConn = new MySqlConnection("Server=localhost;Port=9306;Character set=utf8"))
            {
                myConn.Open();

                var answer = myConn.QueryMultiple("select aoguid, fullname, aolevel from fias where match(@query); show meta;", new { query = queryStr });
                var res = answer.Read<SearchResult>().ToList();
                var meta = answer.Read<MetaResult>().ToList();

                foreach (var searchResult in res)
                {

                }
                sw.Stop();
                return new Result()
                {
                    ElapsedTime = sw.ElapsedMilliseconds / 1000m,
                    TotalFound = Convert.ToInt32(meta.First(v => "total".Equals(v.Variable_name)).Value)
                };
            }

        }
    }
    public class MetaResult
    {
        public string Variable_name { get; set; }
        public string Value { get; set; }
    }
}