using System.Data;
using Utilities;

namespace BytecodeGenerator;

public class FirstLevelBytecodeGenerator
{
    private enum Instructions
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
    
    private const int AddressLenght = 4;
    private const int MemoryShiftParameterLenght = 4;
    
    private class Bracket(uint address)
    {
        public uint Address { get; private set; } = address;
    }
    private List<Bracket> bracketStack;

    private readonly TextReader _streamReader;

    private static readonly int[] SingleOpChars = [44, 46, 91, 93];

    public FirstLevelBytecodeGenerator(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        _streamReader = new StreamReader(fileName);
    }

    public FirstLevelBytecodeGenerator(TextReader streamReader)
    {
        _streamReader = streamReader;
    }

    private void AddOperation(List<byte> bytes, int charToAdd, uint count)
    {
        switch (charToAdd)
        {
            case '+':
                bytes.Add((byte)Instructions.Increment);
                bytes.Add((byte)count);
                break;
            case '-':
                bytes.Add((byte)Instructions.Decrement);
                bytes.Add((byte)count);
                break;
            case '<':
                bytes.Add((byte)Instructions.ShiftDataLeft);
                ByteNumbersHandler.AddBytesToList(bytes, count);
                break;
            case '>':
                bytes.Add((byte)Instructions.ShiftDataRight);
                ByteNumbersHandler.AddBytesToList(bytes, count);
                break;
            case '.':
                bytes.Add((byte)Instructions.PrintValue);
                break;
            case ',':
                bytes.Add((byte)Instructions.ReadValue);
                break;
            case '[':
                bytes.Add((byte)Instructions.LeftBracket);
                Bracket bracket = new Bracket((uint)bytes.Count);
                bracketStack.Add(bracket);
                ByteNumbersHandler.AddBytesToList(bytes, 0);
                break;
            case ']':
                if (bracketStack.Count == 0)
                {
                    throw new SyntaxErrorException("Invalid parenthesis");
                }
                bytes.Add((byte)Instructions.RightBracket);
                    
                int lastItem = bracketStack.Count - 1;
                Bracket bracketPopped = bracketStack[lastItem];
                bracketStack.RemoveAt(lastItem);
                    
                ByteNumbersHandler.SetBytesInList(bytes, (int)bracketPopped.Address, AddressLenght, (uint)bytes.Count + AddressLenght);
                ByteNumbersHandler.AddBytesToList(bytes ,bracketPopped.Address + AddressLenght);
                
                break;
        }
    }

    public byte[] CreateByteCode()
    {
        List<byte> bytes = new List<byte>();
        bracketStack = new List<Bracket>();
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
        
        if (bracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return bytes.ToArray();
    }
}