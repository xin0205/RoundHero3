using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCycler : MonoBehaviour
{

    [SerializeField] private float time;
    [SerializeField] private GameObject go;

    
    private float _time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time >= time)
        {
            _time = 0;
            go.SetActive(false);
            go.SetActive(true);
        }
    }
}
