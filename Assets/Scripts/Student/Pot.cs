using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pot : MonoBehaviour
{ 
    [Header("Settings")]
    //가지고 있는 버섯 리스트 (오브젝트 형태로 저장)
    public List<GameObject> seeds = new List<GameObject>();
    //심을 버섯 인덱스 저장
    public int whatSeed;
    //화분에 심겨진 버섯
    private GameObject pottedSeed;
    //인벤에 있는 버섯 표기 간격 (추후 인벤에 맞게 수정)
    public float seedGap = 1.5f;
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

    public float growth;
    
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
    public ParticleSystem light;

    //테스트용 타이머
    private float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        potCollider = GetComponent<BoxCollider2D>();
        potTransform = GetComponent<Transform>();

        //테스트 인벤 버섯 처음에 안나오게 (테스트용)
        for (int i = 0; i < seeds.Count; i++)
        {
            if (seeds[i].transform != null)
            {
                seeds[i].transform.gameObject.SetActive(false);
            }
        }

        //처음에 물 게이지 표시 안되게
        if (waterMaxGaugeTransform != null)
        {
            waterMaxGaugeTransform.gameObject.SetActive(false);
        }

        Watering.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClickPot())
        {
            //화분에 버섯이 심겨있을 때 물주기
            if (pottedSeed != null)
            {
                IncreaseWaterGauge();

                //게이지 보이게
                waterMaxGaugeTransform.gameObject.SetActive(true);

                //물 다 채워지면 성장
                if (waterGauge >= waterMaxGauge)
                {
                    CompleteWater();
                }
            }

            else
            {
                //인벤에 있는 버섯들 표시 (화분이 비어있다면)
                for (int i = 0; i < seeds.Count; i++)
                {
                    seeds[i].transform.position = new Vector3(startingGap + potTransform.position.x + i*seedGap, potTransform.position.y, potTransform.position.z);
                    seeds[i].transform.gameObject.SetActive(true);
                }
            }
        }

        else
        {
            DecreaseWaterGauge();

            //게이지 사라지기 (게이지가 다 내려갔을 때)
            if (waterGauge <= waterMinGauge)
            {
                waterMaxGaugeTransform.gameObject.SetActive(false);
            }
        }

        UpdateWaterGaugeBar();

        //버섯 심기
        PotSeed();

        //(테스트용) 0.5초마다 의심도 / 돈 표시
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            if (pottedSeed != null)
            {
                Debug.Log($"성장도 : {growth}");
            }
            timer = 0f;
        }
    }

    //화분 클릭 감지
    bool IsClickPot()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            return potCollider == Physics2D.OverlapPoint(mousePos); 
        }
        return false;
    }

    //버섯 클릭 감지
    void PotSeed()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //어떤 버섯을 심을 건지 선택
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D ClickedSeed = Physics2D.OverlapPoint(mousePos);

            if (ClickedSeed != null)
            {
                whatSeed = seeds.IndexOf(ClickedSeed.gameObject);
                if (whatSeed != -1)
                {
                    Potting.Play();
                    //버섯 콜라이더에 막히는거 수정 필요
                    potCollider.offset = new Vector2(potCollider.offset.x, (pottedsize-1)/2);
                    potCollider.size = new Vector2(potCollider.size.x, pottedsize);
                }
            }
            

            //선택한 버섯 심기(위치는 버섯 이미지 받은 후  재지정), 인벤에서 삭제
            switch (whatSeed)
            {
                case 0:
                    {
                    SeedListManage(0);
                    }
                    break;
                
                case 1:
                    {
                    SeedListManage(1);
                    }
                    break;

                case 2:
                    {
                    SeedListManage(2);
                    }
                    break;

                case 3:
                    {
                    SeedListManage(3);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    void SeedListManage(int idx)
    {
        if (pottedSeed != null)
        {
            seeds.Insert(idx, pottedSeed);
            seeds[idx].transform.position = new Vector3(startingGap + potTransform.position.x + idx*seedGap, potTransform.position.y, potTransform.position.z);
            pottedSeed = seeds[idx + 1];
            seeds[idx + 1].transform.position = new Vector3(potTransform.position.x, potTransform.position.y + 1f, potTransform.position.z);
            light.transform.position = seeds[idx + 1].transform.position;
            light.Play();
            seeds.RemoveAt(idx + 1);
        }
        else
        {
            pottedSeed = seeds[idx];
            seeds[idx].transform.position = new Vector3(potTransform.position.x, potTransform.position.y + 1f, potTransform.position.z);
            light.transform.position = seeds[idx].transform.position;
            light.Play();
            seeds.RemoveAt(idx);
        }
    }

    //Book 게이지 올리기
    void IncreaseWaterGauge()
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

    //Book 게이지 다 찼을 때, 초기화 + 의심도 내리기 or 보상 주기
    void CompleteWater()
    {
        waterGauge = 0f;
        growth += 10f;
    }

    //Book 게이지 내리기
    void DecreaseWaterGauge()
    {
        if (waterGauge > waterMinGauge)
            {
                waterGauge -= waterGaugeSpeed * Time.deltaTime;

                waterGauge = Mathf.Max(waterGauge, waterMinGauge);
            }

        Watering.Stop();
    }

    //게이지 움직임
    void UpdateWaterGaugeBar()
    {
        if (waterGaugeTransform != null)
        {
            waterGaugeTransform.localScale = new Vector3(2 * waterGauge / waterMaxGauge, waterGaugeTransform.localScale.y, waterGaugeTransform.localScale.z);
        }
    }
}
