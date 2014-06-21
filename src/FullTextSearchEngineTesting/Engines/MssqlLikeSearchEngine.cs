using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper;
using FullTextSearchEngineTesting.InfoData;

namespace FullTextSearchEngineTesting.Engines
{
    class MssqlLikeSearchEngine : ISearchEngine
    {
        //private SqlConnection conn;
        private SqlCommand _cmd;

        
        public void Dispose()
        {

        }

        private static string Prepare(QueryItem item)
        {
            return "fullname like '%" + item.Text + "%'";
        }

        public Result Run(IEnumerable<QueryItem> searchQuery)
        {
            string queryStr = string.Join(" AND ", searchQuery.Select(Prepare));

            Stopwatch sw = new Stopwatch();

            sw.Start();
            using (var
                conn = new SqlConnection(@"Data Source=work\sqlexpress;Initial Catalog=fias;Integrated Security=true;Connection Timeout=30"))
            {

                var cmdResult =
                    conn.Query<SearchResult>(
                        "select top 20 aoguid Guid, fullname, aolevel Level from cache where " + queryStr).ToList();
                int hits =
                    (int)conn.Query(
                        "select count(aoguid) Count from cache where " + queryStr,
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