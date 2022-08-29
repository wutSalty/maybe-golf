using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpgrade : PlayerUpgrades
{
    [field: Header("Child Class")]
    [field: SerializeField] public float[] PlayerSpeeds { get; private set; }

    private void Start()
    {
        CharMovement.UpdateSpeed(PlayerSpeeds[0], CalculatePercentage(PlayerSpeeds[0], PlayerSpeeds[2]));
    }

    public override void CalculateStatDiff()
    {
        float speedDiff = PlayerSpeeds[CurrentLevel] - PlayerSpeeds[CurrentLevel - 1];
        SetNewAddition("+ " + CalculatePercentage(speedDiff, PlayerSpeeds[2]));
    }

    public override void ApplyUpgrade()
    {
        CharMovement.UpdateSpeed(PlayerSpeeds[CurrentLevel - 1], CalculatePercentage(PlayerSpeeds[CurrentLevel - 1], PlayerSpeeds[2]));
    }
}
