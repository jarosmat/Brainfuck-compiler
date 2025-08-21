using System.Data;
using Utilities;
namespace BytecodeGenerator;

public class SecondLevelBytecodeGenerator : BytecodeGenerator
{

    private readonly Program _syntaxTree = new Program();
    
    private const Fingerprints Fingerprint = Fingerprints.SecondLevelByteCode;

    // for reference about values of these constants see reference docs/programmer.md
    private const int SetMemoryZeroSubstraction = 6;
    private const int MoveDataSubstraction = 13;

    private int _programLenght = 4; // initial value is 4 because there will always be 4 byte fingerprint at the beginning
    
    private static readonly int[] InstructionLengths = [2, 2, 5, 5, 1, 1, 5, 5, 1, 6];
    public SecondLevelBytecodeGenerator(FileInfo file) : base(file, Fingerprint) { }

    public SecondLevelBytecodeGenerator(TextReader streamReader) : base(streamReader, Fingerprint){ }

    private void AddOperation(ExtendedInstructions opCode, byte[]? pars)
    {
        _syntaxTree.AddNewOperation(opCode, pars);
        _programLenght += InstructionLengths[(int)opCode];
    }
    
    private void AddOperation(ExtendedInstructions opCode, int par)
    {
        _syntaxTree.AddNewOperation(opCode, ByteNumbersHandler.GetBytes(par));
        _programLenght += InstructionLengths[(int)opCode];
    }

    private void HandleMoveData(int incrementIndex)
    {
        _programLenght -= MoveDataSubstraction;
        List<Operation> operations = _syntaxTree.GetCurrentScope().Operations;
        
        byte[] pars = new byte[5];
        pars[0] = operations[incrementIndex].Parameters![0];
        
        Operation firstShift = operations[incrementIndex - 1];
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
    
    private bool HandleOptimizations(LoopStart loop)
    {
        bool result = false;
        if (loop.IsSetMemoryZero())
        {
            result = true;
            _programLenght -= SetMemoryZeroSubstraction;
            _syntaxTree.SwapCurrentScope(ExtendedInstructions.SetMemoryZero, null);
        }
        else if (loop.IsMoveDataStartDecrement())
        {
            result = true;
            HandleMoveData(2);
        }
        else if (loop.IsMoveDataStartShift())
        {
            result = true;
            HandleMoveData(1);
        }

        return result;
    }
    
    private void AddOperation(int charToAdd, int count)
    {
        switch (charToAdd)
        {
            case '+':
                AddOperation(ExtendedInstructions.Increment, [(byte)count]);
                break;
            case '-':
                AddOperation(ExtendedInstructions.Decrement, [(byte)count]);
                break;
            case '<':
                AddOperation(ExtendedInstructions.ShiftDataLeft, count);
                break;
            case '>':
                AddOperation(ExtendedInstructions.ShiftDataRight, count);
                break;
            case '.':
                AddOperation(ExtendedInstructions.PrintValue, null);
                break;
            case ',':
                AddOperation(ExtendedInstructions.ReadValue, null);
                break;
            case '[':
                AddOperation(ExtendedInstructions.LeftBracket, null);
                Bracket bracket = new Bracket(_programLenght);
                BracketStack.Add(bracket);
                break;
            case ']':
                if (BracketStack.Count == 0)
                {
                    throw new SyntaxErrorException("Invalid parenthesis");
                }

                LoopStart loop = _syntaxTree.GetCurrentScope();
                Bracket bracketPopped = NotRelatedFunctions.PopFromList(BracketStack);
                
                if (!HandleOptimizations(loop))
                {
                    //can't AddOperation as in other cases because of implementation of Program : LoopStart.AddNewOperation
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
