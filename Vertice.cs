namespace AlgoritmosMTPP_CS
{
    internal class Vertice
    {
        public string Nome;
        public int idCor;
        public List<int> idCores = new List<int>();

        public Vertice(string Nome, int idCor)
        {
            this.Nome = Nome;
            this.idCor = idCor;
        }
    }
}
