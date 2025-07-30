using System.Data;
using Utilities;

namespace BytecodeGenerator;


public class FirstLevelBytecodeGenerator : BytecodeGenerator<SimpleInstructions>
{
    private const int MemoryShiftParameterLenght = 4;
    
    

    public FirstLevelBytecodeGenerator(string fileName) : base(fileName) { }

    public FirstLevelBytecodeGenerator(TextReader streamReader) : base(streamReader){ }

    private void AddOperation(List<byte> bytes, int charToAdd, uint count)
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
                Bracket bracket = new Bracket((uint)bytes.Count);
                _bracketStack.Add(bracket);
                ByteNumbersHandler.AddBytesToList(bytes, 0);
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
                ByteNumbersHandler.AddBytesToList(bytes ,bracketPopped.Address + AddressLenght);
                
                break;
        }
    }

    public override byte[] CreateByteCode()
    {
        List<byte> bytes = new List<byte>();
        int currChar = _streamReader.Read();
        int lastChar = 0;
        uint charCount = 1;
        while (currChar != -1)
        {
            if (currChar != lastChar)
            {
                AddOperation(bytes, lastChar, charCount);
                lastChar = currChar;
                charCount = 1;
            }
            else if (lastChar == '.' || lastChar == ',' || lastChar == '[' || lastChar == ']')
            {
                AddOperation(bytes, lastChar, charCount);
            }
            else
            {
                charCount++;
            }
            
            currChar = _streamReader.Read();
        }
        
        AddOperation(bytes, lastChar, charCount);
        
        if (_bracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return bytes.ToArray();
    }
}