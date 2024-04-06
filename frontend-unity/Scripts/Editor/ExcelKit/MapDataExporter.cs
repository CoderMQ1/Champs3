using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using SquareHero.Hotfix.Map;
using UnityEditor;
using UnityEngine;

namespace Editor.ExcelKit
{
    public static class MapDataExporter
    {
        public static string MapDataPath = "/Bundles/Map/";
        
        
        public static void ExportMapData(string path)
        {
            var root = Application.dataPath + MapDataPath;

            var files = Directory.GetFiles(root, "*.asset");

            List<TrackConfig> trackConfigs = new List<TrackConfig>();
            
            for (int i = 0; i <= files.Length; i++)
            {

                for (int j = 0; j < files.Length; j++)
                {
                    if (files[j].EndsWith($"_{i}.asset"))
                    {
                        var mapDatas = AssetDatabase.LoadAssetAtPath<MapData>($"Assets/{MapDataPath}/{Path.GetFileName(files[j])}");

                        trackConfigs.Add(GetTrackConfig(mapDatas));
                        break;
                    }
                }
            }

            string filePath = path + "/TrackConfig_Export.xlsx";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            Stream fileStream = null;
            try
            {
                
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage p = new ExcelPackage();

                var worksheet = p.Workbook.Worksheets.Add("TrackConfig");
                int col = 1, row = 1;
             
                for (int i = 0; i < trackConfigs.Count; i++)
                {
                    col = 1;
                    var trackConfig = trackConfigs[i];
 
                    worksheet.Cells[row, col].Value = trackConfig.Id;
                    col++;
                    worksheet.Cells[row, col].Value = "";
                    col++;
                    worksheet.Cells[row, col].Value = GetIntArrayString(trackConfig.TrackModule);
                    col++;
                    worksheet.Cells[row, col].Value = GetIntArrayString(trackConfig.TrackModuleNum);

                    row++;
                }
            
                p.SaveAs(filePath);
                p.Dispose();
            }
            catch (Exception e)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                throw;
            }

        }

        public static string GetIntArrayString(int[] array)
        {
            string result = "";

            for (int i = 0; i < array.Length; i++)
            {
                result += array[i] + ",";
            }

            return result.Substring(0, result.Length - 1);
        }

        private static TrackConfig GetTrackConfig(MapData mapData)
        {
            TrackConfig trackConfig = new TrackConfig();
            List<int> trackModule = new List<int>();
            List<int> trackModuleNum = new List<int>();

            for (int i = 0; i < mapData.TileDatas.Count; i++)
            {
                var tileData = mapData.TileDatas[i];
                int count = 0;
                for (int j = i; j < mapData.TileDatas.Count; j++)
                {
                    if (tileData.tileType == mapData.TileDatas[j].tileType)
                    {
                        count++;
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }

                var tileType = (int)tileData.tileType;
                if (tileType < 1000)
                {
                    tileType += 1;
                }
                trackModule.Add(tileType);
                trackModuleNum.Add(count);
            }

            trackConfig.Id = mapData.MapId;
            trackConfig.TrackModule = trackModule.ToArray();
            trackConfig.TrackModuleNum = trackModuleNum.ToArray();
            return trackConfig;
        }



        public class TrackConfig
        {
            public int Id;

            public string Explain;
            public int[] TrackModule;
            public int[] TrackModuleNum;
        }
    }
}