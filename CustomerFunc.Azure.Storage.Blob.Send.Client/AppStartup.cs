using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerFunc.Azure.Storage.Blob.Send.Client
{
    public class AppStartup
    {
        /// <summary>
        /// The azure BLOB storage settings
        /// </summary>
        private readonly AzureBlobStorageSettingsOptions _azureBlobStorageSettings;

        /// <summary>
        /// The configuration root
        /// </summary>
        //private readonly IConfigurationRoot _configurationRoot;

        public AppStartup(IOptions<AzureBlobStorageSettingsOptions> azureBlobStorageSettings)
        {
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
        }

        //public AppStartup(IConfigurationRoot configurationRoot)
        //{
        //    _configurationRoot = configurationRoot;
        //}

        public async Task Run(string[] args)
        {
            var files = GetFiles(_azureBlobStorageSettings.FileFolderPath);
            if (!files.Any())
            {
                Console.WriteLine("Nothing to process...");
                return;
            }
            await UploadFiles(files, _azureBlobStorageSettings.StorageConnectionString, _azureBlobStorageSettings.BlobContainerName);
        }

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
