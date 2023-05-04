using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBBasics.Models
{
   
    [DynamoDBTable("test-template")]  // Table name in DynamoDB
    public class Template
    {
        [DynamoDBHashKey] // The partition key is part of the table's primary key.
        [DynamoDBProperty("templateId")] // Use only if partition key name is different than the C# property
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey] // Secondary Index
        //[DynamoDBProperty("Sector")] // Use only if Secondary Index  name is different than the C# property
        public string Sector { get; set; }
        public List<TemplateDocument> TemplateDocuments { get; set; }
    }
}

// Sort key - optional
//You can use a sort key as the second part of a table's primary key. The sort key allows you to sort or search among all items sharing the same partition key.