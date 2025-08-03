using BytecodeGenerator;
using Utilities;
namespace BytecodeInterpreter;

public class CodeRunnerFactory
{
    private static readonly CodeRunnerFactory? RunnerFactory = null;
    private CodeRunnerFactory(){}

    public CodeRunnerFactory GetFactory()
    {
        if (RunnerFactory != null)
        {
            return RunnerFactory;
        }

        return new CodeRunnerFactory();
    }

    public static CodeRunner GetCodeRunner(byte[] bytes)
    {
        int num = ByteNumbersHandler.GetNumber(bytes, 0, CodeRunner.FingerprintLenght);
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