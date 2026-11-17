using System.Collections.Generic;
using UnityEngine;

public class DemoInventario : MonoBehaviour
{
    public DemoItem itemActual;

    public List<DemoItem> listaInventario;

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
}
