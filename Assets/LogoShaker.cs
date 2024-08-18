using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LogoShaker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOShakePosition(1, new Vector3(2, 2, 0), 10, 90, false, true).SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
