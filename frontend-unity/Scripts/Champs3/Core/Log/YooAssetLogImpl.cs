using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using ILogger = YooAsset.ILogger;

public class YooAssetLogImpl : ILogger
{
    public void Log(string message)
    {
       LogKit.I(message);
    }

    public void Warning(string message)
    {
        LogKit.W(message);
    }

    public void Error(string message)
    {
        LogKit.E(message);
    }

    public void Exception(Exception exception)
    {
        LogKit.E(exception);
    }
}
