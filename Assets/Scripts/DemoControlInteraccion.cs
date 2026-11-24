using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DemoControlInteraccion : MonoBehaviour
{
    public InputActionReference inputInteraccion;
    public Eventos eventos;

    void OnEnable()
    {
        inputInteraccion.action.started += Interactuar;
    }

    void OnDisable()
    {
        inputInteraccion.action.started -= Interactuar;
    }

    private void Interactuar(InputAction.CallbackContext context)
    {
        Debug.Log("INTERACCION");
        eventos.alInteractuar.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != null)
        {
            eventos.alEntrarDeteccion.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject != null)
        {
            eventos.alSalirDeteccion.Invoke();
        }
    }

    [System.Serializable]
    public struct Eventos
    {
        public UnityEvent alEntrarDeteccion;
        public UnityEvent alSalirDeteccion;
        public UnityEvent alInteractuar;
    }
}
