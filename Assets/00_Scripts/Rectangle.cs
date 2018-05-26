using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rectangle : MonoBehaviour
{
    public Color originalColor;
    public int colorIndex;
    public TextMeshPro label;

    Manager manager;
    SpriteRenderer sRend;
    Remote remote;

    void Start()
    {
        manager = Manager.Instance;
        remote = Remote.Instance;
        sRend = GetComponent<SpriteRenderer>();
        label.text = (colorIndex + 1).ToString();
    }

    void OnMouseDown()
    {
        remote.PlayButtonSFX();
        colorIndex++;
        if (colorIndex.Equals(manager.rectangles.Length))
        {
            colorIndex = 0;
        }
        Color nextColor = manager.GetNextColor(colorIndex);
        sRend.color = nextColor;
        label.text = (colorIndex + 1).ToString();
    }

    public void SetNewIndex(Color newColor, int newIndex)
    {
        sRend.color = newColor;
        colorIndex = newIndex;
        label.text = (colorIndex + 1).ToString();
    }
}