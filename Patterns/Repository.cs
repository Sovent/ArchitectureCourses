using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Journalist.WindowsAzure.Storage;
using Journalist.WindowsAzure.Storage.Tables;

namespace Patterns
{
    public interface IUserRepository
    {
        Task<User> GetUser(Guid id);
        Task SaveUser(User user);
    }

    public class CloudCachingRepository : IUserRepository
    {
        public CloudCachingRepository(string connectionString, string tableName, ICache cache)
        {
            var factory = new StorageFactory();
            _cloudTable = factory.CreateTable(connectionString, tableName);
            _cache = cache;
        }

        public async Task<User> GetUser(Guid id)
        {
            var user = await _cache.GetValue<User>(id.ToString());
            if (user != null)
            {
                return user;
            }

            var query = _cloudTable.PrepareEntityPointQuery(id.ToString());
            var result = await query.ExecuteAsync();
            if (result == null)
            {
                return null;
            }

            return new User(
                id, 
                (string) result["Name"],
                (string) result["Address"], 
                (string) result["PhoneNumber"]);
        }

        public async Task SaveUser(User user)
        {
            var operation = _cloudTable.PrepareBatchOperation();
            operation.InsertOrReplace(user.Id.ToString(), new Dictionary<string, object>()
            {
                {"Name", user.Name},
                {"Address", user.Address},
                {"PhoneNumber", user.PhoneNumber}
            });
            await operation.ExecuteAsync();
            await _cache.SaveValue(user.Id.ToString(), user);
        }

        private readonly ICache _cache;
        private readonly ICloudTable _cloudTable;
    }

    public class User
    {
        public User(Guid id, string name, string address, string phoneNumber)
        {
            Id = id;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string Address { get; }

        public string PhoneNumber { get; }
    }
}