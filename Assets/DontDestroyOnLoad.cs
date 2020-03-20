using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public Transform UIManagerTransform;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        transform.SetParent(UIManagerTransform);
    }
}
