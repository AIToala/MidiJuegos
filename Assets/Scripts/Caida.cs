using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caida : MonoBehaviour
{
    public Transform target;
    public float speed;
    public GameObject m1;
    public GameObject m2;
    public GameObject m3;
    public GameObject pr;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,target.position,step);
        if(transform.position == target.position)
        {
            pr.SetActive(false);
            m1.SetActive(true);
            m2.SetActive(true);
            m3.SetActive(true);

        }
    }
}
