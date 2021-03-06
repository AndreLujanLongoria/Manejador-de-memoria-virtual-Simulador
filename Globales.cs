﻿using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_
{
    public enum Estrategia
    {
        FIFO,
        LRU
    }

    public static class Globales
    {
        // Constantes
        public const int TAM_MEMORIA = 2048;
        public const int TAM_PAGINA = 16;

        public static List<string> comandos = new List<string>();
        public static Memoria memoria = new Memoria();
        public static double timestamp = 0.0;
        public static Estrategia estrategia;
        public static Dictionary<int, Proceso> procesos = new Dictionary<int, Proceso>();
        public static Queue<int> filaProcesos = new Queue<int>(); // Queue para FIFO
        public static SortedList<int, double> lruProcesos = new SortedList<int, double>(); // LRU
        public static int topKey = 0;
        public static int contadorSwaps = 0;
        

    }
}
