namespace BytecodeGenerator;

public enum SimpleInstructions
{
    Increment = 0x00,
    Decrement = 0x01,
    ShiftDataLeft = 0x02,
    ShiftDataRight = 0x03,
    PrintValue = 0x04,
    ReadValue = 0x05,
    LeftBracket = 0x06,
    RightBracket = 0x07
}

public enum ExtendedInstructions
{
    Increment = 0x00,
    Decrement = 0x01,
    ShiftDataLeft = 0x02,
    ShiftDataRight = 0x03,
    PrintValue = 0x04,
    ReadValue = 0x05,
    LeftBracket = 0x06,
    RightBracket = 0x07,
    SetMemoryZero = 0x08,
    MoveData = 0x09,
}