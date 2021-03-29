using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFunc.Azure.Storage.Blob.Send.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var services = new ServiceCollection();
            //ConfigureServices(services);
            //var serviceProvider = services.BuildServiceProvider();
            //await serviceProvider.GetService<AppStartup>()
            //    .Run(args);

            var configuration = GetConfiguration();
            var files = GetFiles(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).FullName,configuration.GetCofigurationKeyValue("AzureBlobStorageSettings:FileFolderPath")));
            if (!files.Any())
            {
                Console.WriteLine("Nothing to process...");
                return;
            }
            await UploadFiles(files, configuration.GetCofigurationKeyValue("AzureBlobStorageSettings:StorageConnectionString"), configuration.GetCofigurationKeyValue("AzureBlobStorageSettings:BlobContainerName"));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            //services.AddScoped<AzureBlobStorageSettingsOptions>();
            services.AddScoped<IConfigurationRoot>();
            services.AddTransient<AppStartup>();

            //services.AddOptions();
            //var azureBlobStorageSettingsOptions = new AzureBlobStorageSettingsOptions();
            //services.Configure<AzureBlobStorageSettingsOptions>(o => configuration.GetSection(AzureBlobStorageSettingsOptions.AzureBlobStorageSettings).Bind(azureBlobStorageSettingsOptions));
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>IConfigurationRoot</returns>
        static IConfigurationRoot GetConfiguration() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="sourceFolder">The source folder.</param>
        /// <returns>Collection of FileInfo</returns>
        static IEnumerable<FileInfo> GetFiles(string sourceFolder) =>
            new DirectoryInfo(sourceFolder)
            .GetFiles()
            .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));

        /// <summary>
        /// Uploads the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="blobContainerName">Name of the BLOB container.</param>
        static async Task UploadFiles(IEnumerable<FileInfo> files, string connectionString, string blobContainerName)
        {
            var blobContainerClient = new BlobContainerClient(connectionString, blobContainerName);
            Console.WriteLine("Uploading files to Blobs...");
            foreach (var file in files)
            {
                try
                {
                    var blobClient = blobContainerClient.GetBlobClient(file.Name);
                    using (var fileStream = File.OpenRead(file.FullName))
                    {
                        await blobClient.UploadAsync(fileStream);
                    }
                    Console.WriteLine($"{file.Name} uploaded...");
                    File.Delete(file.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
