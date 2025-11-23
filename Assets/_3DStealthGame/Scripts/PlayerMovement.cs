using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    AudioSource m_AudioSource;

    Animator m_Animator;
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    public TextMeshProUGUI testUI;

    public bool canMove;

    int numPresses;
    int container;

    private List<string> m_OwnedKeys = new List<string>();

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        MoveAction.Enable();
        canMove = true;
        StartCoroutine(Freeze());
        testUI.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (canMove == true)
        {
            testUI.gameObject.SetActive(false);

            var pos = MoveAction.ReadValue<Vector2>();

            float horizontal = pos.x;
            float vertical = pos.y;

            m_Movement.Set(horizontal, 0f, vertical);
            m_Movement.Normalize();
            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
            bool isWalking = hasHorizontalInput || hasVerticalInput;
            m_Animator.SetBool("IsWalking", isWalking);

            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);

            m_Rigidbody.MoveRotation(m_Rotation);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);

            if (isWalking)
            {
                if (!m_AudioSource.isPlaying)
                {
                    m_AudioSource.Play();
                }
            }
            else
            {
                m_AudioSource.Stop();
            }

        }
        else if (canMove == false)
        {
            testUI.text = "John is scared!\nMash Space to Snap Him Out of It!";
            m_Animator.SetBool("IsWalking", false);
            m_AudioSource.Stop();
        }

        spamSpace();
        

    }

    IEnumerator Freeze()
    {
        numPresses = UnityEngine.Random.Range(9, 13);
        container = 0;

        int waitTime = UnityEngine.Random.Range(15, 21);
        yield return new WaitForSeconds(waitTime);
        canMove = false;
        testUI.gameObject.SetActive(true);


    }

    public void spamSpace()
    {
        

        if (container < numPresses && Input.GetKeyDown(KeyCode.Space) && canMove == false)
        {
            container += 1;
            testUI.text = "John is scared!\nMash Space to Snap Him Out of It!";
            
        }
        else if(container >= numPresses && canMove == false)
        {
            canMove = true;
            StartCoroutine(Freeze());
        }

    }

    public void AddKey(string keyName)
    {
        m_OwnedKeys.Add(keyName);
    }

    public bool OwnKey(string keyName)
    {
        return m_OwnedKeys.Contains(keyName);
    }





}