using UnityEngine;
using TMPro;

public partial class Blackboard : MonoBehaviour
{
    public static Blackboard Instance { get; private set; }

    [Header("Noisy Person Settings")]
    [SerializeField] private TextMeshProUGUI noisyPersonText;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetNoisyPerson(string name)
    {
        if (noisyPersonText != null)
        {
            // 떠든사람 텍스트 업데이트
            noisyPersonText.text = $"떠든 사람 : {name}"; 

            Debug.Log("[ 이름 설정 ]");
        }
    }
}