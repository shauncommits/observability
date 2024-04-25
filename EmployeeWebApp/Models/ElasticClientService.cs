using System.Reflection.Metadata;
using Elasticsearch.Net;
using Nest;

namespace EmployeeWebApp.Models
{
    public class ElasticClientService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticClientService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task IndexDocumentAsync<T>(T document, string indexName) where T : class
        {
            var indexResponse = await _elasticClient.IndexAsync(document, idx => idx.Index(indexName));
            if (!indexResponse.IsValid)
            {
                // Handle error
                throw new Exception($"Failed to index document: {indexResponse.DebugInformation}");
            }
        }

        public async Task<T> GetDocumentAsync<T>(string id, string indexName) where T : class
        {
            var getResponse = await _elasticClient.GetAsync<T>(id, idx => idx.Index(indexName));
            if (!getResponse.IsValid)
            {
                // Handle error
                throw new Exception($"Failed to get document: {getResponse.DebugInformation}");
            }

            return getResponse.Source;
        }

        public async Task UpdateDocumentAsync<T>(T document, string id, string indexName) where T : class
        {
            var updateResponse = await _elasticClient.UpdateAsync<T>(id, u => u
                .Index(indexName)
                .Doc(document)
                .Refresh(Refresh.True));

            if (!updateResponse.IsValid)
            {
                // Handle error
                throw new Exception($"Failed to update document: {updateResponse.DebugInformation}");
            }
        }
        
        public async Task DeleteDocumentAsync<T>(string id, string indexName) where T : class
        {
            var deleteResponse = await _elasticClient.DeleteAsync<T>(id, idx => idx.Index(indexName));
            if (!deleteResponse.IsValid)
            {
                // Handle error
                throw new Exception($"Failed to delete document: {deleteResponse.DebugInformation}");
            }
        }

    }
}
