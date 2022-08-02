namespace Console.Menu.lib.src.Main;

public class PerformActionEventArgs : EventArgs
{
    /// <inheritdoc />
    public PerformActionEventArgs(ConsoleKeyInfo userInput, params object[] args)
    {
        UserInput = userInput;
        Args = args;
    }
    public ConsoleKeyInfo UserInput { get; }
    public object[] Args { get; }
}