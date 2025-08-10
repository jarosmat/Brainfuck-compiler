using BytecodeGenerator;
using Utilities;
namespace BytecodeInterpreter;

public class CodeRunnerFactory
{
    private static readonly CodeRunnerFactory? RunnerFactory = null;
    private CodeRunnerFactory(){}

    public static CodeRunnerFactory GetFactory()
    {
        return RunnerFactory ?? new CodeRunnerFactory();
    }

    public CodeRunner GetCodeRunner(byte[] bytes)
    {
        var num = ByteNumbersHandler.GetNumber(bytes, 0, CodeRunner.FingerprintLenght);
        CodeRunner result;
        switch (num)
        {
            case (int)Fingerprints.SimpleByteCode:
                result = new SimpleCodeRunner(bytes);
                break;
            case (int)Fingerprints.FirstLevelByteCode:
                result = new FirstLevelCodeRunner(bytes);
                break;
            case (int)Fingerprints.SecondLevelByteCode:
                result = new SecondLevelCodeRunner(bytes);
                break;
            default:
                throw new IndexOutOfRangeException($"Invalid fingerprint: {num}");
        }

        return result;
    }
}