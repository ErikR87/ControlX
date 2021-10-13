using ControlX.Flow.Contract;
using System.Text.Json;
using Dahomey.Json;
using Dahomey.Json.Serialization.Conventions;

namespace ControlX.Flow.Core.Extensions;

public static class AutomateExtension
{
    public static string ToJson(this IAutomate automate)
    {

        var items = new List<object>();

        foreach (var action in automate.Actions)
            items.Add(action);

        return JsonSerializer.Serialize(automate, JsonConverterHelper.GetJsonSerializerOptions());
    }
}