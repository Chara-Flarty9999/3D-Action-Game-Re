using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WarriorAnimsFREE;
/// <summary>
/// ナイフと名前がついてはいるが実際は指定した方向にスプライトが飛んでいくもの。
///汎用性だけは普通に高い。
///別の空オブジェクトで_rote, _magnificationを設定し、あと座標をInstantiateで設定するだけ。
///サウンドはインスペクターで設定してください。
/// </summary>
public class PenetrationBullet : MonoBehaviour
{

    MeshRenderer mesh;
    Vector3 movement;
    Rigidbody rigidbody;
    AudioSource audioSource;
    /// <summary>
    /// 召喚時の音
    /// </summary>
    [SerializeField] AudioClip hit;
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
    private WarriorController warriorController;
    GameObject player;


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
        player = GameObject.Find("Sana.Airsky_Sorceress");
        warriorController = player.GetComponent<WarriorController>();
        audioSource = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
        rigidbody = this.GetComponent<Rigidbody>();

        transform.rotation = Quaternion.Euler(0, 0, _rote);
        audioSource.PlayOneShot(fly);
        rigidbody.AddForce(warriorController.transform.forward * 70, ForceMode.Impulse);

        Destroy(gameObject, 3);
    }




    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        AudioSource.PlayClipAtPoint(hit, transform.position);
    }
}