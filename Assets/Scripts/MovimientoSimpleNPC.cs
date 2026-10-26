using UnityEngine;
using UnityEngine.AI;

public class MovimientoSimpleNPC : MonoBehaviour
{
    // Atributos
    // Agente a mover
    public NavMeshAgent agente;
    // Posicion destino donde moverlo
    public Vector3 posObjetivo = new Vector3(0f,0f,5f);
    // Objetivo destino en forma de Transform
    public Transform objetivo;


    void Start()
    {
        // Envio al agente a la posicion Vector3 objetivo
        // agente.SetDestination(posObjetivo);
        
        agente.SetDestination(objetivo.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        agente.SetDestination(objetivo.position);
    }
}
