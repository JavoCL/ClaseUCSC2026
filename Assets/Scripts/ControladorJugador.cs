using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControladorJugador : MonoBehaviour
{
    public float saludBase = 100f;
    [SerializeField]
    private float _saludActual;
    public float saludActual
    {
        get { return _saludActual; }
        set
        {
            if(value != _saludActual)
            {
                _saludActual = value;
                alActualizarSaludActual.Invoke(_saludActual);
            }
        }
    }

    public InputActionReference inputAtaque;
    [SerializeField]
    private DemoArma _armaActual;
    public DemoArma armaActual
    {
        get{return _armaActual;}
        set
        {
            if(value != _armaActual)
            {
                _armaActual = value;
            }
        }
    }

    public UnityEvent<float> alActualizarSaludActual;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        saludActual += Time.deltaTime;
    }

    void OnEnable()
    {
        inputAtaque.action.started += Atacar;
    }

    void OnDisable()
    {
        inputAtaque.action.started -= Atacar;
    }

    public void ActivaArma()
    {
        if(armaActual == null) return;

        armaActual.SetDanioActivo(true);
    }

    public void DesactivaArma()
    {
        if(armaActual == null) return;

        armaActual.SetDanioActivo(false);
    }

    private void Atacar(InputAction.CallbackContext context)
    {
        this.gameObject.GetComponent<Animator>().SetTrigger("ataque");
    }

    public void MensajeSalud()
    {
        Debug.Log("Mi salud actual es: " + saludActual);
    }

    public void Metodo(Collider col)
    {
        // Calculando daño desde col
    }
}
