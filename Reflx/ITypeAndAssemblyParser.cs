using System.Reflection;

namespace Reflx;

public interface ITypeAndAssemblyParser {
    Type GetType(TypeAndAssembly typeAndAsm);
    Type GetType(TypeAndAssembly typeAndAsm, Assembly asm);
    TypeAndAssembly Parse(String source);
}