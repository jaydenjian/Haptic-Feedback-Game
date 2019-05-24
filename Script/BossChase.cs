using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossChase : MonoBehaviour
{

    public Transform Player;
    public Animator anim;
    public Slider HealthBar;
    public Slider PlayerHealthBar;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (HealthBar.value <= 0)
        {

            anim.SetBool("IsDead", true);
            anim.SetBool("IsAttacking", false);
            Destroy(this.gameObject, 5);
            return;
        }
        if (PlayerHealthBar.value <= 0)
        {

            anim.SetBool("IsAttacking", false);
            anim.SetBool("IsIdle", true);
            return;
        }


        if (Vector3.Distance(Player.position, this.transform.position) < 20)
        {
            Vector3 Direction = Player.position - this.transform.position;
            Direction.y = 0;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(Direction), 0.1f);

            anim.SetBool("IsIdle", false);
            if (Direction.magnitude > 2)
            {
                this.transform.Translate(0, 0, 0.01f);
                anim.SetBool("IsWalking", true);
                anim.SetBool("IsAttacking", false);
            }
            else
            {
                anim.SetBool("IsAttacking", true);
                anim.SetBool("IsWalking", false);
            }
        }
        else
        {
            anim.SetBool("IsIdle", true);
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsAttacking", false);
        }
    }
}
