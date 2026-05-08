using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pot : MonoBehaviour
{ 
    [Header("Settings")]
    //가지고 있는 버섯 리스트 (오브젝트 형태로 저장)
    public List<GameObject> Spores = new List<GameObject>();
    //심을 버섯 인덱스 저장
    public int whatSpore;
    //화분에 심겨진 버섯
    private GameObject pottedSpore;
    //인벤에 있는 버섯 표기 간격 (추후 인벤에 맞게 수정)
    public float sporeGap = 1.5f;
    //인벤 첫 번째 버섯과 화분 간격
    public float startingGap = 1.7f;
    //버섯 심은 후 화분 히트박스(y)
    public float pottedsize = 1.6f;

    //현재 물 게이지
    public float waterGauge = 0f;
    //최소 물 게이지
    public float waterMinGauge = 0f;
    //최대 물 게이지
    public float waterMaxGauge = 30f;
    //물 게이지 올라가는 속도
    public float waterGaugeSpeed = 10f;
    //성장도
    public float growth;

    public LayerMask Spore;
    
    private BoxCollider2D potCollider;
    private Transform potTransform;
    //최대 물 게이지 Trasform
    public Transform waterMaxGaugeTransform;
    //물 게이지 Transform
    public Transform waterGaugeTransform;

    //사운드
    [Header("Sound")]
    public AudioSource Potting;
    public AudioSource Watering;

    [Header("Particle")]
    public ParticleSystem pottingEffect;

    //테스트용 타이머
    private float timer = 0f;

    private void Start()
    {
        potCollider = GetComponent<BoxCollider2D>();
        potTransform = GetComponent<Transform>();

        DisappearSpores();

        //처음에 물 게이지 표시 안되게
        if (waterMaxGaugeTransform != null)
        {
            waterMaxGaugeTransform.gameObject.SetActive(false);
        }

        InitPotAudio();
    }

    private void Update()
    {
        if (IsClickPot())
        {
            AppearSpores();
            if (pottedSpore != null)
            {
                IncreaseWaterGauge();

                //물 다 채워지면 성장
                if (waterGauge >= waterMaxGauge)
                {
                    CompleteWater();
                }
            }
        }
        else
        {
            DecreaseWaterGauge();
        }

        PotSpore(); //버섯 심기

        UpdateWaterGaugeBar(); //게이지 시각화

        //(테스트용) 0.5초마다 성장도 표시
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            if (pottedSpore != null)
            {
                Debug.Log($"성장도 : {growth}");
                Debug.Log(whatSpore);
            }
            timer = 0f;
        }
    }

    private void InitPotAudio()
    {
        Watering.loop = true;

        Potting.loop = false;
    }
    private bool IsClickPot()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            
            if (hit == potCollider)
            {
                return true;
            }
        }

        return false;
    }

    //버섯 심기
    private void PotSpore()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //선택한 버섯 위치 조정
            LocateSpore();
            
            //선택한 버섯 심기(위치는 버섯 이미지 받은 후 재지정), 인벤에서 삭제
            switch (whatSpore)
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

    private void LocateSpore()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D ClickedSpore = Physics2D.OverlapPoint(mousePos);
        if (ClickedSpore != null)
        {
            if (ClickedSpore.gameObject.layer == LayerMask.NameToLayer("Spore"))
            {
                whatSpore = Spores.IndexOf(ClickedSpore.gameObject);
                Potting.Play();
                //버섯 콜라이더에 막히는거 수정 필요
                potCollider.offset = new Vector2(potCollider.offset.x, (pottedsize-1)/2);
                potCollider.size = new Vector2(potCollider.size.x, pottedsize);
            }
            else
            {
                whatSpore = -1;
            }
        }
        else
        {
            whatSpore = -1;
        }
    }

    private void SporeListManage(int idx)
    {
        if (pottedSpore != null)
        {
            Spores.Insert(idx, pottedSpore);
            Spores[idx].transform.position = new Vector3(startingGap + potTransform.position.x + idx*sporeGap, potTransform.position.y, potTransform.position.z);
            pottedSpore = Spores[idx + 1];
            Spores[idx + 1].transform.position = new Vector3(potTransform.position.x, potTransform.position.y + 1f, potTransform.position.z);
            pottingEffect.transform.position = Spores[idx + 1].transform.position;
            pottingEffect.Play();
            Debug.Log("반짝반짝");
            Spores.RemoveAt(idx + 1);
        }
        else
        {
            pottedSpore = Spores[idx];
            Spores[idx].transform.position = new Vector3(potTransform.position.x, potTransform.position.y + 1f, potTransform.position.z);
            pottingEffect.transform.position = Spores[idx].transform.position;
            pottingEffect.Play();
            Spores.RemoveAt(idx);
        }
    }

    private void IncreaseWaterGauge()
    {
        if (waterGauge < waterMaxGauge)
        {
            waterGauge += waterGaugeSpeed * Time.deltaTime;
            waterGauge = Mathf.Min(waterGauge, waterMaxGauge);
            //물 주는 소리
            if (!Watering.isPlaying)
            {
                Watering.Play();
            }
        }
    }

    private void CompleteWater()
    {
        waterGauge = 0f;
        growth += 10f;
    }

    private void DecreaseWaterGauge()
    {
        if (waterGauge > waterMinGauge)
            {
                waterGauge -= waterGaugeSpeed * Time.deltaTime;

                waterGauge = Mathf.Max(waterGauge, waterMinGauge);
            }

        Watering.Stop();
    }

    private void AppearWaterGauge()
    {
        waterMaxGaugeTransform.gameObject.SetActive(true);
    }

    private void DisappearWaterGauge()
    {
        if (waterGauge <= waterMinGauge) //게이지 사라지기 (게이지가 다 내려갔을 때)
        {
            waterMaxGaugeTransform.gameObject.SetActive(false);
        }
    }

    private void UpdateWaterGaugeBar()
    {
        if (pottedSpore != null)
        {
            if (IsClickPot())
            {
                AppearWaterGauge();
            }
            else
            {
                DisappearWaterGauge();
            }

            if (waterGaugeTransform != null) //게이지 시각화
            {
                waterGaugeTransform.localScale = new Vector3(2 * waterGauge / waterMaxGauge, waterGaugeTransform.localScale.y, waterGaugeTransform.localScale.z);
            }
        }
    }

    private void AppearSpores()
    {
        for (int i = 0; i < Spores.Count; i++)
        {
            Spores[i].transform.position = new Vector3(startingGap + potTransform.position.x + i*sporeGap, potTransform.position.y, potTransform.position.z);
            Spores[i].transform.gameObject.SetActive(true);
        }

    }

    private void DisappearSpores()
    {
        for (int i = 0; i < Spores.Count; i++)
        {
            Spores[i].transform.position = new Vector3(startingGap + potTransform.position.x + i*sporeGap, potTransform.position.y, potTransform.position.z);
            Spores[i].transform.gameObject.SetActive(false);
        }
    
    }
}
