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
        BracketStack = new List<Bracket>();
        Bytes = new List<byte>();
        ByteNumbersHandler.AddBytesToList(Bytes, (int)fingerprint);
    }

    protected BytecodeGenerator(FileInfo file, Fingerprints fingerprint)
    {
        if (!file.Exists)
        {
            throw new FileNotFoundException($"could not find file {file.Name}");
        }

        Bytes = new List<byte>();
        StreamReader = file.OpenText();
        BracketStack = new List<Bracket>();
        ByteNumbersHandler.AddBytesToList(Bytes, (int)fingerprint);
    }

    public abstract byte[] CreateByteCode();
}