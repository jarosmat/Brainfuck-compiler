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
                    Bytes.Add((byte)SimpleInstructions.LeftBracket);
                    var bracket = new Bracket(Bytes.Count);
                    BracketStack.Add(bracket);
                    for (int i = 0; i < AddressLenght; i++)
                    {
                        Bytes.Add(0x00);
                    }
                    break;
                case ']':
                    if (BracketStack.Count == 0)
                    {
                        throw new SyntaxErrorException("Invalid parenthesis");
                    }
                    Bytes.Add((byte)SimpleInstructions.RightBracket);
                    
                    var bracketPopped = NotRelatedFunctions.PopFromList(BracketStack);
                    
                    ByteNumbersHandler.SetBytesInList(Bytes, bracketPopped.Address, AddressLenght, Bytes.Count + AddressLenght);
                    
                    foreach (var byteToAdd in ByteNumbersHandler.GetBytes(bracketPopped.Address - 1))
                    {
                        Bytes.Add(byteToAdd);
                    }
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