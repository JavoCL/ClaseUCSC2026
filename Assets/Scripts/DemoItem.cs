using UnityEngine;
using UnityEngine.Events;

public class DemoItem : MonoBehaviour
{
    public string nombre;
    public string descripcion;
    
    [SerializeField]
    private bool _estaActivo;
    public bool estaActivo
    {
        get{return _estaActivo;}
        set
        {
            if(value != _estaActivo)
            {
                _estaActivo = value;

                if(_estaActivo == true)
                    eventos.alActivarItem.Invoke();
                else
                    eventos.alDesactivarItem.Invoke();
            }
        }
    }
    
    public Eventos eventos;

    // METODOS
    public void ActivarItem() { estaActivo = true; }
    public void DesactivarItem() { estaActivo = false; }


    // EVENTOS
    [System.Serializable]
    public struct Eventos
    {
        public UnityEvent alActivarItem;
        public UnityEvent alDesactivarItem;
    }
}
