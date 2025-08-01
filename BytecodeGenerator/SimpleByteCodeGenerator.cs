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
        List<byte> bytes = new List<byte>();
        int currChar = _streamReader.Read();
        while (currChar != -1)
        {
            switch (currChar)
            {
                case '+':
                    bytes.Add((byte)SimpleInstructions.Increment);
                    break;
                case '-':
                    bytes.Add((byte)SimpleInstructions.Decrement);
                    break;
                case '<':
                    bytes.Add((byte)SimpleInstructions.ShiftDataLeft);
                    break;
                case '>':
                    bytes.Add((byte)SimpleInstructions.ShiftDataRight);
                    break;
                case '.':
                    bytes.Add((byte)SimpleInstructions.PrintValue);
                    break;
                case ',':
                    bytes.Add((byte)SimpleInstructions.ReadValue);
                    break;
                case '[':
                    bytes.Add((byte)SimpleInstructions.LeftBracket);
                    Bracket bracket = new Bracket((uint)bytes.Count);
                    _bracketStack.Add(bracket);
                    for (int i = 0; i < AddressLenght; i++)
                    {
                        bytes.Add(0x00);
                    }
                    break;
                case ']':
                    if (_bracketStack.Count == 0)
                    {
                        throw new SyntaxErrorException("Invalid parenthesis");
                    }
                    bytes.Add((byte)SimpleInstructions.RightBracket);
                    
                    int lastItem = _bracketStack.Count - 1;
                    Bracket bracketPopped = _bracketStack[lastItem];
                    _bracketStack.RemoveAt(lastItem);
                    
                    ByteNumbersHandler.SetBytesInList(bytes, (int)bracketPopped.Address, AddressLenght, (uint)bytes.Count + AddressLenght);
                    
                    foreach (byte byteToAdd in ByteNumbersHandler.GetBytes(bracketPopped.Address - 1))
                    {
                        bytes.Add(byteToAdd);
                    }
                    break;
                }
                currChar = _streamReader.Read();
        }

        if (_bracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return bytes.ToArray();
    }
}