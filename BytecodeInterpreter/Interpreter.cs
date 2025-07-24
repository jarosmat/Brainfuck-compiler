namespace BytecodeInterpreter;

public class Interpreter
{
    public void Run(CodeRunner runner)
    {
        uint codeLen = runner.GetCodeLen();
        CodeRunner.RunFunc[] runtimeFunctions = runner.GetDelegates();
        while (runner.InstructionPointer < codeLen)
        {
            byte instruction = runner.Code[runner.InstructionPointer];
            runner.InstructionPointer++;
            runtimeFunctions[instruction]();
        }
    }
}