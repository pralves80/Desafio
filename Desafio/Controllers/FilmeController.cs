using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Desafio.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Desafio.Controllers
{
    public class FilmeController : Controller
    {
        private const string Key = "883de64b48c143ab6c9b8ee7b7126874";
        private const string URL = "https://api.themoviedb.org/3/movie/popular?api_key=";
        private const string URL_IMAGEM = "https://image.tmdb.org/t/p/w1280";
        private List<Filme> ListaFilmes { get; set; }

        public async Task<IActionResult> Index(String SearchString)
        {
            Task tarefa = GetFilme(Key);
            tarefa.Wait();
            CompletaURL(ListaFilmes);

            if (!String.IsNullOrEmpty(SearchString))
            {
                var movies = from m in ListaFilmes
                             select m;

                movies = movies.Where(s => s.title.Contains(SearchString));
               
                return View(movies);
            }

            return View(ListaFilmes);
        }

        private void CompletaURL(List<Filme> listaFilmes)
        {
            foreach (Filme filme in ListaFilmes)
            {
                filme.poster_path = URL_IMAGEM + filme.poster_path;
            }
        }

        public IActionResult Detalhe(int id)
        {
            Task tarefa = GetFilme(Key);
            tarefa.Wait();
            CompletaURL(ListaFilmes);

            Filme item = ListaFilmes.Find(s => s.id == id);

            return View(item);
        }


        private async Task GetFilme(string tokenAcess)
        {

            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Add("Authorization", "Bearer" + tokenAcess);
                var resposta = await cliente.GetStringAsync(URL+ tokenAcess);

                var ListaFilmesRecuperada = JsonConvert.DeserializeObject<RootFilmes>(resposta);
                ListaFilmes = ListaFilmesRecuperada.results;
            }
        }
    }
}