using System.Reflection.Metadata;

public struct PMC {
    public memCell[] MEM;
    public Int16 AC;
    public Int16 OR;
    public Int16 PC;
    public memCell IR;
}

public class Program {
    public static void Main(String[] Args) {
        PMC pmc = new PMC {
            MEM = new memCell[512]
        };

        bool[] isCode = new bool[512];
        UInt16 maxMem = 0;

        Setup setup = new Setup(Args);

        if (setup.clean) Console.Clear();
        if (Parser.loadPMC(setup.fileName, pmc.MEM, isCode, out pmc.PC, out maxMem)) {
            pmc.IR = pmc.MEM[pmc.PC];
            while (pmc.IR.code != (short)Code.STOP) {
                if (setup.debugMode) Printer.printMem(maxMem, isCode, setup.numBase, setup.uns, pmc);
                pmc.PC += 1;

                pmc.OR = (short)Def.modeHandlers[pmc.IR.mode](pmc);
                Def.codeHandlers[pmc.IR.code](ref pmc);

                pmc.IR = pmc.MEM[pmc.PC];
                if (setup.stop) Console.ReadKey();
                if (setup.clean) Console.Clear();
            }

            Printer.printMem(maxMem, isCode, setup.numBase, setup.uns, pmc);
        }
    }
}