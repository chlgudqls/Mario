using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    //속도 and 방향
    public int nextMove;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        //이곳은 1초에 60번실행함 그래서 invoke나 코루틴함수를 사용함 재귀함수라고하는듯한데
        //자기가 자기를 호출함 
        //move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            //Debug.Log("경고! 이 앞 낭떨어지..");
            Turn();
        }
    }
    void Think()
    {
        nextMove = Random.Range(-1, 2);

        anim.SetInteger("WalkSpeed", nextMove);

        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }
    public void Ondamaged()
    {
        //알파
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //플립
        spriteRenderer.flipY = true;
        //콜라이더
        capsuleCollider.enabled = false;
        //점프다이
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        //죽음
        Invoke("Deactive", 5);


    }
    void Deactive()
    {
        gameObject.SetActive(false);
    }    
    void Turn()
    {
        nextMove *= -1;
        //캔슬다음에 1이 들어가야된다는데 모르겠다 어쨋든 턴할때 x플립이 전환되지않은거니까 
        //쉽고 편하게 생각하면 그런데 
        spriteRenderer.flipX = nextMove == 1;
        //방향이바뀌면 5초를 다시세게함
        CancelInvoke();
        Invoke("Think", 5);
    }
}
