using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using Microsoft.Unity.VisualStudio.Editor;

public class Pot : MonoBehaviour
{ 
    public static event Action<int> OnGrowingDone;
    public static event Action<int> OnMushroomReaped;
    [Header("Settings")]
    public int PotIndex;    //Pot -> PotLocation으로 어느 화분인지에 대한 정보 전달

    [SerializeField] private float _waterGauge = 0f; //현재 물 게이지
    [SerializeField] private float _waterMinGauge = 0f; //최소 물 게이지
    [SerializeField] private float _waterMaxGauge = 30f; //최대 물 게이지
    [SerializeField] private float _waterGaugeSpeed = 10f; //물 게이지 올라가는 속도

    [SerializeField] private float _growth; //성장도
    [SerializeField] private float _maxGrowth = 100f;
    [SerializeField] private LayerMask _spore;

    [SerializeField] private Transform _waterMaxGaugeTransform;
    [SerializeField] private Transform _waterGaugeTransform;
    [HideInInspector] public bool IsPotted;

    private BoxCollider2D _potCollider;
    private Transform _potTransform;
    private bool IsGrown = false;
    
    [Header("Sound")]
    [SerializeField] private AudioSource _Potting;
    [SerializeField] private AudioSource _Watering;

    [Header("Particle")]
    [SerializeField] private ParticleSystem _pottingEffect;

    private float _timer = 0f; //테스트용 타이머

    private void OnEnable()
    {
        _potCollider = GetComponent<BoxCollider2D>();
        _potTransform = GetComponent<Transform>();

        if (_waterMaxGaugeTransform != null) //처음에 물 게이지 표시 안되게
        {
            _waterMaxGaugeTransform.gameObject.SetActive(false);
        }
        InitPotAudio();
    }

    private void Update()
    {
        if (IsClickPot())
        {
            if (IsPotted) //포자 심었을 때
            {
                IncreaseWaterGauge(); //클릭하면 게이지 상승
                if (_waterGauge >= _waterMaxGauge)
                {
                    CompleteWater();
                }
            }

            if (IsGrown && IsClickDownPot()) //버섯이 다 자란 화분을 클릭한 시점에 버섯 수확
            {
                ReapMushroom();
            }
        }

        else
        {
            DecreaseWaterGauge();
        }

        UpdateWaterGaugeBar(); //게이지 시각화

        //(테스트용) 0.5초마다 성장도 표시
        _timer += Time.deltaTime;
        if (_timer >= 0.5f)
        {
            if (IsPotted)
            {
                Debug.Log($"성장도 : {_growth}");
            }
            _timer = 0f;
        }
    }

    private void InitPotAudio()
    {
        _Watering.loop = true;

        _Potting.loop = false;
    }

    private bool IsClickPot() //클릭 유지중인지 확인
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit == _potCollider)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsClickDownPot() //클릭 누른 시점만 확인
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit == _potCollider)
            {
                return true;
            }
        }
        return false;
    }

    private void IncreaseWaterGauge()
    {
        if (_waterGauge < _waterMaxGauge)
        {
            _waterGauge += _waterGaugeSpeed * Time.deltaTime;
            _waterGauge = Mathf.Min(_waterGauge, _waterMaxGauge);
            if (!_Watering.isPlaying)
            {
                _Watering.Play();
            }
        }
    }

    private void CompleteWater()
    {
        _waterGauge = 0f;
        _growth += 20f;
        if (_growth >= _maxGrowth)
        {
            OnGrowingDone?.Invoke(PotIndex);
            IsPotted = false;
            IsGrown = true;
        }
    }

    private void DecreaseWaterGauge()
    {
        if (_waterGauge > _waterMinGauge)
            {
                _waterGauge -= _waterGaugeSpeed * Time.deltaTime;

                _waterGauge = Mathf.Max(_waterGauge, _waterMinGauge);
            }
        _Watering.Stop();
    }

    private void AppearWaterGauge()
    {
        _waterMaxGaugeTransform.gameObject.SetActive(true);
    }

    private void DisappearWaterGauge()
    {
        if (_waterGauge <= _waterMinGauge) //게이지 사라지기 (게이지가 다 내려갔을 때)
        {
            _waterMaxGaugeTransform.gameObject.SetActive(false);
        }
    }

    private void UpdateWaterGaugeBar()
    {
        if (IsPotted)
        {
            if (IsClickPot())
            {
                AppearWaterGauge();
            }
            else
            {
                DisappearWaterGauge();
            }

            if (_waterGaugeTransform != null) //게이지 시각화
            {
                _waterGaugeTransform.localScale = new Vector3(2 * _waterGauge / _waterMaxGauge, _waterGaugeTransform.localScale.y, _waterGaugeTransform.localScale.z);
            }
        }
        else
        {
            _waterMaxGaugeTransform.gameObject.SetActive(false);
        }
    }

    private void ReapMushroom()
    {
        OnMushroomReaped?.Invoke(PotIndex);
        IsGrown = false;
    }
}