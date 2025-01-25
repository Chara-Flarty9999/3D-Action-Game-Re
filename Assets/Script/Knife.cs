using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// ナイフと名前がついてはいるが実際は指定した方向にスプライトが飛んでいくもの。
///汎用性だけは普通に高い。
///別の空オブジェクトで_rote, _magnificationを設定し、あと座標をInstantiateで設定するだけ。
///サウンドはインスペクターで設定してください。
/// </summary>
public class Knife : MonoBehaviour
{

    MeshRenderer mesh;
    Vector2 movement;
    Rigidbody2D rigidbody2d;
    AudioSource audioSource;
    /// <summary>
    /// 召喚時の音
    /// </summary>
    [SerializeField] AudioClip spawn;
    /// <summary>
    /// 飛んでく時の音
    /// </summary>
    [SerializeField] AudioClip fly;
    /// <summary>
    /// ナイフが飛んでく角度
    /// </summary>
    int _rote;
    float _rotation;
    /// <summary>
    /// ナイフの加速度
    /// </summary>
    float _magnification;
    float _waitTime;

    public bool m_play = true;


    /// <summary>
    /// ベクトルから角度を取得する。
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector2 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }

    // Start is called before the first frame update
    void Start()
    {
        //スポーン時の情報を取得する
        audioSource = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
        GameObject spawner = GameObject.Find("Bullet Muzzle");
        KnifeSpawn knife = spawner.GetComponent<KnifeSpawn>();
        rigidbody2d = this.GetComponent<Rigidbody2D>();

        _rote = knife.rote;
        _magnification = knife.magnification;
        transform.rotation = Quaternion.Euler(0, 0, _rote);

        StartCoroutine("Transparent");

        if (m_play == true)
        {
            mesh.material.color -= new Color32(0, 0, 0, 255);
            rigidbody2d.rotation -= 180;
        }


        Destroy(gameObject, 3);
        //Invoke("Destroy");
    }

    IEnumerator Transparent()　//ここで召喚の挙動。ベクトルも取得している。
    {
        if (m_play == true)
        {
            //audioSource.PlayOneShot(spawn);
            for (int i = 0; i < 30; i++)
            {
                mesh.material.color = mesh.material.color + new Color32(0, 0, 0, 9);
                rigidbody2d.rotation += 6;
                yield return new WaitForSeconds(0.01f);

            }
            yield return new WaitForSeconds(0.1f);
        }

        _rotation = transform.localEulerAngles.z;

        movement = AngleToVector2(_rotation);



        //c = 1; //ここで飛んでいく挙動が開始されるようになっている。

        audioSource.PlayOneShot(fly);

        for (int i = 0; i < 50; i++)
        {
            rigidbody2d.AddForce(movement * new Vector2(_magnification, _magnification)); //ForceMode2D.Impulse
            yield return new WaitForSeconds(0.01f);
        }

    }


    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {


    }

    public void Destroy()
    {
        
    }
}