using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ControladorLugarObjetivo : MonoBehaviour
{
    [Header("Configuracion comportamiento")]
    public bool empiezaStart = false;
    private bool _llegoPlayer;
    public bool llegoPlayer
    {
        get { return _llegoPlayer; }
        set
        {
            if(value != _llegoPlayer)
            {
                _llegoPlayer = value;

                if(_llegoPlayer == false)
                {
                    if(tipoObjetivo == TipoObjetivo.repetible)
                    {
                        StartCoroutine(_EsperarLlegadaJugador());
                    }

                    alSalirJugador.Invoke();
                }
            }
        }
    }
    
    public TipoObjetivo tipoObjetivo = TipoObjetivo.unaEjecucion;

    [Header("Configuracion Eventos")]
    public UnityEvent alLlegarJugador;
    public UnityEvent alSalirJugador;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(empiezaStart == true)
        {
            StartCoroutine(_EsperarLlegadaJugador());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(llegoPlayer != true)
            {
                llegoPlayer = true;

                // GENERAMOS X CODIGO PARA CURAR, HABILITAR PUERTA, INTERACCION, ETC
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(tipoObjetivo == TipoObjetivo.repetible)
        {
            if (other.gameObject.tag == "Player")
            {
                llegoPlayer = false;
            }
        }
    }

    IEnumerator _EsperarLlegadaJugador()
    {
        Debug.Log("Comenzamos a esperar al player 🕛");
        yield return new WaitUntil(()=> llegoPlayer == true);
        //yield return new WaitWhile(()=> llegoPlayer == false);
        Debug.Log("Player llegó al objetivo 🏁");
        alLlegarJugador.Invoke();
    }

    public enum TipoObjetivo {unaEjecucion, repetible};
}
