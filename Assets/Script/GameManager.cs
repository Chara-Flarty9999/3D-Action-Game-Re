using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float HP {get; set;}
    [SerializeField] private float MaxHP = 10;
    [SerializeField] GameObject lifeGage;
    [SerializeField] GameObject charaImg;
    private Image characterImage;
    private Image image;

    UnityEvent _getDamage;

    // Start is called before the first frame update
    void Start()
    {
        HP = MaxHP;
        image = lifeGage.GetComponent<Image>();
        characterImage = charaImg.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetDamage_Heal(int change_HP) 
    {
        HP += change_HP;
        characterImage.color = change_HP >= 0 ? new Color(0, 1, 0, 1): new Color(1, 0, 0, 1);
        if (HP > MaxHP) {  HP = MaxHP; }
        image.fillAmount = HP/MaxHP;
        characterImage.DOColor(new Color(1,1,1), 0.8f);
    }
}
