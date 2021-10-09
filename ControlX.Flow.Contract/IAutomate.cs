namespace ControlX.Flow.Contract
{
    public interface IAutomate
    {
        public IAction[]? Actions { get; set; }

        public void SetVariable(IDictionary<string, object> d);

        public void SetVariable(string variable, object value);

        public T GetVariable<T>(string variable);
    }
}
