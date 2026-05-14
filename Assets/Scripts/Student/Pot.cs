using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pot : MonoBehaviour
{ 
    [Header("Settings")]
    [SerializeField] private List<GameObject> _spores = new List<GameObject>(); //가지고 있는 버섯 리스트 (오브젝트 형태로 저장)
    [SerializeField] private int _whatSpore; //심을 버섯 인덱스 저장
    [SerializeField] private float _sporeGap = 1.5f; //인벤에 있는 버섯 표기 간격 (추후 인벤에 맞게 수정)
    [SerializeField] private float _startingGap = 1.7f; //인벤 첫 번째 버섯과 화분 간격
    [SerializeField] private float _pottedsize = 1.6f; //버섯 심은 후 화분 히트박스(y)

    [SerializeField] private float _waterGauge = 0f; //현재 물 게이지
    [SerializeField] private float _waterMinGauge = 0f; //최소 물 게이지
    [SerializeField] private float _waterMaxGauge = 30f; //최대 물 게이지
    [SerializeField] private float _waterGaugeSpeed = 10f; //물 게이지 올라가는 속도

    [SerializeField] private float _growth; //성장도
    [SerializeField] private LayerMask _spore;

    [SerializeField] private Transform _waterMaxGaugeTransform;
    [SerializeField] private Transform _waterGaugeTransform;

    private GameObject _pottedSpore; //화분에 심겨진 버섯
    
    private BoxCollider2D _potCollider;
    private Transform _potTransform;
    
    [Header("Sound")]
    [SerializeField] private AudioSource _Potting;
    [SerializeField] private AudioSource _Watering;

    [Header("Particle")]
    [SerializeField] private ParticleSystem _pottingEffect;

    private float _timer = 0f; //테스트용 타이머

    private void Start()
    {
        _potCollider = GetComponent<BoxCollider2D>();
        _potTransform = GetComponent<Transform>();

        DisappearSpores();

        //처음에 물 게이지 표시 안되게
        if (_waterMaxGaugeTransform != null)
        {
            _waterMaxGaugeTransform.gameObject.SetActive(false);
        }

        InitPotAudio();
    }

    private void Update()
    {
        if (IsClickPot())
        {
            AppearSpores();

            if (_pottedSpore != null) //포자 심었을 때
            {
                IncreaseWaterGauge();

                if (_waterGauge >= _waterMaxGauge)
                {
                    CompleteWater();
                }

                UpdateWaterGaugeBar();
            }
        }

        else
        {
            DecreaseWaterGauge();
        }

        PotSpore();

        //(테스트용) 0.5초마다 성장도 표시
        _timer += Time.deltaTime;
        if (_timer >= 0.5f)
        {
            if (_pottedSpore != null)
            {
                Debug.Log($"성장도 : {_growth}");
            }
            Debug.Log(_whatSpore);
            _timer = 0f;
        }
    }

    private void InitPotAudio()
    {
        _Watering.loop = true;

        _Potting.loop = false;
    }

    bool IsClickPot()
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

    void PotSpore()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            LocateSpore();

            //선택한 버섯 심기(위치는 버섯 이미지 받은 후 재지정), 인벤에서 삭제
            switch (_whatSpore)
            {
                case 0:
                    {
                    SporeListManage(0);
                    }
                    break;
                
                case 1:
                    {
                    SporeListManage(1);
                    }
                    break;

                case 2:
                    {
                    SporeListManage(2);
                    }
                    break;

                case 3:
                    {
                    SporeListManage(3);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void LocateSpore() //포자 클릭 감지
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D ClickedSpore = Physics2D.OverlapPoint(mousePos);
        if (ClickedSpore != null)
        {
            if (ClickedSpore.gameObject.layer == LayerMask.NameToLayer("Spore"))
            {
                _whatSpore = _spores.IndexOf(ClickedSpore.gameObject);
                _Potting.Play();

                _potCollider.offset = new Vector2(_potCollider.offset.x, (_pottedsize-1)/2); //포자 위치 잡기
                _potCollider.size = new Vector2(_potCollider.size.x, _pottedsize);
            }
            else
            {
                _whatSpore = -1;
            }
        }
        else
        {
            _whatSpore = -1;
        }
    }

    private void SporeListManage(int idx)
    {
        if (_pottedSpore != null)
        {
            _spores.Insert(idx, _pottedSpore);
            _spores[idx].transform.position = new Vector3(_startingGap + _potTransform.position.x + idx*_sporeGap, _potTransform.position.y, _potTransform.position.z);
            _pottedSpore = _spores[idx + 1];
            _spores[idx + 1].transform.position = new Vector3(_potTransform.position.x, _potTransform.position.y + 1f, _potTransform.position.z);
            _pottingEffect.transform.position = _spores[idx + 1].transform.position;
            _pottingEffect.Play();
            _spores.RemoveAt(idx + 1);
        }
        else
        {
            _pottedSpore = _spores[idx];
            _spores[idx].transform.position = new Vector3(_potTransform.position.x, _potTransform.position.y + 1f, _potTransform.position.z);
            _pottingEffect.transform.position = _spores[idx].transform.position;
            _pottingEffect.Play();
            _spores.RemoveAt(idx);
        }
    }

    void IncreaseWaterGauge()
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
        _growth += 10f;
    }

    //Book 게이지 내리기
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
        if (_pottedSpore != null)
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
    }

    private void AppearSpores()
    {
        for (int i = 0; i < _spores.Count; i++)
        {
            _spores[i].transform.position = new Vector3(_startingGap + _potTransform.position.x + i*_sporeGap, _potTransform.position.y, _potTransform.position.z);
            _spores[i].transform.gameObject.SetActive(true);
        }

    }

    private void DisappearSpores()
    {
        for (int i = 0; i < _spores.Count; i++)
        {
            _spores[i].transform.position = new Vector3(_startingGap + _potTransform.position.x + i*_sporeGap, _potTransform.position.y, _potTransform.position.z);
            _spores[i].transform.gameObject.SetActive(false);
        }
    
    }
}
