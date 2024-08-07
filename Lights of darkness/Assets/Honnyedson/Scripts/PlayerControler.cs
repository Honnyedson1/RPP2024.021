using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Variaveis: ")] 
    [SerializeField] private int Healf;
    [SerializeField] private float Velocity;
    [SerializeField] private float Jumpforce;
    [SerializeField] private float dash;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool IsJump;
    [SerializeField] private float CuldownAtack;

    [Header("Componentes: ")] 
    [SerializeField] private Rigidbody2D Rig;
    [SerializeField] private Animator Anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    void Start()
    {
        Rig = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CuldownAtack <= 0)
            {

            }
            StartCoroutine("AttackCoroutine");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine("AtackSword");
        }

        if (isAttacking ==  false)
        {
            move();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            Jump();
        }

    }

    void move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float moveAmount = moveInput * Velocity;
        Rig.velocity = new Vector2(moveAmount, Rig.velocity.y);

        if (moveInput > 0 && isAttacking == false && IsJump == false) 
        {
            spriteRenderer.flipX = true;
            //Anim.SetInteger("Transition", 1);
        }
        if (moveInput < 0 && isAttacking == false && IsJump == false) 
        {
            spriteRenderer.flipX = false;
            //Anim.SetInteger("Transition", 1);
        }

        if (moveInput == 0 && isAttacking == false && IsJump == false)
        {
            //Anim.SetInteger("Transition", 0);
        }
    }

    void Jump()
    {
        if (IsJump == false)
        {
            IsJump = true;
            Rig.velocity = new Vector2(Rig.velocity.x, Jumpforce);
            //Anim.SetInteger("Transition", 3);
        }

    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        //Anim.SetInteger("Transition", 2);
        yield return new WaitForSeconds(0.50f);
        isAttacking = false;
    }

    IEnumerator AtackSword()
    {
        isAttacking = true;
        //Anim.SetInteger("Transition", 5);
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            IsJump = false;
        }
    }
}