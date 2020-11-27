using System;
using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_ {

    public class Memoria {
        public List<Pagina> memoriaReal;
        public int paginasLibres;
        public int paginasLibresSwap;
        public List<Pagina> memoriaSwap;
        //Lista de procesos en memoria real

        public Memoria() {
            this.paginasLibres = 128;
            this.paginasLibresSwap = 256;

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
        // Porque regresa el idProceso
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

        public int GetPaginasSwapLibres() {
            return paginasLibresSwap;
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

        public void SwapOut(double numPaginasNecesarias, Proceso proceso) {
            List<int> marcosAsignados = new List<int>();
            int posicionSwap = -1; // #pagina swap a la que se swappeo

            // ESTRATEGIA FIFO
            if(Globales.estrategia == Estrategia.FIFO) {
                if (this.paginasLibres != 0) {
                    // Llenar las paginas aun disponibles en la memoria real
                    foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                        if(this.paginasLibres == 0) break; 

                        // Si la pagina esta disponble, asignar
                        if(pagina.idProceso == -1) {
                            pagina.idProceso = proceso.id;
                            numPaginasNecesarias--;
                            this.paginasLibres--;
                            marcosAsignados.Add(pagina.marco);
                        }
                    }
                } else {
                    // Hacer swapout para las paginas restantes
                    foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                        int idProcesoSwapOut = Globales.filaProcesos.Peek();
                        Proceso procesoSwapOut = Globales.procesos[idProcesoSwapOut];
                        if (numPaginasNecesarias <= 0) break;
                        if(pagina.idProceso == idProcesoSwapOut) {
                            SwapIn(procesoSwapOut, ref posicionSwap);
                            pagina.idProceso = proceso.id;
                            numPaginasNecesarias--;
                            marcosAsignados.Add(pagina.marco);
                            Console.WriteLine($"página #{pagina.marco} del proceso #{idProcesoSwapOut} swappeada al marco #{posicionSwap} del área de swapping");
                        }
                    }
                }
            }

            // ESTRATEGIA LRU
            else {

            }

            // Desplegar marcos utilizados para el proceso
            string arregloMarcos = "";
            foreach(int n in marcosAsignados) {
                arregloMarcos += n.ToString() + ",";
            }
            Console.WriteLine($"Se asignaron los marcos de página [{arregloMarcos.Remove(arregloMarcos.Length - 1)}] al proceso #{proceso.id}");

        }

        public void SwapIn(Proceso proceso, ref int posicionSwap) {
            foreach(Pagina pagina in Globales.memoria.memoriaSwap) {                
                // Si la pagina esta disponble, asignar
                if (this.paginasLibresSwap <= 0) {
                    Console.WriteLine("No hay espacio en memoriaSwap");
                    break;
                }
                if(pagina.idProceso == -1) {
                    pagina.idProceso = proceso.id;
                    this.paginasLibresSwap--;
                    posicionSwap = pagina.marco;
                    break;
                }

            }
        }

    }
}