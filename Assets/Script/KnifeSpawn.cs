using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class KnifeSpawn : MonoBehaviour
{

    [SerializeField] UnityEvent _actions;
    GameManager _gameManager;
    public int damage; //特に使ってない

    float y;
    float x;
    
    //ブラスター専用の情報
    /// <summary>ブラスターの初期位置</summary>
    //public ClossBlaster.StartInfo f_place;
    /// <summary>ブラスターの移動情報</summary>
    //public ClossBlaster.MoveInfo m_place;
    /// <summary>ブラスターのサイズ</summary>
    public float blasterSize;
    /// <summary>ブラスター発射までの遅延時間</summary>
    public int beamWait;
    /// <summary>ブラスターの発射期間(ループ回数)</summary>
    public int beamTime;
    /// <summary>ブラスターの色の指定</summary>
    //public ClossBlaster.BlasterColor blasterColor;


    /// <summary>
    /// 直進するナイフ。
    /// </summary>
    [SerializeField] GameObject m_spawnPrefab = default;
    /// <summary>
    /// 進行方向が指定できるナイフ。(上下左右)
    /// </summary>
    [SerializeField] GameObject m_spawn2Prefab = default;
    /// <summary>
    /// 炸裂弾。
    /// </summary>
    [SerializeField] GameObject m_spawn3Prefab = default;
    /// <summary>
    /// クロスブラスター
    /// </summary>
    [SerializeField] GameObject m_clossBlasterPrefab = default;
    /// <summary>
    /// 通常ブラスター
    /// </summary>
    [SerializeField] GameObject m_normalBlasterPrefab = default;


    public int rote;
    public float blastWaitTime;
    public float magnification;
    GameObject _player;
    public int movedirection = 0;
    public int bulletNumber;

    [SerializeField] AudioClip _heal;
    AudioSource _AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        _AudioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("Sana.Airsky_Sorceress");
        _gameManager = _player.GetComponent<GameManager>();
        //playerController = _player.GetComponent<PlayerController>();
        StartCoroutine("Test");
    }

/*    IEnumerator Test()
    {
        f_place = new ClossBlaster.StartInfo(-3,-3, 0);
        m_place = new ClossBlaster.MoveInfo(0,0, 0);
        blasterSize = 1.2f;
        beamWait = 1;
        beamTime = 1;
        Instantiate(m_clossBlasterPrefab);
        yield return new WaitForEndOfFrame();
        f_place = new ClossBlaster.StartInfo(3,3, 0);
        m_place = new ClossBlaster.MoveInfo(0,0, 0);
        blasterSize = 1.2f;
        beamWait = 1;
        beamTime = 1;
        Instantiate(m_clossBlasterPrefab);
        yield return new WaitForSeconds(1f);
        int gb_di = 0;

        for (int i = 0; i < 60; i++) 
        {
            Vector3 rotatevector3 = AngleToVector2(gb_di); 
            f_place = new ClossBlaster.StartInfo(rotatevector3 * -10, gb_di);
            m_place = new ClossBlaster.MoveInfo(rotatevector3 * -10, gb_di);
            blasterSize = 0.7f;
            beamWait = 0;
            beamTime = 0;
            Instantiate(m_normalBlasterPrefab);
            yield return new WaitForSeconds(0.1f);
            gb_di += 14;
        }

    }*/

    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void BlasterTest()
    {
        StartCoroutine("Test");
    }

    public static Vector3 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }
}
