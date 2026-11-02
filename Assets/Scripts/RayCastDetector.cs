using UnityEngine;
using UnityEngine.Events;

public class RayCastDetector : MonoBehaviour
{
    public Transform origen;
    public float distanciaRayo = 10f;
    public LayerMask layerDeteccion;
    
    [SerializeField]
    private Transform _objetoDetectado;
    public Transform objetoDetectado
    {
        get{return _objetoDetectado;}
        set
        {
            if(value != null && value != _objetoDetectado)
            {
                _objetoDetectado = value;

                alDetectarObjeto.Invoke(_objetoDetectado);
            }
        }
    }
     
    public Transform ultimoDetectado;

    public UnityEvent<Transform> alDetectarObjeto;

    // Update is called once per frame
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
