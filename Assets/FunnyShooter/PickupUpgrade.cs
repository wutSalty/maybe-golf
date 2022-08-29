using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupUpgrade : PlayerUpgrades
{
    [field: Header("Child Class")]
    [field: SerializeField] public float[] PickupRanges { get; private set; }

    private void Start()
    {
        CharMovement.UpdateMagnetRange(PickupRanges[0], CalculatePercentage(PickupRanges[0], PickupRanges[2]));
    }

    public override void CalculateStatDiff()
    {
        float rangeDiff = PickupRanges[CurrentLevel] - PickupRanges[CurrentLevel - 1];
        SetNewAddition("+ " + CalculatePercentage(rangeDiff, PickupRanges[2]));
    }

    public override void ApplyUpgrade()
    {
        CharMovement.UpdateMagnetRange(PickupRanges[CurrentLevel - 1], CalculatePercentage(PickupRanges[CurrentLevel - 1], PickupRanges[2]));
    }
}
