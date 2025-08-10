using BytecodeInterpreter;
using System.Text;
namespace UnitTests;

[Collection("IO Collection")]
public class FirstLevelCodeRunnerTests
{
    private static string Run(byte[] code, string stringToPass)
    {
        using var consoleOut = new IoHandler(stringToPass);
        var firstLevelCodeRunner = new FirstLevelCodeRunner(code);
        var interpreter = new Interpreter();
        interpreter.Run(firstLevelCodeRunner);
        
        return consoleOut.GetOutput();        
    }
    
    private readonly UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
    [Fact]
    public void SimpleAdditionUsingParameter()
    {
        const string input = "";
        const string correctOutput = "A";
        byte[] code = [0x01, 0x00, 0x00, 0x00, 0x00, 0x41, 0x04];
        var output = Run(code, input);
        Assert.Equal(correctOutput, output);
    }
    
    [Fact]
    public void SimpleAdditionAndSubtractionUsingParameter()
    {
        const string input = "";
        const string correctOutput = "zA";
        byte[] code = [0x01, 0x00, 0x00, 0x00, 0x00, 0x7A, 0x04, 0x01, 0x39, 0x04];
        var output = Run(code, input);
        Assert.Equal(correctOutput, output);
    }
    
    [Fact]
    public void LoopAdditionUsingParametersWithShiftsInMemory()
    {
        var randomByte  = _random.Next(48, 125);
        var charToPass = _utf8.GetChars([(byte)randomByte]);
        var input =  new string(charToPass);
        byte[] code = [0x01, 0x00, 0x00, 0x00, 0x05, 0x06, 0x1D, 0x00, 0x00, 0x00, 0x03, 0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x05, 0x00, 0x00, 0x00, 0x01, 0x01, 0x07, 0x05, 0x00, 0x00, 0x00, 0x03, 0x05, 0x00, 0x00, 0x00, 0x04];
        var output = Run(code, input);
        Assert.Equal(input, output);
    }
}