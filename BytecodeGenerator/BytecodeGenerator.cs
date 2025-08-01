using BytecodeInterpreter;
using Utilities;

namespace BytecodeGenerator;

public abstract class BytecodeGenerator <TCommandType>
{
    protected class Bracket(uint address)
    {
        public uint Address { get; private set; } = address;
    }
    protected readonly List<Bracket> _bracketStack;
    protected const int AddressLenght = 4;
    protected readonly TextReader _streamReader;
    private readonly Fingerprints fingerprint;

    protected BytecodeGenerator(TextReader streamReader, Fingerprints fingerprint)
    {
        _streamReader = streamReader;
        _bracketStack = new List<Bracket>();
        this.fingerprint = fingerprint;
    }

    protected BytecodeGenerator(string fileName, Fingerprints fingerprint)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        this.fingerprint = fingerprint;
        _streamReader = new StreamReader(fileName);
        _bracketStack = new List<Bracket>();
    }

    public abstract byte[] CreateByteCode();

    protected void AddGenerationFingerprint(List<byte> bytes, uint fingerprint)
    {
        ByteNumbersHandler.AddBytesToList(bytes, fingerprint);
    }
}