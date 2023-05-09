using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public GameManager gameManager;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;


    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping",true);
            PlaySound("JUMP");
        }

        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f,rigid.velocity.y);
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

    }
    void PlaySound(string action)
    {
        switch (action) { 
            case "JUMP":
                audioSource.clip = audioJump ;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        //이걸 안넣어서 소리가 안남
        audioSource.Play();
    }
        void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        
        if(rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < -maxSpeed)
        {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }
        if(rigid.velocity.y < 0)
        { 
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if(rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    //Debug.Log(rayHit.collider.name);
                    animator.SetBool("isJumping", false);
                }
            }
        }
     }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                Attack(collision.transform);
                PlaySound("ATTACK");
                bool isEnemy = collision.gameObject.name.Contains("Enemy");
                if (isEnemy)
                    gameManager.stagePoint += 100;
            }
            else
            {
                Ondamaged(collision.transform.position);
                PlaySound("DAMAGED");
            }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            PlaySound("ITEM");
            if (isBronze)
            gameManager.stagePoint += 50;
            if (isSilver)
                gameManager.stagePoint += 100;
            if (isGold)
                gameManager.stagePoint += 200;

            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            Debug.Log("깃발");
            PlaySound("FINISH");
            gameManager.NextStage();

        }

    }
    void Attack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.Ondamaged();
    }
    void Ondamaged(Vector2 targetPos)
    {
        gameManager. HealthDown();

        int dirc = rigid.position.x - targetPos.x > 0 ? 1 : -1;
        gameObject.layer = 8;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        animator.SetTrigger("doDamaged");

        Invoke("offDamaged", 3f);
    }
    void offDamaged()
    {
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    public void OnDie()
    {
        //알파
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //플립
        spriteRenderer.flipY = true;
        //콜라이더
        capsuleCollider.enabled = false;
        //점프다이
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
