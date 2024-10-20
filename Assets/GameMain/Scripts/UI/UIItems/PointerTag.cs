using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTag : MonoBehaviour
{

    public GameObject PointerTagGO;

    public void ShowPointerTag(bool isShow)
    {
        PointerTagGO.SetActive(isShow);
    }

    private void OnEnable()
    {
        ShowPointerTag(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
