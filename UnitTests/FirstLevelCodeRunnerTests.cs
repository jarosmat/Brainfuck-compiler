using BytecodeInterpreter;
using System.Text;
namespace UnitTests;

[Collection("IO Collection")]
public class FirstLevelCodeRunnerTests
{
    private readonly UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
    [Fact]
    public void SimpleAdditionUsingParameter()
    {
        using IoHandler consoleOut = new IoHandler("");
        FirstLevelCodeRunner firstLevelCodeRunner = new FirstLevelCodeRunner([0x01, 0x00, 0x00, 0x00, 0x00, 0x41, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(firstLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal("A", output);
    }
    
    [Fact]
    public void SimpleAdditionAndSubtractionUsingParameter()
    {
        using IoHandler consoleOut = new IoHandler("");
        FirstLevelCodeRunner firstLevelCodeRunner = new FirstLevelCodeRunner([0x01, 0x00, 0x00, 0x00, 0x00, 0x7A, 0x04, 0x01, 0x39, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(firstLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal("zA", output);
    }
    
    [Fact]
    public void LoopAdditionUsingParametersWithShiftsInMemory()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        using IoHandler consoleOut = new IoHandler(stringToPass);
        FirstLevelCodeRunner firstLevelCodeRunner = new FirstLevelCodeRunner([0x01, 0x00, 0x00, 0x00, 0x05, 0x06, 0x1D, 0x00, 0x00, 0x00, 0x03, 0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x05, 0x00, 0x00, 0x00, 0x01, 0x01, 0x07, 0x05, 0x00, 0x00, 0x00, 0x03, 0x05, 0x00, 0x00, 0x00, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(firstLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal(stringToPass, output);
    }
}