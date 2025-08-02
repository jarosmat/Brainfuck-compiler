using Utilities;
namespace BytecodeInterpreter;

public class FirstLevelCodeRunner : CodeRunner
{
    /*
    First Level code runner has 8 instructions, it aims to reduce instruction that are multiple behind each other in code to simplify into one operation instead of many
    * 0x00 is Increment, it has 1 byte parameter, where amount to added is specified
    * 0x01 id Decrement,  it has 1 byte parameter, where amount to subtracted is specified
    * 0x02 is Shift Data pointer left, it has 4 byte parameter, where amount to shift is specified
    * 0x03 is Shift Data pointer right, it has 4 byte parameter, where amount to shift is specified
    * 0x04 is Print value to stdin "."
    * 0x05 is Read value from stdin ","
    * 0x06 is left bracket, it has 4 byte parameter, where address of corresponding right bracket is set, parameter is LSB first
    * 0x07 is right bracket, it has 4 byte parameter, where address of corresponding left bracket is set, parameter is LSB first
    */
    private const int AddressLenght = 4;
    private const int MemoryShiftParameterLenght = 4;
    private const int ArithmeticParameterLenght = 1;

    public FirstLevelCodeRunner(byte[] code) : base(code)
    {
    }

    public FirstLevelCodeRunner(string codeAddress) : base(codeAddress)
    {
    }

    public override RunFunc[] GetDelegates()
    {
        return
        [
            Increment, Decrement, ShiftAddressLeft, ShiftAddressRight, PrintValue, ReadValue, LoopStart, IterationEnd
        ];
    }

    protected void Increment()
    {
        Memory[DataPointer] += Code[InstructionPointer];
        InstructionPointer += ArithmeticParameterLenght;
    }

    protected void Decrement()
    {
        Memory[DataPointer] -= Code[InstructionPointer];
        InstructionPointer += ArithmeticParameterLenght;
    }

    protected void ShiftAddressRight()
    {
        DataPointer += ByteNumbersHandler.GetNumberInt(Code, InstructionPointer, MemoryShiftParameterLenght);
        InstructionPointer += MemoryShiftParameterLenght;
    }

    protected void ShiftAddressLeft()
    {
        DataPointer -= ByteNumbersHandler.GetNumberInt(Code, InstructionPointer, MemoryShiftParameterLenght);
        InstructionPointer += MemoryShiftParameterLenght;
    }

    protected void LoopStart()
    {
        if (Memory[DataPointer] != 0)
        {
            InstructionPointer += AddressLenght;
        }
        else
        {
            InstructionPointer = ByteNumbersHandler.GetNumberInt(Code, InstructionPointer, AddressLenght);
        }
    }

    protected void IterationEnd()
    {
        if (Memory[DataPointer] == 0)
        {
            InstructionPointer += AddressLenght;
        }
        else
        {
            InstructionPointer = ByteNumbersHandler.GetNumberInt(Code, InstructionPointer, AddressLenght);
        }
    }
}