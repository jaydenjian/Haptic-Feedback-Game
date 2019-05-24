using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectHit2 : MonoBehaviour
{

    public Slider HealthBar2;
    Animator anim;
    float totoalTime = 0;
    static public bool beAttack2 = false;

    void Update()
    {
        totoalTime += Time.deltaTime;
        if (totoalTime > 0.3f )
        {
 
            beAttack2 = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Weapon")
        {
            totoalTime = 0;

            HealthBar2.value -= 20;
            beAttack2 = true;


        }
        if (other.gameObject.tag == "Tornado")
        {
            HealthBar2.value -= 3;

        }

    }

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }


}
