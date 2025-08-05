using System.Data;
using Utilities;
namespace BytecodeGenerator;

public class SecondLevelBytecodeGenerator : BytecodeGenerator<ExtendedInstructions>
{
    private class Operation(ExtendedInstructions opCode, byte[]? pars)
    {
        public ExtendedInstructions OpCode { get;  } = opCode;
        public byte[]? Parameters { get; set; } = pars;

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

        public void AddOperation(Operation operation)
        {
            Operations.Add(operation);
        }
        
        public override void AddBytes(List<byte> bytes)
        {
            bytes.Add((byte)OpCode);
            if (Parameters != null)
            {
                foreach (var byt in Parameters)
                {
                    bytes.Add(byt);
                }
            }
            else
            {
                throw new Exception("Loop start does not contain address");
            }
            foreach (var operation in Operations)
            {
                operation.AddBytes(bytes);
            }
        }
        
        public bool IsSetMemoryZero()
        {
            bool result = Operations is [{ OpCode: ExtendedInstructions.Increment or ExtendedInstructions.Decrement }];
            return result;
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
            if (opCode == ExtendedInstructions.LeftBracket)
            {
                _scopeStack.Add(_currentScope);
                _currentScope = new LoopStart(pars);
            }
            else if (opCode == ExtendedInstructions.RightBracket)
            {
                _currentScope.AddOperation(new Operation(opCode, pars));
                LoopStart newScope = NotRelatedFunctions.PopFromList(_scopeStack);
                newScope.AddOperation(_currentScope);
                _currentScope = newScope;
            }
            else
            {
                _currentScope.AddOperation(new Operation(opCode, pars));
            }
        }

        public LoopStart GetCurrentScope()
        {
            return _currentScope;
        }

        public void SwapCurrentScope(ExtendedInstructions newInstructionOpCode, byte[]? pars)
        {
            _currentScope = NotRelatedFunctions.PopFromList(_scopeStack);
            _currentScope.AddOperation(new Operation(newInstructionOpCode, pars));
        }
        
        public void SetLoopStartParams(byte[] pars)
        {
            _currentScope.Parameters = pars;
        }

        public override void AddBytes(List<byte> bytes)
        {
            foreach (var operation in Operations)
            {
                operation.AddBytes(bytes);
            }
        }
    }

    private readonly Program _syntaxTree = new Program();
    
    private const Fingerprints Fingerprint = Fingerprints.SecondLevelByteCode;

    private int _programLenght = 4;
    
    private static readonly int[] InstructionLengths = [2, 2, 5, 5, 1, 1, 5, 5, 1, 6];
    public SecondLevelBytecodeGenerator(string fileName) : base(fileName, Fingerprint) { }

    public SecondLevelBytecodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint){ }

    private void AddOperation(int charToAdd, int count)
    {
        switch (charToAdd)
        {
            case '+':
                _syntaxTree.AddNewOperation(ExtendedInstructions.Increment, [(byte)count]);
                _programLenght += InstructionLengths[(int)ExtendedInstructions.Increment];
                break;
            case '-':
                _syntaxTree.AddNewOperation(ExtendedInstructions.Decrement, [(byte)count]);
                _programLenght += InstructionLengths[(int)ExtendedInstructions.Decrement];
                break;
            case '<':
                _syntaxTree.AddNewOperation(ExtendedInstructions.ShiftDataLeft, ByteNumbersHandler.GetBytes(count));
                _programLenght += InstructionLengths[(int)ExtendedInstructions.ShiftDataLeft];
                break;
            case '>':
                _syntaxTree.AddNewOperation(ExtendedInstructions.ShiftDataRight, ByteNumbersHandler.GetBytes(count));
                _programLenght += InstructionLengths[(int)ExtendedInstructions.ShiftDataRight];
                break;
            case '.':
                _syntaxTree.AddNewOperation(ExtendedInstructions.PrintValue, null);
                _programLenght += InstructionLengths[(int)ExtendedInstructions.PrintValue];
                break;
            case ',':
                _syntaxTree.AddNewOperation(ExtendedInstructions.ReadValue, null);
                _programLenght += InstructionLengths[(int)ExtendedInstructions.ReadValue];
                break;
            case '[':
                _programLenght += InstructionLengths[(int)ExtendedInstructions.LeftBracket];
                Bracket bracket = new Bracket(_programLenght);
                BracketStack.Add(bracket);
                _syntaxTree.AddNewOperation(ExtendedInstructions.LeftBracket, null);
                break;
            case ']':
                if (BracketStack.Count == 0)
                {
                    throw new SyntaxErrorException("Invalid parenthesis");
                }

                LoopStart loop = _syntaxTree.GetCurrentScope();
                Bracket bracketPopped = NotRelatedFunctions.PopFromList(BracketStack);

                if (loop.IsSetMemoryZero())
                {
                    // int SetMemoryZeroExplicitLen = 12;
                    // int SetMemoryZeroLen = 1;
                    _programLenght -= 6;
                    _syntaxTree.SwapCurrentScope(ExtendedInstructions.SetMemoryZero, null);
                }
                else
                {
                    _programLenght += InstructionLengths[(int)ExtendedInstructions.RightBracket];

                    _syntaxTree.SetLoopStartParams(ByteNumbersHandler.GetBytes(_programLenght));

                    _syntaxTree.AddNewOperation(ExtendedInstructions.RightBracket,
                        ByteNumbersHandler.GetBytes(bracketPopped.Address));
                }

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
        
        _syntaxTree.AddBytes(Bytes);
        
        return Bytes.ToArray();
    }
}
