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

        public int GetDirReal(int d, int p) {
            int numIndiceDeProceso = d / 16;
            int indice = 0;

            // Buscar en el arreglo de paginas si se encuentra la pagina con el bloque numero num del proceso p
            foreach(Pagina pagina in memoriaReal) {
                if(pagina.idProceso == p && pagina.num == numIndiceDeProceso) {
                    return indice * 16 + d; // Retornar la direccion real
                }
                indice++;
            }

            return -1;
        }

        public void AsignarPaginas(Proceso proceso) {
            double numPagNecesarias = (double)proceso.tam / Globales.TAM_PAGINA;
            numPagNecesarias = Math.Ceiling(numPagNecesarias);
            List<int> marcosAsignados = new List<int>();
            int bytesProceso = proceso.tam;
            int indiceDeProceso = 0;

            // Buscar espacios disponibles y asignarlos
            foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                if(numPagNecesarias <= 0) break; // Todas las paginas fueron asignadas
                
                // Si la pagina esta disponible, asignar
                if(pagina.idProceso == -1) {
                    // Marcar el subindice del pedazo del proceso que quedo en esta pagina
                    pagina.num = indiceDeProceso;
                    indiceDeProceso++;

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
            bool isProcessStillInMemory = true;
            double paginasALiberar = numPaginasNecesarias - this.paginasLibres;
            // Hacer swapout para las paginas que se necesiten
            int idProcesoSwapOut = -1;
            foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                if (paginasALiberar <= 0) break;
                // FIFO
                if(Globales.estrategia == Estrategia.FIFO) {
                    if (!isProcessStillInMemory) {
                        Globales.filaProcesos.Dequeue();
                    }
                    idProcesoSwapOut = Globales.filaProcesos.Peek();
                } 
                // LRU
                else {

                    if (!isProcessStillInMemory) {
                        Globales.lruProcesos.Remove(idProcesoSwapOut);
                    }
                    double max = -1;
                    foreach(KeyValuePair<int, double> time in Globales.lruProcesos) {
                        if (time.Value >= max) {
                            max = time.Value;
                            idProcesoSwapOut = time.Key;
                        }

                    }
                    //idProcesoSwapOut = Globales.stackProcesos.Peek();
                }
                Proceso procesoSwapOut = Globales.procesos[idProcesoSwapOut]; // Obtener los datos del proceso
                if(pagina.idProceso == idProcesoSwapOut) {
                    SwapOutProceso(procesoSwapOut, ref posicionSwap, pagina.num); // Enviar el proceso que se ingresara a la memoria swap CON INDICE DE PROCESO
                    pagina.idProceso = -1;
                    this.paginasLibres++;
                    paginasALiberar--;
                    Console.WriteLine($"Página #{pagina.marco} del proceso #{idProcesoSwapOut} swappeada al marco #{posicionSwap} del área de swapping");
                }
                foreach(Pagina pag in Globales.memoria.memoriaReal) {
                    if(pag.idProceso == idProcesoSwapOut) {
                        isProcessStillInMemory = true;
                        break;
                    } else {
                        isProcessStillInMemory = false;
                    }
                }
            }

            int indiceDeProceso = 0;
            // Llenar las paginas disponibles en la memoria real
            foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                if(numPaginasNecesarias <= 0) break;

                // Si la pagina esta disponible, asignar
                if(pagina.idProceso == -1) {
                    // Marcar el subindice del pedazo del proceso que se asigno a la pagina
                    pagina.num = indiceDeProceso;
                    indiceDeProceso++;

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

        public void SwapOutProceso(Proceso proceso, ref int posicionSwap, int numProceso) {
            foreach(Pagina pagina in Globales.memoria.memoriaSwap) {    

                // Si la pagina esta disponble, asignar
                if (this.paginasLibresSwap <= 0) {
                    Console.WriteLine("No hay espacio en memoria Swap");
                    break;
                }
                if(pagina.idProceso == -1) {
                    pagina.num = numProceso;

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
                    pagina.num = -1;
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

        public void swapUnaPagina() {

            // Identificar pagina a sacar
            int idProcesoSacar = -1;
            if(Globales.estrategia == Estrategia.FIFO) {
                //FIFO
                idProcesoSacar = Globales.filaProcesos.Peek();
            } else {
                //LRU
                double min = int.MaxValue;
                foreach(KeyValuePair<int, double> time in Globales.lruProcesos) {
                    if (time.Value <= min) {
                        min = time.Value;
                        idProcesoSacar = time.Key;
                    }
                }
            }

            // Identificar pagina libre en memoria Swap
            int idPaginaLibre = -1;
            foreach(Pagina pagina in memoriaSwap) {
                if(pagina.idProceso == -1) {
                    idPaginaLibre = pagina.marco;
                    break;
                }
            }
            
            // Mandar la pagina identificada al area de swap
            foreach(Pagina pagina in memoriaReal) {
                if(pagina.idProceso == idProcesoSacar) {
                    // Copiar pagina de memoriaReal a pagina en area de swap
                    Pagina paginaSwap = memoriaSwap[idPaginaLibre];
                    paginaSwap.idProceso = pagina.idProceso;
                    paginaSwap.num = pagina.num;
                    paginaSwap.bytesUsados = pagina.bytesUsados;
                    Console.WriteLine($"Página #{pagina.num} del proceso #{pagina.idProceso} swappeada al marco #{idPaginaLibre} del área de swapping");
                    
                    // Actualizar indices de memoria
                    paginasLibresSwap--;
                    paginasLibres++;

                    // Liberar pagina
                    pagina.idProceso = -1;
                    pagina.num = -1;
                    pagina.bytesUsados = 0;
                    break;
                }
            }

            // Si se uso FIFO, verificar que quedan procesos (Sino sacarlo de la fila)
            if(Globales.estrategia == Estrategia.FIFO) {
                Boolean quedanProcesos = false;

                foreach(Pagina pagina in memoriaReal) {
                    if(pagina.idProceso == idProcesoSacar) {
                        quedanProcesos = true;
                        break;
                    }
                }

                if(!quedanProcesos) {
                    Globales.filaProcesos.Dequeue();
                }
            }

            Globales.timestamp += 0.1;
        }

        public void swapIn(int p, int indicePaginaDelProceso) {
            // Identificar la pagina en memoria swap que se va a meter
            int idPaginaSwap = -1; // Indice donde se encuentra la pagina a meter a memoria real
            foreach(Pagina pagina in memoriaSwap) {
                if(pagina.idProceso == p && pagina.num == indicePaginaDelProceso) {
                    idPaginaSwap = pagina.marco;
                    Console.WriteLine($"Se localizó la página #{indicePaginaDelProceso} del proceso #{p} que estaba en la posición #{pagina.marco} de swapping");
                    break;
                }
            }

            // Identificar el espacio disponible en memoria real
            foreach(Pagina pagina in memoriaReal) {
                if(pagina.idProceso == -1) {
                    pagina.idProceso = memoriaSwap[idPaginaSwap].idProceso;
                    pagina.bytesUsados = memoriaSwap[idPaginaSwap].bytesUsados;
                    pagina.num = memoriaSwap[idPaginaSwap].num;
                    Console.WriteLine($"y se cargo al marco #{pagina.marco}");
                    // Actualizar paginas libres
                    paginasLibres--;
                    break;
                }
            }

            // Librerar pagina en memoriaSwap
            memoriaSwap[idPaginaSwap].idProceso = -1;
            memoriaSwap[idPaginaSwap].num = -1;
            memoriaSwap[idPaginaSwap].bytesUsados = 0;
            paginasLibresSwap++;

            // Checar fila FIFO si el programa metido es el primero (no existe otra pagina de ese proceso en memoriaReal)
            // Volver a meter a fila de procesos
            if(Globales.estrategia == Estrategia.FIFO) {
                int ctr = 0;
                foreach(Pagina pagina in memoriaReal) {
                    if(pagina.idProceso == p) {
                        ctr++;
                    }

                    if(ctr > 1) Globales.filaProcesos.Enqueue(p);
                }
            }

            Globales.timestamp += 1;

        }

        /// <summary>
        /// Funcion para encontrar para encontrar numero de pagina donde se encuentra la direccion virtual del proceso p
        /// </summary>
        /// <param name="d">Int con la direccion virtual del proceso</param>
        /// <param name="p">Int con el numero de proceso</param>
        /// <param name="m"></param>
        /// <return>Int con indice o -1 si no se encuentra</return>
        public int isProcesoEnMemoriaReal(int d, int p) {
            int numIndiceDeProceso = d / 16;
            int indice = 0;

            // Buscar en el arreglo de paginas si se encuentra la pagina con el bloque numero num del proceso p
            foreach(Pagina pagina in memoriaReal) {
                if(pagina.idProceso == p && pagina.num == numIndiceDeProceso) {
                    return indice;
                }
                indice++;
            }

            return -1;
        }

    }
}