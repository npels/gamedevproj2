using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorHandler : MonoBehaviour {

    public Slider hSlider;
    public Slider sSlider;
    public Slider vSlider;

    float hue;
    float saturation;
    float value;

    bool initialized;

    private void Start() {
        initialized = false;
        Color c = GameManager.instance.player.GetComponent<Creature>().GetColor();
        Color.RGBToHSV(c, out hue, out saturation, out value);
        hSlider.value = hue;
        sSlider.value = saturation;
        vSlider.value = value;
        initialized = true;
    }

    public void UpdateColor() {
        if (!initialized) return;
        hue = hSlider.value;
        saturation = sSlider.value;
        value = vSlider.value;

        GameManager.instance.player.GetComponent<Creature>().ChangeColor(hue, saturation, value);
    }
}
