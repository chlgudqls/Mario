using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    //�ӵ� and ����
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
        //�̰��� 1�ʿ� 60�������� �׷��� invoke�� �ڷ�ƾ�Լ��� ����� ����Լ�����ϴµ��ѵ�
        //�ڱⰡ �ڱ⸦ ȣ���� 
        //move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            //Debug.Log("���! �� �� ��������..");
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
        //����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //�ø�
        spriteRenderer.flipY = true;
        //�ݶ��̴�
        capsuleCollider.enabled = false;
        //��������
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        //����
        Invoke("Deactive", 5);


    }
    void Deactive()
    {
        gameObject.SetActive(false);
    }    
    void Turn()
    {
        nextMove *= -1;
        //ĵ�������� 1�� ���ߵȴٴµ� �𸣰ڴ� ��¶�� ���Ҷ� x�ø��� ��ȯ���������Ŵϱ� 
        //���� ���ϰ� �����ϸ� �׷��� 
        spriteRenderer.flipX = nextMove == 1;
        //�����̹ٲ�� 5�ʸ� �ٽü�����
        CancelInvoke();
        Invoke("Think", 5);
    }
}
