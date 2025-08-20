using System.Text;

namespace BytecodeInterpreter;

public abstract class CodeRunner
{
    private const int MemorySize = 30000;
    protected readonly byte[] Memory = new byte[MemorySize];
    private int _dataPointer;
    public const int FingerprintLenght = 4;

    protected int DataPointer
    {
        get => _dataPointer;
        set => _dataPointer = value % MemorySize;
    }

    public int InstructionPointer { get; set; } = FingerprintLenght;
    public readonly byte[] Code;
    
    public delegate void RunFunc();
    public abstract RunFunc[] GetDelegates();
    private static readonly UTF8Encoding Utf8 = new UTF8Encoding();

    protected CodeRunner(string codeAddress)
    {
        if (!File.Exists(codeAddress))
        {
            throw new FileNotFoundException();
        }
        Code = File.ReadAllBytes(codeAddress);
    }

    protected CodeRunner(byte[] code)
    {
        Code = code;
    }
    
    public int GetCodeLen()
    {
        return Code.Length;
    }
    
    protected void PrintValue()
    {
        byte[] bytes = [Memory[DataPointer]];
        Console.Write(Utf8.GetString(bytes));
    }

    protected void ReadValue()
    {
        var intIn = Console.Read();
        char[] charIn = [(char)intIn];
        var bytes = Utf8.GetBytes(charIn);
        if (bytes[0] == 13)
        {
            bytes[0] = 10; // this is just to avoid issues with line breaks on different operating systems 
        }
        Memory[DataPointer] = bytes[0];
    }
}