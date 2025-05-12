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
    [SerializeField, Range(0F, 1F), Tooltip("�ˏo����p�x(Turret�̂�)")]
    private float throwingAngle;

    /// <summary>
    /// �ˏo������W�̃I�u�W�F�N�g
    /// </summary>
    [SerializeField, Tooltip("�ˏo������W(Turret�̂�)")]
    private Transform turretMuzzle;

    /// <summary>
    /// �ˏo������W�̃I�u�W�F�N�g
    /// </summary>
    [SerializeField, Tooltip("�e�𔭎˂���Ԋu")]
    private int turretBulletRate;

    int _turretRateTime = 0;
    GameObject _turretBase;
    GameObject _turretBarrel;
    float projectileSpeed = 20f;

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
        if (_enemyType == EnemyType.Turret)
        {
            targetObject = GameObject.Find("Sana.Airsky_Sorceress");
            _turretBase = transform.Find("TurretBase").gameObject;
            _turretBarrel = transform.Find("TurretBase/TurretBarrelBase").gameObject;
        }
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
                Vector3 dirToTarget = targetObject.transform.position - _turretBase.transform.position;
                Vector3 flatDir = new Vector3(dirToTarget.x, 0f, dirToTarget.z);
                _turretBase.transform.rotation = Quaternion.LookRotation(flatDir);

                Vector3 firingDirection;

                //TryCalculateBallisticVelocity
                if (TryGetVelocity(_turretBarrel.transform.position, targetObject.transform.position, projectileSpeed, out firingDirection))
                {
                    // barrel��forward��firingDirection�ɋ߂Â���iX���̂ݕω��j
                    Quaternion aimRotation = Quaternion.LookRotation(firingDirection);
                    Vector3 euler = aimRotation.eulerAngles;

                    // ��]����: X�������AY��Z���Œ�
                    _turretBarrel.transform.localEulerAngles = new Vector3(euler.x*-1, 0, 0);

                    _turretRateTime++;
                    if (_turretRateTime == 1)
                    {
                        ThrowingBall();
                    }

                    if (_turretRateTime > turretBulletRate * 100)
                    {
                        _turretRateTime = 0;
                    }
                }


                /*_turretBase.transform.LookAt(targetObject.transform.position);
                float saveRotateY = _turretBase.transform.rotation.y;
                float saveRotateW = _turretBase.transform.rotation.w;
                _turretBase.transform.rotation = new Quaternion(throwingAngle, saveRotateY, 0, saveRotateW);*/
                break;
        }
    }

    private void FixedUpdate()
    {
        
    }
    /// <summary>
    /// ���g����Ώۂւ̋����𑪒肵�A���̑��x�Œe���Ώۂɓ͂������x�N�g������������B
    /// </summary>
    /// <param name="origin">�ˏo����ꏊ</param>
    /// <param name="target">�ݒ肳�ꂽ�W�I</param>
    /// <param name="speed">�ˏo���x</param>
    /// <param name="velocity">�ݒ肳�ꂽ�ˏo���x�Œe���Ώۂɓ͂��悤�Ȍ����x�N�g��</param>
    /// <returns>�Ώۂɓ͂����ۂ���Bool�^�ŕԂ��B���łɃx�N�g�����o�͂���B</returns>
    bool TryGetVelocity(Vector3 origin, Vector3 target, float speed, out Vector3 velocity)
    {
        
        
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float g = Physics.gravity.y;

        float v2 = speed * speed;
        float underSqrt = v2 * v2 - g * (g * xz * xz + 2 * y * v2);

        if (underSqrt < 0)
        {
            velocity = Vector3.zero;
            return false; // ���B�s�\
        }

        float sqrt = Mathf.Sqrt(underSqrt);
        float lowAngle = Mathf.Atan2(v2 - sqrt, g * xz); // �Ⴂ�e��
        float angle = lowAngle;

        // direction (XZ plane)
        Vector3 dir = toTargetXZ.normalized;

        velocity = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.Cross(dir, Vector3.up)) * dir * speed;
        return true;
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
            GameObject ball = Instantiate(throwingObject, turretMuzzle.position, Quaternion.identity);

            // �W�I�̍��W
            Vector3 targetPosition = targetObject.transform.position;

            // �ˏo�p�x
            float angle = throwingAngle;

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
    /// <summary>
    /// �G�̏��(�G���)
    /// </summary>
    enum EnemyType
    {
        Object,
        Turret,
        Wander,
        chase
    }
}
