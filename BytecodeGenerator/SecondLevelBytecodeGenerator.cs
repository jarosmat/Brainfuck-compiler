using System.Data;
using Utilities;
namespace BytecodeGenerator;

public class SecondLevelBytecodeGenerator : BytecodeGenerator<ExtendedInstructions>
{
    private class Operation(ExtendedInstructions opCode, byte[]? pars)
    {
        protected ExtendedInstructions OpCode { get;  } = opCode;
        protected byte[]? Parameters { get; } = pars;

        public virtual void AddBytes(List<byte> bytes)
        {
            bytes.Add((byte)OpCode);
            if (Parameters != null)
                foreach (var byt in Parameters)
                {
                    bytes.Add(byt);
                }
        }
    }
    
    private class LoopStart(byte[]? pars) : Operation(ExtendedInstructions.LeftBracket, pars)
    {
        protected readonly List<Operation> Operations = new List<Operation>();

        public void AddOperation(ExtendedInstructions opCode, byte[]? pars)
        {
            Operations.Add(new Operation(opCode, pars));
        }
        
        public override void AddBytes(List<byte> bytes)
        {
            bytes.Add((byte)OpCode);
            if (Parameters != null)
                foreach (var byt in Parameters)
                {
                    bytes.Add(byt);
                }

            foreach (var operation in Operations)
            {
                operation.AddBytes(bytes);
            }
        }
    }
    
    private class Program : LoopStart
    {
        private readonly List<LoopStart> _scopeStack = new List<LoopStart>();
        private LoopStart _currentScope;

        public Program() : base(null)
        {
            _currentScope = this;
        }

        public void AddNewOperation(ExtendedInstructions opCode, byte[]? pars)
        {
            if (OpCode == ExtendedInstructions.LeftBracket)
            {
                _scopeStack.Add(_currentScope);
                _currentScope = new LoopStart(pars);
            }
            else if (OpCode == ExtendedInstructions.RightBracket)
            {
                _currentScope.AddOperation(opCode, pars);
                _currentScope = NotRelatedFunctions.PopFromList(_scopeStack);
            }
            else
            {
                _currentScope.AddOperation(opCode, pars);
            }
        }

        public override void AddBytes(List<byte> bytes)
        {
            foreach (var operation in Operations)
            {
                operation.AddBytes(bytes);
            }
        }
    }

    private List<Operation> SyntaxTree = new List<Operation>();
    
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
