using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Nest;

namespace ElasticSearchIndexCreator
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Please enter everything to start or 'q' to exit");
            string r = Console.ReadLine();

            if ("q".Equals(r, StringComparison.InvariantCultureIgnoreCase))
            {
                AppDomain.Unload(AppDomain.CurrentDomain);
            }

            var config = new ConnectionSettings(new Uri("http://localhost:9200"));
            config.SetDefaultIndex("fias2");

            var client = new ElasticClient(config);
            var conn = new SqlConnection(@"Data Source=work\sqlexpress;Initial Catalog=fias;Integrated Security=True");

            if ("s".Equals(r, StringComparison.InvariantCultureIgnoreCase))
            {
                var sw = new Stopwatch();
                sw.Start();


                foreach (var item in conn.Query<IndexBulkItem>("select aoguid guid, fullname, aolevel level from cache"))
                {
                    var response = client.Index<IndexBulkItem>(item, "fias2", "addr");

                }
                client.Refresh("fias2");

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
