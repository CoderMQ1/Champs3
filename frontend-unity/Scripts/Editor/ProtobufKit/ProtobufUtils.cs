// 
// 2023/12/14

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Editor.ProtobufKit
{
    public class ProtobufUtils
    {
        
        private static readonly char[] SplitChars = { ' ', '\t' };
        private static List<string> _enumList;
        public static void ImportProtobuf(string path)
        {
            var fileName = Path.GetFileName(path);
            if (!fileName.EndsWith(".proto"))
            {
                return;
            }
            
            string s = File.ReadAllText(path);

            ProtobufImportConfig config = ProtobufImportConfig.Load();

            var pathRoot = Application.dataPath + config.ScriptPath;
            if (!Directory.Exists(pathRoot))
            {
                Directory.CreateDirectory(pathRoot);
            }
            StringBuilder sb = new StringBuilder();
            StringBuilder createSb = new StringBuilder();
            StringBuilder initSb = new StringBuilder();
            StringBuilder releaseSb = new StringBuilder();
            sb.Append("using ProtoBuf;\n");
            sb.Append("using System.Collections.Generic;\n");
            sb.Append($"namespace {config.NameSpace}\n");
            sb.Append("{\n");
            
            var lines = s.Split("\n");
            bool isEnumStart = false;
            bool isMsgStart = false;
            foreach (var line in lines)
            {
                string newline = line.Trim();
                
                if (newline == "")
                {
                    continue;
                }
                if (isEnumStart)
                {
                    if (newline == "{")
                    {
                        sb.Append("\t{\n");
                        continue;
                    }
                    if (newline == "}")
                    {
                        isEnumStart = false;
                        sb.Append("\t}\n\n");
                        continue;
                    }
                    sb.Append("\t\t");
                    sb.Append(newline.Replace(";",","));
                    sb.Append("\n");
                }
                
                if (isMsgStart)
                {
                    if (newline == "{")
                    {
                        sb.Append("\t{\n");
                        continue;
                    }

                    if (newline == "}")
                    {
                        isMsgStart = false;
                        //释放函数
                        sb.Append("\t}\n");
                        continue;
                    }

                    if (newline.Trim().StartsWith("//"))
                    {
                        sb.AppendLine(newline);
                        continue;
                    }

                    if (newline.Trim() != "" && newline != "}")
                    {
                        if (newline.StartsWith("repeated"))
                        {
                            Repeated(sb, config.NameSpace, newline);
                        }
                        else
                        {
                            Members(sb, newline, true);
                        }
                    }
                }
                
                if (!isEnumStart && newline.StartsWith("message"))
                {
                    string parentClass = "";
                    isMsgStart = true;
                    string msgName = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

                    if (ss.Length == 2)
                    {
                        parentClass = ss[1].Trim();
                    }

                    
                    
                    // msgOpcode.Add(new OpcodeInfo() { Name = msgName, Opcode = ++startOpcode });
                    // if (!string.IsNullOrEmpty(opcodeClassName))
                    // {
                    //     sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
                    // }
                   
                    sb.Append($"\t[ProtoContract]\n");
                    sb.Append($"\tpublic partial class {msgName}");
                    if (parentClass == "IActorMessage" || parentClass == "IActorRequest" || parentClass == "IActorResponse")
                    {
                        sb.Append($", {parentClass}\n");
                    }
                    else if (parentClass != "")
                    {
                        sb.Append($", {parentClass}\n");
                    }
                    else
                    {
                        sb.Append("\n");
                    }
                    
                                        
                    if (ss.Length == 3)
                    {
                        parentClass = ss[1].Trim();
                        
                        if (ss[2].Trim() == "{")
                        {
                            sb.Append("\t{\n");
                        }
                    }
                    continue;
                }
                
                if (newline.StartsWith("enum"))
                {
                    isEnumStart = true;
                    string parentClass = "";
                    string msgName = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append($"\tpublic enum {msgName} \n");
                    if (ss.Length == 2)
                    {
                        parentClass = ss[1].Trim();
                    }

                    if (ss.Length == 3)
                    {
                        parentClass = ss[1].Trim();
                        if (ss[2].Trim() == "{")
                        {
                            sb.Append("\t{\n");
                        }
                    }
                    
                    
                    // sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
                    // sb.Append($"\t[ProtoContract]\n");
                    
                    continue;
                }

            }
            sb.Append("}\n");
            File.WriteAllText(Application.dataPath + config.ScriptPath + "/ProtobufMessages.cs", sb.ToString());
        }
        private static void Members(StringBuilder sb, string newline, bool isRequired)
        {
            try
            {
                int index = newline.IndexOf(";");
                if (index != -1)
                {
                    newline = newline.Remove(index);
                    string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                    string type = ss[0];
                    string name = ss[1];
                    int n = int.Parse(ss[3]);
                    string typeCs = ConvertType(type);
                    sb.Append($"\t\t[ProtoMember({n})]\n");
                    sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n\n");
                }  
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }
        
        private static void Repeated(StringBuilder sb, string ns, string newline)
        {
            try
            {
                int index = newline.IndexOf(";");
                newline = newline.Remove(index);
                string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[1];
                bool isComstomType = IsCustomType(type);
                type = ConvertType(type);
                string name = ss[2];
                int n = int.Parse(ss[4]);
                
                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic List<{type}> {name} = new List<{type}>();\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }
        
        private static string ConvertType(string type)
        {
            string typeCs = "";
            switch (type)
            {
                case "int16":
                    typeCs = "short";
                    break;
                case "int32":
                    typeCs = "int";
                    break;
                case "bytes":
                    typeCs = "byte[]";
                    break;
                case "uint32":
                    typeCs = "uint";
                    break;
                case "long":
                    typeCs = "long";
                    break;
                case "int64":
                    typeCs = "long";
                    break;
                case "uint64":
                    typeCs = "ulong";
                    break;
                case "uint16":
                    typeCs = "ushort";
                    break;
                default:
                    typeCs = type;
                    break;
            }

            return typeCs;
        }
        
        private static bool IsCustomType(string type)
        {
            if (_enumList.Contains(type))
            {
                return false;
            }
            bool result = true;
            switch (type)
            {
                case "int16":
                    result = false;
                    break;
                case "int32":
                    result = false;
                    break;
                case "int":
                    result = false;
                    break;
                case "bytes":
                    result = false;
                    break;
                case "byte":
                    result = false;
                    break;
                case "uint32":
                    result = false;
                    break;
                case "long":
                    result = false;
                    break;
                case "int64":
                    result = false;
                    break;
                case "uint64":
                    result = false;
                    break;
                case "uint16":
                    result = false;
                    break;
                case "float":
                    result = false;
                    break;
                case "bool":
                    result = false;
                    break;
                case "string":
                    result = false;
                    break;
                default:
                    result = true;
                    break;
            }

            return result;
        }
    }


    public class ProtobufImportConfig
    {
        public string ScriptPath = "/Scripts/champs3/Hotfix/Generate/Messages";
        public string NameSpace = "champs3.Hotfix.Generate";

        public static ProtobufImportConfig Load()
        {
            return new ProtobufImportConfig();
        }
    }
}