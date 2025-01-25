using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class KnifeSpawn : MonoBehaviour
{

    [SerializeField] UnityEvent _actions;
    GameManager _gameManager;
    public int damage; //���Ɏg���ĂȂ�

    float y;
    float x;
    
    //�u���X�^�[��p�̏��
    /// <summary>�u���X�^�[�̏����ʒu</summary>
    //public ClossBlaster.StartInfo f_place;
    /// <summary>�u���X�^�[�̈ړ����</summary>
    //public ClossBlaster.MoveInfo m_place;
    /// <summary>�u���X�^�[�̃T�C�Y</summary>
    public float blasterSize;
    /// <summary>�u���X�^�[���˂܂ł̒x������</summary>
    public int beamWait;
    /// <summary>�u���X�^�[�̔��ˊ���(���[�v��)</summary>
    public int beamTime;
    /// <summary>�u���X�^�[�̐F�̎w��</summary>
    //public ClossBlaster.BlasterColor blasterColor;


    /// <summary>
    /// ���i����i�C�t�B
    /// </summary>
    [SerializeField] GameObject m_spawnPrefab = default;
    /// <summary>
    /// �i�s�������w��ł���i�C�t�B(�㉺���E)
    /// </summary>
    [SerializeField] GameObject m_spawn2Prefab = default;
    /// <summary>
    /// �y���e�B
    /// </summary>
    [SerializeField] GameObject m_spawn3Prefab = default;
    /// <summary>
    /// �N���X�u���X�^�[
    /// </summary>
    [SerializeField] GameObject m_clossBlasterPrefab = default;
    /// <summary>
    /// �ʏ�u���X�^�[
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
