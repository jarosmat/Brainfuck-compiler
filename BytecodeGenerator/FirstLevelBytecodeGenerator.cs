using System.Data;
using Utilities;

namespace BytecodeGenerator;


public class FirstLevelBytecodeGenerator : BytecodeGenerator<SimpleInstructions>
{
    private const int MemoryShiftParameterLenght = 4;
    private const Fingerprints Fingerprint = Fingerprints.FirstLevelByteCode;
    
    public FirstLevelBytecodeGenerator(string fileName) : base(fileName, Fingerprint) { }

    public FirstLevelBytecodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint){ }

    private void AddOperation(List<byte> bytes, int charToAdd, int count)
    {
        switch (charToAdd)
        {
            case '+':
                bytes.Add((byte)SimpleInstructions.Increment);
                bytes.Add((byte)count);
                break;
            case '-':
                bytes.Add((byte)SimpleInstructions.Decrement);
                bytes.Add((byte)count);
                break;
            case '<':
                bytes.Add((byte)SimpleInstructions.ShiftDataLeft);
                ByteNumbersHandler.AddBytesToList(bytes, count);
                break;
            case '>':
                bytes.Add((byte)SimpleInstructions.ShiftDataRight);
                ByteNumbersHandler.AddBytesToList(bytes, count);
                break;
            case '.':
                bytes.Add((byte)SimpleInstructions.PrintValue);
                break;
            case ',':
                bytes.Add((byte)SimpleInstructions.ReadValue);
                break;
            case '[':
                bytes.Add((byte)SimpleInstructions.LeftBracket);
                Bracket bracket = new Bracket(bytes.Count);
                BracketStack.Add(bracket);
                ByteNumbersHandler.AddBytesToList(bytes, 0);
                break;
            case ']':
                if (BracketStack.Count == 0)
                {
                    throw new SyntaxErrorException("Invalid parenthesis");
                }
                bytes.Add((byte)SimpleInstructions.RightBracket);
                    
                int lastItem = BracketStack.Count - 1;
                Bracket bracketPopped = BracketStack[lastItem];
                BracketStack.RemoveAt(lastItem);
                    
                ByteNumbersHandler.SetBytesInList(bytes, bracketPopped.Address, AddressLenght, bytes.Count + AddressLenght);
                ByteNumbersHandler.AddBytesToList(bytes ,bracketPopped.Address + AddressLenght);
                
                break;
        }
    }

    public override byte[] CreateByteCode()
    {
        int currChar = StreamReader.Read();
        int lastChar = 0;
        int charCount = 1;
        while (currChar != -1)
        {
            if (currChar != lastChar)
            {
                AddOperation(Bytes, lastChar, charCount);
                lastChar = currChar;
                charCount = 1;
            }
            else if (lastChar == '.' || lastChar == ',' || lastChar == '[' || lastChar == ']')
            {
                AddOperation(Bytes, lastChar, charCount);
            }
            else
            {
                charCount++;
            }
            
            currChar = StreamReader.Read();
        }
        
        AddOperation(Bytes, lastChar, charCount);
        
        if (BracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return Bytes.ToArray();
    }
}