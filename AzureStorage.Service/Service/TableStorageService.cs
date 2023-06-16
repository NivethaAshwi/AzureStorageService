using AzureStorage.Models;
using AzureStorage.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AzureStorage.Models.TableStorageModel;
using EmployeeEntity = AzureStorage.Models.TableStorageModel.EmployeeEntity;
using Microsoft.Azure.Cosmos.Table;

namespace AzureStorage.Service.Service
{
    public class TableStorageService : IAzureTableStorage
    {
        private const string TableName = "Item";
        private readonly IConfiguration _configuration;
        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
       private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
        public async  Task<GroceryItemEntity> GetGroceryDetails(string category, string id)
        {
            var tableClient = await GetTableClient();
            return await tableClient.GetEntityAsync<GroceryItemEntity>(category, id);
        }

        public async Task<GroceryItemEntity> GrocerryEntityDetails(GroceryItemEntity entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync(entity);
            return entity;
        }

        public async Task DeleteGroceryDetails(string category, string id)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(category, id);
        }
        private async Task<CloudTable> GetcloudTable()
        {
            string _azureConnection = _configuration.GetValue<string>("StorageConnectionString");
            var account = CloudStorageAccount.Parse(_azureConnection);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Employee");
            table.CreateIfNotExists();
            return table;
        }
       
        public async Task<EmployeeEntity>EmployeeData(EmployeeEntity emp)
        {
            EmployeeEntity employeeEntity = new EmployeeEntity(emp.FirstName, emp.LastName);
            employeeEntity.FirstName = emp.FirstName;
            employeeEntity.LastName = emp.LastName;
            employeeEntity.PhoneNumber = emp.PhoneNumber;
            employeeEntity.Email = emp.Email;
            return employeeEntity;

        }
       public async Task<List<EmployeeEntity>> CreateTable(EmployeeEntity emp)
        {
            var tableCon = await GetcloudTable();
            var tableClient = await EmployeeData(emp);
            var query = new TableQuery<EmployeeEntity>();
            TableOperation insertOperation = TableOperation.Insert(tableClient);
            tableCon.Execute(insertOperation);
            var list = tableCon.ExecuteQuery(query).ToList();
            return list;
        }

        public async Task<List<EmployeeEntity>> GetEmployeeTable()
        {
            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Raj");
            var query = new TableQuery<EmployeeEntity>().Where(condition);
            string _azureConnection = _configuration.GetValue<string>("ConnectionStrings:MyAzureTable");
            var account = CloudStorageAccount.Parse(_azureConnection);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Employee");
            var lst = table.ExecuteQuery(query);
            return lst.ToList();
        }

        public async Task<List<EmployeeEntity>> UpdateEmployeeTable(EmployeeEntity employeeDetails)
        {
               var tableCon = await GetcloudTable();
                var tableClient = await EmployeeData(employeeDetails);
                var query = new TableQuery<EmployeeEntity>();
                TableOperation insertOperation = TableOperation.InsertOrMerge(tableClient);
            tableCon.Execute(insertOperation);
                var list = tableCon.ExecuteQuery(query);
                return list.ToList();

           
        }

        public async Task<List<EmployeeEntity>> DeleteEmployeeTable(EmployeeEntity employeeDetails)
        {

            var tableCon = await GetcloudTable();
            var tableClient = await EmployeeData(employeeDetails);
            tableClient.ETag = "*"; // wildcard 
            var query = new TableQuery<EmployeeEntity>();
            TableOperation insertOperation = TableOperation.Delete(tableClient);
            tableCon.Execute(insertOperation);
            var lst = tableCon.ExecuteQuery(query);
            return lst.ToList();

        }
    }
}
