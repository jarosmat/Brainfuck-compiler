using System.Data;
using Utilities;
namespace BytecodeGenerator;

public class SecondLevelBytecodeGenerator : BytecodeGenerator<ExtendedInstructions>
{
    private readonly struct Operation(byte opCode, byte[] pars) 
    {
        public readonly byte OpCode { get;  } = opCode;
        public readonly byte[] Parameters { get; } = pars;

        public void AddBytes(List<byte> bytes)
        {
            bytes.Add(OpCode);
            foreach (var byt in Parameters)
            {
                bytes.Add(byt);
            }
        }
    }
    
    private struct Loop(int startAddress)
    {
        public readonly int Start { get; } = startAddress;
        public int? End { get; set; } = null;
        private readonly List<Operation> _operations = new List<Operation>();

        public void AddOperation(Operation operation)
        {
            _operations.Add(operation);
        }

        public void SetEnd(int end)
        {
            End = end;
        }

        public void AddBytes(List<byte> bytes)
        {
            if (End == null)
            {
                throw new Exception("End address has not been specified");
            }

            foreach (var operation in _operations)
            {
                operation.AddBytes(bytes);
            }
        }
    }

    private List<Operation> SyntaxTree;
    
    private const Fingerprints Fingerprint = Fingerprints.SecondLevelByteCode;
    
    public SecondLevelBytecodeGenerator(string fileName) : base(fileName, Fingerprint) { }

    public SecondLevelBytecodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint){ }

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
                    
                int lastItem = BracketStack.Count - 1;
                Bracket bracketPopped = BracketStack[lastItem];
                BracketStack.RemoveAt(lastItem);
                    
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
