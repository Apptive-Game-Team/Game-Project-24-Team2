using System.Collections;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

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
    private void Start()
    {
        bookCollider = GetComponent<BoxCollider2D>();

        if (bookMaxGaugeTransform != null)
        {
            bookMaxGaugeTransform.gameObject.SetActive(false);
        }

        InitBookAudio();
    }

    private void Update()
    {
        if (IsClickBook())
        {
            IncreaseBookGauge();

            if (bookGauge >= bookMaxGauge)
            {
                CompleteBook();
            }

            //소리 재생(처음 클릭 시점만)
            BookOpenSound();
        }
        else
        {
            DecreaseBookGauge();
        }

        //게이지 시각화
        UpdateBookGaugeBar();

        //(테스트용 추후 삭제 예정) 0.5초마다 의심도 / 돈 표시
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            Debug.Log($"의심도 : {doubtGauge}    돈 : {money}");
            timer = 0f;
        }
    }

    private void InitBookAudio()
    {
        Pencil.loop = true;

        BookOpen.loop = false;
    }

    private bool IsClickBook()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            
            if (hit == bookCollider)
            {
                return true;
            }
        }

        return false;
    }

    private void IncreaseBookGauge()
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

    //Book 게이지 다 찼을 때
    private void CompleteBook()
    {
        bookGauge = 0f; //게이지 초기화
        doubtGauge -= doubtGaugeReduction;
        if (doubtGauge < doubtMinGauge)
        {
            money += (doubtMinGauge - doubtGauge)/doubtGaugeReduction * moneyGet; //보상 지급
            doubtGauge = doubtMinGauge;
        }

    }

    private void DecreaseBookGauge()
    {
        if (bookGauge > bookMinGauge)
            {
                bookGauge -= bookGaugeSpeed * Time.deltaTime;

                bookGauge = Mathf.Max(bookGauge, bookMinGauge);
            }

        Pencil.Stop();
    }

    private void UpdateBookGaugeBar()
    {
        if (IsClickBook())
        {
            AppearBookGauge();
        }
        else
        {
            DisappearBookGauge();
        }

        if (bookGaugeTransform != null) //게이지 시각화
        {
            bookGaugeTransform.localScale = new Vector3(2 * bookGauge / bookMaxGauge, bookGaugeTransform.localScale.y, bookGaugeTransform.localScale.z);
        }
    }

    private void AppearBookGauge()
    {
        bookMaxGaugeTransform.gameObject.SetActive(true);
    }

    private void DisappearBookGauge()
    {
        if (bookGauge <= bookMinGauge) //게이지가 다 내려갔을 때만 안보이게
            {
                bookMaxGaugeTransform.gameObject.SetActive(false);
            }
    }

    private void BookOpenSound() //클릭한 시점에만 소리 재생
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            BookOpen.Play();
        }
    }
}
