/*
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    //public AudioListener listners;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
*/
using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    void Awake() {
    var listeners = FindObjectsOfType<AudioListener>();
    if (listeners.Length > 1) {
        Destroy(gameObject); // 이미 존재하면 새로 생성된 것을 삭제
    } else {
        DontDestroyOnLoad(gameObject); // 없으면 유지
    }
}
}
