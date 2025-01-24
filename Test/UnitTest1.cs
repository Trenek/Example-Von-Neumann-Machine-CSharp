using System.ComponentModel;
using System.Reflection;

namespace Test;

[TestFixture]
public class ParserTests {
    private MethodInfo GetMethod(string methodName)
    {
        if (string.IsNullOrWhiteSpace(methodName))
            Assert.Fail("methodName cannot be null or whitespace");

        var method = (new Parser()).GetType()
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null)
            Assert.Fail(string.Format("{0} method not found", methodName));

        return method;
    }

    [Test]
    public void skipNums() {
        MethodInfo skipNums = GetMethod("skipNums");

        Assert.AreEqual("  Hihi", skipNums.Invoke(null, new object[] { "1234  Hihi" }));
        Assert.AreEqual("  Hihi", skipNums.Invoke(null, new object[] { "  Hihi" }));
        Assert.AreEqual("  Hihi 123", skipNums.Invoke(null, new object[] { "  Hihi 123" }));
        Assert.AreEqual(" 1 Hihi ", skipNums.Invoke(null, new object[] { " 1 Hihi " }));
        Assert.AreEqual("", skipNums.Invoke(null, new object[] { "123" }));
        Assert.AreEqual("", skipNums.Invoke(null, new object[] { "" }));
    }

    [Test]
    public void isWhiteSpace() {
        MethodInfo isWhiteSpace = GetMethod("isWhiteSpace");

        Assert.AreEqual(true, isWhiteSpace.Invoke(null, new object[] { ' ' }));
        Assert.AreEqual(true, isWhiteSpace.Invoke(null, new object[] { '\0' }));
        Assert.AreEqual(false, isWhiteSpace.Invoke(null, new object[] { 'c' }));
        Assert.AreEqual(true, isWhiteSpace.Invoke(null, new object[] { '\t' }));
        Assert.AreEqual(true, isWhiteSpace.Invoke(null, new object[] { '\r' }));
        Assert.AreEqual(true, isWhiteSpace.Invoke(null, new object[] { '\n' }));
    }

    [Test]
    public void skipWhiteSpaces() {
        MethodInfo isWhiteSpace = GetMethod("skipWhiteSpaces");

        Assert.AreEqual("Aha   ", isWhiteSpace.Invoke(null, new object[] { " Aha   " }));
        Assert.AreEqual("Aha   ", isWhiteSpace.Invoke(null, new object[] { "Aha   " }));
        Assert.AreEqual("", isWhiteSpace.Invoke(null, new object[] { "     " }));
        Assert.AreEqual("", isWhiteSpace.Invoke(null, new object[] { "" }));
    }

    [Test]
    public void isCommand() {
        MethodInfo isCommand = GetMethod("isCommand");

        for (int i = 0; i < 16; i += 1) {
            Assert.AreEqual(i, isCommand.Invoke(null, new object[] { Def.codeName[i] }));
        }
        Assert.AreEqual(-1, isCommand.Invoke(null, new object[] { "aha" }));
    }

    [Test]
    public void isMode() {
        MethodInfo isMode = GetMethod("isMode");

        for (int i = 0; i < 4; i += 1) {
            Assert.AreEqual(i, isMode.Invoke(null, new object[] { Def.modeName[i] }));
        }
        Assert.AreEqual(-1, isMode.Invoke(null, new object[] { "aha" }));
    }

    [Test]
    public void checkFor() {
        MethodInfo checkFor = GetMethod("checkFor");

        Assert.AreEqual("d ", checkFor.Invoke(null, new object[] { "aha  d ", "aha" }));
        Assert.AreEqual("", checkFor.Invoke(null, new object[] { "aha   ", "a h a" }));
        Assert.AreEqual(null, checkFor.Invoke(null, new object[] { "aha   ", "a %d a" }));
        Assert.AreEqual("s ", checkFor.Invoke(null, new object[] { "a123a  s ", "a %d a" }));
        Assert.AreEqual("", checkFor.Invoke(null, new object[] { "stop 623 C", "%c %d %m" }));
    }

    [Test]
    public void validateLine() {
        MethodInfo validateLine = GetMethod("validateLine");

        Assert.AreEqual(true, validateLine.Invoke(null, new object[] { "MEM[0]: 1" }));
        Assert.AreEqual(true, validateLine.Invoke(null, new object[] { "MEM [ 0 ] : load C 1" }));
        Assert.AreEqual(true, validateLine.Invoke(null, new object[] { "MEM[0]:load C 1" }));
        Assert.AreEqual(true, validateLine.Invoke(null, new object[] { "MEM[0]: stop" }));
        Assert.AreEqual(false, validateLine.Invoke(null, new object[] { "MEM[0]: stop C 1" }));
        Assert.AreEqual(false, validateLine.Invoke(null, new object[] { "stop C 1" }));
        Assert.AreEqual(false, validateLine.Invoke(null, new object[] { "MEM[0] stop C 1" }));
    }

    [Test]
    public void removeComments() {
        MethodInfo removeComments = GetMethod("removeComments");

        Assert.AreEqual("aspdii ", removeComments.Invoke(null, new object[] { "aspdii ;; ;d;a;f df; efw" }));
    }

    [Test]
    public void getMemPtr() {
        MethodInfo getMemPtr = GetMethod("getMemPtr");

        Assert.AreEqual(0, getMemPtr.Invoke(null, new object[] { "MEM[0]" }));
        Assert.AreEqual(-1, getMemPtr.Invoke(null, new object[] { "MEM[-1]" }));
        Assert.AreEqual(3, getMemPtr.Invoke(null, new object[] { "MEM[3]" }));
        Assert.AreEqual(5, getMemPtr.Invoke(null, new object[] { "MEM[5]" }));
        Assert.AreEqual(511, getMemPtr.Invoke(null, new object[] { "MEM[511]" }));
        Assert.AreEqual(512, getMemPtr.Invoke(null, new object[] { "MEM[512]" }));
        Assert.AreEqual(513, getMemPtr.Invoke(null, new object[] { "MEM[513]" }));
    }

    [Test]
    public void getCode() {
        MethodInfo getCode = GetMethod("getCode");

        for (int i = 0; i < 16; i += 1) {
            Assert.AreEqual(i, getCode.Invoke(null, new object[] { Def.codeName[i] }));
        }
        Assert.AreEqual(-1, getCode.Invoke(null, new object[] { "aha" }));
    }

    [Test]
    public void getMode() {
        MethodInfo getMode = GetMethod("getMode");

        for (int i = 0; i < 4; i += 1) {
            Assert.AreEqual(i, getMode.Invoke(null, new object[] { Def.modeName[i] }));
        }
        Assert.AreEqual(-1, getMode.Invoke(null, new object[] { "aha" }));
    }
}