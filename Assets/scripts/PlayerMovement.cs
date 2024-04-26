using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D collide;
    private CrushedByGround crush;

    [SerializeField] private float coyoteTiming = 0.25f;
    [SerializeField] private float jumpForce = 14;
    [SerializeField] private float bounceForce = 6;
    [SerializeField] private float defaultAcc = 10;
    [SerializeField] private float airAcc = 5;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float terminalVel = 20;
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

    public bool dead;
    public bool freeze;

    private void Start () {
        coyoteTimer = coyoteTiming;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
        crush = GetComponent<CrushedByGround>();
    }
    
    private void Update () {

        if (freeze) {
            anim.SetBool("running", false);
            anim.SetBool("falling", false);
            anim.SetBool("rising", false);
            return;
        }

        float acc = defaultAcc;

        //Game jab messy code
        if (isSpiked() || dead)  {
            if (float.IsNaN(deathTimer)) {
                AudioManager.instance.PlaySound("die");
                deathTimer = deathAnimDuration;
            } else {
                deathTimer -= Time.deltaTime;

                if (deathTimer < 0) {
                    RestartLevel();
                }
            }

            anim.SetBool("dead", true);

            rigid.bodyType = RigidbodyType2D.Static;

            return;
        }

        if (isGrounded()) {
            coyoteTimer = coyoteTiming;

            if (Input.GetButtonDown("Submit")) {
                if (isOnWarp()) {

                    int index = -1;

                    for (int i = 0; i < regions.count; i++) {
                        int minX = regions.startX[i];
                        int minY = regions.startY[i];
                        int maxX = minX + regions.width[i];
                        int maxY = minY + regions.height[i] + 1;

                        float x = transform.position.x;
                        float y = transform.position.y;

                        if (x < maxX && x > minX && y < maxY && y > minY) {
                            index = i;
                            break;
                        }
                    }

                     mirror.setActive(index);

                    crush.enabled = mirror.isActive();
                }
            }

            if (isOnEnemy()) {
                if (Input.GetButton("Jump"))  {
                    rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
                    AudioManager.instance.PlaySound("jump");
                } else {
                    rigid.velocity = new Vector2(rigid.velocity.y, bounceForce);
                }
            }

        } else {
            if (coyoteTimer > 0) {
                coyoteTimer -= Time.deltaTime;
            }
        }

        if (coyoteTimer > 0) {
            if (Input.GetButtonDown("Jump")) {
                rigid.velocity = new Vector2(0, jumpForce);
                AudioManager.instance.PlaySound("jump");
            }

            anim.SetBool("rising", false);
            anim.SetBool("falling", false);

        } else {
            bool rising = (rigid.velocity.y > 0);

            anim.SetBool("rising", rising);
            anim.SetBool("falling", !rising);

            acc = airAcc;
        }

        float input = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(input) > 0.05f) {
            anim.SetBool("running", true);

            sprite.flipX = (rigid.velocity.x < 0);
        } else {
            anim.SetBool("running", false);

            rigid.velocity = new Vector2(rigid.velocity.x*friction, rigid.velocity.y);
        }

        float setSpeed = acc*input + rigid.velocity.x;

        if (setSpeed > moveSpeed) {
            setSpeed = moveSpeed;
        } else if (setSpeed < -moveSpeed) {
            setSpeed = -moveSpeed;
        }

        if (rigid.velocity.y <= -terminalVel) {
            rigid.velocity = new Vector2(setSpeed, -terminalVel);
        } else {
            rigid.velocity = new Vector2(setSpeed, rigid.velocity.y);
        }

        Vector2 nextPos = new Vector2(transform.position.x, transform.position.y);
        Vector3 nextCameraPos = new Vector3(cameraPos.position.x, cameraPos.position.y, cameraPos.position.z);

        bool reload = false;

        if (transform.position.x > size.width && rigid.velocity.x > 0) {
            nextPos.x -= size.width;
            nextCameraPos.x -= size.width;
            mirror.setXScreenOff -= size.width;
            reload = true;
        }

        if (transform.position.x < 0 && rigid.velocity.x < 0) {
            nextPos.x += size.width;
            nextCameraPos.x += size.width;
            mirror.setXScreenOff += size.width;
            reload = true;
        }

        if (transform.position.y > size.height && rigid.velocity.y > 0) {
            nextPos.y -= size.height;
            nextCameraPos.y -= size.height;
            mirror.setYScreenOff -= size.height;
            reload = true;
        }

        if (transform.position.y < 0 && rigid.velocity.y < 0) {
            nextPos.y += size.height;
            nextCameraPos.y += size.height;
            mirror.setYScreenOff += size.height;
            reload = true;
        }

        if (reload) {
            mirror.reload();
        }

        transform.position = nextPos;
        cameraPos.position = nextCameraPos;
    }

    private bool isGrounded () {
        //AHHHHHHHHH
        if (Physics2D.BoxCast(collide.bounds.center, collide.bounds.size, 0f, Vector2.down, .1f, jumpableGround) || isOnWarp() || isOnEnemy()) {
            return true;
        }

        return false;
    }

    private bool isOnWarp () {
        return Physics2D.BoxCast(collide.bounds.center, collide.bounds.size, 0f, Vector2.down, .1f, warpableGround);
    }

    private bool isOnEnemy () {
        return Physics2D.BoxCast(collide.bounds.center, collide.bounds.size, 0f, Vector2.down, .1f, enemyGround);
    }

    private bool isSpiked() {
        if (mirror.isActive()) {
            return false;
        }

        return Physics2D.BoxCast(collide.bounds.center, collide.bounds.size, 0f, Vector2.down, .1f, spikedGround);
    }

    private void RestartLevel () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
