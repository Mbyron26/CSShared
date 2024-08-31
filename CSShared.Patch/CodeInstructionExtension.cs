using System.Reflection.Emit;
using System.Reflection;
using System;
using HarmonyLib;

namespace CSShared.Patch;

public static class CodeInstructionExtension {
    public static CodeInstruction GetLdarg(this MethodInfo method, string argName) {
        var index = GetMethodParameterIndex(method, argName);
        if (!method.IsStatic)
            index++;
        return index switch {
            0 => new CodeInstruction(OpCodes.Ldarg_0),
            1 => new CodeInstruction(OpCodes.Ldarg_1),
            2 => new CodeInstruction(OpCodes.Ldarg_2),
            3 => new CodeInstruction(OpCodes.Ldarg_3),
            _ => new CodeInstruction(OpCodes.Ldarg_S, index)
        };
    }

    public static byte GetMethodParameterIndex(MethodInfo method, string name) {
        var parameters = method.GetParameters();
        for (byte i = 0; i < parameters.Length; ++i) {
            if (parameters[i].Name == name) {
                return i;
            }
        }
        throw new Exception($"The [{name}] parameter cannot be found in the method [{method.DeclaringType.FullName}]");
    }
}