using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 9.0f;
    [SerializeField] float jumpSpeed = 19.0f;
    [SerializeField] float climbSpeed = 7f;
    [SerializeField] Vector2 deathKick = new Vector2(5f, 8f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    
    bool playerHasHoriSpeed, playerHasVertiSpeed;
    bool isAlive;
    float gravityScaleAtStart;
    
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        isAlive = true;
    }

    void Update()
    {
        if (isAlive) {
            Run();
            ClimbLadder();
            FlipSprite();
            Die();
        }
    }

    void OnMove(InputValue value) 
    {
        // make sure player is alive
        if (isAlive) {
            moveInput = value.Get<Vector2>();
        }
    }

    void OnJump(InputValue value)
    {
        // make sure player touching ground and is alive
        if (isAlive) {
            if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && value.isPressed) {
                myRigidbody.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }

    void OnFire(InputValue value)
    {
        if (isAlive) {
            Instantiate(bullet, gun.position, transform.rotation);
        }
    }

    void Run()
    {
        // movement
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        // running animation
        playerHasHoriSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHoriSpeed) {
            myAnimator.SetBool("isRunning", true);
        }
        else {
            myAnimator.SetBool("isRunning", false);
        }
        
    }

    void ClimbLadder()
    {
        // if on ladder
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            // no sliding downwards
            myRigidbody.gravityScale = 0f;

            // movement
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
            myRigidbody.velocity = climbVelocity;

            // animation
            playerHasVertiSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("isClimbing", playerHasVertiSpeed);
        }
        else {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
        }
    }

    void FlipSprite()
    {
        // check if player is moving
        playerHasHoriSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        
        if (playerHasHoriSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Water"))) {
            isAlive = false;
            myAnimator.SetTrigger("isDying");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
