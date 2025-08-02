namespace BytecodeInterpreter;

public class Interpreter
{
    //TODO: Need to add skipping fingerprint
    public void Run(CodeRunner runner)
    {
        int codeLen = runner.GetCodeLen();
        CodeRunner.RunFunc[] runtimeFunctions = runner.GetDelegates();
        while (runner.InstructionPointer < codeLen)
        {
            byte instruction = runner.Code[runner.InstructionPointer];
            runner.InstructionPointer++;
            runtimeFunctions[instruction]();
        }
    }
}