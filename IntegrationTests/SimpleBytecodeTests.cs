using UnitTests;
using Utilities;
using BytecodeInterpreter;
using BytecodeGenerator;
namespace IntegrationTests;

[Collection("IO Collection")]
public class SimpleBytecodeTests
{
    string filename = "/home/mjaros/RiderProjects/Brainfuck/BrainfuckTestPrograms/factor.bf";
    [Fact]
    public void FactorNumberUsingSimpleBytecode()
    {
        using (IoHandler handler = new IoHandler("1234\n"))
        {
            SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(filename);
            byte[] bytes = byteCodeGenerator.CreateByteCode();
            SimpleCodeRunner codeRunner = new SimpleCodeRunner(bytes);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(codeRunner);
            string output = handler.GetOutput();
            Assert.Equal("1234: 2 617\n", output);
        }
    }
}