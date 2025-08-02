using System.Text;

namespace BytecodeInterpreter;

public abstract class CodeRunner
{
    private static readonly int MemorySize = 30000;
    protected readonly byte[] Memory = new byte[MemorySize];
    private int _dataPointer;

    protected int DataPointer
    {
        get => _dataPointer;
        set => _dataPointer = value % MemorySize;
    }
    public int InstructionPointer { get; set; }
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
        int intIn = Console.Read();
        char[] charIn = [(char)intIn];
        byte[] bytes = Utf8.GetBytes(charIn);
        if (bytes[0] == 13)
        {
            bytes[0] = 10;
        }
        Memory[DataPointer] = bytes[0];
    }
}