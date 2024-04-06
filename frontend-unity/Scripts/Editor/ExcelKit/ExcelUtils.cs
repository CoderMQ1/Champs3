using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using ES3Internal;
using LitJson;
using Markdig.Helpers;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;
using QFramework;
using Sirenix.Utilities.Editor;
using SquareHero.Hotfix.Map;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

public class ExcelUtils
{
    public static string ImportFile(string filetype)
    {
        string separator = Path.DirectorySeparatorChar.ToString();
        string path = EditorUtility.OpenFilePanel("选择要导入的文件", "", filetype);
        return path;
    }


    public static void LoadExcel(string path)
    {
        var fileName = Path.GetFileName(path);
        if (!fileName.EndsWith(".xlsx"))
        {
            return;
        }

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string fileNameWithoutCS = fileNameWithoutExtension;
        string cs = "cs";
        if (fileNameWithoutExtension.Contains("@"))
        {
            string[] ss = fileNameWithoutExtension.Split("@");
            fileNameWithoutCS = ss[0];
            cs = ss[1];
        }

        if (cs == "")
        {
            cs = "cs";
        }

        string protoName = fileNameWithoutCS;
        if (fileNameWithoutCS.Contains('_'))
        {
            protoName = fileNameWithoutCS.Substring(0, fileNameWithoutCS.LastIndexOf('_'));
        }

        ExcelPackage p = null;
        Stream stream = null;
        Table table = new Table();
        table.name = protoName;
        try
        {
            stream = File.Open(path, FileMode.Open);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            p = new ExcelPackage(stream);
            
            foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
            {
                Debug.Log($"{worksheet.Name}");
                
                ExportSheetClass(worksheet, table);

                StringBuilder sb = new StringBuilder();
                ExportSheetJson(worksheet, table.name, table.HeadInfos, sb);
                string jsonStr = sb.ToString();
                Debug.Log($"{jsonStr}");
                EditorPrefs.SetString("tableName", table.name);
                EditorPrefs.SetString("tableData", jsonStr);
                GenerateSerializedScriptableObject(table);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
            throw e;
        }
        finally
        {
            p.Dispose();
            stream.Close();
        }
        GenerateSerializedScriptableObject(table);
        Debug.Log("finish");
    }
    
    [DidReloadScripts]
    static void SaveTableData()
    {
        void Exporter(double obj, JsonWriter writer)
        {
            writer.Write(obj.ToString());
        }
        JsonMapper.RegisterExporter((ExporterFunc<double>)Exporter);

        double Importer(string obj)
        {
            double value;
            if (double.TryParse(obj, out value))
            {
                return value;
            }

            return 0f;
        }
            
        JsonMapper.RegisterImporter((ImporterFunc<string, double>)Importer);
        
        Single SingleImporter(double obj)
        {
            return (Single)obj;
        }
        JsonMapper.RegisterImporter((ImporterFunc<double, Single>)SingleImporter);
        
        var tableName = EditorPrefs.GetString("tableName");
        var tableData = EditorPrefs.GetString("tableData");
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(tableData))
        {
            return;
        }
        EditorPrefs.DeleteKey("tableName");
        EditorPrefs.DeleteKey("tableData");
        ExcelGenerateConfig config = new ExcelGenerateConfig();
        string tablePath = $"Assets/{config.AssetSavePath}{tableName}";
        string dataPath = $"{tablePath}/Data";
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        var assemblyCSharp = GetAssemblyCSharp();
        string className = $"{config.NameSpace}.{tableName}Table";
        var type = assemblyCSharp.GetType(className);

        var fieldInfo = type.GetField("Data");
        var instance = JsonMapper.ToObject(tableData, type);

        var data = (IEnumerable)fieldInfo.GetValue(instance);
        var enumerator = data.GetEnumerator();
        int i = 0;
        while (enumerator.MoveNext())
        {
            var current = (Object)enumerator.Current;
            var assetType = current.GetType();
            var idField = assetType.GetField("Id");
            if (idField != null)
            {
                var value = idField.GetValue(current);
                AssetDatabase.CreateAsset(current, $"{dataPath}/{tableName}-{value}.asset");
            }
            else
            {
                AssetDatabase.CreateAsset(current, $"{dataPath}/{tableName}-{i}.asset");
            }

            i++;
        }

        Object obj = (Object)instance;
        AssetDatabase.CreateAsset(obj, $"{tablePath}/{tableName}.asset");
        AssetDatabase.Refresh();
    }

