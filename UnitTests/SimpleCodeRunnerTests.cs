using System.Text;
using BytecodeInterpreter;
namespace UnitTests;

public class SimpleCodeRunnerTests
{
    private UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
    [Fact]
    public void SimpleAddition()
    {
        using (IOHandler consoleOut = new IOHandler(""))
        {
            SimpleCodeRunner simpleCodeRunner = new SimpleCodeRunner([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(simpleCodeRunner);
            string output = consoleOut.GetOutput();
            Assert.Equal("A", output);
        }
    }
    [Fact]
    public void LoopAddition()
    {
        int rndbyte  = _random.Next(48, 125);
        char[] charToPass = _utf8.GetChars([(byte)rndbyte]);
        string stringToPass = new string(charToPass);
        using (IOHandler consoleOut = new IOHandler(stringToPass))
        {
            SimpleCodeRunner simpleCodeRunner = new SimpleCodeRunner([0x05, 0x06, 0x0F, 0x00, 0x00, 0x00, 0x03, 0x00, 0x02, 0x01, 0x07, 0x01, 0x00, 0x00, 0x00, 0x03, 0x04]);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(simpleCodeRunner);
            string output = consoleOut.GetOutput();
            Assert.Equal(stringToPass, output);
        }
    }
}