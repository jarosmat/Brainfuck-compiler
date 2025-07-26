using System.Text;

namespace BytecodeInterpreter;

public abstract class CodeRunner
{
    private static readonly int MemorySize = 30000;
    protected byte[] Memory = new byte[MemorySize];
    private uint _dataPointer;

    protected uint DataPointer
    {
        get => _dataPointer;
        set => _dataPointer = value % (uint)MemorySize;
    }
    public uint InstructionPointer { get; set; }
    public readonly byte[] Code;
    
    public delegate void RunFunc();
    public abstract RunFunc[] GetDelegates();
    protected readonly UTF8Encoding Utf8 = new UTF8Encoding();

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

    protected static uint MergeBytes(byte moreSignificant, byte lessSignificant)
    {
        uint result = (uint)moreSignificant << 8;
        result = result | lessSignificant;
        return result;
    }
}