using System.Data;
using Utilities;

namespace BytecodeGenerator;

public class SimpleByteCodeGenerator : BytecodeGenerator
{
    private const Fingerprints Fingerprint = Fingerprints.SimpleByteCode;

    public SimpleByteCodeGenerator(FileInfo file) : base(file, Fingerprint) { }

    public SimpleByteCodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint) { }

    public override byte[] CreateByteCode()
    {

        int currChar = StreamReader.Read();
        while (currChar != -1)
        {
            switch (currChar)
            {
                case '+':
                    Bytes.Add((byte)SimpleInstructions.Increment);
                    break;
                case '-':
                    Bytes.Add((byte)SimpleInstructions.Decrement);
                    break;
                case '<':
                    Bytes.Add((byte)SimpleInstructions.ShiftDataLeft);
                    break;
                case '>':
                    Bytes.Add((byte)SimpleInstructions.ShiftDataRight);
                    break;
                case '.':
                    Bytes.Add((byte)SimpleInstructions.PrintValue);
                    break;
                case ',':
                    Bytes.Add((byte)SimpleInstructions.ReadValue);
                    break;
                case '[':
                    AddLoopStart((byte)SimpleInstructions.LeftBracket);
                    break;
                case ']':
                    AddLoopEnd((byte)SimpleInstructions.RightBracket, -1); //-1 is because when adding Bracket to bracket stack, the address is set at the first byte of loop strat parameters, but this must be at loop start instruction
                    break;
                }
                currChar = StreamReader.Read();
        }

        if (BracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return Bytes.ToArray();
    }
}