using System.Collections;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Book : MonoBehaviour
{
    [Header("Settings")]
    //책 게이지 올라가는 속도
    public float bookGaugeSpeed = 10f;
    //현재 책 게이지
    public float bookGauge = 0f;
    //최대 책 게이지
    public float bookMaxGauge = 30f;
    //최소 책 게이지
    public float bookMinGauge = 0f;
    //감소 의심도
    public float doubtReduction = 10f;
    //현재 의심 게이지
    public float doubtGauge = 25f;
    //최대 의심 게이지
    public float doubtMaxGauge = 100f;
    //최소 의심 게이지
    public float doubtMinGauge = 0f;
    //의심 게이지 감소 속도
    public float doubtGaugeReduction = 10f;
    //돈
    public float money = 0f;
    //본업 성공 시 얻는 돈
    public float moneyGet = 50f;
    
    private BoxCollider2D bookCollider;
    [Header("PutObject")]
    //BookMaxGauge 넣어주기
    public Transform bookMaxGaugeTransform;
    //BookGauge 넣어주기
    public Transform bookGaugeTransform;

    //사운드
    [Header("Sound")]
    public AudioSource Pencil;
    public AudioSource BookOpen;


    //디버깅 최적화용(나중에 지워야함)
    private float timer = 0f;
    void Start()
    {
        bookCollider = GetComponent<BoxCollider2D>();

        //처음에 책 게이지 표시 안되게
        if (bookMaxGaugeTransform != null)
        {
            bookMaxGaugeTransform.gameObject.SetActive(false);
        }

        Pencil.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Book 클릭 시
        if (IsClickBook())
        {
            //게이지 증가
            IncreaseBookGauge();

            //게이지 나타나기
            bookMaxGaugeTransform.gameObject.SetActive(true);

            //Book 게이지 다 차면 초기화 + 의심도 감소
            if (bookGauge >= bookMaxGauge)
            {
                CompleteBook();
            }

            //소리 재생(처음 시점만)
            if (Input.GetMouseButtonDown(0))
            {
                BookOpen.Play();
            }

        }
        //Book 클릭 아닐 시
        else
        {
            //게이지 감소
            DecreaseBookGauge();

            //게이지 사라지기 (게이지가 다 내려갔을 때)
            if (bookGauge <= bookMinGauge)
            {
                bookMaxGaugeTransform.gameObject.SetActive(false);
            }
        }

        //게이지 시각화
        UpdateBookGaugeBar();

        //(테스트용) 0.5초마다 의심도 / 돈 표시
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            Debug.Log($"의심도 : {doubtGauge}    돈 : {money}");
            timer = 0f;
        }
    }

    //Book에 마우스 클릭 감지
    bool IsClickBook()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            return bookCollider == Physics2D.OverlapPoint(mousePos); 
        }

        return false;
    }

    //Book 게이지 올리기
    void IncreaseBookGauge()
    {
        if (bookGauge < bookMaxGauge)
        {
            bookGauge += bookGaugeSpeed * Time.deltaTime;

            bookGauge = Mathf.Min(bookGauge, bookMaxGauge);
        }

        if (!Pencil.isPlaying)
        {
            Pencil.Play();
        }
            
    }

    //Book 게이지 다 찼을 때, 초기화 + 의심도 내리기 or 보상 주기
    void CompleteBook()
    {
        bookGauge = 0f;
        //의심 게이지 = 0일때, 보상 지급
        if (doubtGauge <= doubtMinGauge)
        {
            money += moneyGet;
        }
        //의심 게이지 != 0일때, 의심 게이지 감소 + 의심 게이지 음수일 때 처리
        else
        {
            doubtGauge -= doubtGaugeReduction;
            if (doubtGauge < doubtMinGauge)
            {
                money += (doubtMinGauge - doubtGauge)/doubtGaugeReduction * moneyGet;
                doubtGauge = doubtMinGauge;
            }
        }
    }

    //Book 게이지 내리기
    void DecreaseBookGauge()
    {
        if (bookGauge > bookMinGauge)
            {
                bookGauge -= bookGaugeSpeed * Time.deltaTime;

                bookGauge = Mathf.Max(bookGauge, bookMinGauge);
            }
        //소리 끄기
        Pencil.Stop();
    }

    //게이지 움직임
    void UpdateBookGaugeBar()
    {
        if (bookGaugeTransform != null)
        {
            bookGaugeTransform.localScale = new Vector3(2 * bookGauge / bookMaxGauge, bookGaugeTransform.localScale.y, bookGaugeTransform.localScale.z);
        }
    }
}
