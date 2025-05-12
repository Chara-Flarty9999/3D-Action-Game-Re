using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("タレット専用の設定")]
    /// <summary>
    /// 射出するオブジェクト
    /// </summary>
    [SerializeField, Tooltip("射出するオブジェクトをここに割り当てる(Normal以外)")]
    private GameObject throwingObject;

    /// <summary>
    /// 標的のオブジェクト
    /// </summary>
    [SerializeField, Tooltip("標的のオブジェクトをここに割り当てる(Normal以外)")]
    private GameObject targetObject;

    /// <summary>
    /// 射出角度
    /// </summary>
    [SerializeField, Range(0F, 1F), Tooltip("射出する角度(Turretのみ)")]
    private float throwingAngle;

    /// <summary>
    /// 射出する座標のオブジェクト
    /// </summary>
    [SerializeField, Tooltip("射出する座標(Turretのみ)")]
    private Transform turretMuzzle;

    /// <summary>
    /// 射出する座標のオブジェクト
    /// </summary>
    [SerializeField, Tooltip("弾を発射する間隔")]
    private int turretBulletRate;

    int _turretRateTime = 0;
    GameObject _turretBase;
    GameObject _turretBarrel;
    float projectileSpeed = 20f;

    [Header("共通設定")]
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
                    // barrelのforwardをfiringDirectionに近づける（X軸のみ変化）
                    Quaternion aimRotation = Quaternion.LookRotation(firingDirection);
                    Vector3 euler = aimRotation.eulerAngles;

                    // 回転制限: X軸だけ、YとZを固定
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
    /// 自身から対象への距離を測定し、一定の速度で弾が対象に届く向きベクトルを検索する。
    /// </summary>
    /// <param name="origin">射出する場所</param>
    /// <param name="target">設定された標的</param>
    /// <param name="speed">射出速度</param>
    /// <param name="velocity">設定された射出速度で弾が対象に届くような向きベクトル</param>
    /// <returns>対象に届くか否かをBool型で返す。ついでにベクトルを出力する。</returns>
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
            return false; // 到達不能
        }

        float sqrt = Mathf.Sqrt(underSqrt);
        float lowAngle = Mathf.Atan2(v2 - sqrt, g * xz); // 低い弾道
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
    /// ボールを射出する
    /// </summary>
    private void ThrowingBall()
    {
        if (throwingObject != null && targetObject != null && _enemyType == EnemyType.Turret)
        {
            // Ballオブジェクトの生成
            GameObject ball = Instantiate(throwingObject, turretMuzzle.position, Quaternion.identity);

            // 標的の座標
            Vector3 targetPosition = targetObject.transform.position;

            // 射出角度
            float angle = throwingAngle;

            // 射出速度を算出
            Vector3 velocity = CalculateVelocity(this.transform.position, targetPosition, angle);

            // 射出
            Rigidbody rid = ball.GetComponent<Rigidbody>();
            rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
        }
        else
        {
            if (_enemyType == EnemyType.Turret)
            {
                throw new System.Exception("タレットではないため発射できません。");
            }
            else
            {
                throw new System.Exception("射出するオブジェクトまたは標的のオブジェクトが未設定です。");
            }
        }
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    /// </summary>
    /// <param name="pointA">射出開始座標</param>
    /// <param name="pointB">標的の座標</param>
    /// <returns>射出速度</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
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
    /// 敵の状態(敵種類)
    /// </summary>
    enum EnemyType
    {
        Object,
        Turret,
        Wander,
        chase
    }
}
