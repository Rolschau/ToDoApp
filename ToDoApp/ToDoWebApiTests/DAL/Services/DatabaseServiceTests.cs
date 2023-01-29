using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hanssens.Net;
using ToDoWebApi.BLL.Models;
using Microsoft.Extensions.Logging;
using ToDoWebApi.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using ToDoWebApi.BLL.Extensions;

namespace ToDoWebApi.DAL.Services.Tests
{

    // ToDo: it seems like test explorer does not like .net6? Try installing NUnit!?!?

    [TestClass()]
    public class DatabaseServiceTests
    {
        private const bool useLogging = true; // Set this to true to log output.

        private static DatabaseService? _databaseService { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
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

            // Logger that does nothing.
            ILogger<ToDoController> logger = new NullLogger<ToDoController>();
            if (useLogging)
            {
                // Logger that writes to console.
                using var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
                logger = logFactory.CreateLogger<ToDoController>();
            }

            _databaseService = new DatabaseService(localStorageConfiguration, logger);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _ = _databaseService ?? throw new ApplicationException(nameof(_databaseService));

            // The field _localStorage is private, use reflection to call the Destroy method.
            //LocalStorage? localStorage = (LocalStorage?)_databaseService?.GetType()?.GetField("_localStorage", BindingFlags.Instance)?.GetValue(_databaseService);
            //localStorage?.Destroy();

            // ...or let us just expose it. Whatever bad things could happen. :D
            _databaseService.DeleteDatabaseAsync();
        }

        [TestMethod()]
        public void CreateAsyncTest()
        {
            _ = _databaseService ?? throw new ApplicationException(nameof(_databaseService));

            // Arrange
            ToDoDTO original = new() { Value = "original" };

            // Act
            ToDoDTO created = _databaseService.CreateAsync(original).Result;

            // Assert
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);
            Assert.IsTrue(created.Id.Length > 1);
            Assert.AreEqual(original.Value, created.Value);
            Assert.IsFalse(created.Done);
            Assert.AreEqual(0, created.Order);
        }

        [TestMethod()]
        public void UpdateAsyncTest()
        {
            _ = _databaseService ?? throw new ApplicationException(nameof(_databaseService));

            // Arrange
            ToDoDTO original = new() { Value = "original" };

            // Act
            ToDoDTO created = _databaseService.CreateAsync(original).Result;
            ToDoDTO modified1 = (ToDoDTO)created.Clone();
            modified1.Done = !modified1.Done;
            ToDoDTO modified2 = created.DeepCopy();
            modified2.Order = modified2.Order = 999;
            ToDoDTO updated1 = _databaseService.UpdateAsync(modified1).Result;
            ToDoDTO updated2 = _databaseService.UpdateAsync(modified2).Result;

            // Assert
            Assert.IsNotNull(updated1);
            Assert.AreNotEqual(original.Done, updated1.Done);
            Assert.AreNotEqual(original.Order, updated2.Order);
            Assert.AreEqual(expected: 999, actual: updated2.Order);
        }

        [TestMethod()]
        public void GetAsyncTest()
        {
            _ = _databaseService ?? throw new ApplicationException(nameof(_databaseService));

            // Arrange
            ToDoDTO original = new() { Value = "original" };
            
            // Act
            ToDoDTO created1 = _databaseService.CreateAsync(original).Result;
            ToDoDTO created2 = _databaseService.CreateAsync(original).Result;
            List<ToDoDTO> todos = _databaseService.GetAsync().Result;

            // Assert
            Assert.IsNotNull(todos);
            Assert.IsNotNull(created1.Id);
            Assert.AreNotEqual(created1.Id, created2.Id);
            Assert.AreEqual(expected: 2, actual: todos.Count);
        }

        [TestMethod()]
        public void GetAsyncTest_id()
        {
            _ = _databaseService ?? throw new ApplicationException(nameof(_databaseService));

            // Arrange
            ToDoDTO original = new() { Value = "original" };

            // Act
            ToDoDTO created = _databaseService.CreateAsync(original).Result;
            Assert.IsNotNull(created.Id); // Added to Resolve nullable warnings in the line below.
            ToDoDTO retrieved = _databaseService.GetAsync(created.Id).Result;

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(expected: created.Id, actual: retrieved.Id);
        }

        [TestMethod()]
        public void DeleteAsyncTest()
        {
            _ = _databaseService ?? throw new ApplicationException(nameof(_databaseService));

            // Arrange
            ToDoDTO original = new() { Value = "original" };

            // Act
            ToDoDTO created = _databaseService.CreateAsync(original).Result;
            Assert.IsNotNull(created.Id); // Added to Resolve nullable warnings in the line below.
            Task task = _databaseService.DeleteAsync(created.Id);
            task.Wait();

            // Assert
            Assert.AreEqual(expected: true, actual: task.IsCompleted);
            Assert.ThrowsException<ArgumentNullException>(() => _databaseService.GetAsync(created.Id));
        }
    }
}