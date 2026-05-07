using UnityEngine;

public class TeacherFSM : MonoBehaviour
{
    enum TeacherState
    {
        Writing,
        Prepare,
        Watching
    }

    TeacherState currentState;

    public Sprite writingSprite;
    public Sprite prepareSprite;
    public Sprite watchingSprite;

    SpriteRenderer sr;
    float timer = 0f;
    float stateDuration = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        ChangeState(TeacherState.Writing);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > stateDuration)
        {
            if (currentState == TeacherState.Writing)
                ChangeState(TeacherState.Prepare);
            else if (currentState == TeacherState.Prepare)
                ChangeState(TeacherState.Watching);
            else if (currentState == TeacherState.Watching)
                ChangeState(TeacherState.Writing);
        }
    }

    void ChangeState(TeacherState newState)
    {
        currentState = newState;
        timer = 0f;

        switch (newState)
        {
            case TeacherState.Writing:
                sr.sprite = writingSprite;
                stateDuration = Random.Range(5f, 10f);
                break;

            case TeacherState.Prepare:
                sr.sprite = prepareSprite;
                stateDuration = 1f;
                break;

            case TeacherState.Watching:
                sr.sprite = watchingSprite;
                stateDuration = Random.Range(1f, 3f);
                break;
        }
    }

    public bool IsWatching()
    {
        return currentState == TeacherState.Watching;
    }
}