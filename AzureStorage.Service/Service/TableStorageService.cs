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
        #region
        //private const string TableName = "EmployeeDetails";
        //private readonly string _storageConnecctionString;
        //private readonly string _storageContainerName;
        //private readonly ILogger<AzureBlobFileService> _logger;
        //private readonly IConfiguration _config;
        //public TableStorageService(IConfiguration configuration, ILogger<AzureBlobFileService> logger)
        //{
        //    _storageConnecctionString = configuration.GetValue<string>("StorageConnectionString");

        //    _logger = logger;
        //    _config = configuration;
        //}
        //private async Task<TableClient> GetTableClient()
        //{
        //    var serviceClient = new TableServiceClient(_config["StorageConnectionString"]);
        //    var tableClient = serviceClient.GetTableClient(TableName);
        //    await tableClient.CreateIfNotExistsAsync();
        //    return tableClient;

        //}
        //public async Task CreateEmployee(EmployeeEntity employee)
        //{
        //    var tableClient = await GetTableClient();
        //    await tableClient.AddEntityAsync(employee);
        //    await Save();
        //}
        //public async Task DeleteEmployee(string name, string id)
        //{
        //    var tableClient = await GetTableClient();
        //    await tableClient.DeleteEntityAsync(name, id);
        //    await Save();
        //}
        //public async Task<EmployeeEntity>GetAllEmployees(string name, string id)
        //{
        //    var tableClient = await GetTableClient();
        //    return await tableClient.GetEntityAsync<EmployeeEntity>(name,id);

        //}
        //public async Task Save()
        //{
        //    await Save();
        //}
        //public async Task<EmployeeEntity> UpdateEmployee(EmployeeEntity employeeDetails)
        //{
        //    var tableClient = await GetTableClient();
        //   await tableClient.UpsertEntityAsync(employeeDetails);
        //    return employeeDetails;


        //}
        #endregion


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
        public async  Task<GroceryItemEntity> GetEntityAsync(string category, string id)
        {
            var tableClient = await GetTableClient();
            return await tableClient.GetEntityAsync<GroceryItemEntity>(category, id);
        }

        public async Task<GroceryItemEntity> UpsertEntityAsync(GroceryItemEntity entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync(entity);
            return entity;
        }

        public async Task DeleteEntityAsync(string category, string id)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(category, id);
        }

       

       

        public async Task<List<EmployeeEntity>> Create(EmployeeEntity emp)
        {
            string _dbCon2 = _configuration.GetValue<string>("StorageConnectionString");
            var account = CloudStorageAccount.Parse(_dbCon2);
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("Employee");

            table.CreateIfNotExists();

            EmployeeEntity employeeEntity = new EmployeeEntity(emp.FirstName, emp.LastName);
            employeeEntity.FirstName = emp.FirstName;
            employeeEntity.LastName = emp.LastName;
            employeeEntity. PhoneNumber = emp.PhoneNumber;
            employeeEntity.Email = emp.Email;
            var query = new TableQuery<EmployeeEntity>();
            TableOperation insertOperation = TableOperation.Insert(employeeEntity);


            table.Execute(insertOperation);
            var list = table.ExecuteQuery(query).ToList();
            return list;
        }
    }
}
