using EnterpriseSaaS.DynamicForms;
using EnterpriseSaaS.FeatureFlags;
using FluentAssertions;
namespace Unit.Tests;

public sealed class PlatformTests
{
    [Fact]
    public async Task Disabled_feature_is_never_enabled()
    {
        var manager = new FeatureManager(new Store(new("beta", false)));
        (await manager.IsEnabledAsync("beta", new(Guid.NewGuid(), null, DateTimeOffset.UtcNow))).Should().BeFalse();
    }

    [Fact]
    public void Form_generator_emits_required_fields()
    {
        var form = new FormSchema(Guid.NewGuid(), "profile", 1, [new("main", "main", 0, [new("name", "name", FormFieldType.Text, true, 0)])]);
        var schema = new JsonFormSchemaGenerator().Generate(form); schema["required"]!.AsArray().Select(x => x!.GetValue<string>()).Should().Contain("name");
    }

    private sealed class Store(FeatureDefinition value) : IFeatureStore
    {
        public Task<FeatureDefinition?> GetAsync(string key, CancellationToken ct) => Task.FromResult<FeatureDefinition?>(value);
    }
}
