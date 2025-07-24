namespace BytecodeInterpreter;

public class SimpleCodeRunner : CodeRunner
{
    /*
     simple code runner has 8 instructions
     * 0x00 is Increment
     * 0x01 id Decrement
     * 0x02 is Shift Data pointer left
     * 0x03 is Shift Data pointer right
     * 0x04 is Print value to stdin
     * 0x05 is Read value from stdin
     * 0x06 is left bracket, it has 2 byte parameter, where address of corresponding right bracket is set, parameter is MSB first
     * 0x07 is right bracket, it has 2 byte parameter, where address of corresponding left bracket is set, parameter is MSB first
     */

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

    private void PrintValue()
    {
        Console.Write(Utf8.GetChars(Memory, (int)DataPointer, 1));
        // Console.Write(Memory[DataPointer].ToString());
    }

    private void ReadValue()
    {
        Memory[DataPointer] = (byte)Console.Read();
    }

    private void LoopStart()
    {
        if (Memory[DataPointer] != 0)
        {
            InstructionPointer += 2;
        }
        else
        {
            InstructionPointer = (uint)MergeBytes(Code[InstructionPointer], Code[InstructionPointer + 1]);
        }
    }

    private void IterationEnd()
    {
        if (Memory[DataPointer] == 0)
        {
            InstructionPointer += 2;
        }
        else
        {
            InstructionPointer = (uint)MergeBytes(Code[InstructionPointer], Code[InstructionPointer + 1]);
        }
    }
}