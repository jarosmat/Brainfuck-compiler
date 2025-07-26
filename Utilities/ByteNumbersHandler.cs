namespace Utilities;

public static class ByteNumbersHandler
{
    public static byte[] GetBytes(uint number)
    {
        byte[] result = BitConverter.GetBytes(number);
        if (!BitConverter.IsLittleEndian)
        {
            result = result.Reverse().ToArray();
        }
        return result;
    }

    public static uint GetNumber(byte[] bytes)
    {
        if (!BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reverse().ToArray();
        }

        uint result = BitConverter.ToUInt32(bytes);
        return result;
    } 
}