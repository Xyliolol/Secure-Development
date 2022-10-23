using FullSearchSampless.Services.Impl;
using FullSearchSampless.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FullSearchSampless
{
    internal class Sample01
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {

                    #region Configure EF DBContext Service (CardStorageService Database)

                    services.AddDbContext<DocumentDbContext>(options =>
                    {
                        options.UseSqlServer(@"data source=DESKTOP-6Q5HT06\SQLEXPRESS;initial catalog=DocumentsDatabase;User Id=DocumentsDatabaseUser;Password=1234;MultipleActiveResultSets=True;App=EntityFramework");
                    });

                    #endregion

                    #region Configure Repositories

                    services.AddTransient<IDocumentRepository, DocumentRepository>();

                    #endregion


                })
                .Build();

            // Сохраним документы в БД
            host.Services.GetRequiredService<IDocumentRepository>().LoadDocuments();


        }
    }
}