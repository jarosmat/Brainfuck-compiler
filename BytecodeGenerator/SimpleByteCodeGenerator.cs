using System.Data;

namespace BytecodeGenerator;

public class SimpleByteCodeGenerator
{
    private enum Instructions
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
    
    private class Bracket(uint address)
    {
        public uint Address { get; private set; } = address;
    }

    private readonly TextReader _streamReader;

    public SimpleByteCodeGenerator(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        _streamReader = new StreamReader(fileName);
    }

    public SimpleByteCodeGenerator(TextReader streamReader)
    {
        _streamReader = streamReader;
    }

    public byte[] CreateByteCode()
    {
        List<byte> bytes = new List<byte>();
        List<Bracket> bracketStack = new List<Bracket>();
        int currChar = _streamReader.Read();
        while (currChar != -1)
        {
            switch (currChar)
            {
                case '+':
                    bytes.Add((byte)Instructions.Increment);
                    break;
                case '-':
                    bytes.Add((byte)Instructions.Decrement);
                    break;
                case '<':
                    bytes.Add((byte)Instructions.ShiftDataLeft);
                    break;
                case '>':
                    bytes.Add((byte)Instructions.ShiftDataRight);
                    break;
                case '.':
                    bytes.Add((byte)Instructions.PrintValue);
                    break;
                case ',':
                    bytes.Add((byte)Instructions.ReadValue);
                    break;
                case '[':
                    Bracket bracket = new Bracket((uint)bytes.Count);
                    bracketStack.Add(bracket);
                    bytes.Add((byte)Instructions.LeftBracket);
                    bytes.Add(0x00);
                    bytes.Add(0x00);
                    break;
                case ']':
                    if (bracketStack.Count == 0)
                    {
                        throw new SyntaxErrorException("Invalid parenthesis");
                    }
                    bytes.Add((byte)Instructions.RightBracket);
                    int lastItem = bracketStack.Count - 1;
                    Bracket bracketPopped = bracketStack[lastItem];
                    bracketStack.RemoveAt(lastItem);
                    byte[] leftAddressBytes = BitConverter.GetBytes(bracketPopped.Address);
                    byte[] rightAddressBytes = BitConverter.GetBytes(bytes.Count + 2);
                    if (!BitConverter.IsLittleEndian)
                    {
                        bytes.Add(leftAddressBytes[^1]);
                        bytes.Add(leftAddressBytes[^2]);
                        bytes[(int)bracketPopped.Address + 1] = rightAddressBytes[^1];
                        bytes[(int)bracketPopped.Address + 2] = rightAddressBytes[^2];
                    }
                    else
                    {
                        bytes.Add(leftAddressBytes[1]);
                        bytes.Add(leftAddressBytes[0]);
                        bytes[(int)bracketPopped.Address + 1] = rightAddressBytes[1];
                        bytes[(int)bracketPopped.Address + 2] = rightAddressBytes[0];
                    }
                    break;
                }
                currChar = _streamReader.Read();
        }

        if (bracketStack.Count != 0)
        {
            throw new SyntaxErrorException("Invalid parenthesis");
        }
        return bytes.ToArray();
    }
}