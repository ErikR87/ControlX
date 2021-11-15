using Dahomey.Json;
using Dahomey.Json.Serialization.Conventions;
using System.Text.Json;

namespace ControlX.Flow.Core;

public static class JsonConverterHelper
{
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.SetupExtensions();

        DiscriminatorConventionRegistry registry = options.GetDiscriminatorConventionRegistry();
        registry.ClearConventions();
        registry.RegisterConvention(new DefaultDiscriminatorConvention<string>(options, "_t"));

        registry.RegisterType<TestAction>();
        registry.RegisterType<FTPAction>();
        registry.RegisterType<MoveFileAction>();
        registry.RegisterType<GreetingAction>();
        registry.RegisterType<FileTransferAction>();

        return options;
    }
}
