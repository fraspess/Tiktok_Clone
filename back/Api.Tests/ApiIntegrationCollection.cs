namespace Api.Tests;

[CollectionDefinition("API integration", DisableParallelization = true)]
public sealed class ApiIntegrationCollection : ICollectionFixture<ApiFactory>
{
}
