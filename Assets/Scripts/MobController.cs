using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobController : MonoBehaviour
{
    public NavMeshAgent agente;
    public Transform target;
    public Animator animatorMob;
    public float velocidad = 0f;
    public float distancia = 0f;
    public bool patrullarEnStart = false;

    // Lista de posiciones donde enviar rutinariamente al mob
    public List<Transform> posPatrulla;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //agente.SetDestination(target.position);
        //agente.speed = 6f;

        if(patrullarEnStart == true)
            Patrullar();
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            distancia = (target.position - agente.transform.position).magnitude;
        }
        velocidad = agente.velocity.magnitude;
        animatorMob.SetFloat("Speed", velocidad);
    }

    public void Patrullar()
    {
        if(posPatrulla == null) return;
        if(posPatrulla.Count == 0) return;

        StartCoroutine(_Patrullar());
    }

    IEnumerator _Patrullar()
    {
        yield return null;

        foreach(Transform t in posPatrulla)
        {
            target = t;
            yield return new WaitForSeconds(2f);
            agente.SetDestination(target.position);
            yield return new WaitWhile(()=> distancia > 1f);
        }

        target = posPatrulla[0];
        yield return new WaitForSeconds(2f);
        agente.SetDestination(target.position);
        yield return new WaitWhile(() => distancia > 1f);
    }

    public void SetTarget(Transform nuevoTarget)
    {
        target = nuevoTarget;
    }
}
