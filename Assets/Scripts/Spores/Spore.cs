using UnityEngine;

public abstract class Spore : ScriptableObject
{
    [SerializeField] public string sporeName = "버섯 포자 이름";
    [SerializeField] public int price = 0;
    [SerializeField] public Sprite icon;
    [SerializeField] public int shopOrder = 0;

    public string SporeName => sporeName;
    public int Price => price;
    public Sprite Icon => icon;
}