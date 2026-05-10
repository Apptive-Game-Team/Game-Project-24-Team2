using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ShopPurchaseHandler))]
[RequireComponent(typeof(AudioSource))]
public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopCanvasPrefab;
    [SerializeField] private GameObject shopItemPrefab;
    private GameObject shopCanvasInstance;
    private Transform content;
    private ShopPurchaseHandler shopPurchaseHandler;
    private AudioSource audioSource;
    private AudioClip openShopSound;
    private AudioClip closeShopSound;
    private void Awake()
    {
        // 상점 창 연결 및 비활성화
        shopCanvasInstance = Instantiate(shopCanvasPrefab);
        shopCanvasInstance.SetActive(false);

        // 상점 아이템 목록 창 연결
        content = shopCanvasInstance.transform.Find("ShopPanel/Scroll View/Viewport/Content");

        // 상점 닫기 버튼 연결
        Button buyButton = shopCanvasInstance.GetComponentInChildren<Button>(true); 
        buyButton.onClick.AddListener(() => CloseShop());

        // 아이템 구매 로직 연결
        shopPurchaseHandler = GetComponent<ShopPurchaseHandler>();
        TextMeshProUGUI moneyTextUI = shopCanvasInstance.GetComponentInChildren<TextMeshProUGUI>(true);
        shopPurchaseHandler.SetMoneyText(moneyTextUI);

        // 상점 여닫는 소리 연결
        audioSource = GetComponent<AudioSource>();
        openShopSound = Resources.Load<AudioClip>("Shop/OpenBellDoor");
        closeShopSound = Resources.Load<AudioClip>("Shop/CloseBellDoor");

        // Resources/Spores에 위치한 spore asset에 따라 상점 아이템 목록 자동 제작
        PopulateShop();
    }

    private void PopulateShop()
    {
        // Resources 폴더의 "Spores" 폴더 안에 있는 모든 Spore.asset 불러오기
        Spore[] allSpores = Resources.LoadAll<Spore>("Spores");

        // spore의 shopOrder에 따라 상점 아이템 목록 정렬
        allSpores = allSpores.OrderBy(spore => spore.shopOrder).ToArray();

        // 상점 아이템 목록 제작
        foreach (Spore spore in allSpores)
        {
            GameObject shopItemInstance = Instantiate(shopItemPrefab, content); // 상점 아이템 목록에 ShopItem 출력

            RectTransform rect = shopItemInstance.GetComponent<RectTransform>(); // 이 코드 없으면 아이템이 안 보임;;
            rect.localScale = Vector3.one;

            ShopItem shopItem = shopItemInstance.GetComponent<ShopItem>(); // 각 아이템 정보를 spore 에셋과 연동
            shopItem.SetData(spore); 
            
            Button buyButton = shopItemInstance.GetComponentInChildren<Button>(true); // 구매 버튼을 구매 로직과 연결
            buyButton.onClick.AddListener(() => shopPurchaseHandler.TryPurchase(spore));
        }
    }

    public void OpenShop() 
    {
        shopCanvasInstance.SetActive(true);
        audioSource.PlayOneShot(openShopSound);
    }

    public void CloseShop()
    {
        shopCanvasInstance.SetActive(false);
        audioSource.PlayOneShot(closeShopSound);
    }
}