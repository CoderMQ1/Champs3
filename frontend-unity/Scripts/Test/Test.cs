using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text Fps;

    private int _fps;

    private int _updateCount;

    private float _time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _updateCount++;
        _time += Time.deltaTime;
        if (_time >= 2)
        {
            _fps = _updateCount / 2;

            _updateCount = 0;
            _time = 0;
            Fps.text = _fps.ToString();
        }

    }
}
