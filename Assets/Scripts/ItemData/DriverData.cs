using UnityEngine;

[CreateAssetMenu(fileName = "DriverData", menuName = "Game Data/Driver Data")]
public class DriverData : ItemData
{
    [Header("기본 능력치")]
    public float basePedalSense;
    public float baseRecoverySpeed;
    public float baseProblemSolving;

    [Header("성장 계수")]
    public float growthPedalSense;
    public float growthRecoverySpeed;
    public float growthProblemSolving;
}