namespace Integration.Tests;

public sealed class SmokeTests
{
    [Fact]
    public void Integration_environment_is_explicit() => Assert.Null(Environment.GetEnvironmentVariable("THIS_VARIABLE_SHOULD_NOT_EXIST"));
}
