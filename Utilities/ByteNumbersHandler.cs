namespace Utilities;

public static class ByteNumbersHandler
{
    public static byte[] GetBytes(int number)
    {
        byte[] result = BitConverter.GetBytes(number);
        if (!BitConverter.IsLittleEndian)
        {
            result = result.Reverse().ToArray();
        }
        return result;
    }
    
    public static void SetBytesInList(List<byte> bytes, int index, int count, int number)
    {
        byte[] bytesToChange = GetBytes(number);
        for (int i = 0; i < count; i++)
        {
            bytes[index + i] = bytesToChange[i];
        }
    }

    public static void AddBytesToList(List<byte> bytes, int number)
    {
        foreach (byte byteToAdd in ByteNumbersHandler.GetBytes(number))
        {
            bytes.Add(byteToAdd);
        }
    }
    
    public static int GetNumber(byte[] bytes)
    {
        if (!BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reverse().ToArray();
        }

        int result = BitConverter.ToInt32(bytes);
        return result;
    }
    
    public static int GetNumberInt(byte[] bytes, int index, int count)
    {
        return GetNumber(GetByteSpan(bytes, index, count));
    }
    
    public static byte[] GetByteSpan(byte[] bytes, int index, int count)
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