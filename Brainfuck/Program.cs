using BytecodeGenerator;
using BytecodeInterpreter;
using Utilities;
using System.CommandLine;

namespace Brainfuck;
class Program
{
    static int Main(string[] args)
    {
        BytecodeGenerator.BytecodeGenerator bytecodeGenerator;
        BytecodeGeneratorFactory factory = BytecodeGeneratorFactory.GetFactory();
        CodeRunnerFactory codeRunnerFactory = CodeRunnerFactory.GetFactory();
        Interpreter interpreter = new Interpreter();
        
        var rootCommand = new RootCommand("Virtual machine for Brainfuck programing language");
        Option<int> versionOption = new Option<int>("-v");
        versionOption.Description = "Version of bytecode to use: 0 for Simple, 1 for first level, 2 for second level";
        versionOption.DefaultValueFactory = result => 2;
        
        Option<FileInfo> dumpBytecode = new Option<FileInfo>("-b");
        
        Argument<FileInfo> file = new Argument<FileInfo>("file to run");
        
        rootCommand.Options.Add(versionOption);
        rootCommand.Arguments.Add(file);
        rootCommand.Options.Add(dumpBytecode);
        
        
        rootCommand.SetAction(parseResult =>
        {
            FileInfo? fileInfo = parseResult.GetValue(file);
            int genType = parseResult.GetValue(versionOption);
            if (fileInfo != null) bytecodeGenerator = factory.GetBytecodeGenerator(genType, fileInfo);
            else
            {
                Console.WriteLine("Source file not specified");
                return 1; 
            }
        
            byte[] bytes = bytecodeGenerator.CreateByteCode();
            FileInfo? bytecodeDump = parseResult.GetValue(dumpBytecode);
            if (bytecodeDump != null)
            {
                AssemblyTranslator.PrintAssembly(bytes, bytecodeDump.CreateText());
            }
            
            CodeRunner codeRunner = codeRunnerFactory.GetCodeRunner(bytes);
            interpreter.Run(codeRunner);
            
            
            return 0;
        });
        
        ParseResult parseResult = rootCommand.Parse(args);
        
        return parseResult.Invoke();
    }
}