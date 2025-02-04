using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Enemy : MonoBehaviour
{

    [SerializeField] float _enemyLife = 20;
    Rigidbody rb;
    MeshRenderer mesh;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDamage_Heal(int change_HP)
    {
        _enemyLife += change_HP;
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
}
