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
    [SerializeField] GameObject spawner;

    //下三つにそれぞれ使うオブジェクトを入れてくれ
    /// <summary>シンプルな攻撃。貫通性能はなく、与ダメが高い。</summary>
    [SerializeField] GameObject normalBullet; //通常弾
    /// <summary>貫通性能を持った攻撃。複数の敵に攻撃ができ、装甲を貫くが与ダメは若干減る。</summary>
    [SerializeField] GameObject penetrationBullet; //貫通弾
    /// <summary>爆発性能を持った攻撃。放物線を描く挙動で飛び、壁などにぶつかると爆発する。二種のダメージ判定がある。</summary>
    [SerializeField] GameObject explodeBullet; //炸裂弾

    private BulletType bulletType;
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
        if (bulletType == BulletType.Normal)
        {
            Instantiate(normalBullet, spawner.transform);
        }
        else if (bulletType == BulletType.Penetration)
        {
            Instantiate(penetrationBullet, spawner.transform);
        }
        else 
        {
            Instantiate(explodeBullet, spawner.transform);
        }
    }

    enum BulletType
    {
        Normal,
        Penetration,
        explode
    }
}
