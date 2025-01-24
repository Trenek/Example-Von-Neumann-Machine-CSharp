class Printer {
    private static int getMaxLength(UInt16 maxMem, memCell[] MEM, bool[] isCode, int numBase) {
        int result = 0;
        int current = 0;
        
        for (UInt16 i = 0; i <= maxMem; i += 1) {
            current = isCode[i] ? MEM[i].arg : MEM[i].val;
            
            if (Math.Abs(current) > result) {
                result = Math.Abs(current);
            }
        }
        
        return (int)(Math.Log(result, numBase)) + 2;
    }

    public static void printMem(UInt16 maxMem, bool[] isCode, int numBase, bool uns, PMC pmc) {
        int length = getMaxLength(maxMem, pmc.MEM, isCode, numBase);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[AC {0}] [OR {1}]\n", Convert.ToString(pmc.AC, numBase), Convert.ToString(pmc.OR, numBase));
        for (UInt16 i = 0; i <= maxMem; i += 1) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (i == pmc.PC) Console.BackgroundColor = ConsoleColor.Magenta;
            Console.Write("MEM [ {0:D" + ((int)Math.Log10(maxMem) + 1) + "} ]: ", i);
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("{0,-5} ", isCode[i] ? Def.codeName[pmc.MEM[i].code] : "");
            
            if (pmc.MEM[i].code != (short)Code.STOP || isCode[i] == false) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("{0,-1} ", isCode[i] ? Def.modeName[pmc.MEM[i].mode] : "");
                
                Console.ForegroundColor = ConsoleColor.Red;
                Int16 val = isCode[i] ? pmc.MEM[i].arg : pmc.MEM[i].val;
                if (uns) val = Math.Abs(val);
                Console.Write("{0," + length + "}", Convert.ToString(val, numBase));
            }
            if (i == pmc.PC) Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
}