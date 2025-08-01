using System.Data;
using BytecodeInterpreter;
using Utilities;

namespace BytecodeGenerator;

public enum SimpleInstructions
{
    Increment = 0x00,
    Decrement = 0x01,
    ShiftDataLeft = 0x02,
    ShiftDataRight = 0x03,
    PrintValue = 0x04,
    ReadValue = 0x05,
    LeftBracket = 0x06,
    RightBracket = 0x07
}
public class SimpleByteCodeGenerator : BytecodeGenerator<SimpleInstructions>
{
    private const Fingerprints Fingerprint = Fingerprints.SimpleByteCode;

    public SimpleByteCodeGenerator(string fileName) : base(fileName, Fingerprint) { }

    public SimpleByteCodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint) { }

    public override byte[] CreateByteCode()
    {

        int currChar = _streamReader.Read();
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
                    Bytes.Add((byte)SimpleInstructions.LeftBracket);
                    Bracket bracket = new Bracket((uint)Bytes.Count);
                    _bracketStack.Add(bracket);
                    for (int i = 0; i < AddressLenght; i++)
                    {
                        Bytes.Add(0x00);
                    }
                    break;
                case ']':
                    if (_bracketStack.Count == 0)
                    {
                        throw new SyntaxErrorException("Invalid parenthesis");
                    }
                    Bytes.Add((byte)SimpleInstructions.RightBracket);
                    
                    int lastItem = _bracketStack.Count - 1;
                    Bracket bracketPopped = _bracketStack[lastItem];
                    _bracketStack.RemoveAt(lastItem);
                    
                    ByteNumbersHandler.SetBytesInList(Bytes, (int)bracketPopped.Address, AddressLenght, (uint)Bytes.Count + AddressLenght);
                    
                    foreach (byte byteToAdd in ByteNumbersHandler.GetBytes(bracketPopped.Address - 1))
                    {
                        Bytes.Add(byteToAdd);
                    }
                    break;
                }
                currChar = _streamReader.Read();
        }

        if (_bracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return Bytes.ToArray();
    }
}