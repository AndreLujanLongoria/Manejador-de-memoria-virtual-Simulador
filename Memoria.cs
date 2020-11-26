using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_ {

    public class Memoria {
        public List<Pagina> memoriaReal;
        public List<Pagina> memoriaSwap;

        public Memoria() {
            memoriaReal = new List<Pagina>(128);
            memoriaSwap = new List<Pagina>(256);
        }

    }
}