    static void ExportSheetJson(ExcelWorksheet worksheet, string name,
        Dictionary<string, HeadInfo> classField,  StringBuilder sb)
    {
        sb.Append("{\"Data\":[");
        for (int row = 6; row <= worksheet.Dimension.End.Row; ++row)
        {
            string prefix = worksheet.Cells[row, 2].Text.Trim();
            if (prefix.Contains("#"))
            {
                continue;
            }

            if (prefix == "")
            {
                prefix = "cs";
            }
            
            if (worksheet.Cells[row, 3].Text.Trim() == "")
            {
                continue;
            }

            sb.Append("{");
            // sb.Append($"\"_t\":\"{name}\"");
            for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldName = worksheet.Cells[4, col].Text.Trim();
                if (!classField.ContainsKey(fieldName))
                {
                    continue;
                }

                HeadInfo headInfo = classField[fieldName];

                if (headInfo == null)
                {
                    continue;
                }
                
                string fieldN = headInfo.FieldName;
                var empty = col == 3 ? "" : ",";
                sb.Append($"{empty}\"{fieldN}\":{Convert(headInfo.FieldType, worksheet.Cells[row, col].Text.Trim())}");
            }


            sb.Append("},\n");
            
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]}\n");
        
    }
    
    private static string Convert(string type, string value)
    {
        switch (type)
        {
            case "uint[]":
            case "int[]":
            case "int32[]":
            case "long[]":
                return $"[{value}]";
            case "string[]":
            case "int[][]":
                return $"[{value}]";
            case "int":
            case "uint":
            case "int32":
            case "int64":
            case "long":
            case "float":
            case "double":
                if (value == "")
                {
                    return "0";
                }

                return value;
            case "string":
                value = value.Replace("\\", "\\\\");
                value = value.Replace("\"", "\\\"");
                return $"\"{value}\"";
            default:
                throw new Exception($"不支持此类型: {type}");
        }
    }

    static void ExportSheetClass(ExcelWorksheet worksheet, Table table)
    {
        const int row = 2;
        for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
        {
            string prefix = worksheet.Cells[row, col].Text.Trim();
            if (prefix.Contains("#"))
            {
                continue;
            }
            if (worksheet.Name.StartsWith("#"))
            {
                continue;
            }

            string fieldName = worksheet.Cells[row + 2, col].Text.Trim();
            if (fieldName == "")
            {
                continue;
            }

            if (table.HeadInfos.ContainsKey(fieldName))
            {
                continue;
            }

            string fieldCS = worksheet.Cells[row, col].Text.Trim().ToLower();
            if (fieldCS.Contains("#"))
            {
                table.HeadInfos[fieldName] = null;
                continue;
            }

            if (fieldCS == "")
            {
                fieldCS = "cs";
            }

            if (table.HeadInfos.TryGetValue(fieldName, out var oldClassField))
            {
                if (oldClassField.FieldCS != fieldCS)
                {
                    Debug.Log(
                        $"field cs not same: {worksheet.Name} {fieldName} oldcs: {oldClassField.FieldCS} {fieldCS}");
                }

                continue;
            }

            string fieldDesc = worksheet.Cells[row + 1, col].Text.Trim();
            string fieldType = worksheet.Cells[row + 3, col].Text.Trim();

            table.HeadInfos[fieldName] = new HeadInfo(fieldCS, fieldDesc, fieldName, fieldType, ++table.Index);
        }
    }


    static void GenerateSerializedScriptableObject(Table table)
    {
        StringBuilder sb = new StringBuilder();
        ExcelGenerateConfig config = new ExcelGenerateConfig();
        CultureInfo cuinfo = new CultureInfo("zh");
        DateTime dt = DateTime.Now;
        string lBrace = "{";
        string rBrace = "}";
        string tableSymbol = "\t";
        // HeadInfo
        sb.Append($"//{dt.ToString(cuinfo)}");
        sb.AppendLine();
        // Reference
        sb.Append("using UnityEngine;");
        sb.AppendLine();
        // NameSpace
        sb.Append($"namespace {config.NameSpace}");
        sb.AppendLine();
        sb.Append(lBrace);
        sb.AppendLine();
        // Class
        sb.Append($"{tableSymbol}public class {table.name} : ScriptableObject");
        sb.AppendLine();
        sb.Append($"{tableSymbol}{lBrace}");
        sb.AppendLine();
        //Property


        foreach (var item in table.HeadInfos)
        {
            sb.Append($"{tableSymbol}{tableSymbol}//{item.Value.FieldDesc}");
            sb.AppendLine();
            sb.Append($"{tableSymbol}{tableSymbol}public {item.Value.FieldType} {item.Value.FieldName};");
            sb.AppendLine();
        }


        sb.Append($"{tableSymbol}{rBrace}");
        sb.AppendLine();
        sb.Append(rBrace);
        sb.AppendLine();

        string scriptsPath = $"{Application.dataPath}/{config.ScriptPath}";
        if (!Directory.Exists(scriptsPath))
        {
            Directory.CreateDirectory(scriptsPath);
        }

        string filePath = $"{Application.dataPath}/{config.ScriptPath}{table.name}.cs";
        Debug.Log(filePath);
        
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"generate cs file : {filePath}");
        sb.Clear();
        string tableFileName = $"{table.name}Table";
        sb.Append($"//{dt.ToString(cuinfo)}");
        sb.AppendLine();
        // Reference
        sb.Append("using UnityEngine;");
        sb.Append("using System.Collections.Generic;");
        sb.AppendLine();
        // NameSpace
        sb.Append($"namespace {config.NameSpace}");
        sb.AppendLine();
        sb.Append(lBrace);
        sb.AppendLine();
        // Class
        sb.Append($"{tableSymbol}public class {tableFileName} : ScriptableObject");
        sb.AppendLine();
        sb.Append($"{tableSymbol}{lBrace}");
        sb.AppendLine();
        sb.Append($"{tableSymbol}{tableSymbol}public List<{table.name}> Data;");
        sb.AppendLine();
        sb.Append($"{tableSymbol}{rBrace}");
        sb.AppendLine();
        sb.Append(rBrace);
        sb.AppendLine();
        filePath = $"{Application.dataPath}/{config.ScriptPath}{tableFileName}.cs";
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"generate cs file : {filePath}");
        AssetDatabase.Refresh();
    }

    public static Assembly GetAssemblyCSharp()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies)
        {
            if (a.FullName.StartsWith("Hotfix"))
            {
                return a;
            }
        }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp.dll");
        return null;
    }
}

class HeadInfo
{
    public string FieldCS;
    public string FieldDesc;
    public string FieldName;
    public string FieldType;
    public int FieldIndex;

    public HeadInfo(string cs, string desc, string name, string type, int index)
    {
        this.FieldCS = cs;
        this.FieldDesc = desc;
        this.FieldName = name;
        this.FieldType = type;
        this.FieldIndex = index;
    }
}

class Table
{
    public bool C;
    public bool S;
    public int Index;
    public string name;
    public Dictionary<string, HeadInfo> HeadInfos = new Dictionary<string, HeadInfo>();
}

public class ExcelGenerateConfig
{
    public string ScriptPath = "Scripts/SquareHero/Hotfix/Generate/ExcelModel/";
    public string NameSpace = "SquareHero.Hotfix.Generate";
    public string AssetSavePath = "Bundles/Excel/";
}