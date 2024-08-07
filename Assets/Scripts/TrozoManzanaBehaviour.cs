using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrozoManzanaBehaviour : MonoBehaviour    
{
    public GameObject m1;
    RaycastHit2D hit;
    Camera cam;
    Vector3 pos;
    Vector3 mousepos;
    Transform focus;
    bool isDrag;


    // Start is called before the first frame update
    void Start()
    {
        isDrag = false;
        cam = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
        hit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition));
            if(hit.collider != null ) {
                focus = hit.transform;
                print("CLICKED = "+hit.collider.transform.name);
                isDrag= true;

            
            
            }
        }else if(Input.GetMouseButtonUp(0) && isDrag == true)
        {
            m1.SetActive(false);
            isDrag=false;

        }else if (isDrag == true)
        {
            mousepos = Input.mousePosition;
            mousepos.z = -cam.transform.position.z;
            pos = cam.ScreenToWorldPoint(mousepos);
            focus.position = new Vector3(pos.x, pos.y, focus.position.z);
        }
        
    }
}
