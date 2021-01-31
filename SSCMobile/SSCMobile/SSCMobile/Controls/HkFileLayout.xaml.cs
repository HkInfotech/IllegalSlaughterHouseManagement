using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSCMobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HkFileLayout : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(HkFileLayout), default(string), BindingMode.TwoWay);

        public static readonly BindableProperty ButtonTextProperty =
        BindableProperty.Create(nameof(ButtonText), typeof(string), typeof(HkFileLayout), default(string), BindingMode.TwoWay);

        public static readonly BindableProperty FilesProperty =
        BindableProperty.Create(nameof(Files), typeof(IEnumerable), typeof(HkFileLayout), default(IEnumerable), BindingMode.TwoWay);

        public static readonly BindableProperty HasFilesProperty =
        BindableProperty.Create(nameof(HasFiles), typeof(bool), typeof(HkFileLayout), default(bool), BindingMode.TwoWay);

        public static readonly BindableProperty AddCommandProperty =
            BindableProperty.Create(nameof(AddCommand), typeof(ICommand), typeof(HkFileLayout), null, BindingMode.TwoWay);

        public static readonly BindableProperty ButtonBackgroundColorProperty =
            BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(HkFileLayout), Color.Gray, BindingMode.TwoWay);

        public static readonly BindableProperty RequiredColorProperty =
            BindableProperty.Create(nameof(RequiredColor), typeof(Color), typeof(HkFileLayout), Color.Transparent, BindingMode.TwoWay);

        public event EventHandler AddClicked;
        public event EventHandler<ComplaintImagesDTO> FileClicked;

        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public IEnumerable Files
        {
            get { return (IEnumerable)GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }

        public bool HasFiles
        {
            get { return (bool)GetValue(HasFilesProperty); }
            set { SetValue(HasFilesProperty, value); }
        }

        public Color ButtonBackgroundColor
        {
            get { return (Color)GetValue(ButtonBackgroundColorProperty); }
            set { SetValue(ButtonBackgroundColorProperty, value); }
        }

        public Color RequiredColor
        {
            get { return (Color)GetValue(RequiredColorProperty); }
            set { SetValue(RequiredColorProperty, value); }
        }

        public HkFileLayout()
        {
            InitializeComponent();
        }

        private void AddFile_Clicked(object sender, EventArgs e)
        {
            AddCommand?.Execute(null);
            AddClicked?.Invoke(sender, e);
        }

        private void FileTapped(object sender, EventArgs e)
        {
            var layout = (FlexLayout)sender;
            var tap = (TapGestureRecognizer)layout.GestureRecognizers[0];
            var selectedFile = tap?.CommandParameter as ComplaintImagesDTO;
            FileClicked?.Invoke(sender, selectedFile);
        }
    }
}