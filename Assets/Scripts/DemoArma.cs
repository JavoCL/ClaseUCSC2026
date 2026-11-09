using Unity.VisualScripting;
using UnityEngine;

public class DemoArma : MonoBehaviour
{
    [Header("Estadisticas arma")]
    public Tipo tipo = Tipo.espada_corta;
    public float danio = 10f;
    public bool danioActivo = false;
    // Objetivo del golpe del arma
    public DemoObjetivo objetivo;

    void OnTriggerEnter(Collider other)
    {
        // Si no esta activo el daño del arma, termino la deteccion
        if(danioActivo == false) return;

        // Si esta activo, prosigo.
        // Si el elemento que golpee tiene un componente de Objetivo..
        if (other.gameObject.GetComponent<DemoObjetivo>() != null)
        {
            // ... asigno al objetivo el elemento que golpee
            objetivo = other.gameObject.GetComponent<DemoObjetivo>();
            // Le aviso que lo golpié
            objetivo.esGolpeado = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Si no esta activo el daño del arma, termino la deteccion
        if(danioActivo == false) return;

        // Si esta activo, prosigo.
        // Si el elemento que golpee tiene un componente de Objetivo..
        if (other.gameObject.GetComponent<DemoObjetivo>() != null)
        {
            // ... Le aviso que lo "terminó" el golpe
            objetivo.esGolpeado = false;
            // Y luego desasigno al objetivo el elemento que golpee
            objetivo = null;
        }
    }

    public void SetDanioActivo(bool valor) { danioActivo = valor; }


    public enum Tipo{espada_corta, espada_larga, lanza, mazo, hacha1Mano, hacha2Manos}
}
