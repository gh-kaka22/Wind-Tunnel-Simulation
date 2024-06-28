using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnle : MonoBehaviour
{
    // Start is called before the first frame update

    public void UpdateSize(Vector3 size)
    {
        Vector3 currentScale = transform.localScale;
        if(size.x > 1){
            currentScale.x = size.x;
        }
        else if(size.y > 1){
            currentScale.y = size.y;
        }
        else{
            currentScale.z = size.z;
        }
        
        transform.localScale = currentScale;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}