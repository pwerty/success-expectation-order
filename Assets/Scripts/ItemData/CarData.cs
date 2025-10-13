using UnityEngine;

public enum CarRarity { R, SR, SSR }

[CreateAssetMenu(fileName = "CarData", menuName = "Game Data/Car Data")]
public class CarData : ItemData
{
    public CarRarity rarity;

    [Header("기본 능력치")]
    public float baseSpeed;
    public float baseDrift;
    public float baseAcceleration;

    [Header("성장 계수")]
    public float growthSpeed;
    public float growthDrift;
    public float growthAcceleration;
}