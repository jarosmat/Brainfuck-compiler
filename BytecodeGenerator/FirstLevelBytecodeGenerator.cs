using System.Data;
using Utilities;

namespace BytecodeGenerator;


public class FirstLevelBytecodeGenerator : BytecodeGenerator<SimpleInstructions>
{
    private const int MemoryShiftParameterLenght = 4;
    private const Fingerprints Fingerprint = Fingerprints.FirstLevelByteCode;
    
    public FirstLevelBytecodeGenerator(string fileName) : base(fileName, Fingerprint) { }

    public FirstLevelBytecodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint){ }

    private void AddOperation(int charToAdd, int count)
    {
        switch (charToAdd)
        {
            case '+':
                Bytes.Add((byte)SimpleInstructions.Increment);
                Bytes.Add((byte)count);
                break;
            case '-':
                Bytes.Add((byte)SimpleInstructions.Decrement);
                Bytes.Add((byte)count);
                break;
            case '<':
                Bytes.Add((byte)SimpleInstructions.ShiftDataLeft);
                ByteNumbersHandler.AddBytesToList(Bytes, count);
                break;
            case '>':
                Bytes.Add((byte)SimpleInstructions.ShiftDataRight);
                ByteNumbersHandler.AddBytesToList(Bytes, count);
                break;
            case '.':
                Bytes.Add((byte)SimpleInstructions.PrintValue);
                break;
            case ',':
                Bytes.Add((byte)SimpleInstructions.ReadValue);
                break;
            case '[':
                Bytes.Add((byte)SimpleInstructions.LeftBracket);
                Bracket bracket = new Bracket(Bytes.Count);
                BracketStack.Add(bracket);
                ByteNumbersHandler.AddBytesToList(Bytes, 0);
                break;
            case ']':
                if (BracketStack.Count == 0)
                {
                    throw new SyntaxErrorException("Invalid parenthesis");
                }
                Bytes.Add((byte)SimpleInstructions.RightBracket);
                    
                Bracket bracketPopped = NotRelatedFunctions.PopFromList(BracketStack);
                    
                ByteNumbersHandler.SetBytesInList(Bytes, bracketPopped.Address, AddressLenght, Bytes.Count + AddressLenght);
                ByteNumbersHandler.AddBytesToList(Bytes ,bracketPopped.Address + AddressLenght);
                
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
                AddOperation(lastChar, charCount);
                lastChar = currChar;
                charCount = 1;
            }
            else if (lastChar == '.' || lastChar == ',' || lastChar == '[' || lastChar == ']')
            {
                AddOperation( lastChar, charCount);
            }
            else
            {
                charCount++;
            }
            
            currChar = StreamReader.Read();
        }
        
        AddOperation(lastChar, charCount);
        
        if (BracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return Bytes.ToArray();
    }
}