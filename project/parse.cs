public class Parser {
    private static string skipNums(string line) {
        return new string(line.SkipWhile(Char.IsNumber).ToArray());
    }

    private static bool isWhiteSpace(char a) {
        return a == 0 || Char.IsWhiteSpace(a);
    }

    private static string skipWhiteSpaces(string line) {
        return line == null ? null : new string(line.SkipWhile(Char.IsWhiteSpace).ToArray());
    }

    private static int isCommand(string line) {
        string a = new string(line.TakeWhile(a => false == isWhiteSpace(a)).ToArray());
        return Array.IndexOf(Def.codeName, a);
    }

    private static int isMode(string line) {
        string a = new string(line.TakeWhile(a => false == isWhiteSpace(a)).ToArray());
        return Array.IndexOf(Def.modeName, a);
    }

    private static string checkFor(string a, string b) {
        int blockLengthB = 0;
        
        a = skipWhiteSpaces(a);
        b = skipWhiteSpaces(b);
        
        while (a != null && b.Length != 0 && a.Length != 0) {
            blockLengthB = b.IndexOf(' ') == -1 ? b.Length : b.IndexOf(' ');

            if (new string(b.Take(2).ToArray()) == "%c" && -1 != isCommand(a)) {
                a = new string(a.Skip(Def.codeName[isCommand(a)].Length).ToArray());
                b = new string(b.Skip(2).ToArray());
            }
            else if (new string(b.Take(2).ToArray()) == "%m" && -1 != isMode(a)) {
                a = new string(a.Skip(1).ToArray());
                b = new string(b.Skip(2).ToArray());
            }
            else if (new string(b.Take(2).ToArray()) == "%d" && (Char.IsNumber(a[0]) || ((a[0] == '-' || a[0] == '+') && Char.IsNumber(a[1])))) {
                a = skipNums(new string(a.Skip(1).ToArray()));
                b = new string(b.Skip(2).ToArray());
            }
            else if (new string(a.Take(blockLengthB).ToArray()) == new string(b.Take(blockLengthB).ToArray())) {
                a = new string(a.Skip(blockLengthB).ToArray());
                b = new string(b.Skip(blockLengthB).ToArray());
            }
            else {
                a = null;
            }
            
            a = skipWhiteSpaces(a);
            b = skipWhiteSpaces(b);
        }
        
        if (a != null && a.Length == 0 && b.Length != 0) {
            a = null;
        }
        
        return a;
    }

    private static bool validateLine(string line) {
        string tempLine;
        bool good = false;
        
        line = skipWhiteSpaces(line);
        
        if (line.Length == 0) good = true;
        else if (null != (line = checkFor(line, "MEM [ %d ] :"))) {
            if (line.Length == 0) good = true;
            else if (null != (tempLine = checkFor(line, "%d"))) {
                if (tempLine.Length == 0) good = true;
            }
            else if (null != (tempLine = checkFor(line, Def.codeName[(int)Code.STOP]))) {
                if (tempLine.Length == 0) good = true;
            }
            else if (null != (tempLine = checkFor(line, "%c %m %d"))) {
                if (tempLine.Length == 0) good = true;
            }
        }
        
        return good;
    }

    private static string removeComments(string line) {
        return new string(line.TakeWhile(a => a != ';').ToArray());
    }

    private static int getMemPtr(string line) {
        string numSpace = new string(line.SkipWhile(a => a != '[').Skip(1).ToArray());
        string num = new string(skipWhiteSpaces(numSpace).TakeWhile(Char.IsNumber).ToArray());
        return num.Length > 0 && Char.IsNumber(num[0]) ? Int32.Parse(num) : -1;
    }

    private static int getCode(string line) {
        int result = 15;
        
        while (result >= 0 && null == checkFor(line, Def.codeName[result])) result -= 1;
        
        return result;
    }

    private static int getMode(string line) {
        int result = 3;
        
        while (result >= 0 && null == checkFor(line, Def.modeName[result])) result -= 1;
        
        return result;
    }

    public static bool loadPMC(string fileName, memCell[] MEM, bool[] isCode, out Int16 PC, out UInt16 maxMemOut) {
        bool isOk = true;
        UInt16 maxMem = 0;
        PC = 0;
        
        using (StreamReader file = new StreamReader(fileName)) {
            string buffer;
            int tempInt = 0;
            bool f = true;
            int memAddress = 0;

            Console.ForegroundColor = ConsoleColor.Red;
            while (file.Peek() >= 0) {
                buffer = removeComments(file.ReadLine());
                
                if (validateLine(buffer)) {
                    if ((memAddress = getMemPtr(buffer)) != -1) {
                        maxMem = (UInt16)(maxMem > memAddress ? maxMem : memAddress);
                        MEM[memAddress].val = 0;
                        
                        char[] t = buffer.SkipWhile(a => a != ':').ToArray();
                        t[0] = ' ';
                        string line = new string(t);
                        if (-1 != (tempInt = getCode(line))) {
                            f = false;
                            isCode[memAddress] = true;
                            MEM[memAddress].code = (Int16)tempInt;
                            line = new string(line.Skip(line.IndexOf(Def.codeName[MEM[memAddress].code])).ToArray().Skip(Def.codeName[MEM[memAddress].code].Length).ToArray());

                            if (-1 != (tempInt = getMode(line))) {
                                MEM[memAddress].mode = (Int16)tempInt;
                                line = new string(line.Skip(line.IndexOf(Def.modeName[MEM[memAddress].mode])).ToArray().Skip(Def.modeName[MEM[memAddress].mode].Length).ToArray());
                            }
                        }
                        else {
                            isCode[memAddress] = false;
                        }
                        if (f) PC += 1;
                        if (null != checkFor(line, "%d")) {
                            tempInt = Int32.Parse(line);
                            MEM[memAddress].arg = (short)tempInt;
                        }
                    }
                }
                else {
                    Console.Write("Syntax Error '{0}'\n", buffer);
                    isOk = false;
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        maxMemOut = maxMem;
        return isOk;
    }
}