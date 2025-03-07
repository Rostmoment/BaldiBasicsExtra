using BBE.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine.Rendering;

namespace BBE.Helpers
{
    public struct CodeInstructionInfo
    {
        private OpCode opcode;
        public OpCode OpCode
        {
            get
            {
                if (opcode == null)
                    return OpCodes.Nop;
                return opcode;
            }
        }
        public string operand;
        public CodeInstructionInfo(OpCode opCode, string operand)
        {
            this.opcode = opCode;
            this.operand = operand;
        }
        public CodeInstructionInfo(OpCode opCode)
        {
            this.opcode = opCode;
            this.operand = null;
        }
    }
    public static class TranspilerHelper
    {
        public static void DebugAllInstructionsInfo(this IEnumerable<CodeInstruction> instructions)
        {
            for (int x = 0; x < instructions.Count(); x++)
            {
                UnityEngine.Debug.Log("Index: " + x + "/Opcode: " + instructions.ToList()[x].opcode + "/Operand: " + instructions.ToList()[x].operand + "/Instruction" + instructions.ToList()[x]);
            }
        }
        public static IEnumerable<CodeInstruction> Replace(this IEnumerable<CodeInstruction> instructions, MethodInfo info, bool isVirtual, params CodeInstructionInfo[] infos)
        {
            if (isVirtual)
                return instructions.Replace(new CodeInstruction(OpCodes.Callvirt, info), infos);
            return instructions.Replace(new CodeInstruction(OpCodes.Call, info), infos);
        }
        public static IEnumerable<CodeInstruction> Replace(this IEnumerable<CodeInstruction> instructions, Type classType, string methodName, bool isVirtual, params CodeInstructionInfo[] infos)
        {
            return instructions.Replace(AccessTools.Method(classType, methodName), isVirtual, infos);
        }
        public static IEnumerable<CodeInstruction> Replace(this IEnumerable<CodeInstruction> instructions, CodeInstruction[] toReplace, params CodeInstructionInfo[] infos)
        {
            List<CodeInstruction> result = new List<CodeInstruction>();
            int max = instructions.Count() - infos.Length;
            bool found = false;
            for (int i = 0; i < instructions.Count(); i++)
            {
                if (i > max)
                    return instructions;
                if (!found)
                {
                    bool res = true;
                    for (int n = 0; n < infos.Length; n++)
                    {
                        if (!res)
                            break;
                        CodeInstructionInfo info = infos[n];
                        if (info.OpCode != instructions.ElementAt(i + n).opcode)
                            res = false;
                        if (info.operand != null)
                        {
                            if (!instructions.ElementAt(i).OperandIs(info.operand))
                            {
                                res = false;
                            }
                        }
                    }
                    found = res;
                    if (res)
                    {
                        result.AddRange(toReplace);
                        i += infos.Length;
                    }
                }
                result.Add(instructions.ElementAt(i));
            }
            return result;
        }
        public static IEnumerable<CodeInstruction> Replace(this IEnumerable<CodeInstruction> instructions, CodeInstruction toReplace, params CodeInstructionInfo[] infos) =>
            instructions.Replace(new CodeInstruction[] { toReplace }, infos);
        public static IEnumerable<CodeInstruction> Remove(this IEnumerable<CodeInstruction> instructions, params OpCode[] opcodes) => instructions.Remove(opcodes.Select(x => new CodeInstructionInfo(x)).ToArray());
        public static IEnumerable<CodeInstruction> Remove(this IEnumerable<CodeInstruction> instructions, params CodeInstructionInfo[] infos)
        {
            List<CodeInstruction> result = new List<CodeInstruction>();
            int max = instructions.Count() - infos.Length;
            bool found = false;
            for (int i = 0; i<instructions.Count(); i++)
            {
                if (i > max)
                    return instructions;
                if (!found)
                {
                    bool res = true;
                    for (int n = 0; n < infos.Length; n++)
                    {
                        if (!res)
                            break;
                        CodeInstructionInfo info = infos[n];
                        if (info.OpCode != instructions.ElementAt(i + n).opcode)
                            res = false;
                        if (info.operand != null)
                        {
                            if (!instructions.ElementAt(i).OperandIs(info.operand))
                            {
                                res = false;
                            }
                        }
                    }
                    found = res;
                    if (res)
                        i += infos.Length;
                }
                result.Add(instructions.ElementAt(i));
            }
            return result;
        }
        public static bool OperandIs(this CodeInstruction i, string operand)
        {
            return i.operand != null && i.operand.ToString().ToLower().Contains(operand.ToLower());
        }
    }
}
