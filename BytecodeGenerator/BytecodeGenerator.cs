using Utilities;

namespace BytecodeGenerator;

public abstract class BytecodeGenerator
{
    protected class Bracket(int address)
    {
        public int Address { get; private set; } = address;
    }

    protected readonly List<Bracket> BracketStack;
    protected const int AddressLenght = 4;
    protected readonly TextReader StreamReader;
    protected readonly List<byte> Bytes;

    protected BytecodeGenerator(TextReader streamReader, Fingerprints fingerprint)
    {
        StreamReader = streamReader;
        BracketStack = [];
        Bytes = [];
        ByteNumbersHandler.AddBytesToList(Bytes, (int)fingerprint);
    }

    protected BytecodeGenerator(FileInfo file, Fingerprints fingerprint)
    {
        if (!file.Exists)
        {
            throw new FileNotFoundException($"could not find file {file.Name}");
        }

        Bytes = [];
        StreamReader = file.OpenText();
        BracketStack = [];
        ByteNumbersHandler.AddBytesToList(Bytes, (int)fingerprint);
    }

    protected void AddInstruction(byte instruction, byte[]? parameters)
    {
        Bytes.Add(instruction);
        if (parameters != null)
            Bytes.AddRange(parameters);
    }
    
    protected void AddInstruction(byte instruction, int parameter)
    {
        Bytes.Add(instruction);
        ByteNumbersHandler.AddBytesToList(Bytes, parameter);
    }
    public abstract byte[] CreateByteCode();
}