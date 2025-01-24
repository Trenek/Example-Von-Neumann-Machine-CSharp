public struct memCell {
    public Int16 _val;
    
    public Int16 arg {
        get { return (Int16)(this.val & 0x1FF); }
        set { 
            this.val &= ~0x1FF;
            this.val |= (Int16)(0x1FF & value);
        }
    }
    
    public Int16 mode {
        get { return (Int16)((this.val >> 9) & 0x3); }
        set { 
            this.val &= ~(0x3 << 9);
            this.val |= (Int16)((value & 0x3) << 9);
        }
    }
    
    public Int16 code {
        get { return (Int16)((this.val >> 11) & 0xF); }
        set {
            this.val &= ~(0xF << 11);
            this.val |= (Int16)((value & 0xF) << 11);
        }
    }
    
    public Int16 val {
        get { return this._val; }
        set { this._val = value; }
    }
};

public enum Code {
    STOP,
    LOAD,
    STORE,
    JUMP,
    JNEG,
    JZERO,
    ADD,
    SUB,
    MULT,
    DIV,
    AND,
    OOR,
    NOT,
    CMP,
    SHZ,
    SHC
};

public enum Mode {
    Current,
    Direct,
    Indirect,
    Numbered
};

public delegate int ModeHandler(PMC arg);
public delegate void CodeHandler(ref PMC arg);

public class Def {
    public static readonly string[] codeName = {
        "stop",
        "load",
        "store",
        "jump",
        "jneg",
        "jzero",
        "add",
        "sub",
        "mult",
        "div",
        "and",
        "or",
        "not",
        "cmp",
        "shz",
        "shc"
    };
    
    public static readonly string[] modeName = {
        "C",
        "D",
        "I",
        "N"
    };

    public static readonly ModeHandler[] modeHandlers = {
        a => a.IR.arg,
        a => a.MEM[a.IR.arg].val,
        a => a.MEM[a.MEM[a.IR.arg].val].val,
        a => a.MEM[a.AC + a.IR.arg].val
    };

    public static readonly CodeHandler[] codeHandlers = {
        (ref PMC a) => { },
        (ref PMC a) => a.AC = a.OR,
        (ref PMC a) => a.MEM[a.OR].val = a.AC,
        (ref PMC a) => a.PC = a.OR,
        (ref PMC a) => a.PC = a.AC < 0 ? a.OR : a.PC,
        (ref PMC a) => a.PC = a.AC == 0 ? a.OR : a.PC,
        (ref PMC a) => a.AC += a.OR,
        (ref PMC a) => a.AC -= a.OR,
        (ref PMC a) => a.AC *= a.OR,
        (ref PMC a) => a.AC /= a.OR,
        (ref PMC a) => a.AC &= a.OR,
        (ref PMC a) => a.AC |= a.OR,
        (ref PMC a) => a.AC = (Int16)~a.OR,
        (ref PMC a) => a.AC = (Int16)(a.AC == a.OR ? -1 : 0),
        (ref PMC a) => a.AC = (Int16)((a.OR < 0) ? a.AC >> -a.OR : a.AC << a.OR),
        (ref PMC a) => a.AC = (Int16)((a.OR < 0) ? (a.AC << (16 - -a.OR)) | (a.AC >> -a.OR) :
                                                   (a.AC >> (16 - a.OR)) | (a.AC << a.OR))
    };
}
