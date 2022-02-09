using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HijackParticleColour : MonoBehaviour
{
    private ParticleSystem pSys;

    // Start is called before the first frame update
    void Awake()
    {
        float hueValue = PlayerPrefs.GetFloat("ColourPicker", 0);

        pSys = GetComponent<ParticleSystem>();
        var pMain = pSys.main;
        pMain.startColor = new ParticleSystem.MinMaxGradient(Color.HSVToRGB(hueValue, 0.75f, 1f));
    }
}
