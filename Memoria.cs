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
            int bytesProceso = proceso.tam;

            // Buscar espacios disponibles y asignarlos
            foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                if(numPagNecesarias <= 0) break; // Todas las paginas fueron asignadas
                
                // Si la pagina esta disponible, asignar
                if(pagina.idProceso == -1) {
                    pagina.idProceso = proceso.id;
                    numPagNecesarias--;
                    this.paginasLibres--;
                    marcosAsignados.Add(pagina.marco);
                    if (bytesProceso >= 16) {
                        pagina.bytesUsados = 16;
                        bytesProceso = bytesProceso - 16;
                    } else {
                        pagina.bytesUsados = bytesProceso;
                        bytesProceso = bytesProceso - bytesProceso;
                    }
                }
            }
            // Modificar timestamp
            Globales.timestamp = Globales.timestamp + 1.0;

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
            int bytesProceso = proceso.tam;

            if (this.paginasLibres != 0) {
                // Llenar las paginas aun disponibles en la memoria real
                foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                    if(this.paginasLibres == 0) break; 

                    // Si la pagina esta disponible, asignar
                    if(pagina.idProceso == -1) {
                        pagina.idProceso = proceso.id;
                        numPaginasNecesarias--;
                        this.paginasLibres--;
                        marcosAsignados.Add(pagina.marco);
                        if (bytesProceso >= 16) {
                            pagina.bytesUsados = 16;
                            bytesProceso = bytesProceso - 16;
                        } else {
                            pagina.bytesUsados = bytesProceso;
                            bytesProceso = bytesProceso - bytesProceso;
                        }
                    }
    
                }
            } else {
                // Hacer swapout para las paginas restantes
                int idProcesoSwapOut = -1;
                foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                    // FIFO
                    if(Globales.estrategia == Estrategia.FIFO) {
                        idProcesoSwapOut = Globales.filaProcesos.Peek();
                    } 
                    // LRU
                    else {
                        idProcesoSwapOut = Globales.stackProcesos.Peek();
                    }
                    Proceso procesoSwapOut = Globales.procesos[idProcesoSwapOut]; // Obtener los datos del proceso
                    if (numPaginasNecesarias <= 0) break;
                    if(pagina.idProceso == idProcesoSwapOut) {
                        SwapIn(procesoSwapOut, ref posicionSwap); // Enviar el proceso que se ingresara a la memoria swap
                        pagina.idProceso = proceso.id;
                        numPaginasNecesarias--;
                        marcosAsignados.Add(pagina.marco);
                        if (bytesProceso >= 16) {
                            pagina.bytesUsados = 16;
                            bytesProceso = bytesProceso - 16;
                        } else {
                            pagina.bytesUsados = bytesProceso;
                            bytesProceso = bytesProceso - bytesProceso;
                        }
                        Console.WriteLine($"página #{pagina.marco} del proceso #{idProcesoSwapOut} swappeada al marco #{posicionSwap} del área de swapping");
                    }
                }
            }
            // Modificar contador de swaps
            Globales.contadorSwaps++;

            // Modificar timestamp
            Globales.timestamp += 1.0;

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

        public void liberarProceso(Proceso proceso) {
            bool procesoEncontrado = false;
            double numPagNecesarias = proceso.tam / Globales.TAM_PAGINA;
            numPagNecesarias = Math.Ceiling(numPagNecesarias);
            List<int> marcosLiberadosReal = new List<int>();
            List<int> marcosLiberadosSwap = new List<int>();

            foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                if(numPagNecesarias <= 0) break;
                if(pagina.idProceso == proceso.id) {
                    procesoEncontrado = true;
                    pagina.idProceso = -1;
                    pagina.bytesUsados = 0;
                    this.paginasLibres++;
                    numPagNecesarias--;
                    marcosLiberadosReal.Add(pagina.marco);

                }
            }
            if (numPagNecesarias > 0) {
                foreach(Pagina paginaS in Globales.memoria.memoriaSwap) {
                    if(numPagNecesarias <= 0) break;
                    if(paginaS.idProceso == proceso.id) {
                        procesoEncontrado = true;
                        paginaS.idProceso = -1;
                        paginaS.bytesUsados = 0;
                        this.paginasLibresSwap++;
                        numPagNecesarias--;
                        marcosLiberadosSwap.Add(paginaS.marco);

                    }
                }
            }

            if (!procesoEncontrado) {
                Console.WriteLine($"El proceso #{proceso.id} no se encuentra en la memoria o ya fue liberado.");
            } else {
                Console.WriteLine($"Liberar los marcos de página ocupados por el proceso #{proceso.id}");

                // Desplegar marcos utilizados para el proceso
                string arregloMarcos = "";
                // Desplegar marcos de memoria real si hay
                foreach(int n in marcosLiberadosReal) {
                    arregloMarcos += n.ToString() + ",";
                }
                if (arregloMarcos != "") {
                    Console.WriteLine($"Se liberan los marcos de memoria real: [{arregloMarcos.Remove(arregloMarcos.Length - 1)}]");
                }
                // Desplegar marcos de memoria swapping si hay
                arregloMarcos = "";
                foreach(int n in marcosLiberadosSwap) {
                    arregloMarcos += n.ToString() + ",";
                }
                if (arregloMarcos != "") {
                    Console.WriteLine($"Se liberan los marcos del área de swapping: [{arregloMarcos.Remove(arregloMarcos.Length - 1)}]");
                }
            }   
        }
    }
}