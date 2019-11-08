using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.Helpers
{
    public class StorageAccountHelper
    {
        public string storageConnectionString;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        public string StorageConnectionString
        {
            get {return storageConnectionString;}
            set
            {
            }
        }

        public async Task SendMessageAsync(string messageText, string queueName)
        {
            storageAccount = CloudStorageAccount.Parse(this.storageConnectionString);
            queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            CloudQueueMessage queueMessage = new CloudQueueMessage(messageText);
            await queue.AddMessageAsync(queueMessage);
        }

    }
}
