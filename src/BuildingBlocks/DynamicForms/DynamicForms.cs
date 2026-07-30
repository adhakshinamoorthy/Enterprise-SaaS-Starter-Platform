using System.Text.Json.Nodes;
namespace EnterpriseSaaS.DynamicForms;

public enum FormFieldType { Text, Textarea, Number, Date, Dropdown, Checkbox }
public sealed record FormOption(string Value, string LabelKey);
public sealed record VisibilityRule(string FieldKey, string Operator, JsonNode? Value);
public sealed record FormField(string Key, string LabelKey, FormFieldType Type, bool Required, int Order, IReadOnlyList<FormOption>? Options = null, VisibilityRule? VisibleWhen = null, decimal? Min = null, decimal? Max = null, string? Pattern = null);
public sealed record FormSection(string Key, string TitleKey, int Order, IReadOnlyList<FormField> Fields);
public sealed record FormSchema(Guid Id, string Key, int Version, IReadOnlyList<FormSection> Sections);
public interface IFormSchemaGenerator { JsonObject Generate(FormSchema form); }
public sealed class JsonFormSchemaGenerator : IFormSchemaGenerator
{
    public JsonObject Generate(FormSchema form)
    {
        var properties = new JsonObject(); var required = new JsonArray();
        foreach (var field in form.Sections.SelectMany(x => x.Fields).OrderBy(x => x.Order)) { var p = new JsonObject { ["title"] = field.LabelKey, ["type"] = field.Type switch { FormFieldType.Number => "number", FormFieldType.Checkbox => "boolean", _ => "string" } }; if (field.Options is not null) p["enum"] = new JsonArray(field.Options.Select(x => JsonValue.Create(x.Value)).ToArray()); properties[field.Key] = p; if (field.Required) required.Add(field.Key); }
        return new JsonObject { ["$schema"] = "https://json-schema.org/draft/2020-12/schema", ["title"] = form.Key, ["type"] = "object", ["properties"] = properties, ["required"] = required };
    }
}
