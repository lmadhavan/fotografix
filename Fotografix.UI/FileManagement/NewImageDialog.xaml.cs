﻿using Windows.UI.Xaml.Controls;

namespace Fotografix.UI.FileManagement
{
    public sealed partial class NewImageDialog : ContentDialog
    {
        private readonly NewImageParameters parameters;

        public NewImageDialog(NewImageParameters parameters)
        {
            this.parameters = parameters;
            this.InitializeComponent();
        }
    }
}
