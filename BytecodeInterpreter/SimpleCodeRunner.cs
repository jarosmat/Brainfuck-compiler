using Utilities;
namespace BytecodeInterpreter;

public class SimpleCodeRunner : CodeRunner
{
    /*
     simple code runner has 8 instructions
     * 0x00 is Increment
     * 0x01 id Decrement
     * 0x02 is Shift Data pointer left
     * 0x03 is Shift Data pointer right
     * 0x04 is Print value to stdin "."
     * 0x05 is Read value from stdin ","
     * 0x06 is left bracket, it has 4 byte parameter, where address of corresponding right bracket is set, parameter is LSB first
     * 0x07 is right bracket, it has 4 byte parameter, where address of corresponding left bracket is set, parameter is LSB first
     */
    private const int AddressLenght = 4; // represent lenght in bytes of address for jumps in cycles 
    public SimpleCodeRunner(byte[] code) : base(code){}

    public SimpleCodeRunner(string codeAddress) : base(codeAddress){}
    
    public override RunFunc[] GetDelegates()
    {
        return [Increment, Decrement, ShiftAddressLeft, ShiftAddressRight, PrintValue, ReadValue, LoopStart, IterationEnd];
    }

    private void Increment()
    {
        Memory[DataPointer] += 1;
    }

    private void Decrement()
    {
        Memory[DataPointer] -= 1;
    }
    
    private void ShiftAddressRight()
    {
        DataPointer += 1;
    }
    
    private void ShiftAddressLeft()
    {
        DataPointer -= 1;
    }

    private void LoopStart()
    {
        if (Memory[DataPointer] != 0)
        {
            InstructionPointer += AddressLenght;
        }
        else
        {
            InstructionPointer = ByteNumbersHandler.GetNumber(Code, InstructionPointer, AddressLenght);
        }
    }

    private void IterationEnd()
    {
        InstructionPointer = ByteNumbersHandler.GetNumber(Code, InstructionPointer, AddressLenght);
    }
}