using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerFunc.Azure.Storage.Blob.Send.Client
{
    /// <summary>
    /// AzureBlobStorageSettings
    /// </summary>
    public class AzureBlobStorageSettingsOptions
    {
        public const string AzureBlobStorageSettings = "AzureBlobStorageSettings";
        /// <summary>
        /// Gets or sets the storage connection string.
        /// </summary>
        /// <value>
        /// The storage connection string.
        /// </value>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the BLOB container.
        /// </summary>
        /// <value>
        /// The name of the BLOB container.
        /// </value>
        public string BlobContainerName { get; set; }

        /// <summary>
        /// Gets or sets the file folder path.
        /// </summary>
        /// <value>
        /// The file folder path.
        /// </value>
        public string FileFolderPath { get; set; }
    }
}
