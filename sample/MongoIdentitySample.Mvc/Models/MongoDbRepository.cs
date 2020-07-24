using AspNetCore.Identity.MongoDbCore.IntegrationTests.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Main.Mvc.Models
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        bool Insert(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);
        IList<TEntity>
        SearchFor(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> GetAll();
        TEntity GetById(Guid id);
    }
    public abstract class EntityBase
    {

        public Guid Id { get; set; }
    }
    public class MongoDbRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        private MongoDatabase database;
        private MongoCollection<TEntity> collection;
        public MongoDbRepository()
        {
            GetDatabase();
            GetCollection();
        }

        public bool Insert(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
             collection.Insert(entity);
            return true;
        }

        public bool Update(TEntity entity)
        {
            if (entity.Id == null)
                return Insert(entity);

            return collection
            .Save(entity)
            .DocumentsAffected > 0;
        }

        public bool Delete(TEntity entity)
        {
            return collection
            .Remove(Query.EQ("_id", entity.Id))
            .DocumentsAffected > 0;
        }

        public IList<TEntity>
        SearchFor(Expression<Func<TEntity, bool>> predicate)
        {
            return collection
            .AsQueryable<TEntity>()
            .Where(predicate.Compile())
            .ToList();
        }

        public IList<TEntity> GetAll()
        {
            return collection.FindAllAs<TEntity>().ToList();
        }

        public TEntity GetById(Guid id)
        {
            return collection.FindOneByIdAs<TEntity>(id);
        }

        #region Private Helper Methods  
        private void GetDatabase()
        {
            var client = new MongoClient(GetConnectionString());
            var server = client.GetServer();

            database = server.GetDatabase(GetDatabaseName());
        }

        private string GetConnectionString()
        {
            // return ConfigurationManager
            // .AppSettings
            // .Get(“MongoDbConnectionString”)
            // .Replace(“{ DB_NAME}”, GetDatabaseName());
            return Container.MongoDbIdentityConfiguration.MongoDbSettings.ConnectionString;
        }
        ///Container.MongoDbIdentityConfiguration.MongoDbSettings.ConnectionString,
             //   Container.MongoDbIdentityConfiguration.MongoDbSettings.DatabaseName

        private string GetDatabaseName()
        {
            //return ConfigurationManager
            //.AppSettings
            //.Get(“MongoDbDatabaseName”); 
            return Container.MongoDbIdentityConfiguration.MongoDbSettings.DatabaseName;


        }

        private void GetCollection()
        {
            collection = database
            .GetCollection<TEntity>(typeof(TEntity).Name);
        }
        #endregion
    }
}
