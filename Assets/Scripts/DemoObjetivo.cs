using UnityEngine;
using UnityEngine.Events;

public class DemoObjetivo : MonoBehaviour
{
    [SerializeField]
    private bool _esGolpeado;
    public bool esGolpeado
    {
        get{return _esGolpeado;}
        set
        {
            // Si hay un cambio en el valor...
            if(value != _esGolpeado)
            {
                // ... asigno el nuevo valor.
                _esGolpeado = value;

                // En el caso de que se haga daño, invoco el evento
                if(_esGolpeado == true)
                    alSerGolpeado.Invoke(danioRecibido);
                Debug.Log("DAÑO RECIBIDO: " + danioRecibido);
            }
        }
    }
    
    public float danioRecibido = 0f;
    public UnityEvent<float> alSerGolpeado;

    public void Log(string mensaje)
    {
        Debug.Log(mensaje);
    }
}
