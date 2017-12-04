//|---------------------------------------------------------------|
//|                         AZURE QUEUE STORAGE                   |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2017 - Present         |
//|---------------------------------------------------------------|
//|                         AZURE QUEUE STORAGE                   |
//|---------------------------------------------------------------|

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Azure;

namespace AzureQueueStorageSample
{
    /// <summary>
    /// Azure queue processor class
    /// </summary>
    public class AzureQueueProcessor
    {
        #region Public Methods 

        /// <summary>
        /// Process Azure queues
        /// </summary>
        /// <returns>Task object</returns>
        public async Task ProcessAzureQueues()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            // Create the queue name
            string queueName = "azurequeuesample"; // Upper case not allowed !

            // Create or reference an existing queue.
            CloudQueue queue = CreateCloudQueueAsync(queueName).Result;
            if (queue != null)
                await Process3QueuesAsync(queue);

        }

        #endregion

        #region Private Methods 

        /// <summary>
        /// Create a cloud queue 
        /// </summary>
        /// <param name="queueName">Queue name value</param>
        /// <returns>CloudQueue task object</returns>
        private async Task<CloudQueue> CreateCloudQueueAsync(string queueName)
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a queue client for interacting with the queue service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            Console.WriteLine("1. Create a cloud queue");

            CloudQueue queue = queueClient.GetQueueReference(queueName);
            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error occurred !");
                Console.WriteLine(exception);
                return null;
            }

            return queue;
        }

        /// <summary>
        /// Process 3 queues
        /// </summary>
        /// <param name="cloudQueue">CloudQueue object</param>
        /// <returns>Task object</returns>
        private async Task Process3QueuesAsync(CloudQueue cloudQueue)
        {
            // Enqueue 3 message
            await Enqueue3MessagesAsync(cloudQueue);

            // Dequeue 2 message
            await Dequeue2MessagesAsync(cloudQueue);

            // Update 1 message but should the update reflect after 5 second
            await Update1EnqueuedMessageAsync(cloudQueue);

            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(string.Empty);
            try
            {
                Console.WriteLine("5. Trying to delete the queue before the content update process is completed.");
                cloudQueueMessage = cloudQueue.GetMessage();
                cloudQueue.DeleteMessage(cloudQueueMessage);
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception occurred !");
                Console.WriteLine(exception);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("6. Waiting 6 seconds before trying to get the content of the queue.");
            Thread.Sleep(6000);
            Console.WriteLine("7. Trying to to get the content of queue after waiting 6 seconds.");
            cloudQueueMessage = cloudQueue.GetMessage();
            Console.WriteLine(string.Concat("Processing message content and delete the queue [", cloudQueueMessage.AsString, "]"));

        }

        /// <summary>
        /// Enqueue 3 queues
        /// </summary>
        /// <param name="cloudQueue">CloudQueue object</param>
        /// <returns>Task object</returns>
        private async Task Enqueue3MessagesAsync(CloudQueue cloudQueue)
        {
            // Enqueue 3 messages by which to demonstrate batch retrieval
            Console.WriteLine("2. Enqueue 3 messages.");
            for (int i = 0; i < 3; i++)
            {
                await cloudQueue.AddMessageAsync(new CloudQueueMessage(string.Format("{0} - {1}", "Sample queue content", i + 1)));
            }

            Console.WriteLine("Enqueue 3 messages is completed !");

        }

        /// <summary>
        /// Dequeue 2 messages
        /// </summary>
        /// <param name="cloudQueue">CloudQueue object</param>
        /// <returns>Task object</returns>
        private async Task Dequeue2MessagesAsync(CloudQueue cloudQueue)
        {
            // Dequeuing 2 messages
            Console.WriteLine("3. Dequeuing 2 messages.");
            foreach (CloudQueueMessage cloudQueueMessage in await cloudQueue.GetMessagesAsync(2, TimeSpan.FromSeconds(5), null, null))
            {
                Console.WriteLine(string.Concat("Processing message content and delete the queue [", cloudQueueMessage.AsString, "]"));
                await cloudQueue.DeleteMessageAsync(cloudQueueMessage);
            }

            Console.WriteLine("Dequeue 2 messages is completed !");

        }

        /// <summary>
        /// Update 1 queue
        /// </summary>
        /// <param name="cloudQueue">CloudQueue object</param>
        /// <returns>Task object</returns>
        private async Task Update1EnqueuedMessageAsync(CloudQueue cloudQueue)
        {
            Console.WriteLine("4. Change the contents of of the last queued message");
            CloudQueueMessage cloudQueueMessage = await cloudQueue.GetMessageAsync();
            cloudQueueMessage.SetMessageContent("Queue is update with new content.");
            await cloudQueue.UpdateMessageAsync(
                cloudQueueMessage,
                TimeSpan.FromSeconds(5),  // Wait 5 seconds till the content is visible
                MessageUpdateFields.Content |
                MessageUpdateFields.Visibility);
        }

        #endregion

    }
}
