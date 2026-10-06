using UnityEngine;
using UnityEngine.Events;

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

    public void MensajeSalud()
    {
        Debug.Log("Mi salud actual es: " + saludActual);
    }

    public void Metodo(Collider col)
    {
        // Calculando daño desde col
    }
}
