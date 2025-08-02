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

    public static byte[] GetBytes(int number)
    {
        byte[] result = BitConverter.GetBytes(number);
        if (!BitConverter.IsLittleEndian)
        {
            result = result.Reverse().ToArray();
        }
        return result;
    }
    
    public static void SetBytesInList(List<byte> bytes, int index, int count, uint number)
    {
        byte[] bytesToChange = GetBytes(number);
        for (int i = 0; i < count; i++)
        {
            bytes[index + i] = bytesToChange[i];
        }
    }

    public static void AddBytesToList(List<byte> bytes, uint number)
    {
        foreach (byte byteToAdd in ByteNumbersHandler.GetBytes(number))
        {
            bytes.Add(byteToAdd);
        }
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

    public static uint GetNumber(byte[] bytes, uint index, int count)
    {
        return GetNumber(GetByteSpan(bytes, index, count));
    }

    public static byte[] GetByteSpan(byte[] bytes, uint index, int count)
    {
        byte[] result = new byte[count];
        int newIndex = (int)index;
        for (int i = 0; i < count; i++)
        {
            result[i] = bytes[index + i];
        }

        return result;
    }
}