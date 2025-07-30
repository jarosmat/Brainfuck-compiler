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

    protected BytecodeGenerator(TextReader streamReader)
    {
        _streamReader = streamReader;
        _bracketStack = new List<Bracket>();
    }

    protected BytecodeGenerator(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        _streamReader = new StreamReader(fileName);
        _bracketStack = new List<Bracket>();
    }

    public abstract byte[] CreateByteCode();
}