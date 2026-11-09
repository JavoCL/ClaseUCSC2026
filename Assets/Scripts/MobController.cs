using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MobController : MonoBehaviour
{
    public NavMeshAgent agente;
    public Comportamiento comportamiento = Comportamiento.APuntoObjetivo;
    // Propiedad de objetivo o punto a seguir
    [SerializeField]
    private Transform _target;
    public Transform target
    {
        get{return _target;}
        set
        {
            if(value != null && value != _target)
            {
                _target = value;

                alCambiarObjetivo.Invoke(_target);

                switch(comportamiento)
                {
                    case Comportamiento.APuntoObjetivo:
                        MandarAPuntoObjetivo();
                        break;
                    case Comportamiento.Seguir:
                        SeguirObjetivo();
                        break;
                    case Comportamiento.Patrullar:
                        Patrullar();
                        break;
                }
            }
        }
    }
    public Animator animatorMob;
    public float velocidad = 0f;
    public float distancia = 100f;
    public float toleranciaDist = 1f;
    public bool patrullarEnStart = false;
    [SerializeField]
    private bool _llegoDestino; 
    public bool llegoDestino
    {
        get{return _llegoDestino;}
        set
        {
            if(value != _llegoDestino)
            {
                _llegoDestino = value;

                if(_llegoDestino == true)
                    alLlegarDestino.Invoke(target);
            }
        }
    }

    // Lista de posiciones donde enviar rutinariamente al mob
    public List<Transform> posPatrulla;

    public UnityEvent<Transform> alCambiarObjetivo;
    public UnityEvent<Transform> alLlegarDestino;

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
        // if(target != null)
        // {
        //     distancia = (target.position - agente.transform.position).magnitude;
        //     Debug.Log("DISTANCIA: " + distancia);
        // }
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
            yield return new WaitWhile(()=> distancia > toleranciaDist);
        }

        target = posPatrulla[0];
        yield return new WaitForSeconds(2f);
        agente.SetDestination(target.position);
        yield return new WaitWhile(() => distancia > toleranciaDist);
    }

    IEnumerator _MandarAPuntoObjetivo()
    {
        agente.SetDestination(target.position);
        yield return new WaitWhile(()=> distancia > toleranciaDist);
        llegoDestino = true;
    }

    public void SetTarget(Transform nuevoTarget)
    {
        target = nuevoTarget;
    }

    public void ResetTarget(){target = null;}

    public void MandarAPuntoObjetivo()
    {
        CalcularDistanciaPuntoTarget();
        StartCoroutine(_MandarAPuntoObjetivo());
    }

    public bool calculandoDistancia = false;

    IEnumerator _CalcularDistanciaPuntoTarget()
    {
        if(target != null && agente != null)
        {
            Vector3 puntoTarget = target.position;

            calculandoDistancia = true;
            while (calculandoDistancia == true)
            {
                distancia = (puntoTarget - agente.transform.position).magnitude;
                yield return null;
            }
        }
        yield return null;
    }

    public void CalcularDistanciaPuntoTarget()
    {
        StartCoroutine(_CalcularDistanciaPuntoTarget());
    }

    public void DetenerCalculoDistancia()
    {
        calculandoDistancia = false;
    }

    public void SeguirObjetivo()
    {
        CalcularDistanciaTR();
        StartCoroutine(_SeguirObjetivo());
    }

    IEnumerator _SeguirObjetivo()
    {
        while(target != null)
        {
            if(distancia > toleranciaDist)
                agente.SetDestination(target.position);
            yield return null;
        }
    }

    public void CalcularDistanciaTR()
    {
        StartCoroutine(_CalcularDistanciaTR());
    }

    IEnumerator _CalcularDistanciaTR()
    {
        calculandoDistancia = true;

        while(calculandoDistancia == true)
        {
            distancia = (target.position - agente.transform.position).magnitude;
            yield return null;
        }
    }

    public enum Comportamiento {APuntoObjetivo, Seguir, Patrullar};
}
