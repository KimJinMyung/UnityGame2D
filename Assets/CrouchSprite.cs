using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchSprite : MonoBehaviour
{
    public float CrouchHeiht = 0.7f;
    private GameObject Collider2D;
    private Rigidbody2D rd;
    public Sprite changeSprite;
    private Sprite normalSprite;
    private SpriteRenderer spriteRenderer;
    private Vector2 normalHeight;
    private bool isCrouch = false;
    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        Collider2D = transform.GetChild(0).GetChild(0).gameObject;
        spriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        //GetComponent<CapsuleCollider2D>();
        normalHeight = Collider2D.transform.localScale;
        normalSprite = spriteRenderer.sprite;
    }

    private void Update()
    {
        //isCrouch = Input.GetKey(KeyCode.Space);
        if (isCrouch)
        {            
            Collider2D.transform.localScale = new Vector2(Collider2D.transform.localScale.x, 0.7f);
            spriteRenderer.sprite = changeSprite;
        }
        else
        {
            Collider2D.transform.localScale = normalHeight;
            spriteRenderer.sprite = normalSprite;
        }
    }

    private void OnCrouch(InputValue inputValue)
    {
        isCrouch = inputValue.isPressed;
    }
}
