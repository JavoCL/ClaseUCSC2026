using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class DemoItem : MonoBehaviour
{
    public string nombre;
    [TextArea(1,5)]
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

    //
    public IAudioGenerator.Serializable audioItem;
    public Eventos eventos;

    // METODOS
    public void ActivarItem() { estaActivo = true; }
    public void DesactivarItem() { estaActivo = false; }

    public void AsignarAudioItem()
    {
        
    }

    // EVENTOS
    [System.Serializable]
    public struct Eventos
    {
        public UnityEvent alActivarItem;
        public UnityEvent alDesactivarItem;
    }
}
