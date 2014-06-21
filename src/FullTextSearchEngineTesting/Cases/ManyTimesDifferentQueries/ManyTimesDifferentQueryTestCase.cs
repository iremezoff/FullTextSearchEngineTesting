using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using FullTextSearchEngineTesting.InfoData;
using FullTextSearchEngineTesting.Strategies;

namespace FullTextSearchEngineTesting.Cases.ManyTimesDifferentQueries
{
    abstract class ManyTimesDifferentQueryTestCase : ITestCase
    {
        private const int Times = 1000;

        private const int RowsCount = 1083000;

        protected readonly string[] Queries;

        public string Name
        {
            get
            {
                string name = "";
                name += Times + " разных запросов, ";

                if (this.GetType().Name.Contains("FullJoin"))
                    name += "полное вхождение";
                else
                {
                    name += "частичное вхождение";
                }

                return name;
            }
        }

        private static readonly Regex Regex = new Regex(@"[^\p{IsCyrillic}0-9,]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected ManyTimesDifferentQueryTestCase()
        {
            Queries = new string[Times];
            InitVocabulary();
        }

        private void InitVocabulary()
        {
            var fi = new FileInfo(string.Format("many_times_different_{0}_queries.txt", Times));

            if (fi.Exists)
            {
                FillFromFile(fi, Queries);
                return;
            }

            GenerateNewQueries(fi, Queries);
        }

        private static void GenerateNewQueries(FileInfo fi, string[] queries)
        {
            using (var sw = new StreamWriter(fi.Open(FileMode.CreateNew, FileAccess.Write, FileShare.Read)))
            {
                using (var conn =
                    new SqlConnection(@"Data Source=work\sqlexpress;Initial Catalog=fias;Integrated Security=True"))
                {
                    var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                    for (int i = 0; i < queries.Length; i++)
                    {


                        string word;

                        do
                        {
                            var row = random.Next(RowsCount);
                            var result = conn.Query(
                                "WITH orderedCache AS (SELECT ROW_NUMBER() OVER(order by aoguid) AS RowNum, aoguid, fullname, aolevel FROM cache) "
                                + "SELECT aoguid, fullname, aolevel FROM orderedCache WHERE RowNum = @pos",
                                new { pos = row })
                                .Single();
                            word = ((string)result.fullname.ToString()).Split(' ').LastOrDefault(s => s.Length > 4);
                        } while (word == null || queries.Contains(Regex.Replace(word, "")));

                        queries[i] = Regex.Replace(word, "");
                        sw.WriteLine(queries[i]);
                    }
                }
            }
        }

        private static void FillFromFile(FileInfo fi, string[] strings)
        {
            string line;
            using (var sr = new StreamReader(fi.Open(FileMode.Open)))
            {
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    strings[i++] = line;
                    if (i == strings.Length) break;
                }
            }
        }

        protected abstract IEnumerable<QueryItem> CreateQuery(int item);

        public virtual CaseResult Perform(ISearchEngine engine, IStrategy strategy)
        {
            return strategy.Run(engine, CreateQuery, Times);
        }
    }
}