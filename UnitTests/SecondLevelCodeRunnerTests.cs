using BytecodeInterpreter;
using System.Text;
namespace UnitTests;

[Collection("IO Collection")]
public class SecondLevelCodeRunnerTests
{
    private static string Run(byte[] code, string stringToPass)
    {
        using var consoleOut = new IoHandler(stringToPass);
        var firstLevelCodeRunner = new SecondLevelCodeRunner(code);
        var interpreter = new Interpreter();
        interpreter.Run(firstLevelCodeRunner);
        return consoleOut.GetOutput();        
    }
    
    private readonly UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
        [Fact]
    public void SetMemoryZero()
    {
        var randomByte  = _random.Next(48, 125);
        var charToPass = _utf8.GetChars([(byte)randomByte]);
        var stringToPass = new string(charToPass);
        const string expectedOutput = "A";
        byte[] code = [0x02, 0x00, 0x00, 0x00, 0x05, 0x08, 0x00, 0x41, 0x04];
        var output = Run(code, stringToPass);
        Assert.Equal(expectedOutput, output);
    }

    [Fact]
    public void MoveDataToEmptyCell()
    {
        var randomByte  = _random.Next(48, 125);
        var charToPass = _utf8.GetChars([(byte)randomByte]);
        var stringToPass = new string(charToPass);
        byte[] code = [0x02, 0x00, 0x00, 0x00, 0x05, 0x09, 0x01, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04];
        var output = Run(code, stringToPass);
        Assert.Equal(stringToPass, output);
    }

    [Fact]
    public void MoveDataToEmptyCellMultiply()
    {
        const string input = "A";
        const string expectedOutput = "A";
        byte[] code = [0x02, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x09, 0x05, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04];
        var output = Run(code, input);
        Assert.Equal(expectedOutput, output);
    }

    [Fact]
    public void MoveDataToNonEmptyCell()
    {
        var randomByte  = _random.Next(48, 124);
        var charToGet = _utf8.GetChars([(byte)(randomByte + 1)]);
        var charToPass = _utf8.GetChars([(byte)randomByte]);
        var stringToPass = new string(charToPass);
        var stringToGet = new string(charToGet);
        byte[] code = [0x02, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x06, 0x00, 0x00, 0x00, 0x05, 0x09, 0x01, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04];
        var output = Run(code, stringToPass);
        Assert.Equal(stringToGet, output);
    }

    [Fact]
    public void MoveDataToCellOnTheLeft()
    {
        var randomByte  = _random.Next(48, 125);
        var charToPass = _utf8.GetChars([(byte)randomByte]);
        var stringToPass = new string(charToPass);
        byte[] code = [0x02, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x05, 0x09, 0x01, 0xFA, 0xFF, 0xFF, 0xFF, 0x02, 0x06, 0x00, 0x00, 0x00, 0x04];
        var output = Run(code, stringToPass);
        Assert.Equal(stringToPass, output);
    }
}