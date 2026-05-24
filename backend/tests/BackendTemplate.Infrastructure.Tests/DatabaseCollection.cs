using BackendTemplate.Testing.Common;
using Xunit;

namespace BackendTemplate.Infrastructure.Tests;

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
