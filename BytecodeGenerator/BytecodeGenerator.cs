using Utilities;

namespace BytecodeGenerator;

public abstract class BytecodeGenerator<TCommandType>
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

    protected BytecodeGenerator(string fileName, Fingerprints fingerprint)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        Bytes = new List<byte>();
        StreamReader = new StreamReader(fileName);
        BracketStack = new List<Bracket>();
        ByteNumbersHandler.AddBytesToList(Bytes, (int)fingerprint);
    }

    public abstract byte[] CreateByteCode();
}