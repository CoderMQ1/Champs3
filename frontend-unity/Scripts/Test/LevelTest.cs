using System.Collections;
using System.Collections.Generic;
using champs3.Hotfix;
using champs3.Hotfix.GameLogic;
using champs3.Hotfix.Map;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    public GameStateManager GameStateManager;

    public int MapId = 1;
    // Start is called before the first frame update
    void Start()
    {
        var initializeSync = ResourceManager.Instance.InitializeSync(() =>
        {
            MapCreater.Instance.StartCreateMap(MapId);
        });
        StartCoroutine(initializeSync);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
