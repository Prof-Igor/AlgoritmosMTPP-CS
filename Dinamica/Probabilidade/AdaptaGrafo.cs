using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dinamica.Program;

namespace Probabilidade
{
    internal class AdaptaGrafo
    {
        public static Tuple<List<Vertice>, List<Tuple<int, int>>> VerificaCiclo(Grafo G, Vertice ini, Vertice v, Tuple<List<Vertice>, List<Tuple<int, int>>> ciclo, List<int> historico)
        {
            if (G.listaVertices.Count == G.idHistorico.Count)
                return ciclo;

            foreach (Vertice filho in v.arestas)
            {
                if (!historico.Contains(filho.indice))
                {
                    if (ciclo.Item1.Contains(ini))
                        return ciclo;
                    if (!G.idHistorico.Contains(filho.indice) && !ciclo.Item1.Contains(filho) && filho != ini)
                    {
                        ciclo.Item1.Add(filho);
                        historico.Add(filho.indice);
                        ciclo.Item2.Add(new Tuple<int, int>(v.indice, filho.indice));
                        ciclo.Item2.Add(new Tuple<int, int>(filho.indice, v.indice));

                        VerificaCiclo(G, ini, filho, ciclo, historico);

                        if (ciclo.Item1.Last() == filho)
                            ciclo.Item1.Remove(filho);
                    }
                    else if (ciclo.Item1.Count() >= 2 && !ciclo.Item1.Contains(filho))
                    {
                        ciclo.Item1.Add(filho);
                        historico.Add(filho.indice);
                        if (ciclo.Item1.Contains(ini))
                        {
                            ciclo.Item2.Add(new Tuple<int, int>(v.indice, filho.indice));
                            ciclo.Item2.Add(new Tuple<int, int>(filho.indice, v.indice));
                            return ciclo;
                        }
                        ciclo.Item1.RemoveAt(ciclo.Item1.Count() - 1);
                    }
                }
            }
            return ciclo;
        }

        public static void AdaptarOGrafo(Grafo G, Grafo G2, Vertice v)
        {
            Tuple<List<Vertice>, List<Tuple<int, int>>> cicloCompleto = VerificaCiclo(G, v, v, new Tuple<List<Vertice>, List<Tuple<int, int>>>(new List<Vertice>(), new List<Tuple<int, int>>()), new List<int>());

            if (cicloCompleto.Item1.Count > 2)
            {
                foreach (var item in cicloCompleto.Item1)
                {
                    if (item != v)
                    {
                        G.idHistorico.Add(item.indice);
                        G2.listaVertices.Add(item);
                    }
                }
                foreach (var item in cicloCompleto.Item2)
                {
                    if (G2.listaVertices.Contains(G.listaVertices[item.Item1]) && G2.listaVertices.Contains(G.listaVertices[item.Item2]))
                        G2.listaVertices[G2.listaVertices.IndexOf(G.listaVertices[item.Item1])].arestasNovas.Add(G2.listaVertices[G2.listaVertices.IndexOf(G.listaVertices[item.Item2])]);
                }
                foreach (var item in G.listaVertices)
                {
                    if (!G.idHistorico.Contains(item.indice))
                    {
                        foreach (var item2 in cicloCompleto.Item1)
                        {
                            if (!G.idHistorico.Contains(item.indice))
                            {
                                if (G.listaVertices[item2.indice].arestas.Contains(item))
                                {
                                    G.idHistorico.Add(item.indice);

                                    G2.listaVertices.Add(item);
                                    G2.listaVertices[G2.listaVertices.IndexOf(item)].arestasNovas.Add(item2);
                                    G2.listaVertices[G2.listaVertices.IndexOf(item2)].arestasNovas.Add(item);

                                    if (G2.listaVertices.Count - G.listaVertices.Count != 0)
                                    {
                                        AdaptarOGrafo(G, G2, item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item in v.arestas)
                {
                    if (!G.idHistorico.Contains(item.indice))
                    {
                        G.idHistorico.Add(item.indice);

                        G2.listaVertices.Add(item);
                        G2.listaVertices[G2.listaVertices.IndexOf(v)].arestasNovas.Add(item);
                        G2.listaVertices[G2.listaVertices.IndexOf(item)].arestasNovas.Add(v);

                        if (G2.listaVertices.Count - G.listaVertices.Count != 0)
                        {
                            AdaptarOGrafo(G, G2, item);
                        }
                    }
                }
            }
        }

        public static Grafo AdaptarGrafo(Grafo G)
        {
            Grafo G2 = new Grafo();
            G2.listaVertices.Add(G.listaVertices[0]);
            G2.listaCores = new List<string>(G.listaCores);

            G.idHistorico.Add(0);
            AdaptarOGrafo(G, G2, G.listaVertices[0]);
            return G2;
        }
    }
}
