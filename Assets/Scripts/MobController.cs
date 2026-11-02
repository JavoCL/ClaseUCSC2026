using UnityEngine;
using UnityEngine.AI;

public class MobController : MonoBehaviour
{
    public NavMeshAgent agente;
    public Transform target;
    public Animator animatorMob;
    public float velocidad = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        velocidad = agente.velocity.magnitude;

        animatorMob.SetFloat("Speed", velocidad);
    }
}
