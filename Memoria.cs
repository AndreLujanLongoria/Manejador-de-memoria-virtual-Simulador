using System;
using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_ {

    public class Memoria {
        public List<Pagina> memoriaReal;
        public int paginasLibres;
        public List<Pagina> memoriaSwap;

        public Memoria() {
            this.paginasLibres = 128;

            memoriaReal = new List<Pagina>(128);
            
            // Definir memoria y sus marcos
            for(int i = 0; i < 128; i++) {
                memoriaReal.Add(new Pagina(-1, 0, -1, i, 0));
            }

            memoriaSwap = new List<Pagina>(256);
            for(int i = 0; i < 256; i++) {
                memoriaSwap.Add(new Pagina(-1, 0, -1, i, 0));
            }
        }

        public int GetNumMarco(int idProceso) {

            foreach(Pagina p in memoriaReal) {
                if(p.idProceso == idProceso) {
                    return idProceso;
                }
            }
            return -1;
        }

        public Pagina GetPagina(int idMarco) {
            return memoriaReal[idMarco];
        }

        public int GetPaginasLibres() {
            return paginasLibres;
        }

        public void AsignarPaginas(Proceso proceso) {
            double numPagNecesarias = proceso.tam / Globales.TAM_PAGINA;
            numPagNecesarias = Math.Ceiling(numPagNecesarias);
            List<int> marcosAsignados = new List<int>();

            // Buscar espacios disponibles y asignarlos
            foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                if(numPagNecesarias <= 0) break; // Todas las paginas fueron asignadas
                
                // Si la pagina esta disponble, asignar
                if(pagina.idProceso == -1) {
                    pagina.idProceso = proceso.id;
                    numPagNecesarias--;
                    this.paginasLibres--;
                    marcosAsignados.Add(pagina.marco);
                }
            }

            // Desplegar marcos utilizados para el proceso
            string arregloMarcos = "";
            foreach(int n in marcosAsignados) {
                arregloMarcos += n.ToString() + ",";
            }
            Console.WriteLine($"Se asignaron los marcos de página [{arregloMarcos.Remove(arregloMarcos.Length - 1)}] al proceso #{proceso.id}");
        }

        public void SwapOut(int numPaginasNecesarias) {
            // ESTRATEGIA FIFO
            if(Globales.estrategia == Estrategia.FIFO) {
                while(numPaginasNecesarias > 0) {
                    int idProcesoSacar = Globales.filaProcesos.Peek();

                }

            } 
            // ESTRATEGIA LRU
            else {

            }
        }

    }
}