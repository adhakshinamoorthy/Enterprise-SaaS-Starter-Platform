namespace Architecture.Tests;

public sealed class DependencyTests
{
    [Fact]
    public void Domain_projects_do_not_reference_infrastructure()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src"));
        foreach (var file in Directory.GetFiles(root, "*.Domain.csproj", SearchOption.AllDirectories))
            Assert.DoesNotContain("Infrastructure", File.ReadAllText(file), StringComparison.OrdinalIgnoreCase);
    }
}
