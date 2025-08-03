using System.Text;
using BytecodeInterpreter;
namespace UnitTests;

[Collection("IO Collection")]
public class SimpleCodeRunnerTests
{
    private readonly UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
    [Fact]
    public void SimpleAddition()
    {
        using IoHandler consoleOut = new IoHandler("");
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
        using (IoHandler consoleOut = new IoHandler(stringToPass))
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
    
    
}