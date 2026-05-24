using BackendTemplate.Testing.Common;
using Xunit;

namespace BackendTemplate.Api.Tests;

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
