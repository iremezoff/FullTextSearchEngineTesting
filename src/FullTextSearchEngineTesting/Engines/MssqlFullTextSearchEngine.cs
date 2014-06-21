using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.Engines
{
    class MssqlFullTextSearchEngine : ISearchEngine
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
            string queryStr = string.Join(" AND ", searchQuery.Select(Prepare));

            Stopwatch sw = new Stopwatch();

            sw.Start();
            using (var
                conn = new SqlConnection(@"Data Source=work\sqlexpress;Initial Catalog=fias;Integrated Security=true"))
            {

                var cmdResult =
                    conn.Query<SearchResult>(
                        "select top 20 aoguid Guid, fullname, aolevel Level from cache where CONTAINS(fullname, @query)",
                        new { query = queryStr }).ToList();
                int hits =
                    (int)conn.Query(
                        "select count(aoguid) Count from cache where CONTAINS(fullname, @query)",
                        new { query = queryStr }).Single().Count;
                //int hits=0;

                sw.Stop();

                foreach (var o in cmdResult)
                {

                }

                return new Result() { ElapsedTime = sw.ElapsedMilliseconds / 1000m, TotalFound = hits };
            }
        }
    }
}