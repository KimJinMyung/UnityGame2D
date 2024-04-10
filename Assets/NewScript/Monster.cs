using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float Hp = 5;
    [SerializeField] private bool isDead = false;
    [SerializeField] private int Parts = 1;

    private Rigidbody2D aaa;
    private SpriteRenderer[] Aiming;
    private bool isBleeding = false;

    public GameObject dropMagazine;

    private void Awake()
    {
        aaa= GetComponent<Rigidbody2D>();
        Aiming = new SpriteRenderer[3];
    }

    private void Start()
    {
        Aiming[0] = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Aiming[1] = transform.GetChild(1).GetComponent<SpriteRenderer>();
        Aiming[2] = transform.GetChild(2).GetComponent<SpriteRenderer>();

        OffTargeting();
    }

    public void Hurt(float damage)
    {
        if (Parts == 2)
        {
            //if(rb == null) rb = GetComponent<Rigidbody2D>();
            if(transform.rotation.z == 0)
            {
                aaa.constraints = RigidbodyConstraints2D.None;
                transform.Rotate(0, 0, -60);
            }            
        }
        Debug.Log($"{damage}만큼의 공격을 받았습니다.");
        Hp -= damage;

        if (!isBleeding)
        {
            isBleeding= true;
            StartCoroutine(Bleeding());
        }
        if (Hp <= 0)
        {
            isDead = true;
            Die();
        }
    }
    public void Die()
    {
        StopAllCoroutines();
        //transform.gameObject.SetActive(false);
        if (transform.rotation.z == 0)
        {
            aaa.constraints = RigidbodyConstraints2D.None;
            transform.Rotate(0, 0, -60);
        }
        OffTargeting();
        gameObject.layer = 7;
        Drop();
    }

    private void Drop()
    {
        Item item = new Item();
        item.bulletCount = Random.Range(0, 16);
        Vector2 dropPos = transform.position;
        GameObject newMagazine = Instantiate(dropMagazine);
        newMagazine.GetComponent<Magazine>().SetMagazine(item, true);
        newMagazine.transform.position = dropPos;
    }

    private void OnTargeting(bool isHeadAiming, bool isLegAiming)
    {
        if (isHeadAiming && !isLegAiming)
        {
            Aiming[0].enabled = true;
            Aiming[1].enabled = false;
            Aiming[2].enabled = false;
        }
        else if (!isHeadAiming && isLegAiming)
        {
            Aiming[0].enabled = false;
            Aiming[1].enabled = false;
            Aiming[2].enabled = true;
        }
        else
        {
            Aiming[0].enabled = false;
            Aiming[1].enabled = true;
            Aiming[2].enabled = false;
        }
    }

    public void HitParts()
    {
        int line = 0;
        foreach (var i in Aiming)
        {
            if (i.enabled)
            {
                Parts = line;
            }
            line++;
        }
    }

    private void OffTargeting()
    {
        Aiming[0].enabled = false;
        Aiming[1].enabled = false;
        Aiming[2].enabled = false;
    }

    ////오버 로딩
    public void SetTargeted()
    {
        if(isDead) return;
        OffTargeting();
    }

    public void SetTargeted(bool isHeadAiming, bool isLegAiming)
    {
        if(isDead) return;
        OnTargeting(isHeadAiming, isLegAiming);
    }

    private IEnumerator Bleeding()
    {
        while (!isDead)
        {            
            // 주기적으로 피해를 입히는 간격(예: 1초)을 기다립니다.
            yield return new WaitForSeconds(2f);

            // 피해를 입히는 부분
            Hurt(1);
            Debug.Log("Bleeding...");
        }
    }
}
