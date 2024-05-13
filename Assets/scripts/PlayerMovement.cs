using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

    static float accScale = 1f/10f;

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer sprite;
    private CapsuleCollider2D collide;
    private CrushedByGround crush;
    private BoxCollider2D groundedBox;

    [SerializeField] private float coyoteTiming = 0.25f;
    [SerializeField] private float jumpForce = 14;
    [SerializeField] private float bounceForce = 6;
    [SerializeField] private float defaultAcc = 50f;
    [SerializeField] private float airAcc = 10f;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float terminalVel = 20;
    [SerializeField] private float jumpSpeedMult = 1.1f;
    [SerializeField] private float shortJumpScalar = 0.99f;
    [SerializeField] private float deathAnimDuration = 1.5f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask warpableGround;
    [SerializeField] private LayerMask enemyGround;
    [SerializeField] private LayerMask spikedGround;
    [SerializeField] private SizeDefinition size;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private MirrorDim mirror;
    [SerializeField] private MirrorRegions regions;
    [SerializeField] private float friction = 0.8f;

    private float coyoteTimer = 0;
    private float deathTimer = float.NaN;

    private bool dead;
    private bool freeze;
    private bool toggleMirror = false;

    private void Start () {
        coyoteTimer = coyoteTiming;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        collide = GetComponent<CapsuleCollider2D>();
        crush = GetComponent<CrushedByGround>();
        groundedBox = GetComponent<BoxCollider2D>();
    }
    
    public void win ()
    {

        anim.SetTrigger("win");
        freeze = true;
    }

    private void Update()
    {
        
        //Handled the rising / falling edge mirror inputs on a faster thread
        if (Input.GetButtonDown("Submit"))
        {
            toggleMirror = true;
        }
        
        if (Input.GetButtonUp("Submit"))
        {
            toggleMirror = false;
        }

        
    }

    private void FixedUpdate () {

        //Don't update when frozen
        if (Time.timeScale == 0) return;

        //Used for when the has won
        if (freeze) {
            //Cancel animatation
            anim.SetBool("running", false);
            anim.SetBool("falling", false);
            anim.SetBool("rising", false);

            //Prevent the endless fall issue (not really nessary with rigid.Sleep();)
            wrapAround();

            //Lock the player position
            rigid.Sleep();
            rigid.velocity = new Vector2(0, 0);

            //Exit early
            return;
        }

        //Check if the player should be killed this round
        if (isSpiked())
        {
            kill();
        }

        //Progress the death timer
        if (dead)  {
            
            deathTimer -= Time.deltaTime;

            if (deathTimer < 0) {
                RestartLevel();
            }

            //Exit early
            return;
        }

        //Check if the player's grounded check intersects with the ground
        bool grounded = isGrounded();
        
        if (grounded) {

            //Reset coyoteTiming
            coyoteTimer = coyoteTiming;

            //If the toggleMirror button was pressed, activate or deactive the mirrors
            if (toggleMirror) {

                //Reset the input
                toggleMirror = false;

                //Check if the player can
                if (isOnWarp()) {

                    //Prevent the player from jumping at the same time
                    coyoteTimer = 0;

                    //Initialize the index
                    int index = -1;

                    //Check all regions to find which one has the player fully in it
                    //This does priortize the dimensions in order of definition, but they should
                    //functionally never overlap
                    for (int i = 0; i < regions.count; i++) {

                        //Calculate min / max
                        int minX = regions.startX[i];
                        int minY = regions.startY[i];
                        int maxX = minX + regions.width[i];
                        int maxY = minY + regions.height[i] + 1;//Add 1 to acount for standing on top of it

                        //Get player pos relative to the level size
                        float x = transform.position.x % size.width;
                        float y = transform.position.y % size.height;

                        //Check and exit on success
                        if (x < maxX && x > minX && y < maxY && y > minY) {
                            index = i;
                            break;
                        }
                    }

                    //This handles setting up the mirror
                    mirror.setActive(index);
                }
            }

            //This aplies movement physics for bouncing on the blocker enemies
            if (isOnEnemy()) {

                if (Input.GetButton("Jump"))  {
                    //This allows the player to bounce with the height of a jump
                    rigid.velocity = new Vector2(rigid.velocity.x * jumpSpeedMult, jumpForce);
                    AudioManager.instance.PlaySound("jump");
                } else {
                    //This keeps the player bouncing
                    rigid.velocity = new Vector2(rigid.velocity.x, bounceForce);
                }
            }

        } else {

            //If the player is not grounded, decrease the coyote timer until 0
            if (coyoteTimer > 0)
            {
                coyoteTimer -= Time.deltaTime;
            }
        }

        //Track if the player jumped to modify horizontal speed later
        bool jumped = false;

        //The player can only jump if the coyoteTimer is greater than 0
        //When ground it always should be, and it stays that way for a
        //short period after walking off a ledge
        if (coyoteTimer > 0) {

            //Check if the jump input is held
            if (Input.GetButton("Jump")) {

                //Track for later
                jumped = true;

                //Apply the jump force
                rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
                AudioManager.instance.PlaySound("jump");

                //Prevent a coyote timer double jump
                coyoteTimer = 0;
            }

            //Set the players anim state to grounded
            anim.SetBool("rising", false);
            anim.SetBool("falling", false);

        } else {

            //If the player is airborne, set the correct animation
            bool rising = (rigid.velocity.y > 0);

            anim.SetBool("rising", rising);
            anim.SetBool("falling", !rising);
        }


        //Set the default grounded acceleration
        float acc = defaultAcc * accScale;

        //Overwrite with airborne acceleration if not grounded
        if (!grounded)
        {
            acc = airAcc*accScale;
        }

        //Get the horizontal input value
        float input = Input.GetAxisRaw("Horizontal");

        //Set the animation run state based on input, not actual speed
        if (Mathf.Abs(input) > 0.05f) {

            //If outside of the dead zone, actually start running
            anim.SetBool("running", true);
        } else {
            //If near 0, stop running animation
            anim.SetBool("running", false);

            //and slow the player with friction
            rigid.velocity = new Vector2(rigid.velocity.x*friction, rigid.velocity.y);
        }
        
        //Determin how fast the player could go next frame
        float setSpeed = acc*input + rigid.velocity.x;

        //Cap the movement speed for next frame
        if (setSpeed > moveSpeed) {
            setSpeed = moveSpeed;
        } else if (setSpeed < -moveSpeed) {
            setSpeed = -moveSpeed;
        }

        //Get the falling speed of the player
        float ySpeed = rigid.velocity.y;

        //If the player is going up, not grounded and stopped holding jump
        //slow the vertical speed to do a short jump
        if (rigid.velocity.y > 0 && !grounded && !Input.GetButton("Jump"))
        {
            ySpeed *= shortJumpScalar;

        } else if (rigid.velocity.y <= -terminalVel) {
            //Cap the negative speed of the player
            ySpeed = -terminalVel;
        }

        //Modify horizontal speed if the player just started a jump
        if (jumped)
        {
            setSpeed = setSpeed * jumpSpeedMult;
        }

        //Apply the new speed values to the rigid body
        rigid.velocity = new Vector2(setSpeed, ySpeed);

        //If the speed is fast enough, update the animation state
        if (Mathf.Abs(rigid.velocity.x) > 0.1f)
        {

            //Flip the sprite if going left
            sprite.flipX = (rigid.velocity.x < 0);
        }


        wrapAround();
    }

    private bool isGrounded () {
        //Check if the box intersects with default ground or other types
        if (Physics2D.BoxCast(collide.bounds.center, groundedBox.bounds.size, 0f, Vector2.down, .1f, jumpableGround) || isOnWarp() || isOnEnemy()) {
            return true;
        }

        return false;
    }

    private bool isOnWarp () {
        //Check if the box intersects with warpable ground
        return Physics2D.BoxCast(collide.bounds.center, groundedBox.bounds.size, 0f, Vector2.down, .1f, warpableGround);
    }

    private bool isOnEnemy () {
        //Check if the box intersects with enemy ground
        return Physics2D.BoxCast(collide.bounds.center, groundedBox.bounds.size, 0f, Vector2.down, .1f, enemyGround);
    }

    private bool isSpiked() {
        //The player cannot get killed by spikes while a mirror portal is active
        if (mirror.isActive()) {
            return false;
        }

        //Check if the box intersects with spike boxes
        return Physics2D.BoxCast(collide.bounds.center, groundedBox.bounds.size, 0f, Vector2.down, .1f, spikedGround);
    }

    private void RestartLevel () {
        //Shortcut to restart the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void wrapAround ()
    {
        Vector2 nextPos = new Vector2(transform.position.x, transform.position.y);
        Vector3 nextCameraPos = new Vector3(cameraPos.position.x, cameraPos.position.y, cameraPos.position.z);
        Vector2 bounds = collide.bounds.size;

        bool reload = false;

        if (transform.position.x > size.width + bounds.x && rigid.velocity.x > 0)
        {
            nextPos.x -= size.width;
            nextCameraPos.x -= size.width;
            mirror.setXScreenOff -= size.width;
            reload = true;

            if (EnemyController.CurrentController != null)
            {
                EnemyController.CurrentController.wrapAroundX(true);
            }
        }

        if (transform.position.x < -bounds.x && rigid.velocity.x < 0)
        {
            nextPos.x += size.width;
            nextCameraPos.x += size.width;
            mirror.setXScreenOff += size.width;
            reload = true;

            if (EnemyController.CurrentController != null)
            {
                EnemyController.CurrentController.wrapAroundX(false);
            }
        }

        if (transform.position.y > size.height + bounds.y && rigid.velocity.y > 0)
        {
            nextPos.y -= size.height;
            nextCameraPos.y -= size.height;
            mirror.setYScreenOff -= size.height;
            reload = true;

            if (EnemyController.CurrentController != null)
            {
                EnemyController.CurrentController.wrapAroundY(true);
            }
        }

        if (transform.position.y < -bounds.y && rigid.velocity.y < 0)
        {
            nextPos.y += size.height;
            nextCameraPos.y += size.height;
            mirror.setYScreenOff += size.height;
            reload = true;

            if (EnemyController.CurrentController != null)
            {
                EnemyController.CurrentController.wrapAroundY(false);
            }
        }

        if (reload)
        {
            mirror.reload();
        }

        transform.position = nextPos;
        cameraPos.position = nextCameraPos;
    }


    public void kill ()
    {
        if (freeze) return;
        if (dead) return;

        dead = true;

        AudioManager.instance.PlaySound("die");
        deathTimer = deathAnimDuration;
        LevelTransition.instance.EndLevel();
        anim.SetBool("dead", true);

        rigid.bodyType = RigidbodyType2D.Static;
        collide.enabled = false;
    }
}
