using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor;
using System.Windows;
using MarkdownSharp;
using Newtonsoft.Json;
using RazorEngine;
using RazorEngine.Templating;

namespace Woo
{
    class WebSource
    {
        private const string IndexTemplateFile = "Source/Template/Index.cshtml";
        private const string DetailTemplateFile = "Source/Template/Detail.cshtml";
        private const string DataFile = "Source/Data/Article.json";

        private static WebSource _instance;

        public static WebSource Instance
        {
            get { return _instance ?? (_instance = new WebSource());}
        }

        private ObservableCollection<Article> _articles;
        /// <summary>
        /// 作品集
        /// </summary>
        public IEnumerable<Article> Articles
        {
            get { return _articles; }
        }

        public string DetailTemplate { get; private set; }
        public string IndexTemplate { get; private set; }

        public void Load()
        {
            IndexTemplate = File.ReadAllText(IndexTemplateFile);
            DetailTemplate = File.ReadAllText(DetailTemplateFile);
            _articles = JsonConvert.DeserializeObject<ObservableCollection<Article>>(File.ReadAllText(DataFile)) ?? new ObservableCollection<Article>();
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(Articles);
            File.WriteAllText(DataFile, json);
        }

        public Article AddArticle()
        {
            Article article = new Article()
            {
                ID = WebSource.Instance.Articles.Count() + 1,
                Title = "New",
                Link = "New",
                Author = "Roy",
                Content = "New",
                Description = "New",
            };

            while (_articles.Any(a => a.Title == article.Title || a.Link == article.Link))
            {
                article.Title += "s";
                article.Link += "s";
            }
            _articles.Add(article);
            Save();
            return article;
        }

        public bool RemoveArticle(Article article)
        {
            bool ret = _articles.Remove(article);
            if (ret)
            {
                Save();
            }
            return ret;
        }

        public bool EditArticle(int id, string title, string link, string content, string description)
        {
            var article = _articles.FirstOrDefault(a => a.ID == id);
            if (article == null)
            {
                return false;
            }
            if (_articles.Any(a => a.ID != id && (a.Title == title || a.Link == link)))
            {
                return false;
            }
            article.Link = link;
            article.Author = "Roy";
            article.Content = content;
            article.Title = title;
            article.WriteTime = DateTime.Now;
            article.Description = description;
            Save();
            return true;
        }

        public void Export()
        {
            try
            {
                string dirPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Directory.Exists(dirPath + "/output/"))
                {
                    Directory.Delete(dirPath + "/output", true);
                }
                Directory.CreateDirectory(dirPath + "/output");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            Markdown mk = new Markdown();
            //生成首页
            string textIndex = Engine.Razor.RunCompile(WebSource.Instance.IndexTemplate, "templateIndex", null, new
            {
                Articles = Articles
            });
            string indexFilePath = string.Format("{0}/output/{1}", AppDomain.CurrentDomain.BaseDirectory, "");
            Directory.CreateDirectory(indexFilePath);
            File.WriteAllText(indexFilePath + "/index.html", textIndex);

            foreach (Article article in Articles)
            {
                string content = mk.Transform(article.Content);
                var model = new
                {
                    ID=  article.ID,
                    Title = article.Title,
                    Content = content,
                    Link = article.Link,
                    WriteTime = article.WriteTime,
                    Description = article.Description,
                    Author = article.Author,
                };
                string text = Engine.Razor.RunCompile(WebSource.Instance.DetailTemplate, "templateDetail", null, model);
                string outputFilePath = string.Format("{0}/output/{1}", AppDomain.CurrentDomain.BaseDirectory, article.Link);
                Directory.CreateDirectory(outputFilePath);
                File.WriteAllText(outputFilePath + "/index.html", text);
            }

            MessageBox.Show("Export success!");
        }
    }
}
