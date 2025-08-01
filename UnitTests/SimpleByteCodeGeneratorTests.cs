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
        Assert.Equal([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x03, 0x03, 0x02, 0x02, 0x05, 0x04], result);
    }
    [Fact]
    public void SimpleLoopGeneration()
    {
        StringReader stringReader = new StringReader("[+]++-->><<,.[->+<]");
        SimpleByteCodeGenerator byteCodeGenerator = new SimpleByteCodeGenerator(stringReader);
        byte[] result = byteCodeGenerator.CreateByteCode();
        Assert.Equal([0x00, 0x00, 0x00, 0x00, 0x06, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x07, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x03, 0x03, 0x02, 0x02, 0x05, 0x04, 0x06, 0x27, 0x00, 0x00, 0x00, 0x01, 0x03, 0x00, 0x02, 0x07, 0x19, 0x00, 0x00, 0x00], result);
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
        byte[] correctResult = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x1E, 0x00, 0x00, 0x00, 0x01, 0x06, 0x18, 0x00, 0x00, 0x00, 0x03, 0x07, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x07, 0x07, 0x00, 0x00, 0x00, 0x01, 0x01, 0x06, 0x4C,0x00, 0x00, 0x00, 0x00, 0x06, 0x3C, 0x00, 0x00, 0x00,0x03,0x06, 0x36,0x00, 0x00, 0x00, 0x07, 0x2C, 0x00, 0x00, 0x00, 0x02, 0x07, 0x26, 0x00, 0x00, 0x00, 0x06, 0x47, 0x00, 0x00, 0x00, 0x02, 0x07, 0x3C, 0x00, 0x00, 0x00, 0x07, 0x20, 0x00, 0x00, 0x00];
        byte[] actualResult = byteCodeGenerator.CreateByteCode();
        Assert.Equal(correctResult, actualResult);
    }
}