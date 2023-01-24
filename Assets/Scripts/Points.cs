using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour

{
    //public GameObject point;
    public Rigidbody r;

    // Start is called before the first frame update
    void Start()
    {
       r = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    
  
    //Destroy(point);
    {
        if(collision.gameObject.tag == "Points")
    {
        Destroy(collision.gameObject);
    }
    UpdateScoreTimer.score += 1;
    }
}
