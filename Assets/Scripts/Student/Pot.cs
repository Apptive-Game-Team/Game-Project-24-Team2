using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.SceneManagement;

public class Pot : MonoBehaviour
{ 
    public static event Action<Item> OnMushroomReaped;
    public static event Action OnPotPlaced;
    public static event Action OnSporePlaced;

    [Header("Settings")]
    //포자 관련
    [SerializeField] private GameObject[] _spores;
    [SerializeField] private LayerMask _spore;

    //버섯 관련
    [SerializeField] private GameObject[] _mushrooms;
    [SerializeField] private Item[] _mushroomItems;

    //물 게이지 관련
    [SerializeField] private float _waterGauge = 0f; //현재 물 게이지
    [SerializeField] private float _waterMinGauge = 0f; //최소 물 게이지
    [SerializeField] private float _waterMaxGauge = 30f; //최대 물 게이지
    [SerializeField] private float _waterGaugeSpeed = 10f; //물 게이지 올라가는 속도
    [SerializeField] private Transform _waterMaxGaugeTransform;
    [SerializeField] private Transform _waterGaugeTransform;

    //성장도 관련
    [SerializeField] private float _growth; //성장도
    [SerializeField] private float _maxGrowth = 100f;

    //bool
    [HideInInspector] public bool IsSporePlaced = false;
    [HideInInspector] public bool IsPotPlaced = false;
    
    [Header("Sound")]
    [SerializeField] private AudioSource _Potting;
    [SerializeField] private AudioSource _Watering;

    [Header("Particle")]
    [SerializeField] private ParticleSystem _pottingEffect;

    private SpriteRenderer _sprite;
    private BoxCollider2D _potCollider;
    private bool _isGrown = false;
    private int _whatspore;
    private float _timer = 0f; //테스트용 타이머

    // 화분의 상태를 저장할 클래스와 정적(Static) 배열
    public class PotState
    {
        public bool IsPotPlaced;
        public bool IsSporePlaced;
        public int WhatSpore;
        public float WaterGauge;
        public float Growth;
        public bool IsGrown;
    }
    private static PotState[] _savedStates;

