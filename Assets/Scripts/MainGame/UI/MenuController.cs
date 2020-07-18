using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public class MenuController : MonoBehaviour
{
    private Vector3 originalSize;
    public Transform LookAtPoint;
    private List<GameObject> children;
    public SteamVR_LaserPointer laserPointer;

    public GameObject startingPanel;

    private GameObject oldPanel;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            originalSize = transform.localScale;
            oldPanel = startingPanel;
            gameObject.SetActive(false);
        }
        else
        {
            originalSize = transform.localScale;
            oldPanel = startingPanel;
        }
    }
    void Start()
    {
        children = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }
        
    }


    public void ToggleAssistMode(Text text)
    {
        text.text = text.text == "Assist Mode On" ? "Assist Mode Off" : "Assist Mode On";
    }
    

    // public void ChangeMenu(GameObject menuToShow)
    // {
    //     foreach (var child in children)
    //     {
    //         child.SetActive(false);
    //     }
    //     menuToShow.SetActive(true);
    // }

    void OnEnable()
    {
        laserPointer.enabled = true;
        // transform.localScale = originalSize/2;
        // LeanTween.scale(gameObject, originalSize, 0.2f);
    }

    public void ChangePanel(GameObject newPanel)
    {
        newPanel.SetActive(true);
        oldPanel.SetActive(false);
        oldPanel = newPanel;
    }

    void OnDisable()
    {
        laserPointer.enabled = false;
        // LeanTween.scale(gameObject, Vector3.zero, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (LookAtPoint != null)
        {
            // transform.rotation = Quaternion.LookRotation(transform.position - LookAtPoint.transform.position);
        }
    }
}
