using UnitTests;
using Utilities;
using BytecodeInterpreter;
using BytecodeGenerator;
namespace IntegrationTests;

[Collection("IO Collection")]
public class SimpleBytecodeTests
{
    string filename = "../../../../BrainfuckTestPrograms/factor.bf";
    [Fact]
    public void FactorNumberUsingSimpleBytecode()
    {
        using (IoHandler handler = new IoHandler("1234\n"))
        {
            CodeRunnerFactory factory = CodeRunnerFactory.GetFactory();
            SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(filename);
            byte[] bytes = byteCodeGenerator.CreateByteCode();
            CodeRunner codeRunner = factory.GetCodeRunner(bytes);
            Interpreter interpreter = new Interpreter();
            interpreter.Run(codeRunner);
            string output = handler.GetOutput();
            Assert.Equal("1234: 2 617\n", output);
        }
    }
}