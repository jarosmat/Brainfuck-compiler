using Utilities;

namespace BytecodeInterpreter;

public class SecondLevelCodeRunner : FirstLevelCodeRunner
{
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
        // Not sure how parameters should work, I need to set multiplication, and target node
        // One byte for multiplication should be ok, since more is not practical and 4 bytes for address
        // Going to write some tests and then implement it, ok after instruction first byte is multiplication,
        // and 4 byte for relative position of node since I cant calculate exact address at compile time, these 4 bytes will be signed int
        throw new NotImplementedException();
    }
}