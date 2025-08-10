using Utilities;
namespace BytecodeGenerator;

public class Operation(ExtendedInstructions opCode, byte[]? pars)
{
    public ExtendedInstructions OpCode { get;  } = opCode;
    public byte[]? Parameters { get; set; } = pars;

    public virtual void AddBytes(List<byte> bytes)
    {
        bytes.Add((byte)OpCode);
        if (Parameters == null) return;
        bytes.AddRange(Parameters);
    }
}
    
public class LoopStart(byte[]? pars) : Operation(ExtendedInstructions.LeftBracket, pars)
{
    public readonly List<Operation> Operations = [];

    public void AddOperation(Operation operation)
    {
        Operations.Add(operation);
    }
    
    public override void AddBytes(List<byte> bytes)
    {
        bytes.Add((byte)OpCode);
        if (Parameters != null)
        {
            bytes.AddRange(Parameters);
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
        var result = Operations is [{ OpCode: ExtendedInstructions.Increment or ExtendedInstructions.Decrement }];
        return result;
    }

    public bool IsMoveDataStartDecrement()
    {
        var result = Operations is [
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
        var result = Operations is [
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
    
public class Program : LoopStart
{
    private readonly List<LoopStart> _scopeStack = [];
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