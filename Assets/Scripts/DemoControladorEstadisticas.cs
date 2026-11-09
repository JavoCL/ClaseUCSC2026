using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DemoControladorEstadisticas : MonoBehaviour
{
    [SerializeField]
    private bool _estaVivo;
    public bool estaVivo
    {
        get{return _estaVivo;}
        set
        {
            if(value != _estaVivo)
            {
                _estaVivo = value;

                // Si se murió, invoca el evento asociado
                if(_estaVivo == false)
                {
                    eventos.alMorir.Invoke();
                }
            }
        }
    }
    public Estadisticas estadisticasBase;
    public Estadisticas estadisticasActuales;

    [SerializeField]
    private float _saludActEspejo;
    public float saludActEspejo
    {
        get{return _saludActEspejo;}
        set
        {
            if(value != _saludActEspejo)
            {
                _saludActEspejo = value;

                eventos.alCambiarSalud.Invoke(_saludActEspejo);
            }
        }
    }

    [Header("Configuracion Eventos")]
    public Eventos eventos;

    void Start()
    {
        estadisticasActuales = estadisticasBase;
        estaVivo = (estadisticasActuales.salud > 0f);

        eventos.alComenzar.Invoke();
    }

    void Update()
    {
        saludActEspejo = estadisticasActuales.salud;

        if(estaVivo == false) return;
    }

    // METODOS
    public void CalcularDanio(float danioEntrante)
    {
        if(danioEntrante <= 0f) return;

        estadisticasActuales.salud -= danioEntrante;

        if(estadisticasActuales.salud <= 0f)
        {
            estaVivo = false;
            estadisticasActuales.salud = 0f;
        }
    }

    public void DestruirConDelay(float delay = 0f)
    {
        StartCoroutine(_DestruirConDelay(delay));
    }

    IEnumerator _DestruirConDelay(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        GameObject.Destroy(this.gameObject);
    }

    // DEFINICION DATOS
    [System.Serializable]
    public struct Estadisticas
    {
        public float salud;
        public float ataque;
        public float defensa;
        public float resistencia;
        public float velocidad;
    }

    [System.Serializable]
    public struct Eventos
    {
        public UnityEvent alComenzar;
        public UnityEvent<float> alCambiarSalud;
        public UnityEvent alMorir;
    }
}
