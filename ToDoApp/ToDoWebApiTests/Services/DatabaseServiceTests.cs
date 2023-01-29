using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDoWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanssens.Net;
using System.Reflection;
using ToDoWebApi.Models;
using NuGet.Frameworks;

namespace ToDoWebApi.Services.Tests
{

    // ToDo: it seems like test explorer does not like .net6? Try installing NUnit!?!?

    [TestClass()]
    public class DatabaseServiceTests
    {
        private DatabaseService? _databaseService;

        [ClassInitialize]
        public void SetupEnvironment()
        {
            // setup a configuration with encryption enabled (defaults to 'false')
            // note that adding EncryptionSalt is optional, but recommended
            var localStorageConfiguration = new LocalStorageConfiguration()
            {
                EnableEncryption = true,
                EncryptionSalt = "todosalt",
                Filename = "todotests"
                //ToDo: consider making a separate Filename per user id eg. SSO like IdentityServer4 (OAuth/OpenID protocols, ASP.NET Core).
            };
            _databaseService = new DatabaseService(localStorageConfiguration);
        }

        [ClassCleanup]
        public void TakedownEnvironment()
        {
            // The field _localStorage is private, use reflection to call the Destroy method.
            //LocalStorage? localStorage = (LocalStorage?)_databaseService?.GetType()?.GetField("_localStorage", BindingFlags.Instance)?.GetValue(_databaseService);
            //localStorage?.Destroy();

            // ...or let us just expose it. Whatever bad things could happen. :D
            _databaseService?.DeleteDatabaseAsync();
        }

        [TestMethod()]
        public void CreateAsyncTest()
        {
            // Arrange
            ToDoDTO original = new() { Value = "One" };

            // Act
            ToDoDTO? created = _databaseService?.CreateAsync(original)?.Result;

            // Assert
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);
            Assert.IsTrue(created.Id.Length > 1);
            Assert.AreEqual(original.Value, created.Value);
            Assert.IsFalse(created.Done);
        }
    }
}