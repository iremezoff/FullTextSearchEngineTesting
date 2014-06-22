using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullTextSearchEngineTesting.InfoData;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace FullTextSearchEngineTesting.Engines
{
    class LuceneSearchEngine : ISearchEngine
    {
        private Analyzer _analyzer;
        private Directory _directory;
        private Searcher _searcher;

        public LuceneSearchEngine()
        {
            _analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            _directory = FSDirectory.Open("LuceneIndex");
            if (!IndexReader.IndexExists(_directory))
            {
                var writer = new IndexWriter(_directory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            }

            _searcher = new IndexSearcher(_directory);
        }

        public void Dispose()
        {
            _analyzer.Close();
            _analyzer.Dispose();
            _directory.Dispose();
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

            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "fullname", _analyzer);
            var query = parser.Parse(queryStr);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Do the search
            TopDocs hits = _searcher.Search(query, 20);


            foreach (var topDoc in hits.ScoreDocs)
            {
                var doc = _searcher.Doc(topDoc.Doc);
                var fn = doc.GetField("fullname");
                var id = doc.GetField("id");
            }
            sw.Stop();

            return new Result() { ElapsedTime = sw.ElapsedMilliseconds / 1000m, TotalFound = hits.TotalHits };
        }
    }
}