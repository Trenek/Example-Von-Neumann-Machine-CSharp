public class Setup {
    public String fileName;
    public int numBase;
    public bool uns;
    public bool debugMode;
    public bool stop;
    public bool clean;

    public Setup(String[] Args) {
        int i = 0;

        fileName = "main.vnm";
        numBase = 10;
        uns = false;
        debugMode = false;
        stop = false;
        clean = false;

        while (i < Args.Length) {
            switch (Args[i]) {
                case "-f":
                    this.fileName = Args[i + 1];
                    i += 1;
                    break;
                case "-base":
                    this.numBase = Int32.Parse(Args[i + 1]);
                    break;
                case "-signed":
                    this.uns = false;
                    break;
                case "-unsigned":
                    this.uns = true;
                    break;
                case "-debugMode":
                    this.debugMode = true;
                    if (i + 1 < Args.Length) switch (Args[i + 1]) {
                        case "cs":
                            this.stop = true;
                            this.clean = true;
                            i += 1;
                            break;
                        case "c":
                            this.clean = true;
                            i += 1;
                            break;
                        case "s":
                            this.stop = true;
                            i += 1;
                            break;
                    }
                    break;
                default:
                    Console.Write("Niepoprawny argument '{0}'", Args[i]);
                    break;
            }
            
            i += 1;
        }
    }
}
