using ControlX.Flow.Contract;
using System.Text.Json;
using Dahomey.Json;
using Dahomey.Json.Serialization.Conventions;

namespace ControlX.Flow.Core.Extensions;

public static class AutomateExtension
{
    public static JsonSerializerOptions JsonSerializerOptions {
        get
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.SetupExtensions();

            DiscriminatorConventionRegistry registry = options.GetDiscriminatorConventionRegistry();
            registry.ClearConventions();
            registry.RegisterConvention(new DefaultDiscriminatorConvention<string>(options, "_t"));
            registry.RegisterType<FTPAction>();
            registry.RegisterType<TestAction>();
            registry.DiscriminatorPolicy = DiscriminatorPolicy.Always;

            return options;
        }
    }

    public static string ToJson(this IAutomate automate)
    {

        var items = new List<object>();

        foreach (var action in automate.Actions)
            items.Add(action);

        return JsonSerializer.Serialize(automate, JsonSerializerOptions);
    }
}