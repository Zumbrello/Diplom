using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diplom.UserControls;

namespace Diplom
{
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            var contentControl = this.FindControl<ContentControl>("ContentControl") ?? throw new InvalidOperationException("ContentControl not found");
            NavigationManager.Initialize(contentControl);
            ShowUserControl();
        }

        private void ShowUserControl()
        {
            ContentControl.Content = new MapControl();
        }
    }
}