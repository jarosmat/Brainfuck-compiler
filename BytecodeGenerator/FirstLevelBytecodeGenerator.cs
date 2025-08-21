using System.Data;
using Utilities;

namespace BytecodeGenerator;


public class FirstLevelBytecodeGenerator : BytecodeGenerator
{
    private const int MemoryShiftParameterLenght = 4;
    private const Fingerprints Fingerprint = Fingerprints.FirstLevelByteCode;
    
    public FirstLevelBytecodeGenerator(FileInfo file) : base(file, Fingerprint) { }

    public FirstLevelBytecodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint){ }

    private void AddOperation(int charToAdd, int count)
    {
        switch (charToAdd)
        {
            case '+':
                AddInstruction((byte)SimpleInstructions.Increment, [(byte)count]);
                break;
            case '-':
                AddInstruction((byte)SimpleInstructions.Decrement, [(byte)count]);
                break;
            case '<':
                AddInstruction((byte)SimpleInstructions.ShiftDataLeft, count);
                break;
            case '>':
                AddInstruction((byte)SimpleInstructions.ShiftDataRight, count);
                break;
            case '.':
                AddInstruction((byte)SimpleInstructions.PrintValue, null);
                break;
            case ',':
                AddInstruction((byte)SimpleInstructions.ReadValue, null);
                break;
            case '[':
                var bracket = new Bracket(Bytes.Count + 1);
                BracketStack.Add(bracket);
                AddInstruction((byte)SimpleInstructions.LeftBracket, 0);
                break;
            case ']':
                if (BracketStack.Count == 0)
                {
                    throw new SyntaxErrorException("Invalid parenthesis");
                }
                var bracketPopped = NotRelatedFunctions.PopFromList(BracketStack);
                ByteNumbersHandler.SetBytesInList(Bytes, bracketPopped.Address, AddressLenght, Bytes.Count + AddressLenght + 1);
                
                AddInstruction((byte)SimpleInstructions.RightBracket, bracketPopped.Address + AddressLenght);
                
                break;
        }
    }

    public override byte[] CreateByteCode()
    {
        var currChar = StreamReader.Read();
        var lastChar = 0;
        var charCount = 1;
        while (currChar != -1)
        {
            if (currChar != '.' && currChar != ',' && currChar != '[' && currChar != ']' && currChar != '+' &&
                currChar != '-' && currChar != '<' && currChar != '>')
            {
                currChar = StreamReader.Read();
                continue;
            }
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