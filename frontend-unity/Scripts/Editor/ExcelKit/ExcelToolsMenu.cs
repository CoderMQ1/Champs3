using System.Collections;
using System.Collections.Generic;
using System.IO;
using Editor.ExcelKit;
using UnityEditor;
using UnityEngine;

public static class ExcelToolsMenu
{
    [MenuItem("Tools/ExcelKits/ReadOneExcel")]
    public static void ReadOneExcel()
    {
        var filePath = ExcelUtils.ImportFile("xlsx");
        ExcelUtils.LoadExcel(filePath);
    }
    [MenuItem("Tools/ExcelKits/ExportMapData")]
    public static void ExportMapData()
    {
        string separator = Path.DirectorySeparatorChar.ToString();
        string path = EditorUtility.OpenFolderPanel("请选择要导出的目录", "../../champs3-config/", "");
        
        
        MapDataExporter.ExportMapData(path);
    }
}
