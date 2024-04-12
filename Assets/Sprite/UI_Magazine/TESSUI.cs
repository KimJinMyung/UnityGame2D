using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TESSUI : MonoBehaviour
{
    public GameObject targetObject;
    public Image targetImage;
    public Sprite[] saveSptrite;
    private void Awake()
    {
        targetImage = targetObject.GetComponent<Image>();
    }

    public void ChangerImage()
    {
        targetImage.sprite = saveSptrite[0];
    }
}
