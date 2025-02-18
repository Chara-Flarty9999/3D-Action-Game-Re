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
///敵用のスクリプトである。
/// </summary>
public class EnemyExplodeBullet : MonoBehaviour
{
    Collider collider;
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
    [SerializeField] AudioClip explode;
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
    [SerializeField] GameObject player;
    [SerializeField] GameObject explode_Effect;
    GameObject[] delete_exploEffect;


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
        audioSource = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
        rigidbody = this.GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        transform.rotation = Quaternion.Euler(0, 0, _rote);
        audioSource.PlayOneShot(fly);
        //rigidbody.AddForce((player.transform.forward / 30 + player.transform.up / 15) * 150, ForceMode.Impulse);
    }




    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collision)
    {
        string tagcheck = collision.gameObject.tag;
        if (tagcheck != "Player" && tagcheck != "Outside_Explode" && tagcheck != "Inside_Explode")
        {
            Instantiate(explode_Effect, transform.position, Quaternion.identity);
            mesh.enabled = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.useGravity = false;
            collider.enabled = false;
            delete_exploEffect = GameObject.FindGameObjectsWithTag("Outside_Explode");
            AudioSource.PlayClipAtPoint(explode, transform.position);
            for (int i = 0; i < delete_exploEffect.Length; i++)
            {
                Destroy(delete_exploEffect[i], 1.8f);
            }
            Destroy(gameObject, 1.8f);
        }
    }

    public void OnDestroy()
    {

    }
}