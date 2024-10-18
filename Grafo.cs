namespace AlgoritmosMTPP_CS
{
    internal class Grafo
    {
        public int nVertices;
        public int nArestas;
        public int nCores;
        public List<Vertice> vertices = new List<Vertice>();
        public List<string> cores = new List<string>();
        public List<List<int>> idArestas = new List<List<int>>();
        public List<Tuple<int, int>> rotulos = new List<Tuple<int, int>>();
        public List<List<List<int>>> calcBase;
        public List<List<List<int>>> calcAtual;
        public List<int> idHistorico = new List<int>();

        public Grafo(int nVertices, int nCores, int nArestas)
        {
            this.nVertices = nVertices;
            this.nArestas = nArestas;
            this.nCores = nCores;

            for (int i = 0; i < nVertices; i++)
            {
                idArestas.Add(new List<int>());
            }

            for (int i = 0; i < nVertices; i++)
            {
                rotulos.Add(new Tuple<int, int>(0, 0));
            }

            // Inicializa CalcBase como uma lista de listas de listas
            calcBase = new List<List<List<int>>>(nVertices);
            for (int i = 0; i < nVertices; i++)
            {
                var innerList = new List<List<int>>(nVertices);
                for (int j = 0; j < nVertices; j++)
                {
                    innerList.Add(new List<int>(nVertices)); // Adiciona uma lista vazia para cada célula
                }
                calcBase.Add(innerList); // Adiciona a linha (lista) à matriz
            }

            // Inicializa CalcAtual de forma semelhante
            calcAtual = new List<List<List<int>>>(nVertices);
            for (int i = 0; i < nVertices; i++)
            {
                var innerList = new List<List<int>>(nVertices);
                for (int j = 0; j < nVertices; j++)
                {
                    innerList.Add(new List<int>(nVertices)); // Adiciona uma lista vazia para cada célula
                }
                calcAtual.Add(innerList); // Adiciona a linha (lista) à matriz
            }
        }

        public void limpaCalcAtual()
        {
            calcAtual = new List<List<List<int>>>(nVertices);
            for (int i = 0; i < nVertices; i++)
            {
                var innerList = new List<List<int>>(nVertices);
                for (int j = 0; j < nVertices; j++)
                {
                    innerList.Add(new List<int>(nVertices)); // Adiciona uma lista vazia para cada célula
                }
                calcAtual.Add(innerList); // Adiciona a linha (lista) à matriz
            }
        }

        public static Grafo CriarGrafo(string instancia)
        {
            StreamReader dadosFile = File.OpenText(instancia);

            Grafo grafo = new Grafo(int.Parse(dadosFile.ReadLine()), int.Parse(dadosFile.ReadLine()), int.Parse(dadosFile.ReadLine()));

            for (int i = 0; i < grafo.nVertices; i++)
            {
                string[] cores = dadosFile.ReadLine().Split(',');

                if (grafo.cores.Find(x => x == cores[1]) == null)
                    grafo.cores.Add(cores[1]);
                if (grafo.vertices.Find(x => x.Nome == cores[0]) == null)
                    grafo.vertices.Add(new Vertice(cores[0], grafo.cores.IndexOf(grafo.cores.Find(x => x == cores[1]))));
            }

            for (int i = 0; i < grafo.nArestas; i++)
            {
                string[] aresta = dadosFile.ReadLine().Split(',');

                grafo.idArestas[grafo.vertices.IndexOf(grafo.vertices.Find(x => x.Nome == aresta[0]))].Add(grafo.vertices.IndexOf(grafo.vertices.Find(x => x.Nome == aresta[1])));
                grafo.idArestas[grafo.vertices.IndexOf(grafo.vertices.Find(x => x.Nome == aresta[1]))].Add(grafo.vertices.IndexOf(grafo.vertices.Find(x => x.Nome == aresta[0])));
            }

            return grafo;
        }

        public static Tuple<List<Vertice>, List<Tuple<int, int>>> VerificaCiclo(Grafo G, int ini, int v, Tuple<List<Vertice>, List<Tuple<int, int>>> ciclo, List<int> historico)
        {
            if (G.idArestas[v].Except(G.idHistorico).ToList().Count == 0)
                return ciclo;

            foreach (int filho in G.idArestas[v])
            {
                if (!historico.Contains(filho))
                {
                    if (ciclo.Item1.Contains(G.vertices[ini]))
                        return ciclo;
                    if (!G.idHistorico.Contains(filho) && !ciclo.Item1.Contains(G.vertices[filho]) && filho != ini)
                    {
                        ciclo.Item1.Add(G.vertices[filho]);
                        historico.Add(filho);
                        ciclo.Item2.Add(new Tuple<int, int>(v, filho));
                        ciclo.Item2.Add(new Tuple<int, int>(filho, v));

                        VerificaCiclo(G, ini, filho, ciclo, historico);

                        if (ciclo.Item1.Last() == G.vertices[filho])
                            ciclo.Item1.Remove(G.vertices[filho]);
                    }
                    else if (ciclo.Item1.Count() >= 2 && !ciclo.Item1.Contains(G.vertices[filho]))
                    {
                        ciclo.Item1.Add(G.vertices[filho]);
                        historico.Add(filho);
                        if (ciclo.Item1.Contains(G.vertices[ini]))
                        {
                            ciclo.Item2.Add(new Tuple<int, int>(v, filho));
                            ciclo.Item2.Add(new Tuple<int, int>(filho, v));
                            return ciclo;
                        }
                        ciclo.Item1.RemoveAt(ciclo.Item1.Count() - 1);
                    }
                }
            }
            return ciclo;
        }

        public static void AdaptaGrafo(Grafo G, Grafo G2, int v)
        {
            Tuple<List<Vertice>, List<Tuple<int, int>>> cicloCompleto = VerificaCiclo(G, v, v, new Tuple<List<Vertice>, List<Tuple<int, int>>>(new List<Vertice>(), new List<Tuple<int, int>>()), new List<int>());

            if (cicloCompleto.Item1.Count > 2)
            {
                foreach (var item in cicloCompleto.Item1)
                {
                    if (item != G.vertices[v])
                    {
                        G.idHistorico.Add(G.vertices.IndexOf(G.vertices.Find(x => x.Nome == item.Nome)));
                        G2.vertices.Add(item);
                    }
                }
                foreach (var item in cicloCompleto.Item2)
                {
                    if (G2.vertices.Contains(G.vertices[item.Item1]) && G2.vertices.Contains(G.vertices[item.Item2]))
                        G2.idArestas[G2.vertices.IndexOf(G.vertices[item.Item1])].Add(G2.vertices.IndexOf(G.vertices[item.Item2]));
                }
                foreach (var item in G.vertices)
                {
                    if (!G.idHistorico.Contains(G.vertices.IndexOf(G.vertices.Find(x => x.Nome == item.Nome))))
                    {
                        foreach (var item2 in cicloCompleto.Item1)
                        {
                            if (!G.idHistorico.Contains(G.vertices.IndexOf(G.vertices.Find(x => x.Nome == item.Nome))))
                            {
                                if (G.idArestas[G.vertices.IndexOf(G.vertices.Find(x => x.Nome == item2.Nome))].Contains(G.vertices.IndexOf(G.vertices.Find(x => x.Nome == item.Nome))))
                                {
                                    G.idHistorico.Add(G.vertices.IndexOf(G.vertices.Find(x => x.Nome == item.Nome)));

                                    G2.vertices.Add(item);
                                    G2.idArestas[G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == item.Nome))].Add(G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == item2.Nome)));
                                    G2.idArestas[G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == item2.Nome))].Add(G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == item.Nome)));

                                    if (G2.vertices.Count - G.vertices.Count != 0)
                                    {
                                        AdaptaGrafo(G, G2, G.vertices.IndexOf(G.vertices.Find(x => x.Nome == item.Nome)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item in G.idArestas[v])
                {
                    if (!G.idHistorico.Contains(item))
                    {
                        G.idHistorico.Add(item);

                        G2.vertices.Add(G.vertices[item]);
                        G2.idArestas[G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == G.vertices[v].Nome))].Add(G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == G.vertices[item].Nome)));
                        G2.idArestas[G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == G.vertices[item].Nome))].Add(G2.vertices.IndexOf(G2.vertices.Find(x => x.Nome == G.vertices[v].Nome)));

                        if (G2.vertices.Count - G.vertices.Count != 0)
                        {
                            AdaptaGrafo(G, G2, item);
                        }
                    }
                }
            }
        }

        public static Grafo AdaptarGrafo(Grafo G)
        {
            Grafo G2 = new Grafo(G.nVertices, G.nCores, G.nArestas);
            G2.vertices.Add(G.vertices[0]);
            G2.cores = new List<string>(G.cores);

            G.idHistorico.Add(0);
            AdaptaGrafo(G, G2, 0);
            return G2;
        }
    }
}
