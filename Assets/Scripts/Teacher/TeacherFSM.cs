using UnityEngine;
using System;

public class TeacherFSM : MonoBehaviour
{
    [SerializeField] private Sprite writingSprite;
    [SerializeField] private Sprite prepareSprite;
    [SerializeField] private Sprite watchingSprite;
    [SerializeField] private AudioClip warningSound;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // TeacherManager의 이벤트를 구독하여 화면 업데이트 수행
        TeacherManager.OnTeacherTookNotes += SetWritingSprite;
        TeacherManager.OnTeacherGaveSignal += HandleTeacherGaveSignal;
        TeacherManager.OnTeacherLookedBack += SetWatchingSprite;

        // 켜지는 순간 현재 매니저의 상태를 읽어와서 즉시 동기화
        if (TeacherManager.Instance != null)
        {
            SyncSprite(TeacherManager.Instance.CurrentState);
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        TeacherManager.OnTeacherTookNotes -= SetWritingSprite;
        TeacherManager.OnTeacherGaveSignal -= HandleTeacherGaveSignal;
        TeacherManager.OnTeacherLookedBack -= SetWatchingSprite;
    }

    private void SyncSprite(TeacherManager.TeacherState state)
    {
        switch (state)
        {
            case TeacherManager.TeacherState.Writing: SetWritingSprite(); break;
            case TeacherManager.TeacherState.Prepare: SetPrepareSprite(); break;
            case TeacherManager.TeacherState.Watching: SetWatchingSprite(); break;
        }
    }

    private void SetWritingSprite()
    {
        if (sr != null) sr.sprite = writingSprite;
    }

    private void SetPrepareSprite()
    {
        if (sr != null) sr.sprite = prepareSprite;
    }

    private void HandleTeacherGaveSignal()
    {
        SetPrepareSprite();

        if (warningSound != null)
        {
            // 씬이 전환되어도 사운드가 끊기지 않도록 파괴되지 않는 일회용 게임 오브젝트 생성
            GameObject soundObj = new GameObject("WarningSoundPlayer");
            DontDestroyOnLoad(soundObj);
            
            AudioSource tempSource = soundObj.AddComponent<AudioSource>();
            tempSource.clip = warningSound;
            tempSource.Play();
            
            // 클립의 재생 시간이 끝나면 일회용 오브젝트 자동 파괴
            Destroy(soundObj, warningSound.length);
        }
    }

    private void SetWatchingSprite()
    {
        if (sr != null) sr.sprite = watchingSprite;
    }
}