namespace Utilities;

public static class ByteNumbersHandler
{
    public static byte[] GetBytes(int number)
    {
        var result = BitConverter.GetBytes(number);
        if (!BitConverter.IsLittleEndian)
        {
            result = result.Reverse().ToArray();
        }
        return result;
    }
    
    public static void SetBytesInList(List<byte> bytes, int index, int count, int number)
    {
        var bytesToChange = GetBytes(number);
        for (var i = 0; i < count; i++)
        {
            if (bytesToChange != null) bytes[index + i] = bytesToChange[i];
        }
    }

    public static void AddBytesToList(List<byte> bytes, int number)
    {
        bytes.AddRange(GetBytes(number));
    }
    
    public static int GetNumber(byte[]? bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException($"Argument bytes is null");
        }
        if (!BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reverse().ToArray();
        }

        var result = BitConverter.ToInt32(bytes);
        return result;
    }
    
    public static int GetNumber(byte[] bytes, int index, int count)
    {
        return GetNumber(GetByteSpan(bytes, index, count));
    }
    
    public static byte[] GetByteSpan(byte[] bytes, int index, int count)
    {
        var result = new byte[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = bytes[index + i];
        }

        return result;
    }
}