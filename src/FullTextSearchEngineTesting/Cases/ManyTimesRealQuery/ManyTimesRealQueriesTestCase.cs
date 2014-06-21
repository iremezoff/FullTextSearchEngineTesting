using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using FullTextSearchEngineTesting.InfoData;
using FullTextSearchEngineTesting.Strategies;

namespace FullTextSearchEngineTesting.Cases.ManyTimesRealQuery
{
    class ManyTimesRealQueriesTestCase : ITestCase
    {
        protected int Times = 1000;
        protected readonly string[][] Queries;
        private const int RowsCount = 1083000;
        private static readonly Regex Regex = new Regex(@"[^\p{IsCyrillic}0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public string Name
        {
            get
            {
                string name = Times + " запросов, реальный запрос";

                return name;
            }
        }

        public ManyTimesRealQueriesTestCase()
        {
            Queries = new string[Times][];
            //Queries = new[]
            //{
            //    new[] {new QueryItem(){Text = "Кировская", Join = TextJoin.Full},new QueryItem(){Text = "Уржумский", Join = TextJoin.Full}, new QueryItem(){Text="Лен", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "Алтайский", Join = TextJoin.Full},new QueryItem(){Text = "Бийский", Join = TextJoin.Full}, new QueryItem(){Text="Шук", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "Крым", Join = TextJoin.Full},new QueryItem(){Text = "Бахчисарайский", Join = TextJoin.Full}, new QueryItem(){Text="Шевч", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "Новосибирская", Join = TextJoin.Full},new QueryItem(){Text = "Бердск", Join = TextJoin.Full}, new QueryItem(){Text="Молод", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "Тюменская", Join = TextJoin.Full},new QueryItem(){Text = "Вагайский", Join = TextJoin.Full}, new QueryItem(){Text="Строи", Join = TextJoin.Partial}},
            //    //new[] {new QueryItem(){Text = "Пермский", Join = TextJoin.Full},new QueryItem(){Text = "Кудымкар", Join = TextJoin.Full}, new QueryItem(){Text="Чех", Join = TextJoin.Partial}},
            //    //new[] {new QueryItem(){Text = "Свердловская", Join = TextJoin.Full},new QueryItem(){Text = "Каменск-Уральский", Join = TextJoin.Full}, new QueryItem(){Text="Ми", Join = TextJoin.Partial}},
            //    //new[] {new QueryItem(){Text = "Ханты-Мансийский", Join = TextJoin.Full},new QueryItem(){Text = "Советский", Join = TextJoin.Full}, new QueryItem(){Text="Зелё", Join = TextJoin.Partial}},
            //    //new[] {new QueryItem(){Text = "Алтай", Join = TextJoin.Full},new QueryItem(){Text = "Чемальский", Join = TextJoin.Full}, new QueryItem(){Text="Школ", Join = TextJoin.Partial}},
            //    //new[] {new QueryItem(){Text = "Вологодская", Join = TextJoin.Full},new QueryItem(){Text = "Череповец", Join = TextJoin.Full}, new QueryItem(){Text="Гаг", Join = TextJoin.Partial}}
            //};
            InitVocabulary();
        }

        private void InitVocabulary()
        {
            var fi = new FileInfo(string.Format("many_times_real_{0}_queries_with_last_part.txt", Times));

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

        protected IEnumerable<QueryItem> CreateQuery(int num)
        {
            var list = new List<QueryItem>();
            //for (int i = 0; i < Queries[num].Length - 1; i++)
            //{
            //    list.Add(new QueryItem() { Text = "\"" + Queries[num][i] + "\"", Join = TextJoin.Full });
            //}
            var query = Queries[num];

            list.Add(new QueryItem()
            {
                Join = TextJoin.Full,
                Text = string.Format(@"""{0}""", string.Join(", ", query.Take(query.Length - 1)))
            });

            list.Add(new QueryItem() { Text = Queries[num].Last(), Join = TextJoin.Partial });
            return list;
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

                        string lastSubStr;

                        do
                        {
                            var row = random.Next(RowsCount);
                            var result = conn.Query(
                                "WITH orderedCache AS (SELECT ROW_NUMBER() OVER(order by aoguid) AS RowNum, aoguid, fullname, aolevel FROM cache) "
                                + "SELECT aoguid, fullname, aolevel FROM orderedCache WHERE RowNum = @pos",
                                new { pos = row })
                                .Single();
                            words =
                                ((string)result.fullname.ToString()).Split(',').Select(s => s.Trim())
                                    .Where(s => s.Length > 4)
                                    .Select(s => Regex.Replace(s, " "));
                            lastSubStr = words.Last().Split(' ').FirstOrDefault(s => s.Length > 5);
                        } while (words.Count() < 2 || queries.Any(q => q != null && q.SequenceEqual(words)) || lastSubStr == null);

                        queries[i] = words.ToArray();
                        queries[i][queries[i].Length - 1] = lastSubStr.Substring(0, lastSubStr.Length / 2);
                        sw.WriteLine(string.Join("|", queries[i]));
                    }
                }
            }
        }

        public CaseResult Perform(ISearchEngine engine, IStrategy strategy)
        {
            return strategy.Run(engine, CreateQuery, Times);
        }
    }
}