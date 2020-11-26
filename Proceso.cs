namespace Manejador_de_memoria_virtual__Simulador_ {
    public class Proceso {

        public int id;
        public int tam;
        public int numPageFaults;
        public double tiempoInicio;
        public double tiempoFinal;

        public Proceso(int id, int tam, int numPageFaults, double tiempoInicio, double tiempoFinal) {
            this.id = id;
            this.tam = tam;
            this.numPageFaults = numPageFaults;
            this.tiempoInicio = tiempoInicio;
            this.tiempoFinal = tiempoFinal;
        }
    }
}