using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuTween : MonoBehaviour
{
    private Vector3 originalPosition;
    public enum MenuTweenIn
    {
        VerticalSlide,
        HorizontalSlide,
        Instant
    }

    public MenuTweenIn tweenIn;
    public MenuTweenIn tweenOut;
    private MenuController menuController;
    public bool PlayOnEnable = true;

    void Start()
    {
        menuController = GetComponentInParent<MenuController>();
    }

    private void SlideTowards(Vector3 direction, Action completeFunction)
    {
        originalPosition = transform.position;
        transform.position = new Vector3(originalPosition.x + direction.x, originalPosition.y + direction.y, originalPosition.z + direction.z);
        LeanTween.move(gameObject, originalPosition, 0.25f).setOnComplete(completeFunction);
    }
    private void SlideFrom(Vector3 direction, Action completeFunction)
    {
        originalPosition = transform.position;
        Vector3 destination = new Vector3(originalPosition.x + direction.x, originalPosition.y + direction.y, originalPosition.z + direction.z);
        LeanTween.move(gameObject, destination, 0.25f).setOnComplete(completeFunction);
    }

    void OnEnable()
    {
        if (!PlayOnEnable) return;
        switch (tweenIn)
        {
            case MenuTweenIn.VerticalSlide:
                SlideTowards(new Vector3(0, -0.25f, 0), null);
                break;
            case MenuTweenIn.HorizontalSlide:
                SlideTowards(new Vector3(-0.25f, 0, 0), null);
                break;
        }
    }
    public void OnClose(GameObject newPanel)
    {
        switch (tweenOut)
        {
            case MenuTweenIn.Instant:
                menuController.ChangePanel(newPanel);
                break;
            case MenuTweenIn.VerticalSlide:
                SlideFrom(new Vector3(0, -0.25f, 0), () =>
                {
                    transform.position = originalPosition;
                    menuController.ChangePanel(newPanel);
                });
                break;
            case MenuTweenIn.HorizontalSlide:
                SlideFrom(new Vector3(-0.25f, 0, 0), () =>
                {
                    transform.position = originalPosition;
                    menuController.ChangePanel(newPanel);
                });
                break;
        }
    }

}
