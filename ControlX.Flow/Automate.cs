using ControlX.Flow.Contract;
using ControlX.Hub.Contract;
using ControlX.Utilities;
using Dahomey.Json.Attributes;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ControlX.Flow.Core
{
    [JsonDiscriminator(nameof(Automate))]
    public class Automate : IAutomate
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();
        private int _step = 0;

        public IAction[]? Actions {  get; set; }

        public async static Task Execute(Automate automate, ILogger logger, IHubService hubService, params object[] vars)
        {
            if(automate.Actions == null)
                throw new ArgumentNullException(nameof(automate.Actions));
            
            foreach(var v in vars)
            {
                var d = v.ToDictionary();
                automate.SetVariable(d);
            }
                

            foreach(var action in automate.Actions)
            {

                // resolve variables
                var props = action.GetType().GetProperties();
                foreach(var prop in props)
                    if (prop.PropertyType == typeof(string))
                    {
                        var val = (string)prop.GetValue(action);

                        if (val == null)
                            continue;

                        var newVal = automate.GetVariable<string>(val);
                        if(val != newVal)
                            prop.SetValue(action, newVal);
                    }
                
                //assign logger
                action.SetLogger(logger);

                //assign services
                action.SetServices(hubService);

                // run action
                await action.RunAsync();
                automate._step++;
            }

            logger.LogInformation("Flow finished");
        }

        public async static Task Execute(IAction[] actions, ILogger logger, IHubService hubService, params object[] vars)
        {
            // create flow instance
            var flow = new Automate();
            
            foreach(var action in actions)
                action.Automate = flow;
            flow.Actions = actions;
            await Execute(flow, logger, hubService, vars);
        }

        public async static Task Execute(string file, ILogger logger, IHubService hubService, params object[] vars)
        {
            logger.LogInformation($"Flow started: {file}");

            var flowJson = File.ReadAllText(file);
            var tmpAutomate = FromJson(flowJson);
            await Execute(tmpAutomate.Actions, logger, hubService, vars);
        }

        public void SetVariable(IDictionary<string, object> d)
        {
            foreach(var e in d)
            {
                _data.Add(e.Key, e.Value);
            }
        }

        public void SetVariable(string variable, object value)
        {
            _data[variable] = value;
        }

        public T GetVariable<T>(string variable)
            => !variable.StartsWith('$') && typeof(T) == typeof(string) ? 
                (T)(variable as object) : 
                (T)_data[variable.Substring(1)];


        public static IAutomate FromJson(string json)
        {
            return JsonSerializer.Deserialize<Automate>(json, JsonConverterHelper.GetJsonSerializerOptions());
        }
    }
}