using System.Collections;
using System.Collections.Generic;
using Editor.ProtobufKit;
using UnityEditor;
using UnityEngine;

public static class ProtobufToolsMenu
{
    [MenuItem("Tools/ProtobufKit/Import")]
    public static void ReadOneExcel()
    {
        var filePath = ExcelUtils.ImportFile("proto");
        ProtobufUtils.ImportProtobuf(filePath);
    }
}
