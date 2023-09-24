using System.Reflection;
using System.Text.RegularExpressions;
using Exy;

namespace Reflx;

public class TypeAndAssemblyParser : ITypeAndAssemblyParser {
    const String TypeAndAssemblyRegex = "^(?<TypeN>.+)(?:,\\s{1,}?)(?<AsmN>.+)$";
    readonly Regex typeNameRgx = new Regex(TypeAndAssemblyRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    TypeAndAssemblyParser() { }

    // NOTE: Used this instead of static class because of https://stackoverflow.com/a/7707369/1181782
    static TypeAndAssemblyParser instance;
    public static TypeAndAssemblyParser Instance => instance ??= new TypeAndAssemblyParser();

    public TypeAndAssembly Parse(String source) {
        String typeName = typeNameRgx.Match(source).Groups["TypeN"].Value;
        String asmName = typeNameRgx.Match(source).Groups["AsmN"].Value;
        if (String.IsNullOrEmpty(typeName) || String.IsNullOrEmpty(asmName))
            throw new UnintendedBehaviorException($"Wrong Type or Assembly. '{source}'.");

        return new TypeAndAssembly(typeName, asmName);
    }

    public Type GetType(TypeAndAssembly typeAndAsm) {
        if (typeAndAsm == null)
            throw new ArgumentNullException(nameof(typeAndAsm));

        Assembly asm = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SingleOrDefault(appDAsm => appDAsm.GetName().Name == typeAndAsm.Assembly);

        if (asm == null)
            throw new UnintendedBehaviorException($"Assembly '{typeAndAsm.Assembly}' was not found in the current AppDomain.");

        return GetType(typeAndAsm, asm);
    }

    public Type GetType(TypeAndAssembly typeAndAsm, Assembly asm) {
        if (typeAndAsm == null)
            throw new ArgumentNullException(nameof(typeAndAsm));

        if (asm == null)
            throw new ArgumentNullException(nameof(asm));

        Func<Type, Boolean> wasTheHackishAssemblyNameMatch = asmType => asmType.FullName.Replace("+", ".") == typeAndAsm.Type;
        Type type = asm
            .GetTypes()
            .FirstOrDefault(wasTheHackishAssemblyNameMatch);

        if (type == null)
            throw new UnintendedBehaviorException($"Type '{typeAndAsm.Type}' was not found. Assembly '{typeAndAsm.Assembly}'.");

        return type;
    }
}