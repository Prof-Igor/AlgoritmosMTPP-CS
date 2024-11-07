
using Probabilidade;
using System.Diagnostics;

namespace Dinamica
{
    internal class Program
    {
        public class Vertice
        {
            public int indice;
            public string nome;
            public string cor;
            public List<string> cores = new List<string>();
            public List<Vertice> arestas = new List<Vertice>();
            public List<Vertice> arestasNovas = new List<Vertice>();

            public Vertice(string nome, string cor, int indice)
            {
                this.nome = nome;
                this.cor = cor;
                this.indice = indice;
            }
        }

        public class Grafo
        {
            public List<Vertice> listaVertices = new List<Vertice>();
            public List<string> listaCores = new List<string>();
            public List<int> idHistorico = new List<int>();

            public Grafo() { }
        }

        public static Grafo criarGrafo(string instancia)
        {
            StreamReader dadosFile = File.OpenText(instancia);

            int nVertices = int.Parse(dadosFile.ReadLine());
            int nCores = int.Parse(dadosFile.ReadLine());
            int nArestas = int.Parse(dadosFile.ReadLine());

            Grafo grafo = new Grafo();

            for (int i = 0; i < nVertices; i++)
            {
                string[] verticeCor = dadosFile.ReadLine().Split(',');

                if (grafo.listaCores.Find(x => x == verticeCor[1]) == null)
                    grafo.listaCores.Add(verticeCor[1]);
                if (grafo.listaVertices.Find(x => x.nome == verticeCor[0]) == null)
                    grafo.listaVertices.Add(new Vertice(verticeCor[0], verticeCor[1], grafo.listaVertices.Count));
            }

            for (int i = 0; i < nArestas; i++)
            {
                string[] aresta = dadosFile.ReadLine().Split(',');

                int indiceUm = grafo.listaVertices.IndexOf(grafo.listaVertices.Find(x => x.nome == aresta[0]));
                int indiceDois = grafo.listaVertices.IndexOf(grafo.listaVertices.Find(x => x.nome == aresta[1]));

                grafo.listaVertices[indiceUm].arestas.Add(grafo.listaVertices[indiceDois]);
                grafo.listaVertices[indiceDois].arestas.Add(grafo.listaVertices[indiceUm]);
            }

            return grafo;
        }

        public static Vertice RandomWeightedChoice(List<Vertice> elementos)
        {
            Random rand = new Random();
            int index = rand.Next(elementos.Count);  // Escolhe um índice aleatório baseado na quantidade de elementos.
            return elementos[index];  // Retorna o elemento escolhido aleatoriamente.
        }

        public static void calculaCores(Vertice vertice, Vertice? ant, Grafo grafo)
        {
            vertice.cores.Add(vertice.cor);
            if (ant != null)
                vertice.cores= new HashSet<string>(vertice.cores.Concat(ant.cores)).ToList();

            var filhosDisponiveis = new List<Vertice>(vertice.arestas);

            while (filhosDisponiveis.Count > 0)
            {
                Vertice filho = RandomWeightedChoice(filhosDisponiveis); // Escolha aleatória de um filho.
                filhosDisponiveis.Remove(filho);
                
                if (!grafo.idHistorico.Contains(filho.indice))
                {
                    grafo.idHistorico.Add(filho.indice);
                    calculaCores(filho, vertice, grafo);
                }
            }
        }

        public static (int, double) calcular(Grafo grafo) {
            List<string> maiorListaCores = new List<string>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int j = 0; j < 100; j++)
            {
                foreach (Vertice raiz in grafo.listaVertices)
                {
                    //if (raiz.arestas.Count == 1) {

                    grafo.idHistorico.Add(grafo.listaVertices.IndexOf(raiz));
                    calculaCores(raiz, null, grafo);
                    grafo.idHistorico.Clear();
                    foreach (Vertice i in grafo.listaVertices)
                    {
                        if (i.cores.Count > maiorListaCores.Count)
                        {
                            maiorListaCores = new List<string>(i.cores);
                        }
                        i.cores.Clear();
                    }

                    if (maiorListaCores.Count == grafo.listaCores.Count)
                        break;
                }

                if (j == 0 || j == 9)
                {
                    using (StreamWriter escreveResultado = new StreamWriter("../../../Resultados/Cacto/TropicalAlg " + DateTime.Now.Day.ToString().Replace("/", "-") + ".txt", true))
                        escreveResultado.Write($"{maiorListaCores.Count};{stopwatch.Elapsed.TotalSeconds};");
                }
                //} 
            }

            stopwatch.Stop();
            double tempoExecucao = stopwatch.Elapsed.TotalSeconds;
            return (maiorListaCores.Count, tempoExecucao);
        }

        static void Main(string[] args)
        {
            string caminhoInstancia = "../../../Instancias/MTPP/Exemplo 1.txt"; // Substitua com o caminho real do arquivo
            Grafo grafo = criarGrafo(caminhoInstancia);

            Grafo grafoAdaptado = AdaptaGrafo.AdaptarGrafo(grafo);

            // Exibir resultados (exemplo simples)
            foreach (var vertice in grafoAdaptado.listaVertices)
            {
                Console.WriteLine($"Vertice: {vertice.nome}, Cor: {vertice.cor}");
            }
            /*
            string arquivoInstancias = "../../../Instancias/Cacto/nomes.txt";

            using (StreamReader instancias = new StreamReader(arquivoInstancias))
            {
                int numInstancias = int.Parse(instancias.ReadLine().Trim());
                string caminhoResultado = "../../../Resultados/Cacto/";
                
                for (int i = 0; i < numInstancias; i++)
                {
                    string nomeGrafo = instancias.ReadLine().Trim();
                    using (StreamWriter escreveResultado = new StreamWriter(caminhoResultado + "TropicalAlg " + DateTime.Now.Day.ToString().Replace("/", "-") + ".txt", true))
                        escreveResultado.Write($"{nomeGrafo};");
                    string arquivoInstancia = $"../../../Instancias/Cacto/{nomeGrafo}.txt";
                    Grafo grafo = criarGrafo(arquivoInstancia);
                    (int resposta, double tempoExecucao) = calcular(grafo);
                    using (StreamWriter escreveResultado = new StreamWriter(caminhoResultado + "TropicalAlg " + DateTime.Now.Day.ToString().Replace("/", "-") + ".txt", true))
                        escreveResultado.WriteLine($"{resposta};{tempoExecucao}");
                    Console.WriteLine($"{resposta}-{tempoExecucao:F7}");
                }
            }
            */
        }
    }
}