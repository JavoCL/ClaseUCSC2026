using UnityEngine;
using UnityEngine.Events;

public class RayCastDetector : MonoBehaviour
{
    // Origen del raycast detector
    public Transform origen;
    // Que tan largo es el rayo de deteccion
    public float distanciaRayo = 10f;
    // Cual es la capa donde va a detectar objetos
    public LayerMask layerDeteccion;
    
    // Propiedad de objeto detectado
    // Si cambia cual fue el objeto, invoco el evento
    // con la referencia de ese nuevo objeto
    [SerializeField]
    private Transform _objetoDetectado;
    public Transform objetoDetectado
    {
        get{return _objetoDetectado;}
        set
        {
            // Si detecto un objetivo y es distinto que el anterior...
            if(value != null && value != _objetoDetectado)
            {
                // ... realizo la asignacion
                _objetoDetectado = value;
                // ... e invoco el evento pasando el nuevo objetivo como parametro
                // a quien se suscriba
                alDetectarObjeto.Invoke(_objetoDetectado);
            }
        }
    }
     
     // Objetivo anterior detectado
    public Transform ultimoDetectado;

    public UnityEvent<Transform> alDetectarObjeto;

    void Update()
    {
        Vector3 posOrigen = origen.position;
        Vector3 direccion = origen.forward;

        RaycastHit hit;
        bool detectoCol = false;

        detectoCol = Physics.Raycast(posOrigen, direccion, out hit, distanciaRayo, layerDeteccion);

        Color colorRayo = detectoCol == true ? Color.red : Color.green;
        Debug.DrawRay(posOrigen,direccion*distanciaRayo,colorRayo);

        if(detectoCol == true)
        {
            Debug.Log("Estoy mirando algo: " + hit.transform.gameObject.name);
            objetoDetectado = hit.transform;
            ultimoDetectado = objetoDetectado;
        }
        else objetoDetectado = null;
    }

    public void Log(string mensaje)
    {
        Debug.Log(mensaje);
    }
}
