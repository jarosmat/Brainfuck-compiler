using System.Text;
using BytecodeInterpreter;
namespace UnitTests;

public class CodeRunnerTests
{
    private readonly UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
    [Fact]
    public void SimpleAddition()
    {
        using IOHandler consoleOut = new IOHandler("");
        SimpleCodeRunner simpleCodeRunner = new SimpleCodeRunner([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(simpleCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal("A", output);
    }
    [Fact]
    public void LoopAddition()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        string output;
        // Console.WriteLine(stringToPass);
        using (IOHandler consoleOut = new IOHandler(stringToPass))
        {
            SimpleCodeRunner simpleCodeRunner = new SimpleCodeRunner([
                0x05, 0x06, 0x0F, 0x00, 0x00, 0x00, 0x03, 0x00, 0x02, 0x01, 0x07, 0x01, 0x00, 0x00, 0x00, 0x03, 0x04
            ]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(simpleCodeRunner);
            output = consoleOut.GetOutput();
            // Console.WriteLine($"the output: {output}");
        }

        Assert.Equal(stringToPass, output);
    }
    
    [Fact]
    public void SimpleAdditionUsingParameter()
    {
        using IOHandler consoleOut = new IOHandler("");
        FirstLevelCodeRunner firstLevelCodeRunner = new FirstLevelCodeRunner([0x00, 0x41, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(firstLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal("A", output);
    }
    
    [Fact]
    public void SimpleAdditionAndSubtractionUsingParameter()
    {
        using IOHandler consoleOut = new IOHandler("");
        FirstLevelCodeRunner firstLevelCodeRunner = new FirstLevelCodeRunner([0x00, 0x7A, 0x04, 0x01, 0x39, 0x04]);
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
        using IOHandler consoleOut = new IOHandler(stringToPass);
        FirstLevelCodeRunner firstLevelCodeRunner = new FirstLevelCodeRunner([0x05, 0x06, 0x0F, 0x00, 0x00, 0x00, 0x03, 0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x05, 0x00, 0x00, 0x00, 0x01, 0x01, 0x07, 0x01, 0x00, 0x00, 0x00, 0x03, 0x05, 0x00, 0x00, 0x00, 0x04]);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(firstLevelCodeRunner);
        string output = consoleOut.GetOutput();
        Assert.Equal(stringToPass, output);
    }

    [Fact]
    public void SetMemoryZero()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        using (IOHandler consoleOut = new IOHandler(stringToPass))
        {
            SecondLevelCodeRunner secondLevelCodeRunner =
                new SecondLevelCodeRunner([0x05, 0x08, 0x00, 0x41, 0x04]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(secondLevelCodeRunner);
            string output = consoleOut.GetOutput();
            Assert.Equal("A", output);
        }
    }

    [Fact]
    public void MoveDataToEmptyCell()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        using (IOHandler consoleOut = new IOHandler(stringToPass))
        {
            SecondLevelCodeRunner secondLevelCodeRunner =
                new SecondLevelCodeRunner([0x05, 0x09, 0x01, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(secondLevelCodeRunner);
            string output = consoleOut.GetOutput();
            Assert.Equal(stringToPass, output);
        }
    }

    [Fact]
    public void MoveDataToEmptyCellMultiply()
    {
        using (IOHandler consoleOut = new IOHandler("A"))
        {
            SecondLevelCodeRunner secondLevelCodeRunner =
                new SecondLevelCodeRunner([0x00, 0x0D, 0x09, 0x05, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(secondLevelCodeRunner);
            string output = consoleOut.GetOutput();
            Assert.Equal("A", output);
        }
    }

    [Fact]
    public void MoveDataToNonEmptyCell()
    {
        int randomByte  = _random.Next(48, 124);
        char[] charToGet = _utf8.GetChars([(byte)(randomByte + 1)]);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        string stringToGet = new string(charToGet);
        using (IOHandler consoleOut = new IOHandler(stringToPass))
        {
            SecondLevelCodeRunner secondLevelCodeRunner =
                new SecondLevelCodeRunner([0x03, 0x06, 0x00, 0x00, 0x00, 0x00, 0x01, 0x05, 0x09, 0x01, 0x06, 0x00, 0x00, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x04]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(secondLevelCodeRunner);
            string output = consoleOut.GetOutput();
            Assert.Equal(stringToGet, output);
        }
    }

    [Fact]
    public void MoveDataToCellOnTheLeft()
    {
        int randomByte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)randomByte]);
        string stringToPass = new string(charToPass);
        using (IOHandler consoleOut = new IOHandler(stringToPass))
        {
            SecondLevelCodeRunner secondLevelCodeRunner =
                new SecondLevelCodeRunner([0x03, 0x06, 0x00, 0x00, 0x00, 0x05, 0x09, 0x01, 0xFA, 0xFF, 0xFF, 0xFF, 0x02, 0x06, 0x00, 0x00, 0x00, 0x04]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(secondLevelCodeRunner);
            string output = consoleOut.GetOutput();
            Assert.Equal(stringToPass, output);
        }
    }
}