using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        public Slider HealthBar;
        string power = "";
        bool StormFlag = false;
        Animator anim;
        // float Timer=0f;
        private float TornadoCastDistance = 1000.0f;
        public GameObject Tornado;
        //float TornadoTotoalTime = 0;
        float AttackFalseTime = 0;

        static public bool stormflag;



        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            anim = GetComponent<Animator>();

        }


        private void Update()
        {
            AttackFalseTime += Time.deltaTime;
            int A0 = Interact_Arduino.analogValues.a;//up buttom
            int A1 = Interact_Arduino.analogValues.b;//right buttom
            int A2 = Interact_Arduino.analogValues.c;//left buttom
            int A4 = Interact_Arduino.analogValues.e;//G_sensor
            int A5 = Interact_Arduino.analogValues.f;//stormskill buttom

            if (HealthBar.value <= 0)
            {

                anim.SetBool("IsDead", true);
                anim.SetBool("IsAttacking", false);

                return;
            }

            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }


            if (A4 < 300 || Input.GetButtonDown("Fire1"))
            {
                anim.SetBool("AttackFlag", true);


            }
            else
            {
                anim.SetBool("AttackFlag", false);
            }

            /******************************************************************/
            if (DetectHit.beAttack) power += "d150q";
            else if (DetectHit2.beAttack2) power += "d150q";
            else if (DetectHit3.beAttack3) power += "d150q";
            else if (DetectHit4.beAttack4) power += "d150q";
            else if (DetectHit5.beAttack5) power += "d150q";
            else if (DetectHit6.beAttack6) power += "d150q";
            else if (DetectHit7.beAttack7) power += "d150q";
            else if (DetectHit8.beAttack8) power += "d150q";
            else if (DetectHit9.beAttack9) power += "d150q";
            else if (DetectHit10.beAttack10) power += "d150q";
            else if (DetectHit11.beAttack11) power += "d150q";
            else if (DetectHit12.beAttack12) power += "d150q";
            else if (DetectHit13.beAttack13) power += "d150q";
            else if (DetectHit14.beAttack14) power += "d150q";
            else if (DetectHit15.beAttack15) power += "d150q";
            else if (DetectHit16.beAttack16) power += "d150q";
            else if (DetectHit17.beAttack17) power += "d150q";
            else if (DetectHit18.beAttack18) power += "d150q";
            else if (DetectHit19.beAttack19) power += "d150q";
            else if (DetectHit20.beAttack20) power += "d150q";
            else power += "d0q";

            /***********************************************************************/
            if (A5 > 512 || Input.GetButtonDown("Fire2"))
            {
                //Debug.Log(A5);
                StormFlag = !StormFlag;
                anim.SetBool("StormFlag", StormFlag);
                power += "b255qc255q";
                AttackFalseTime = 0;
                Vector3 Point;
                Point = m_Character.transform.position + m_Character.transform.forward * TornadoCastDistance;

                GameObject go = Instantiate(Tornado, m_Character.transform.position, m_Character.transform.rotation) as GameObject;
                go.transform.LookAt(Point);
                go.GetComponent<Rigidbody>().AddForce(go.transform.forward * 100);//cast speed


                Destroy(go, 10);
            }
            if (AttackFalseTime > 0.5f)
            {

                anim.SetBool("StormFlag", false);
                power += "b0qc0q";
            }

            stormflag = anim.GetBool("StormFlag");


            Interact_Arduino.Instance.powerBuffer.Add(power);
            power = "";


        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            int A0 = Interact_Arduino.analogValues.a;//up buttom
            int A1 = Interact_Arduino.analogValues.b;//right buttom
            int A2 = Interact_Arduino.analogValues.c;//left buttom
            // read inputs

            float h = 0;
            if (A1 > 512)
            {
                h = A1;
            }
            else if (A2 > 512)
            {
                h -= A2;
            }
            //float v = CrossPlatformInputManager.GetAxis("Vertical");
            float v = A0;
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
