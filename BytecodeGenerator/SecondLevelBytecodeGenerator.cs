using System.Data;
using Utilities;
namespace BytecodeGenerator;

public class SecondLevelBytecodeGenerator : BytecodeGenerator
{

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
