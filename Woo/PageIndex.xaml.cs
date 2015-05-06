using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using RazorEngine;
using RazorEngine.Text;

namespace Woo
{
    /// <summary>
    /// PageIndex.xaml 的交互逻辑
    /// </summary>
    public partial class PageIndex : Page
    {
        readonly ObservableCollection<Article> _titles = new ObservableCollection<Article>();
        public PageIndex()
        {
            InitializeComponent();

            ListViewTitle.ItemsSource = WebSource.Instance.Articles;
        }

        private void ListViewTitle_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ListViewTitle.SelectedItem as Article;
            if (item != null)
            {
                Debug.Assert(NavigationService != null, "NavigationService != null");
                Page pg = new PageEdit(item);
                NavigationService.Navigate(pg);
            }
        }

        private void ButtonRemove_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var article = btn.DataContext as Article;
                if (article != null)
                {
                    string text = string.Format("确定要删除文章：{0}吗？", article);
                    if (MessageBox.Show(text, "Alert", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        WebSource.Instance.RemoveArticle(article);
                    }
                }
            }
        }

        private void BtnOutput_OnClick(object sender, RoutedEventArgs e)
        {
            WebSource.Instance.Export();
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            WebSource.Instance.AddArticle();
        }
    }
}
