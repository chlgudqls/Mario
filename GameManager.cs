using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //점수,스테이지 관리
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int Health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;


    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void HealthDown()
    {
        if(Health > 1)
        { 
            Health--;
            UIhealth[Health].color = new Color(1, 0, 0, 0.4f);
        }
        else
            {
            UIhealth[Health].color = new Color(1, 0, 0, 0.4f);
            player.OnDie();
            Debug.Log("죽음");
            RestartBtn.SetActive(true);
        }
    }
    public void NextStage()
    {
        //렝쓰 -1 왜 한건지 확인하기
        if (stageIndex < Stages.Length -1)
        {
            Debug.Log("NextStage" + Stages.Length);
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            // 이걸 해야만 다음스테이지에서 플레이어가 다음스테이지 시작위치로 가는지 확인
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            //다음 스테이지 없음
            Time.timeScale = 0;
            Debug.Log("게임 클리어! ");
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            RestartBtn.SetActive(true);
        }
            totalPoint += stagePoint;
            stagePoint = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(Health > 1)
            {
                PlayerReposition();
            //collision.attachedRigidbody.velocity = Vector3.zero;
            //collision.transform.position = new Vector3(-11, 5, -1);
            }
                HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-11, 5, -1);
        player.VelocityZero();
    }
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0); 
    }
}
