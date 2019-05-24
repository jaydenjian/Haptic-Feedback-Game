using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetectHit : MonoBehaviour
{

    public Slider HealthBar;
    //Animator anim;
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "ZombieHand")
        {
            HealthBar.value -= 10;

        }
        if (other.gameObject.tag == "BossSword")
        {
            HealthBar.value -= 20;

        }

    }

}
