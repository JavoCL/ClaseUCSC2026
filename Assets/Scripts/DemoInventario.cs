using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DemoInventario : MonoBehaviour
{
    [SerializeField]
    private DemoItem _itemActual;
    public DemoItem itemActual
    {
        get{return _itemActual;}
        set
        {
            if(value != _itemActual)
            {
                _itemActual = value;

                eventos.alCambiarItemActual.Invoke(_itemActual);
            }
        }
    }

    public List<DemoItem> listaInventario;

    public Eventos eventos;

    void Start()
    {
        if(listaInventario != null)
            if(listaInventario.Count > 0)
                itemActual = listaInventario[0];
    }

    public void ActivarItemActual()
    {
        if(itemActual != null)
            itemActual.ActivarItem();
    }

    public void DesactivarItemActual()
    {
        if(itemActual != null)
            itemActual.DesactivarItem();
    }

    // DEFINICION DE DATOS

    [System.Serializable]
    public struct Eventos
    {
        public UnityEvent<DemoItem> alCambiarItemActual;
    }
}
