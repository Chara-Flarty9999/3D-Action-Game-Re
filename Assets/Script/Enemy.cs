using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("�^���b�g��p�̐ݒ�")]
    /// <summary>
    /// �ˏo����I�u�W�F�N�g
    /// </summary>
    [SerializeField, Tooltip("�ˏo����I�u�W�F�N�g�������Ɋ��蓖�Ă�(Normal�ȊO)")]
    private GameObject throwingObject;

    /// <summary>
    /// �W�I�̃I�u�W�F�N�g
    /// </summary>
    [SerializeField, Tooltip("�W�I�̃I�u�W�F�N�g�������Ɋ��蓖�Ă�(Normal�ȊO)")]
    private GameObject targetObject;

    /// <summary>
    /// �ˏo�p�x
    /// </summary>
    [SerializeField, Range(0F, 90F), Tooltip("�ˏo����p�x(Turret�̂�)")]
    private float ThrowingAngle;

    GameObject _turretBase;

    [Header("���ʐݒ�")]
    [SerializeField] float _maxEenemyLife = 20;
    [SerializeField] EnemyType _enemyType;
    float _enemyLife;
    Rigidbody rb;
    MeshRenderer mesh;
    SpriteRenderer spriteRenderer;
    GameObject enemyLifeGage;
    GameObject enemyCanvas;
    Image gage_image;


    // Start is called before the first frame update
    void Start()
    {
        _enemyLife = _maxEenemyLife;
        enemyLifeGage = transform.Find("Canvas/EnemyLifeGageRoot/EnemyLifeGage").gameObject;
        _turretBase = transform.Find("TurretBase").gameObject;
        enemyCanvas = transform.GetChild(0).gameObject;
        gage_image = enemyLifeGage.GetComponent<Image>();
        rb = GetComponent<Rigidbody>();
        enemyCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_enemyType)
        {
            case EnemyType.Turret:
                Vector3 playerPos = targetObject.transform.position;
                playerPos.y = transform.position.y;
                _turretBase.transform.LookAt(playerPos);
                break;
        }
    }

    private void FixedUpdate()
    {
        
    }

    public void DealDamage_Heal(int change_HP)
    {
        enemyCanvas?.SetActive(true);
        _enemyLife += change_HP;
        gage_image.fillAmount = _enemyLife / _maxEenemyLife;
        //mesh.material.color = change_HP >= 0 ? new Color(0, 1, 0, 1) : new Color(1, 0, 0, 1);
        if (_enemyLife <= 0)
        {
            Destroy(gameObject, 1f);
        }
        else
        {
            //mesh.material.DOColor(new Color(1, 1, 1), 0.8f);
        }
    }

    /// <summary>
    /// �{�[�����ˏo����
    /// </summary>
    private void ThrowingBall()
    {
        if (throwingObject != null && targetObject != null && _enemyType == EnemyType.Turret)
        {
            // Ball�I�u�W�F�N�g�̐���
            GameObject ball = Instantiate(throwingObject, this.transform.position, Quaternion.identity);

            // �W�I�̍��W
            Vector3 targetPosition = targetObject.transform.position;

            // �ˏo�p�x
            float angle = ThrowingAngle;

            // �ˏo���x���Z�o
            Vector3 velocity = CalculateVelocity(this.transform.position, targetPosition, angle);

            // �ˏo
            Rigidbody rid = ball.GetComponent<Rigidbody>();
            rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
        }
        else
        {
            if (_enemyType == EnemyType.Turret)
            {
                throw new System.Exception("�^���b�g�ł͂Ȃ����ߔ��˂ł��܂���B");
            }
            else
            {
                throw new System.Exception("�ˏo����I�u�W�F�N�g�܂��͕W�I�̃I�u�W�F�N�g�����ݒ�ł��B");
            }
        }
    }

    /// <summary>
    /// �W�I�ɖ�������ˏo���x�̌v�Z
    /// </summary>
    /// <param name="pointA">�ˏo�J�n���W</param>
    /// <param name="pointB">�W�I�̍��W</param>
    /// <returns>�ˏo���x</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // �ˏo�p�����W�A���ɕϊ�
        float rad = angle * Mathf.PI / 180;

        // ���������̋���x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // ���������̋���y
        float y = pointA.y - pointB.y;

        // �Ε����˂̌����������x�ɂ��ĉ���
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // �����𖞂����������Z�o�ł��Ȃ����Vector3.zero��Ԃ�
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    private void OnDestroy()
    {
        GameManager.leftEnemyBox--;
    }

    enum EnemyType
    {
        Object,
        Turret,
        Wander,
        chase
    }
}
