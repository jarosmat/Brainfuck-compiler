using BytecodeInterpreter;
using System.Text;
namespace UnitTests;

[Collection("IO Collection")]
public class SecondLevelCodeRunnerTests
{
    private readonly UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
        [Fact]
    public void SetMemoryZero()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        using IoHandler consoleOut = new IoHandler(stringToPass);
        SecondLevelCodeRunner secondLevelCodeRunner =
            new SecondLevelCodeRunner([0x05, 0x08, 0x00, 0x41, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(secondLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal("A", output);
    }

    [Fact]
    public void MoveDataToEmptyCell()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        using IoHandler consoleOut = new IoHandler(stringToPass);
        SecondLevelCodeRunner secondLevelCodeRunner =
            new SecondLevelCodeRunner([0x05, 0x09, 0x01, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(secondLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal(stringToPass, output);
    }

    [Fact]
    public void MoveDataToEmptyCellMultiply()
    {
        using IoHandler consoleOut = new IoHandler("A");
        SecondLevelCodeRunner secondLevelCodeRunner =
            new SecondLevelCodeRunner([0x00, 0x0D, 0x09, 0x05, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(secondLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal("A", output);
    }

    [Fact]
    public void MoveDataToNonEmptyCell()
    {
        int randomByte  = _random.Next(48, 124);
        char[] charToGet = _utf8.GetChars([(byte)(randomByte + 1)]);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        string stringToGet = new string(charToGet);
        using IoHandler consoleOut = new IoHandler(stringToPass);
        SecondLevelCodeRunner secondLevelCodeRunner =
            new SecondLevelCodeRunner([0x03, 0x06, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x06, 0x00, 0x00, 0x00, 0x05, 0x09, 0x01, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(secondLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal(stringToGet, output);
    }

    [Fact]
    public void MoveDataToCellOnTheLeft()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        using IoHandler consoleOut = new IoHandler(stringToPass);
        SecondLevelCodeRunner secondLevelCodeRunner =
            new SecondLevelCodeRunner([0x03, 0x06, 0x00, 0x00, 0x00, 0x05, 0x09, 0x01, 0xFA, 0xFF, 0xFF, 0xFF, 0x02, 0x06, 0x00, 0x00, 0x00, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(secondLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal(stringToPass, output);
    }
}