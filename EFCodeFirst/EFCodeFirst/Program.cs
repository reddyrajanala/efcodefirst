using System;
using System.Linq;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using System.Data.Entity.Infrastructure.Interception;

namespace EFCodeFirst
{
    class Program
    {
        public static void Main(string[] args)
        {

            var loggerConfig = new LoggerConfiguration()
                                .MinimumLevel.Debug()
                                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                                {
                                    AutoRegisterTemplate = true,
                                });

            var logger = loggerConfig.CreateLogger();

            logger.Information("AppStarted");

            DbInterception.Add(new CustomEFInterceptor(logger));
            using (var db = new BloggingContext())
            {
                // Create and save a new Blog 
                Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();

                var blog = new Blog { Name = name };
                db.Blogs.Add(blog);
                db.SaveChanges();

                // Display all Blogs from the database 
                var query = from b in db.Blogs
                            orderby b.Name
                            select b;

                Console.WriteLine("All blogs in the database:");
                logger.Information("All blogs in the database:");
                foreach (var item in query)
                {
                    Console.WriteLine(item.Name);
                    logger.Information(item.Name);
                }

                logger.Information("Done");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }



    }

}
