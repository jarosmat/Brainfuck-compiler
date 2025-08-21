namespace Utilities;

public class AssemblyTranslator
{
    private static readonly AssemblyTranslator? RunnerFactory = null;
    private AssemblyTranslator(){}


    public static AssemblyTranslator GetTranslator()
    {
        return RunnerFactory ?? new AssemblyTranslator();
    }

    public static void PrintAssembly(byte[] code, StreamWriter streamWriter)
    {
        var version = ByteNumbersHandler.GetNumber(code, 0, 4);
        
        switch (version)
        {
            case 0:
                PrintAssemblySimpleBytecode(code, streamWriter);
                break;
            case 1:
                PrintAssemblyFirstLevelBytecode(code, streamWriter);
                break;
            case 2:
                PrintAssemblySecondLevelBytecode(code, streamWriter);
                break;
            default:
                Console.WriteLine("Code fingerprint is not valid");
                break;
        }
        
        streamWriter.Close();
    }

    private static bool CheckCorrectFingerprint(byte[] code, Fingerprints expectedFingerprint)
    {
        return ByteNumbersHandler.GetNumber(code, 0, 4) == (int)expectedFingerprint;
    }

    private static void PrintFingerPrint(StreamWriter streamWriter, Fingerprints fingerprint)
    {
        streamWriter.WriteLine((int)fingerprint);
    }

    private static void PrintBytes(StreamWriter streamWriter, byte[] bytes, int index, int span)
    {
        for (var i = 0; i < span; i++)
        {
            streamWriter.Write($"{bytes[index + i]} ");
        }
        streamWriter.Write("\n");
    }
    
    private static void PrintAssemblySimpleBytecode(byte[] code, StreamWriter streamWriter)
    {
        if (!CheckCorrectFingerprint(code, Fingerprints.SimpleByteCode))
        {
            throw new Exception("Wrong fingerprint for this code");
        }
        
        PrintFingerPrint(streamWriter, Fingerprints.SimpleByteCode);
        
        var index = 4;

        while (index < code.Length)
        {
            var instruction = code[index];
            index++;
            switch (instruction)
            {
                case 0x00:
                    streamWriter.WriteLine("add");
                    break;
                case 0x01:
                    streamWriter.WriteLine("sub");
                    break;
                case 0x02:
                    streamWriter.WriteLine($"shl");
                    break;
                case 0x03:
                    streamWriter.WriteLine("shr");
                    break;
                case 0x04:
                    streamWriter.WriteLine("out");
                    break;
                case 0x05:
                    streamWriter.WriteLine("in");
                    break;
                case 0x06:
                    streamWriter.Write($"lbr ");
                    for (var i = 0; i < 4; i++)
                    {
                        streamWriter.Write($"{code[index + i]} ");
                    }
                    streamWriter.Write("\n");
                    index += 4;
                    break;
                case 0x07:
                    streamWriter.Write($"rbr ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
            }
        }
    }

    private static void PrintAssemblyFirstLevelBytecode(byte[] code, StreamWriter streamWriter)
    {
        if (!CheckCorrectFingerprint(code, Fingerprints.FirstLevelByteCode))
        {
            throw new Exception("Wrong fingerprint for this code");
        }
        
        PrintFingerPrint(streamWriter, Fingerprints.FirstLevelByteCode);
        
        var index = 4;

        while (index < code.Length)
        {
            var instruction = code[index];
            index++;
            switch (instruction)
            {
                case 0x00:
                    streamWriter.WriteLine($"add {code[index]}");
                    index++;
                    break;
                case 0x01:
                    streamWriter.WriteLine($"sub {code[index]}");
                    index++;
                    break;
                case 0x02:
                    streamWriter.Write($"shl ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
                case 0x03:
                    streamWriter.Write($"shr ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
                case 0x04:
                    streamWriter.WriteLine("out");
                    break;
                case 0x05:
                    streamWriter.WriteLine("in");
                    break;
                case 0x06:
                    streamWriter.Write($"lbr ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
                case 0x07:
                    streamWriter.Write($"rbr ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
            }
        }
    }
    
    private static void PrintAssemblySecondLevelBytecode(byte[] code, StreamWriter streamWriter)
    {
        if (!CheckCorrectFingerprint(code, Fingerprints.SecondLevelByteCode))
        {
            throw new Exception("Wrong fingerprint for this code");
        }

        PrintFingerPrint(streamWriter, Fingerprints.SecondLevelByteCode);
        
        int index = 4;

        while (index < code.Length)
        {
            byte instruction = code[index];
            index++;
            switch (instruction)
            {
                case 0x00:
                    streamWriter.WriteLine($"add {code[index]}");
                    index++;
                    break;
                case 0x01:
                    streamWriter.WriteLine($"sub {code[index]}");
                    index++;
                    break;
                case 0x02:
                    streamWriter.Write($"shl ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
                case 0x03:
                    streamWriter.Write($"shr ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
                case 0x04:
                    streamWriter.WriteLine("out");
                    break;
                case 0x05:
                    streamWriter.WriteLine("in");
                    break;
                case 0x06:
                    streamWriter.Write($"lbr ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
                case 0x07:
                    streamWriter.Write($"rbr ");
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
                case 0x08:
                    streamWriter.WriteLine("s0");
                    break;
                case 0x09:
                    streamWriter.Write("mv ");
                    streamWriter.Write($"{code[index]} ");
                    index++;
                    PrintBytes(streamWriter, code, index, 4);
                    index += 4;
                    break;
            }
        }
    }
}