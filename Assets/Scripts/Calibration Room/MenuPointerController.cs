using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class MenuPointerController : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;
    public GameObject Menu;
    private List<GameObject> menuItems;
    private Color _highlightedColor;
    private Color _pressedColor;
    private Color _normalColor;
    private Color _lastColor;

    private void Awake()
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

        if (Menu != null)
        {
            menuItems = new List<GameObject>();
            List<Button> buttonList = new List<Button>(Menu.GetComponentsInChildren<Button>(true));
            foreach (var button in buttonList)
            {
                menuItems.Add(button.gameObject);
            }
            _highlightedColor = menuItems[0].GetComponent<Button>().colors.highlightedColor;
            _normalColor = menuItems[0].GetComponent<Button>().colors.normalColor;
            _pressedColor = menuItems[0].GetComponent<Button>().colors.pressedColor;
            _lastColor = _normalColor;
        }
        else
        {
            Debug.Log("Warning: No menu attached to script.");
        }

    }

    private void PointerClick(object sender, PointerEventArgs e)
    {
        if (!menuItems.Contains(e.target.gameObject)) return;

        e.target.GetComponentInChildren<Image>().color = _pressedColor;
        e.target.GetComponent<Button>().onClick.Invoke();
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (!menuItems.Contains(e.target.gameObject)) return;
        if (_lastColor == _highlightedColor) return;

        e.target.GetComponentInChildren<Image>().color = _highlightedColor;
        _lastColor = _highlightedColor;
        // Debug.Log("Button was entered");
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (!menuItems.Contains(e.target.gameObject)) return;
        if (_lastColor == _normalColor) return;
        e.target.GetComponentInChildren<Image>().color = _normalColor;
        _lastColor = _normalColor;
        // Debug.Log("Button was exited");
    }
}
