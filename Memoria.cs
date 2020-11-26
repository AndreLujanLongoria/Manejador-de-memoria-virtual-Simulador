using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_ {

    public class Memoria {
        public List<Pagina> memoriaReal;
        public int paginasLibres;
        public List<Pagina> memoriaSwap;

        public Memoria() {
            this.paginasLibres = 128;

            memoriaReal = new List<Pagina>(128);
            int numMarco = 0;
            
            // Definir memoria y sus marcos
            foreach(Pagina p in memoriaReal) {
                p.marco = numMarco;
                p.bitRes = 0;
                p.idProceso = -1;
                p.bytesUsados = 0;

                numMarco++;
            }

            numMarco = 0;
            memoriaSwap = new List<Pagina>(256);
            foreach(Pagina p in memoriaSwap) {
                p.marco = numMarco;
                p.bitRes = 0;
                p.idProceso = -1;
                p.bytesUsados = 0;

                numMarco++;
            }
        }

        public void asignarPagina(int numMarco, Proceso proceso) {

        }

    }
}