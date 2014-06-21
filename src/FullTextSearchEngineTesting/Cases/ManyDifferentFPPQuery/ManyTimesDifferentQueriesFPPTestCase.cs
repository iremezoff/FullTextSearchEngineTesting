using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using FullTextSearchEngineTesting.InfoData;
using FullTextSearchEngineTesting.Strategies;

namespace FullTextSearchEngineTesting.Cases.ManyDifferentFPPQuery
{
    class ManyTimesDifferentQueriesFPPTestCase : ITestCase
    {
        private static readonly Regex Regex = new Regex(@"[^\p{IsCyrillic}0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected readonly string[][] Queries;
        private const int RowsCount = 1083000;
        private const int Times = 100;

        public ManyTimesDifferentQueriesFPPTestCase()
        {
            Queries = new string[Times][];
            InitVocabulary();
            //queries = new[]
            //{
            //    new[] {new QueryItem(){Text = "���������", Join = TextJoin.Full},new QueryItem(){Text = "����", Join = TextJoin.Partial}, new QueryItem(){Text="���", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "���������", Join = TextJoin.Full},new QueryItem(){Text = "���", Join = TextJoin.Partial}, new QueryItem(){Text="���", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "����", Join = TextJoin.Full},new QueryItem(){Text = "��", Join = TextJoin.Partial}, new QueryItem(){Text="�����", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "�������������", Join = TextJoin.Full},new QueryItem(){Text = "����", Join = TextJoin.Partial}, new QueryItem(){Text="�����", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "���������", Join = TextJoin.Full},new QueryItem(){Text = "���", Join = TextJoin.Partial}, new QueryItem(){Text="�����", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "��������", Join = TextJoin.Full},new QueryItem(){Text = "�����", Join = TextJoin.Partial}, new QueryItem(){Text="���", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "������������", Join = TextJoin.Full},new QueryItem(){Text = "�����", Join = TextJoin.Partial}, new QueryItem(){Text="��", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "�����-����������", Join = TextJoin.Full},new QueryItem(){Text = "�����", Join = TextJoin.Partial}, new QueryItem(){Text="���", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "�����", Join = TextJoin.Full},new QueryItem(){Text = "���", Join = TextJoin.Partial}, new QueryItem(){Text="����", Join = TextJoin.Partial}},
            //    new[] {new QueryItem(){Text = "�����������", Join = TextJoin.Full},new QueryItem(){Text = "����", Join = TextJoin.Partial}, new QueryItem(){Text="���", Join = TextJoin.Partial}}
            //};
        }

        private void InitVocabulary()
        {
            var fi = new FileInfo(string.Format("many_times_different_{0}_queries_full_part_part.txt", Times));

            if (fi.Exists)
            {
                FillFromFile(fi, Queries);
                return;
            }

            GenerateNewQueries(fi, Queries);
        }

        private IEnumerable<QueryItem> CreateQuery(int num)
        {
            var list = new List<QueryItem>();
            list.Add(Queries[num].Take(1).Select(q => new QueryItem() { Join = TextJoin.Full, Text = q }).First());
            list.AddRange(Queries[num].Skip(1).Select(q => new QueryItem() { Text = q.Substring(0, q.Length / 2), Join = TextJoin.Partial }));
            return list;
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
                            words =
                                ((string)result.fullname.ToString()).Split(' ')
                                    .Where(s => s.Length > 4)
                                    .Take(3)
                                    .Select(s => Regex.Replace(s, ""));
                        } while (words.Count() < 3 || queries.Any(q => q != null && q.SequenceEqual(words)));

                        queries[i] = words.ToArray();
                        sw.WriteLine(string.Join("|", queries[i]));
                    }
                }
            }
        }

        public string Name
        {
            get
            {
                string name = Times + " �������� �����+�����+�����";

                return name;
            }
        }

        public CaseResult Perform(ISearchEngine engine, IStrategy strategy)
        {
            return strategy.Run(engine, CreateQuery, Times);
        }
    }
}