using System.Globalization;
namespace EnterpriseSaaS.Localization;

public interface ILocalizationStore { Task<string?> FindAsync(Guid? tenantId, string culture, string key, CancellationToken cancellationToken); }
public interface ITextLocalizer { Task<string> GetAsync(string key, CultureInfo culture, Guid? tenantId = null, CancellationToken cancellationToken = default); }
public sealed class DatabaseTextLocalizer(ILocalizationStore store) : ITextLocalizer
{ public async Task<string> GetAsync(string key, CultureInfo culture, Guid? tenantId = null, CancellationToken cancellationToken = default) => await store.FindAsync(tenantId, culture.Name, key, cancellationToken) ?? await store.FindAsync(null, culture.Name, key, cancellationToken) ?? key; }
