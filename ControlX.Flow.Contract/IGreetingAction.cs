namespace ControlX.Flow.Contract;

public interface IGreetingAction : IAction
{
    public string Name { get; set; }
}