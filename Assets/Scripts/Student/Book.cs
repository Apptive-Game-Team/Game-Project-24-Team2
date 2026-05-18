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
    public float _bookGaugeSpeed = 10f;
    //현재 책 게이지
    [SerializeField] private float _bookGauge = 0f;
    //최대 책 게이지
    [SerializeField] private float _bookMaxGauge = 30f;
    //최소 책 게이지
    [SerializeField] private float _bookMinGauge = 0f;
    //현재 의심 게이지
    [SerializeField] private float _doubtGauge = 25f;
    //최대 의심 게이지 (아직 안쓰임)
    //[SerializeField] private float _doubtMaxGauge = 100f;
    //최소 의심 게이지
    [SerializeField] private float _doubtMinGauge = 0f;
    //의심 게이지 감소 속도
    [SerializeField] private float _doubtGaugeReduction = 10f;
    //돈
    [SerializeField] private float _money = 0f;
    //본업 성공 시 얻는 돈
    [SerializeField] private float _moneyGet = 50f;
    
    private BoxCollider2D _bookCollider;

    [Header("PutObject")]
    //_bookMaxGauge 넣어주기
    [SerializeField] private Transform _bookMaxGaugeTransform;
    //_bookGauge 넣어주기
    [SerializeField] private Transform _bookGaugeTransform;

    //사운드
    [Header("Sound")]
    [SerializeField] private AudioSource _Pencil;
    [SerializeField] private AudioSource _BookOpen;


    //디버깅 최적화용(나중에 지워야함)
    private float _timer = 0f;
    private void Start()
    {
        _bookCollider = GetComponent<BoxCollider2D>();

        if (_bookMaxGaugeTransform != null)
        {
            _bookMaxGaugeTransform.gameObject.SetActive(false);
        }

        InitBookAudio();
    }

    private void Update()
    {
        if (IsClickBook())
        {
            IncreaseBookGauge();

            if (_bookGauge >= _bookMaxGauge)
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
        _timer += Time.deltaTime;
        if (_timer >= 0.5f)
        {
            Debug.Log($"의심도 : {_doubtGauge}    돈 : {_money}");
            _timer = 0f;
        }
    }

    private void InitBookAudio()
    {
        _Pencil.loop = true;

        _BookOpen.loop = false;
    }

    bool IsClickBook()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit == _bookCollider)
            {
                return true;
            }
        }

        return false;
    }

    private void IncreaseBookGauge()
    {
        if (_bookGauge < _bookMaxGauge)
        {
            _bookGauge += _bookGaugeSpeed * Time.deltaTime;

            _bookGauge = Mathf.Min(_bookGauge, _bookMaxGauge);
        }

        if (!_Pencil.isPlaying)
        {
            _Pencil.Play();
        }  
    }

    private void CompleteBook()
    {
        _bookGauge = 0f; //게이지 초기화
        _doubtGauge -= _doubtGaugeReduction;
        if (_doubtGauge < _doubtMinGauge)
        {
            _money += (_doubtMinGauge - _doubtGauge)/_doubtGaugeReduction * _moneyGet; //보상 지급
            _doubtGauge = _doubtMinGauge;
        }
    }

    private void DecreaseBookGauge()
    {
        if (_bookGauge > _bookMinGauge)
            {
                _bookGauge -= _bookGaugeSpeed * Time.deltaTime;

                _bookGauge = Mathf.Max(_bookGauge, _bookMinGauge);
            }
        _Pencil.Stop();
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

        if (_bookGaugeTransform != null) //게이지 시각화
        {
            _bookGaugeTransform.localScale = new Vector3(2 * _bookGauge / _bookMaxGauge, _bookGaugeTransform.localScale.y, _bookGaugeTransform.localScale.z);
        }
    }

    private void AppearBookGauge()
    {
        _bookMaxGaugeTransform.gameObject.SetActive(true);
    }

    private void DisappearBookGauge()
    {
        if (_bookGauge <= _bookMinGauge) //게이지가 다 내려갔을 때만 안보이게
            {
                _bookMaxGaugeTransform.gameObject.SetActive(false);
            }
    }

    private void BookOpenSound() //클릭한 시점에만 소리 재생
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _BookOpen.Play();
        }
    }
}
