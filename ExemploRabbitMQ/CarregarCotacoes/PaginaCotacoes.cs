using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace CarregarCotacoes
{
    public class PaginaCotacoes
    {
        private SeleniumConfigurations _configurations;
        private IWebDriver _driver;

        public PaginaCotacoes(SeleniumConfigurations seleniumConfigurations)
        {
            _configurations = seleniumConfigurations;

            FirefoxOptions optionsFF = new FirefoxOptions();
            optionsFF.AddArgument("--headless");
            
            _driver = new FirefoxDriver(
                _configurations.CaminhoDriverFirefox
                , optionsFF);
        }

        public void CarregarPagina()
        {
            _driver.Manage().Timeouts().PageLoad =
                TimeSpan.FromSeconds(30);
            _driver.Navigate().GoToUrl(
                _configurations.UrlPaginaCotacoes);
        }

        public List<Cotacao> ObterCotacoes()
        {
            List<Cotacao> cotacoes = new List<Cotacao>();
            var tableCotacoes = _driver.FindElement(
                By.ClassName("quatro-colunas"));
            var rowsCotacoes = tableCotacoes.FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"));
            foreach (var rowCotacao in rowsCotacoes)
            {
                var dadosCotacao =
                    rowCotacao.FindElements(By.TagName("td"));

                Cotacao cotacao = new Cotacao();
                cotacao.NomeMoeda =
                    dadosCotacao[0].FindElement(
                        By.TagName("a")).GetAttribute("innerHTML");
                cotacao.DtUltimaCarga = DateTime.Now;
                cotacao.ValorCompra = Convert.ToDouble(
                    dadosCotacao[1].GetAttribute("innerHTML"));
                cotacao.ValorVenda = Convert.ToDouble(
                    dadosCotacao[2].GetAttribute("innerHTML"));
                cotacao.Variacao =
                    dadosCotacao[3].FindElement(By.TagName("span")).Text;

                cotacoes.Add(cotacao);
            }

            return cotacoes;
        }

        public void Fechar()
        {
            _driver.Quit();
            _driver = null;
        }
    }
}