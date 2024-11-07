using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dinamica.Program;

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
            public List<(int, List<string>)> baseCores = new List<(int, List<string>)>();
            public List<Vertice> arestas = new List<Vertice>();

            public Vertice(string nome, string cor, int indice)
            {
                this.nome = nome;
                this.cor = cor;
                this.indice = indice;
                this.cores.Add(cor);
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

        public static List<Vertice> verificaCiclo(Grafo grafo, Vertice u, List<Vertice> ciclo, Vertice ant, List<Vertice> historico)
        {
            if (ciclo.Contains(u))
                return ciclo;

            foreach (Vertice filho in ciclo.Last().arestas)
            {
                if (ciclo.Contains(u))
                    return ciclo;
                else if (filho == ant)
                    continue;
                else if (historico.Contains(filho))
                    break;

                ciclo.Add(filho);
                historico.Add(filho);
                verificaCiclo(grafo, u, ciclo, ciclo[ciclo.Count - 2], historico);
            }

            if (ciclo.Contains(u))
                return ciclo;
            else
            {
                ciclo.RemoveAt(ciclo.Count - 1);
                return ciclo;
            }
        }

        public static (List<Vertice>, List<List<Vertice>>) buscaFilhos(Grafo grafo, Vertice raiz)
        {
            List<Vertice> filhosArvore = new List<Vertice>();
            List<List<Vertice>> filhosCiclo = new List<List<Vertice>>();

            foreach (Vertice filho in raiz.arestas)
            {
                if (!grafo.idHistorico.Contains(filho.indice))
                {
                    List<Vertice> ciclo = new List<Vertice> {filho};
                    List<Vertice> historico = new List<Vertice> {filho};
                    verificaCiclo(grafo, raiz, ciclo, raiz, historico);

                    if (ciclo.Count == 0)
                    {
                        filhosArvore.Add(filho);
                        grafo.idHistorico.Add(filho.indice);
                    }
                    else
                    {
                        ciclo.RemoveAt(ciclo.Count - 1);
                        List<Vertice> listaCiclo = new List<Vertice>(ciclo);
                        filhosCiclo.Add(listaCiclo);
                        foreach (Vertice c in ciclo)
                        {
                            grafo.idHistorico.Add(c.indice);
                        }
                    }
                }
            }

            // Remove ciclos repetidos (em sentido inverso)
            for (int i = 0; i < filhosCiclo.Count; i++)
            {
                for (int j = 0; j < filhosCiclo.Count; j++)
                {
                    if (filhosCiclo[i][0] == filhosCiclo[j][filhosCiclo[j].Count - 1] &&
                        filhosCiclo[i][filhosCiclo[i].Count - 1] == filhosCiclo[j][0])
                    {
                        filhosCiclo.RemoveAt(j);
                    }
                }
            }

            return (filhosArvore, filhosCiclo);
        }

        public static List<string> coresCiclo(Vertice inicio, Vertice fim, Vertice proximo, List<Vertice> ciclo)
        { 
            List<string> cores = new List<string>();

            cores.Add(inicio.cor);

            if (inicio == fim)
            {
                return cores;
            }

            foreach (Vertice vertice in proximo.arestas) {
                if (ciclo.Contains(vertice) && vertice != inicio)
                {
                    cores = new HashSet<string>(cores.Concat(coresCiclo(proximo, fim, vertice, ciclo))).ToList();
                }
            }

            cores.Add(fim.cor);
            return new HashSet<string>(cores).ToList();
        }

        public static void calculaCores(Vertice vertice, Vertice? ant, Grafo grafo)
        {
            if (ant != null)
                vertice.cores = new HashSet<string>(vertice.cores.Concat(ant.cores)).ToList();

            (List<Vertice>, List<List<Vertice>>) filhos = buscaFilhos(grafo, vertice);

            foreach (Vertice filhoArvore in filhos.Item1)
            {
                calculaCores(filhoArvore, vertice, grafo);
            }

            foreach (List<Vertice> filhoCiclo in filhos.Item2)
            {
                List<Vertice> lados = new List<Vertice>();
                foreach (var item in vertice.arestas)
                {
                    if (filhoCiclo.Contains(item))
                        lados.Add(item);
                }

                foreach (Vertice filho in filhoCiclo)
                {
                    if (!filho.baseCores.Contains((vertice.indice, new List<string>())))
                    {
                        List<string> coresDir = coresCiclo(vertice, filho, lados[0], filhoCiclo);
                        List<string> coresEsq = coresCiclo(vertice, filho, lados[1], filhoCiclo);
                        if (coresDir.Count > coresEsq.Count)
                            filho.baseCores.Add((vertice.indice, coresDir));
                        else
                            filho.baseCores.Add((vertice.indice, coresEsq));
                    }

                    filho.cores = new HashSet<string>(filho.cores.Concat(filho.baseCores.First(x => x.Item1 == vertice.indice).Item2)).ToList(); 
                    calculaCores(filho, vertice, grafo);
                }
            }
        }

        public static (int, double) calcular(Grafo grafo) {
            List<string> maiorListaCores = new List<string>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (Vertice raiz in grafo.listaVertices)
            {
                //if (raiz.arestas.Count == 1) {
                for (int i = 0; i < 1; i++)
                {
                    grafo.idHistorico.Add(grafo.listaVertices.IndexOf(raiz));
                    calculaCores(raiz, null, grafo);
                    grafo.idHistorico.Clear();
                    foreach (Vertice j in grafo.listaVertices)
                    {
                        if (j.cores.Count > maiorListaCores.Count)
                        {
                            maiorListaCores = new List<string>(j.cores);
                        }
                        j.cores.Clear();
                    }

                    if (maiorListaCores.Count == grafo.listaCores.Count)
                        break;
                }
                //}
            }

            stopwatch.Stop();
            double tempoExecucao = stopwatch.Elapsed.TotalSeconds;
            return (maiorListaCores.Count, tempoExecucao);
        }

        static void Main(string[] args)
        {
            string arquivoInstancias = "../../../Instancias/Cacto/nomes.txt";

            using (StreamReader instancias = new StreamReader(arquivoInstancias))
            {
                int numInstancias = int.Parse(instancias.ReadLine().Trim());
                string caminhoResultado = "../../../Resultados/Cacto/";
                using (StreamWriter escreveResultado = new StreamWriter(caminhoResultado + "TropicalAlg " + DateTime.Now.ToString().Replace("/", "-").Replace(":", ".") + ".txt", true))
                {
                    for (int i = 0; i < numInstancias; i++)
                    {
                        string nomeGrafo = instancias.ReadLine().Trim();
                        escreveResultado.Write($"{nomeGrafo}-");
                        string arquivoInstancia = $"../../../Instancias/Cacto/{nomeGrafo}.txt";
                        Grafo grafo = criarGrafo(arquivoInstancia);
                        (int resposta, double tempoExecucao) = calcular(grafo);
                        escreveResultado.WriteLine($"{resposta}-{tempoExecucao:F7}");
                        Console.WriteLine($"{resposta}-{tempoExecucao:F7}");
                    }
                }
            }
        }
    }
}
