namespace BytecodeGenerator;

public class BytecodeGeneratorFactory
{
    private static readonly BytecodeGeneratorFactory? RunnerFactory = null;
    private BytecodeGeneratorFactory(){}

    public static BytecodeGeneratorFactory GetFactory()
    {
        if (RunnerFactory != null)
        {
            return RunnerFactory;
        }

        return new BytecodeGeneratorFactory();
    }

    public BytecodeGenerator GetBytecodeGenerator(int num, FileInfo file)
    {
        BytecodeGenerator result;
        switch (num)
        {
            case (int)Fingerprints.SimpleByteCode:
                result = new SimpleByteCodeGenerator(file);
                break;
            case (int)Fingerprints.FirstLevelByteCode:
                result = new FirstLevelBytecodeGenerator(file);
                break;
            case (int)Fingerprints.SecondLevelByteCode:
                result = new SecondLevelBytecodeGenerator(file);
                break;
            default:
                throw new IndexOutOfRangeException($"Invalid option argument: {num}");
        }
        return result;
    }
}