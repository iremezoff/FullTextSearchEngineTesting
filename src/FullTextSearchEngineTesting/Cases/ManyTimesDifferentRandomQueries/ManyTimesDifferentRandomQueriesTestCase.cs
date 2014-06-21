using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using FullTextSearchEngineTesting.InfoData;
using FullTextSearchEngineTesting.Strategies;

namespace FullTextSearchEngineTesting.Cases.ManyTimesDifferentRandomQueries
{
    class ManyTimesDifferentRandomQueriesTestCase : ITestCase
    {
        private static readonly Regex Regex = new Regex(@"[^\p{IsCyrillic}0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected const int Times = 1000;

        private const int RowsCount = 1083000;

        protected readonly string[][] Queries;

        public string Name
        {
            get
            {
                string name = string.Empty;
                
                name += Times + " запросов, из нескольких частей (не менее 2х) по частичному вхождению";

                return name;
            }
        }

        public ManyTimesDifferentRandomQueriesTestCase()
        {
            Queries = new string[Times][];
            InitVocabulary();
        }

        private void InitVocabulary()
        {
            var fi = new FileInfo(string.Format("many_times_different_{0}_random_partial_queries.txt", Times));

            if (fi.Exists)
            {
                FillFromFile(fi, Queries);
                return;
            }

            GenerateNewQueries(fi, Queries);
        }

        private static void FillFromFile(FileInfo fi, string[][] strings)
        {
            string line;
            using (var sr = new StreamReader(fi.Open(FileMode.Open)))
            {
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var items = line.Split('|');
                    strings[i++] = items;
                    if (i == strings.Length) break;
                }
            }
        }

        private static void GenerateNewQueries(FileInfo fi, string[][] queries)
        {
            using (var sw = new StreamWriter(fi.Open(FileMode.CreateNew, FileAccess.Write, FileShare.Read)))
            {
                using (var conn =
                    new SqlConnection(@"Data Source=work\sqlexpress;Initial Catalog=fias;Integrated Security=True"))
                {
                    var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                    for (int i = 0; i < queries.Length; i++)
                    {
                        IEnumerable<string> words;

                        do
                        {
                            var row = random.Next(RowsCount);
                            var result = conn.Query(
                                "WITH orderedCache AS (SELECT ROW_NUMBER() OVER(order by aoguid) AS RowNum, aoguid, fullname, aolevel FROM cache) "
                                + "SELECT aoguid, fullname, aolevel FROM orderedCache WHERE RowNum = @pos",
                                new { pos = row })
                                .Single();
                            words = ((string)result.fullname.ToString()).Split(' ').Where(s => s.Length > 5).ToArray();
                        } while (words.Count() < 3 || queries.Any(q => q != null && q.SequenceEqual(words)));

                        queries[i] = words.Select(w => Regex.Replace(w, "")).Select(w => w.Substring(0, w.Length / 2)).ToArray();
                        sw.WriteLine(string.Join("|", queries[i]));
                    }
                }
            }
        }

        protected static IEnumerable<QueryItem> CreateQuery(IEnumerable<string> strQuery)
        {
            return strQuery.Select(s => new QueryItem() { Text = s, Join = TextJoin.Partial });
        }

        public CaseResult Perform(ISearchEngine engine, IStrategy strategy)
        {
            return strategy.Run(engine, i => CreateQuery(Queries[i]), Times);
        }
    }
}
