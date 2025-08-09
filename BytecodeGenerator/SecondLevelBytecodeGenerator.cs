using System.Data;
using Utilities;
namespace BytecodeGenerator;

public class SecondLevelBytecodeGenerator : BytecodeGenerator
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
        public readonly List<Operation> Operations = new List<Operation>();

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

        public bool IsMoveDataStartDecrement()
        {
            bool result = Operations is [
                    { OpCode: ExtendedInstructions.Decrement }, { OpCode: ExtendedInstructions.ShiftDataLeft },
                    { OpCode: ExtendedInstructions.Increment }, { OpCode: ExtendedInstructions.ShiftDataRight }
                ] or
                [
                    { OpCode: ExtendedInstructions.Decrement }, { OpCode: ExtendedInstructions.ShiftDataRight },
                    { OpCode: ExtendedInstructions.Increment }, { OpCode: ExtendedInstructions.ShiftDataLeft }
                ]; 
            return result &&
                   ByteNumbersHandler.GetNumber(Operations[1].Parameters) ==
                   ByteNumbersHandler.GetNumber(Operations[3].Parameters) && Operations[0].Parameters![0] == 0x01;
        }

        public bool IsMoveDataStartShift()
        {
            bool result = Operations is [
                    { OpCode: ExtendedInstructions.ShiftDataLeft }, { OpCode: ExtendedInstructions.Increment },
                    { OpCode: ExtendedInstructions.ShiftDataRight }, { OpCode: ExtendedInstructions.Decrement }
                ] or
                [
                    { OpCode: ExtendedInstructions.ShiftDataRight }, { OpCode: ExtendedInstructions.Increment },
                    { OpCode: ExtendedInstructions.ShiftDataLeft }, { OpCode: ExtendedInstructions.Decrement }
                ];
            return result &&
                   Operations[0].Parameters != null &&
                   Operations[2].Parameters != null &&
                   ByteNumbersHandler.GetNumber(Operations[0].Parameters) ==
                   ByteNumbersHandler.GetNumber(Operations[2].Parameters) && Operations[3].Parameters![0] == 0x01;
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
        
        public void SetLoopStartParams(byte[]? pars)
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
    public SecondLevelBytecodeGenerator(FileInfo file) : base(file, Fingerprint) { }

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
                    _programLenght -= 6; //Last bracket was not added
                    _syntaxTree.SwapCurrentScope(ExtendedInstructions.SetMemoryZero, null);
                }
                else if (loop.IsMoveDataStartDecrement())
                {
                    // Console.WriteLine("in here");
                    //int MoveDataExplicitLen = 2 + 2 + 5 + 5 + 5 + 5 = 24;
                    //int MoveDataLen = 6
                    _programLenght -= 13; //Last bracket was not added and lenght of this instruction was not added
                    List<Operation> operations = _syntaxTree.GetCurrentScope().Operations;
                    byte[] pars = new byte[5];
                    pars[0] = operations[2].Parameters![0];
                    Operation firstShift = operations[1];
                    byte[]? shift = firstShift.Parameters;
                    if (firstShift.OpCode == ExtendedInstructions.ShiftDataLeft)
                    {
                        shift = ByteNumbersHandler.GetBytes( - ByteNumbersHandler.GetNumber(firstShift.Parameters));
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        pars[i + 1] = shift![i];
                    }
                    _syntaxTree.SwapCurrentScope(ExtendedInstructions.MoveData, pars);
                }
                else if (loop.IsMoveDataStartShift())
                {
                    //int MoveDataExplicitLen = 2 + 2 + 5 + 5 + 5 + 5 = 24;
                    //int MoveDataLen = 6
                    _programLenght -= 19; //Last bracket was not added and lenght of this instruction was not added
                    List<Operation> operations = _syntaxTree.GetCurrentScope().Operations;
                    byte[] pars = new byte[5];
                    pars[0] = operations[1].Parameters![0];
                    Operation firstShift = operations[0];
                    byte[]? shift = firstShift.Parameters;
                    if (firstShift.OpCode == ExtendedInstructions.ShiftDataLeft)
                    {
                        shift = ByteNumbersHandler.GetBytes( - ByteNumbersHandler.GetNumber(firstShift.Parameters));
                    }
                
                    for (int i = 0; i < 4; i++)
                    {
                        pars[i + 1] = shift![i];
                    }
                    _syntaxTree.SwapCurrentScope(ExtendedInstructions.MoveData, pars);
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
        
        _syntaxTree.AddBytes(Bytes);
        
        return Bytes.ToArray();
    }
}
