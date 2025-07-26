using System.Data;
using BytecodeGenerator;
using System.Text;
namespace UnitTests;

public class SimpleByteCodeGeneratorTests
{
    private UTF8Encoding _utf8 = new UTF8Encoding();
    private readonly Random _random = new Random();
    [Fact]
    public void SimpleGeneration()
    {
        StringReader streamReader = new StringReader("++-->><<,.");
        SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(streamReader);
        byte[] result = byteCodeGenerator.CreateByteCode();
        Assert.Equal([0x00, 0x00, 0x01, 0x01, 0x03, 0x03, 0x02, 0x02, 0x05, 0x04], result);
    }
    [Fact]
    public void SimpleLoopGeneration()
    {
        StringReader stringReader = new StringReader("[+]++-->><<,.[->+<]");
        SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(stringReader);
        byte[] result = byteCodeGenerator.CreateByteCode();
        Assert.Equal([0x06, 0x00, 0x07, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x03, 0x03, 0x02, 0x02, 0x05, 0x04, 0x06, 0x00, 0x1B, 0x01, 0x03, 0x00, 0x02, 0x07, 0x00, 0x11], result);
    }

    [Fact]
    public void WrongParenthesis1()
    {
        StringReader stringReader = new StringReader("[[[[]]]");
        SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(stringReader);
        Assert.Throws<SyntaxErrorException>(() => byteCodeGenerator.CreateByteCode());
    }
    
    [Fact]
    public void WrongParenthesis2()
    {
        StringReader stringReader = new StringReader("[[]]]");
        SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(stringReader);
        Assert.Throws<SyntaxErrorException>(() => byteCodeGenerator.CreateByteCode());
    }

    [Fact]
    public void NestedLoopGeneration()
    {
        StringReader stringReader = new StringReader("+++[-[>]+]--[+[>[]<][<]]");
        SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(stringReader);
        byte[] correctResult = [0x00, 0x00, 0x00, 0x06, 0x00, 0x12, 0x01, 0x06, 0x00, 0x0E, 0x03, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x03, 0x01, 0x01, 0x06, 0x00,0x30,0x00, 0x06, 0x00,0x26,0x03,0x06,0x00,0x22,0x07,0x00,0x1C,0x02,0x07,0x00,0x18,0x06,0x00,0x2D,0x02,0x07,0x00,0x26,0x07,0x00,0x14];
        byte[] actualResult = byteCodeGenerator.CreateByteCode();
        Assert.Equal(correctResult, actualResult);
    }
}