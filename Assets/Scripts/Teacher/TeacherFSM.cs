using UnityEngine;
using System;

public class TeacherFSM : MonoBehaviour
{
    [SerializeField] private Sprite writingSprite;
    [SerializeField] private Sprite prepareSprite;
    [SerializeField] private Sprite watchingSprite;
    [SerializeField] private AudioClip warningSound;

    private SpriteRenderer sr;
    private AudioSource audioSource;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // TeacherManager의 이벤트를 구독하여 화면 업데이트 수행
        TeacherManager.OnTeacherTookNotes += SetWritingSprite;
        TeacherManager.OnTeacherGaveSignal += SetPrepareSprite;
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
        TeacherManager.OnTeacherGaveSignal -= SetPrepareSprite;
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
        if (audioSource != null && warningSound != null)
            audioSource.PlayOneShot(warningSound);
    }

    private void SetWatchingSprite()
    {
        if (sr != null) sr.sprite = watchingSprite;
    }
}