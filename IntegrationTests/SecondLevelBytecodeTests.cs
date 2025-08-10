using BytecodeGenerator;
using BytecodeInterpreter;
using UnitTests;
namespace IntegrationTests;

[Collection("IO Collection")]
public class SecondLevelBytecodeTests
{
    private readonly FileInfo _info = new FileInfo("../../../../BrainfuckTestPrograms/factor.bf");
    [Fact]
    public void FactorNumberUsingSimpleBytecode()
    {
        using var handler = new IoHandler("1234\n");
        var factory = CodeRunnerFactory.GetFactory();
        var byteCodeGenerator = new SecondLevelBytecodeGenerator(_info);
        var bytes = byteCodeGenerator.CreateByteCode();
        var codeRunner = factory.GetCodeRunner(bytes);
        var interpreter = new Interpreter();
        interpreter.Run(codeRunner);
        var output = handler.GetOutput();
        Assert.Equal("1234: 2 617\n", output);
    }   
}