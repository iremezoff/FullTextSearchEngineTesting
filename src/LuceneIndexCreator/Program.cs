using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace LuceneIndexCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter everything to start or 'q' to exit");
            string r = Console.ReadLine();

            if ("q".Equals(r, StringComparison.InvariantCultureIgnoreCase))
            {
                AppDomain.Unload(AppDomain.CurrentDomain);
            }

            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Directory directory = FSDirectory.Open("LuceneIndex");

            var conn = new SqlConnection(@"Data Source=work\sqlexpress;Initial Catalog=fias;Integrated Security=True");

            if ("s".Equals(r, StringComparison.InvariantCultureIgnoreCase))
            {
                var sw = new Stopwatch();
                sw.Start();

                IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

                foreach (var item in conn.Query<IndexBulkItem>("select aoguid guid, fullname, aolevel level from cache"))
                {
                    Document doc = new Document();
                    doc.Add(new Field("id", item.Guid.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("fullname", item.Fullname.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    writer.AddDocument(doc);
                }
                writer.Optimize();
                writer.Close();
                sw.Stop();
                Console.WriteLine("Time of indexing: " + sw.ElapsedMilliseconds / 1000m);
            }
        }
    }

    class IndexBulkItem
    {
        public string Fullname { get; set; }
        public Guid Guid { get; set; }
        public int Level { get; set; }
    }
}
