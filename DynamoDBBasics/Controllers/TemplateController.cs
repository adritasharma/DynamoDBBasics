using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using DynamoDBBasics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamoDBBasics.Controllers
{
    [Route("api/templates")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly DynamoDBContext _dbContext;

        public TemplateController(IAmazonDynamoDB dynamoDBClient)
        {
            _dbContext = new DynamoDBContext(dynamoDBClient);
        }

        [HttpPost]
        public async Task SaveTemplate([FromBody] Template template)
        {
            template.TemplateId = Guid.NewGuid().ToString();
            template.TemplateDocuments.ForEach(item =>
            {
                item.DocumentId = Guid.NewGuid().ToString();
            });
            await _dbContext.SaveAsync(template);
        }

        public async Task<IEnumerable<Template>> GetAllTemplates()
        {
            return await _dbContext.ScanAsync<Template>(new List<ScanCondition>()).GetRemainingAsync();
        }

        [HttpGet]
        [Route("{templateId}")]
        public async Task<Template> GetTemplateById(string templateId)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition>
                {
                    // Its Propertyname. Our partition key is "templateId", but we use C# property name
                    new ScanCondition("TemplateId", ScanOperator.Equal, templateId)
                }
            };
            var res = await _dbContext.QueryAsync<Template>(templateId, config).GetRemainingAsync();

            return res.FirstOrDefault();
        }

        [HttpGet]
        [Route("sector/{sector}")]
        public async Task<List<Template>> GetTemplatesByLob(string sector)
        {

            // It's attribute name (Same name as in DynamoDB. If it was "sector" in dynamodb, then we had to us that)
            var myQF = new QueryFilter("Sector", QueryOperator.Equal, sector);
            var conf = new QueryOperationConfig()
            {
                IndexName = "Sector-index",
                Filter = myQF
            };


            return await _dbContext.FromQueryAsync<Template>(conf).GetRemainingAsync();

        }

        [HttpPut]
        public async Task UpdateTemplete([FromBody] Template template)
        {
            template.TemplateDocuments.ForEach(item =>
            {
                if (item.DocumentId == "")
                {
                    item.DocumentId = Guid.NewGuid().ToString();
                }
            });
            await _dbContext.SaveAsync(template);
        }

        [HttpDelete]
        [Route("{templateId}")]
        public async Task DeleteTemplateById(string templateId)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition>
                {
                    // Its Propertyname. Our partition key is "templateId", but we use C# property name
                    new ScanCondition("TemplateId", ScanOperator.Equal, templateId)
                }
            };
            var res = await _dbContext.QueryAsync<Template>(templateId, config).GetRemainingAsync();

            res.ForEach(async item =>
            {
                await _dbContext.DeleteAsync(item);
            });
        }


    }
}
