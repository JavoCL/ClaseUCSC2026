using UnityEngine;

public class DemoControladorObjetoActual : MonoBehaviour
{
    public Item itemActual;

    public class Item
    {
        public string nombre;
        public int cantidad;
    }

    public class Arma : Item
    {
        public DemoArma arma;
    }
}