    private void Awake()
    {
        if (_savedStates == null)
        {
            _savedStates = new PotState[4];
            for (int i = 0; i < 4; i++) _savedStates[i] = new PotState();
        }

        for (int i = 0; i < _spores.Length; i++)
        {
            _spores[i].SetActive(false);
            _mushrooms[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _potCollider = GetComponent<BoxCollider2D>();

        if (_waterMaxGaugeTransform != null) //처음에 물 게이지 표시 안되게
        {
            _waterMaxGaugeTransform.gameObject.SetActive(false);
        }
        InitPotAudio();

        InventorySlot.OnDragStarted += HandleDragStarted;
        InventorySlot.OnDragCanceled += HandleDragCanceled;
        InventorySlot.OnDragEndedWorld += HandleDragEndedWorld;
    }

    private void Start()
    {
        // 씬이 열릴 때 이전 상태 복구
        int index = transform.GetSiblingIndex();
        if (index >= 0 && index < 4)
        {
            PotState state = _savedStates[index];
            IsPotPlaced = state.IsPotPlaced;
            IsSporePlaced = state.IsSporePlaced;
            _whatspore = state.WhatSpore;
            _waterGauge = state.WaterGauge;
            _growth = state.Growth;
            _isGrown = state.IsGrown;

            if (IsPotPlaced) ShowPot();
            else HideGuide(); // 설치되지 않은 화분은 완벽히 숨김

            if (IsSporePlaced && !_isGrown)
            {
                _spores[_whatspore].SetActive(true);
            }
            if (_isGrown)
            {
                Evolution(_whatspore);
            }
        }
    }

    private void OnDisable()
    {
        InventorySlot.OnDragStarted -= HandleDragStarted;
        InventorySlot.OnDragCanceled -= HandleDragCanceled;
        InventorySlot.OnDragEndedWorld -= HandleDragEndedWorld;
    }

    private void OnDestroy()
    {
        // 씬이 파괴될 때 현재 화분의 상태를 저장
        int index = transform.GetSiblingIndex();
        if (_savedStates != null && index >= 0 && index < 4)
        {
            PotState state = _savedStates[index];
            state.IsPotPlaced = IsPotPlaced;
            state.IsSporePlaced = IsSporePlaced;
            state.WhatSpore = _whatspore;
            state.WaterGauge = _waterGauge;
            state.Growth = _growth;
            state.IsGrown = _isGrown;
        }
    }

    private void Update()
    {
        if (IsClickPot())
        {
            if (IsSporePlaced) //포자 심었을 때
            {
                IncreaseWaterGauge(); //클릭하면 게이지 상승
                if (_waterGauge >= _waterMaxGauge)
                {
                    CompleteWater();
                }
            }
            if (_isGrown && IsClickDownPot()) //버섯이 다 자란 화분을 클릭한 시점에 버섯 수확
            {
                ReapMushroom();
            }
        }

        else
        {
            DecreaseWaterGauge();
        }

        UpdateWaterGaugeBar(); //게이지 시각화
    }

    public void ShowGuide() //드래그 중 화분 반투명하게 표시
    {
        _sprite.enabled = true;
        SetAlpha(0.5f);
    }

    public void HideGuide()
    {
        _sprite.enabled = false;
    }

    public void ShowPot() //정확한 위치에 화분 배치 시 화분을 완전 불투명하게 표시
    {
        _sprite.enabled = true;
        SetAlpha(1f);
    }

    public void ShowSpore(Item item)
    {
        switch (item)
        {
            case PencilSporeItem:
                _spores[0].SetActive(true);
                _whatspore = 0;
                break;
            case TextbookSporeItem:
                _spores[1].SetActive(true);
                _whatspore = 1;
                break;
            case BlackboardSporeItem:
                _spores[2].SetActive(true);
                _whatspore = 2;
                break;
            case MealSporeItem:
                _spores[3].SetActive(true);
                _whatspore = 3;
                break;
            default:
                break;
        }
    }

    private void Evolution(int whatspore) //포자 -> 버섯 형태변환
    {
        _spores[whatspore].SetActive(false);
        _mushrooms[whatspore].SetActive(true);
    }

    private void SetAlpha(float alpha) //투명도 조절
    {
        Color color = _sprite.color;
        color.a = alpha;
        _sprite.color = color;
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
            Evolution(_whatspore);
            IsSporePlaced = false;
            _isGrown = true;
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
        if (IsSporePlaced)
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
        Item mushroomItem = GetMushroomItemBySpore(_whatspore);
        
        if (mushroomItem != null)
        {
            OnMushroomReaped?.Invoke(mushroomItem);
        }
        else
        {
            Debug.LogError($"버섯 아이템을 로드할 수 없습니다 (_whatspore={_whatspore})");
        }
        
        _isGrown = false;

    }

    private Item GetMushroomItemBySpore(int sporeType)
    {
        return sporeType switch
        {
            0 => Resources.Load<Item>("Items/MushroomItems/PencilMushroomItem"),
            1 => Resources.Load<Item>("Items/MushroomItems/TextbookMushroomItem"),
            2 => Resources.Load<Item>("Items/MushroomItems/BlackboardMushroomItem"),
            3 => Resources.Load<Item>("Items/MushroomItems/MealMushroomItem"),
            _ => null
        };
    }

    private void HandleDragStarted(Item item)
    {
        if (!IsPotPlaced)
        {
            ShowGuide();
        }
    }

    private void HandleDragCanceled()
    {
        if (!IsPotPlaced)
        {
            HideGuide();
        }
    }

    private void HandleDragEndedWorld(Item item, Vector2 mousePos, Action<bool> onResult)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit == _potCollider)
        {
            if (item is PotItem)
            {
                if (!IsPotPlaced)
                {
                    ShowPot();
                    IsPotPlaced = true;
                    OnPotPlaced?.Invoke();
                    onResult?.Invoke(true);
                }
                else onResult?.Invoke(false);
            }
            else // 포자인 경우
            {
                if (IsPotPlaced && !IsSporePlaced)
                {
                    ShowSpore(item);
                    IsSporePlaced = true;
                    OnSporePlaced?.Invoke();
                    onResult?.Invoke(true);
                }
                else onResult?.Invoke(false);
            }
        }
    }
}