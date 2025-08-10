namespace BytecodeInterpreter;

public class Interpreter
{
    public void Run(CodeRunner runner)
    {
        var codeLen = runner.GetCodeLen();
        var runtimeFunctions = runner.GetDelegates();
        while (runner.InstructionPointer < codeLen)
        {
            var instruction = runner.Code[runner.InstructionPointer];
            runner.InstructionPointer++;
            runtimeFunctions[instruction]();
        }
    }
}