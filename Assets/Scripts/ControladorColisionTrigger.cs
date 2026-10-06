using UnityEngine;
using UnityEngine.Events;

public class ControladorColisionTrigger : MonoBehaviour
{
    public string tagEsperado = "";
    public UnityEvent<Collider> onTriggerEnterEvent;
    public UnityEvent onTriggerStayEvent;
    public UnityEvent onTriggerExitEvent;


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == tagEsperado)
            onTriggerEnterEvent.Invoke(other);
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == tagEsperado)
            onTriggerStayEvent.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == tagEsperado)
            onTriggerExitEvent.Invoke();
    }

    public void Mensaje(string mensaje)
    {
        Debug.Log(mensaje);
    }
}
