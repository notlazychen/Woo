using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MarkdownSharp;
using RazorEngine;
using RazorEngine.Templating;

namespace Woo
{
    /// <summary>
    /// PageEdit.xaml 的交互逻辑
    /// </summary>
    public partial class PageEdit : Page
    {
        private readonly Article _editArticle;
        public PageEdit(Article article)
        {
            _editArticle = article;
            InitializeComponent();

            ArticleTitle.Text = article.Title;
            ArticleContent.Text = article.Content;
            ArticleLink.Text = article.Link;
            ArticleDescription.Text = article.Description;
            _ow = new Window
            {
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                Opacity = 0.01,
                Background = Brushes.White,
                Owner = Application.Current.MainWindow,
                ShowInTaskbar = false,
                ShowActivated = false,
                Focusable = false
            };

            ArticleContentPreview.SizeChanged += ArticleContentPreview_SizeChanged;
            this.MouseMove += PageEdit_MouseMove;
            _ow.Show();

            this.Loaded += PageEdit_Loaded;
        }

        void PageEdit_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.Assert(NavigationService != null, "NavigationService != null");
            NavigationService.Navigating += NavigationService_Navigating;
        }

        void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            _ow.Close();
        }
        
        void PageEdit_MouseMove(object sender, MouseEventArgs e)
        {
            SetWebViewPosition();
        }

        void SetWebViewPosition()
        {
            var p = ArticleContentPreview.PointToScreen(new Point(0, 0));
            _ow.Left = p.X;
            _ow.Top = p.Y;
        }

        private readonly Window _ow;

        void ArticleContentPreview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetWebViewPosition();
            _ow.Width = ArticleContentPreview.ActualWidth - 20;
            _ow.Height = ArticleContentPreview.ActualHeight;
        }

        private void AritcleContent_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Markdown mk = new Markdown();
            string html = string.Format(Htmltemp, mk.Transform(ArticleContent.Text));
            ArticleContentPreview.NavigateToString(html);
        }

        private const string Htmltemp = "<!DOCTYPE html><html lang=\"zh-cn\"><head><meta charset=\"utf-8\"/></head><body>{0}</body>";

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            //string result = Engine.Razor.RunCompile(WebSource.Instance.DetailTemplate, "templateDetail", null, _editArticle);
            string link = ArticleLink.Text;
            if (!WebSource.Instance.EditArticle(_editArticle.ID, ArticleTitle.Text, link, ArticleContent.Text, ArticleDescription.Text))
            {
                MessageBox.Show("重复的标题或链接！");
                return;
            }
            Debug.Assert(NavigationService != null, "NavigationService != null");
            //_ow.Close();
            NavigationService.GoBack();
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.Assert(NavigationService != null, "NavigationService != null");
            NavigationService.GoBack();
        }
    }
}
