using Utilities;

namespace BytecodeInterpreter;

public class SecondLevelCodeRunner : FirstLevelCodeRunner
{
    private const int MoveDataMemoryShift = 4;
    private const int MoveDataCoefficientLenght = 1;

    public SecondLevelCodeRunner(byte[] code) : base(code)
    {
    }

    public SecondLevelCodeRunner(string codeAddress) : base(codeAddress)
    {
    }

    public override RunFunc[] GetDelegates()
    {
        return
        [
            Increment, Decrement, ShiftAddressLeft, ShiftAddressRight, PrintValue, ReadValue, LoopStart, IterationEnd, SetMemoryZero, MoveData
        ];
    }

    private void SetMemoryZero()
    {
        Memory[DataPointer] = 0;
    }

    private void MoveData()
    {
        if (Memory[DataPointer] == 0)
        {
            InstructionPointer += MoveDataCoefficientLenght + MoveDataMemoryShift;
            return;
        }

        int coefficient = Code[InstructionPointer];
        InstructionPointer += MoveDataCoefficientLenght;
        var shift = ByteNumbersHandler.GetNumber(Code, InstructionPointer, MoveDataMemoryShift);
        InstructionPointer += MoveDataMemoryShift;

        Memory[DataPointer + shift] += (byte)(Memory[DataPointer] * coefficient);
        Memory[DataPointer] = 0;
    }
}