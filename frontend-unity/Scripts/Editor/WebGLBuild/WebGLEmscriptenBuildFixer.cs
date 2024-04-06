using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor.WebGLBuild
{
    public class WebGLEmscriptenBuildFixer : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;
        public void OnPreprocessBuild(BuildReport report)
        {
            Environment.SetEnvironmentVariable("PYTHONUTF8", "1");
        }
    }

}