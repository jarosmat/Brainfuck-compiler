using System.Text;

namespace BytecodeInterpreter;

public abstract class CodeRunner
{
    private static readonly int MemorySize = 30000;
    protected byte[] Memory = new byte[MemorySize];
    protected uint DataPointer = 0;
    public uint InstructionPointer { get; set; }
    public readonly byte[] Code;
    
    public delegate void RunFunc();
    public abstract RunFunc[] GetDelegates();
    protected UTF8Encoding Utf8 = new UTF8Encoding();

    protected CodeRunner(string codeAddress)
    {
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

    protected static int MergeBytes(byte moreSignificant, byte lessSignificant)
    {
        int result = moreSignificant << 8;
        result = result | lessSignificant;
        return result;
    }
}