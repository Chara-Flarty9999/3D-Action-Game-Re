using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float HP {get; set;}
    [SerializeField] private float MaxHP = 10;
    [SerializeField] GameObject lifeGage;
    [SerializeField] GameObject charaImg;
    [SerializeField] GameObject spawner;

    //���O�ɂ��ꂼ��g���I�u�W�F�N�g�����Ă���
    /// <summary>�V���v���ȍU���B�ђʐ��\�͂Ȃ��A�^�_���������B</summary>
    [SerializeField] GameObject normalBullet; //�ʏ�e
    /// <summary>�ђʐ��\���������U���B�����̓G�ɍU�����ł��A���b���т����^�_���͎኱����B</summary>
    [SerializeField] GameObject penetrationBullet; //�ђʒe
    /// <summary>�������\���������U���B��������`�������Ŕ�сA�ǂȂǂɂԂ���Ɣ�������B���̃_���[�W���肪����B</summary>
    [SerializeField] GameObject explodeBullet; //�y���e

    [SerializeField] BulletType bulletType;
    private Image characterImage;
    private Image lifeImage;

    UnityEvent _getDamage;

    // Start is called before the first frame update
    void Start()
    {
        HP = MaxHP;
        lifeImage = lifeGage.GetComponent<Image>();
        characterImage = charaImg.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            if(bulletType == BulletType.Explode)
            {
                bulletType = BulletType.Normal;
            }
            else
            {
                bulletType++;
            }
        }
    }

    public void GetDamage_Heal(int change_HP) 
    {
        HP += change_HP;
        characterImage.color = change_HP >= 0 ? new Color(0, 1, 0, 1): new Color(1, 0, 0, 1);
        if (HP > MaxHP) {  HP = MaxHP; }
        lifeImage.fillAmount = HP/MaxHP;
        characterImage.DOColor(new Color(1,1,1), 0.8f);
    }

    public void BulletShoot()
    {
        GameObject bullet = bulletType switch
        {
            BulletType.Normal => normalBullet,
            BulletType.Penetration => penetrationBullet,
            BulletType.Explode => explodeBullet,
        };

        Instantiate(bullet, new Vector3(spawner.transform.position.x, spawner.transform.position.y, spawner.transform.position.z), Quaternion.identity);
    }

    enum BulletType
    {
        Normal,
        Penetration,
        Explode
    }
}
