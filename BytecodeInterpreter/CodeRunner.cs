using System.Text;

namespace BytecodeInterpreter;

public abstract class CodeRunner
{
    private static readonly uint MemorySize = 30000;
    protected readonly byte[] Memory = new byte[MemorySize];
    private uint _dataPointer;

    protected uint DataPointer
    {
        get => _dataPointer;
        set => _dataPointer = value % MemorySize;
    }
    public uint InstructionPointer { get; set; }
    public readonly byte[] Code;
    
    public delegate void RunFunc();
    public abstract RunFunc[] GetDelegates();
    protected static readonly UTF8Encoding Utf8 = new UTF8Encoding();

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
    
    public uint GetCodeLen()
    {
        return (uint)Code.Length;
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