using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.Services.Abstractions
{
    public abstract class BaseRepository
    {
        protected readonly CloudTable _table;

        protected BaseRepository(string tableName, string storageConnectionString)
        {
            _table = InitialiseTableAsync(tableName, storageConnectionString).Result;
        }

        private async Task<CloudTable> InitialiseTableAsync(string tableName, string storageConnectionString)
        {

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}
