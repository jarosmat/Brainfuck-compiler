using BytecodeGenerator;
using BytecodeInterpreter;
using Utilities;
using System.CommandLine;

namespace Brainfuck;

internal static class Program
{
    private static int Main(string[] args)
    {
        BytecodeGenerator.BytecodeGenerator bytecodeGenerator;
        var factory = BytecodeGeneratorFactory.GetFactory();
        var codeRunnerFactory = CodeRunnerFactory.GetFactory();
        var interpreter = new Interpreter();
        
        var rootCommand = new RootCommand("Virtual machine for Brainfuck programing language");
        var versionOption = new Option<int>("-v")
        {
            Description = "Version of bytecode to use: 0 for Simple, 1 for first level, 2 for second level",
            DefaultValueFactory = _ => 2
        };

        var dumpBytecode = new Option<FileInfo>("-b")
        {
            Description = "Specify file where to export assembly of compiled code"
        };

        var file = new Argument<FileInfo>("file to run");
        
        rootCommand.Options.Add(versionOption);
        rootCommand.Arguments.Add(file);
        rootCommand.Options.Add(dumpBytecode);
        
        
        rootCommand.SetAction(parseResult =>
        {
            var fileInfo = parseResult.GetValue(file);
            var genType = parseResult.GetValue(versionOption);
            if (fileInfo == null)
            {
                Console.WriteLine("Source file not specified");
                return 1;
            }
            bytecodeGenerator = factory.GetBytecodeGenerator(genType, fileInfo);

            var bytes = bytecodeGenerator.CreateByteCode();
            var bytecodeDump = parseResult.GetValue(dumpBytecode);
            if (bytecodeDump != null)
            {
                AssemblyTranslator.PrintAssembly(bytes, bytecodeDump.CreateText());
            }
            
            var codeRunner = codeRunnerFactory.GetCodeRunner(bytes);
            interpreter.Run(codeRunner);
            
            
            return 0;
        });
        
        var parseResult = rootCommand.Parse(args);
        
        return parseResult.Invoke();
    }
}