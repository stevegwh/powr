using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class MenuPointerController : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;

    private void Awake()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        if (laserPointer != null)
        {
            laserPointer.PointerIn += PointerInside;
            laserPointer.PointerOut += PointerOutside;
            laserPointer.PointerClick += PointerClick;
        }
        else
        {
            Debug.Log("Warning: No laser pointer attached to script.");
        }
    }

    private void OnDisable()
    {
        if (laserPointer == null) return;
        laserPointer.PointerIn -= PointerInside;
        laserPointer.PointerOut -= PointerOutside;
        laserPointer.PointerClick -= PointerClick;
    }

    private void PointerClick(object sender, PointerEventArgs e)
    {
        e.target.GetComponent<Image>().color = e.target.GetComponent<Button>().colors.pressedColor;
        e.target.GetComponent<Button>().onClick.Invoke();
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        e.target.GetComponent<Image>().color = e.target.GetComponent<Button>().colors.highlightedColor;
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        e.target.GetComponent<Image>().color = e.target.GetComponent<Button>().colors.normalColor;
    }
}
