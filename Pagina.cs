namespace Manejador_de_memoria_virtual__Simulador_  {
    public class Pagina {
        public int num;
        public int bitRes;
        public int idProceso;
        public int marco;
        public int sizeBytes = 16;

        public Pagina(int num, int bitRes, int idProceso, int marco) {
            this.num = num;
            this.bitRes = bitRes;
            this.idProceso = idProceso;
            this.marco = marco;
        }
    }
}