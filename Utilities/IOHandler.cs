namespace Utilities;

public class IoHandler : IDisposable
{
    private readonly StringWriter _newOutput;
    private readonly TextWriter _originalOutput;
    private readonly StringReader _newInput;
    private readonly TextReader _originalInput;

    public IoHandler(string stringToRead)
    {
        _newOutput = new StringWriter();
        _originalOutput = Console.Out;
        _newInput = new StringReader(stringToRead);
        _originalInput = Console.In;
        Console.SetIn(_newInput);
        Console.SetOut(_newOutput);
    }

    public string GetOutput()
    {
        return _newOutput.GetStringBuilder().ToString();
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _newOutput.Dispose();
        Console.SetIn(_originalInput);
        _newInput.Dispose();
    }
}   