using MvvmHelpers;
using Prism.Events;

using SSCMobile;

using SSCMobile.Helpers;
using SSCMobile.Models.Common;
using SSCMobile.ViewModels;
using SSCMobileServiceBus.OfflineSync.Models;
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
    public partial class StemmonsListView : ContentView
    {
        #region Properties
        public Object SelectedItem { get; set; }
        const string sortAscedingPath = IconFonts.ArrowUp;
        const string sortDescendingPath = IconFonts.ArrowDown;
        private IEnumerable _staticList;
        private IEnumerable StemmonsListViewStaticItems
        {
            get
            {
                return _staticList;
            }
            set
            {
                _staticList = value;
            }

        }
        private IEnumerable SearchableItems { get; set; }
        //private int count;
        #endregion

        #region Delegate 
        public delegate void StemmonsListViewDelegate(object args);
        private delegate void ListViewItemSelectedDelegate(object orgs);
        public static StemmonsListViewDelegate OnStemmonsListViewGroupHeaderTapped { get; set; }
        private ListViewItemSelectedDelegate OnListViewItemSelected { get; set; }
        #endregion

        #region Constructor 
        public StemmonsListView()
        {
            InitializeComponent();

            imgSortBy.Text = sortDescendingPath;
            imgSortBy.IsVisible = false;
            //gridHeader.IsVisible = false;
            txtSearchBar.IsVisible = false;

            // this delegate invoked when listview details item is tapped
            OnListViewItemSelected = (object args) =>
            {
                if (ItemSelectedCommand != null)
                {
                    if (ItemSelectedCommand.CanExecute(args))
                        ItemSelectedCommand.Execute(args);
                }
            };

            //// this delegate invoked when grouped listview header is tapped
            OnStemmonsListViewGroupHeaderTapped = (object args) =>
            {
                if (GroupHeaderCommand.CanExecute(args))
                    GroupHeaderCommand.Execute(args);
            };
        }
        #endregion

        #region Item Source Property  
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(ItemsSource),
        returnType: typeof(IEnumerable),
        declaringType: typeof(StemmonsListView),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: ItemsSourcePropertyChanged);

        private static void ItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StemmonsListView)bindable;
            if (control != null)
            {
                control.lstStemmons.ItemsSource = (IEnumerable)newValue;
                control.StemmonsListViewStaticItems = (IEnumerable)newValue;

                //control.lstFooter.IsVisible = ((IEnumerable<object>)StemmonsListViewStaticItems).AnyExtended();
            }
        }
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)base.GetValue(ItemsSourceProperty); }
            set { base.SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        #region Searchable Item Source Property  
        public static readonly BindableProperty SearchableItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(SearchableItemsSource),
        returnType: typeof(IEnumerable),
        declaringType: typeof(StemmonsListView),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: SearchableItemsSourcePropertyChanged);

        private static void SearchableItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StemmonsListView)bindable;
            if (control != null)
            {
                control.SearchableItems = (IEnumerable)newValue;
            }
        }

        public IEnumerable SearchableItemsSource
        {
            get { return (IEnumerable)base.GetValue(SearchableItemsSourceProperty); }
            set { base.SetValue(SearchableItemsSourceProperty, value); }
        }

        #endregion

        #region ItemTemplate Property
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        propertyName: nameof(ItemTemplate),
        returnType: typeof(DataTemplate),
        declaringType: typeof(StemmonsListView),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: ItemTemplatePropertyChanged
       );
        private static void ItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StemmonsListView)bindable;
            if (control != null)
            {
                control.lstStemmons.ItemTemplate = (DataTemplate)newValue;
            }
        }
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)base.GetValue(ItemTemplateProperty); }
            set { base.SetValue(ItemTemplateProperty, value); }
        }

        #endregion

        #region Group Header Template Property
        public static readonly BindableProperty GroupHeaderTemplateProperty = BindableProperty.Create(
        propertyName: nameof(GroupHeaderTemplate),
        returnType: typeof(DataTemplate),
        declaringType: typeof(StemmonsListView),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: GroupHeaderTemplatePropertyChanged
       );

        private static void GroupHeaderTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StemmonsListView)bindable;
            if (control != null)
            {
                var headerTemplate = (DataTemplate)newValue;
                if (headerTemplate != null)
                {
                    control.lstStemmons.IsGroupingEnabled = true;
                    control.lstStemmons.GroupHeaderTemplate = headerTemplate;
                }
            }

        }

        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)base.GetValue(GroupHeaderTemplateProperty); }
            set { base.SetValue(GroupHeaderTemplateProperty, value); }
        }
        public ICommand GroupHeaderCommand
        {
            get { return (ICommand)GetValue(GroupHeaderCommandProperty); }
            set { SetValue(GroupHeaderCommandProperty, value); }
        }
        #endregion

        #region SelectedCommand & GroupHeaderCommand Property
        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(
         propertyName: nameof(ItemSelectedCommand),
         returnType: typeof(ICommand),
         declaringType: typeof(StemmonsListView),
         defaultValue: null,
         defaultBindingMode: BindingMode.TwoWay
         );
        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        public static readonly BindableProperty GroupHeaderCommandProperty = BindableProperty.Create(
        propertyName: nameof(GroupHeaderCommand),
        returnType: typeof(ICommand),
        declaringType: typeof(StemmonsListView),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay
        );
        #endregion

        #region Separator Styling
        #region Separator Visibility
        public static readonly BindableProperty SeparatorVisibilityTypeProperty = BindableProperty.Create(
        propertyName: nameof(SeparatorVisibilityType),
        returnType: typeof(SeparatorVisibility),
        declaringType: typeof(StemmonsListView),
        defaultValue: SeparatorVisibility.None,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: SeparatorVisibilityTypePropertyChanged
        );
        private static void SeparatorVisibilityTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StemmonsListView)bindable;
            if (control != null)
            {
                var sepVisibility = (SeparatorVisibility)newValue;

                control.lstStemmons.SeparatorVisibility = sepVisibility;

            }

        }

        public SeparatorVisibility SeparatorVisibilityType
        {
            get { return (SeparatorVisibility)GetValue(SeparatorVisibilityTypeProperty); }
            set { SetValue(SeparatorVisibilityTypeProperty, value); }
        }
        #endregion
        #region Separator Color
        public static readonly BindableProperty SeparatorColorProperty = BindableProperty.Create(
        propertyName: nameof(SeparatorColor),
        returnType: typeof(Color),
        declaringType: typeof(StemmonsListView),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: SeparatorColorPropertyChanged
        );
        private static void SeparatorColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StemmonsListView)bindable;
            if (control != null)
            {
                var sepColor = (Color)newValue;
                if (sepColor != null)
                {
                    control.lstStemmons.SeparatorColor = sepColor;
                }
            }

        }

        public Color SeparatorColor
        {
            get { return (Color)GetValue(SeparatorColorProperty); }
            set { SetValue(SeparatorColorProperty, value); }
        }
        #endregion
        #endregion

        #region ListViewItem SelectedEvent
        private void ListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                if (Device.RuntimePlatform == Device.UWP)
                {
                    // To fix UWP Xamarin Forms crash issue when opening some of case items
                    lstStemmons.ItemsSource = null;
                    lstStemmons.ItemsSource = ItemsSource;
                }

                ItemSelectedCommand?.Execute(e.SelectedItem);
                ((ListView)sender).SelectedItem = null;
            }

            lstStemmons.SelectedItem = null;
        }
        #endregion

        #region ListViewItem OnItemAppearing Event

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var listview = (ListView)sender;

            if (listview.IsRefreshing) return;

            if (LoadItemsCommand == null) return;

            if (!((IEnumerable<object>)StemmonsListViewStaticItems).AnyExtended()) return;

            int currentIndex = e.ItemIndex;
            int lastItemIndex = ((IEnumerable<object>)StemmonsListViewStaticItems).Count() - 1;
            var lastItem = ((IEnumerable<object>)StemmonsListViewStaticItems).ElementAt(lastItemIndex);
            bool isLast = false;

            if (listview.IsGroupingEnabled)
            {
                if (((IEnumerable<object>)lastItem).Any())
                {
                    int totalCount = 0;
                    foreach (var item in ((IEnumerable<object>)StemmonsListViewStaticItems))
                    {
                        totalCount += ((IEnumerable<object>)item).Count();
                    }

                    totalCount += ((IEnumerable<object>)StemmonsListViewStaticItems).Count() - 1;
                    isLast = totalCount == currentIndex;
                }
            }
            else
            {
                isLast = lastItemIndex == currentIndex;
            }

            if (isLast)
            {
                lstFooter.IsVisible = true;
                LoadItemsCommand.Execute(e.Item);
            }
        }

        public ICommand LoadItemsCommand
        {
            get { return (ICommand)GetValue(LoadItemsCommandProperty); }
            set { SetValue(LoadItemsCommandProperty, value); }
        }

        public static readonly BindableProperty LoadItemsCommandProperty = BindableProperty.Create(
        nameof(LoadItemsCommand),
        typeof(ICommand),
        typeof(StemmonsListView),
        null,
        BindingMode.OneWay);

        #endregion

        #region Pagination
        public IInfiniteListLoader Paginator
        {
            get => (IInfiniteListLoader)GetValue(PaginatorProperty);
            set => SetValue(PaginatorProperty, value);
        }

        public static readonly BindableProperty PaginatorProperty = BindableProperty.Create(
            nameof(Paginator),
            typeof(IInfiniteListLoader),
            typeof(StemmonsListView));
        #endregion

        #region Get more data from server
        public ICommand GetMoreFromServerCommand
        {
            get { return (ICommand)GetValue(GetMoreFromServerCommandProperty); }
            set { SetValue(GetMoreFromServerCommandProperty, value); }
        }

        public static readonly BindableProperty GetMoreFromServerCommandProperty = BindableProperty.Create(
        nameof(GetMoreFromServerCommand),
        typeof(ICommand),
        typeof(StemmonsListView),
        null,
        BindingMode.OneWay);
        #endregion

        #region Loading
        public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(StemmonsListView), default(bool), BindingMode.TwoWay);

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly BindableProperty IsUpToDateProperty =
        BindableProperty.Create(nameof(IsUpToDate), typeof(bool), typeof(StemmonsListView), default(bool), BindingMode.TwoWay);

        public bool IsUpToDate
        {
            get { return (bool)GetValue(IsUpToDateProperty); }
            set { SetValue(IsUpToDateProperty, value); }
        }
        #endregion

        #region Search By Property  
        public static readonly BindableProperty SearchByProperty = BindableProperty.Create(
        propertyName: nameof(SearchByField),
        returnType: typeof(string),
        declaringType: typeof(StemmonsListView),
        defaultValue: string.Empty,
        defaultBindingMode: BindingMode.TwoWay
       );
        public string SearchByField
        {
            get { return (string)base.GetValue(SearchByProperty); }
            set { base.SetValue(SearchByProperty, value); ShowHideSearchingTextBox(value); }

        }
        private void ShowHideSearchingTextBox(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                txtSearchBar.IsVisible = true;
                //gridHeader.IsVisible = true;
                if (string.IsNullOrEmpty(SortByField))
                {
                    txtSearchBar.HorizontalOptions = LayoutOptions.FillAndExpand;
                }
            }
        }
        #endregion

        #region Sort By Property  
        public static readonly BindableProperty SortOrdersProperty = BindableProperty.Create(
        propertyName: nameof(SortOrders),
        returnType: typeof(SortOrder),
        declaringType: typeof(StemmonsListView),
        defaultValue: new SortOrder(),
        defaultBindingMode: BindingMode.TwoWay
        );
        public SortOrder SortOrders
        {
            get { return (SortOrder)base.GetValue(SortOrdersProperty); }
            set { base.SetValue(SortOrdersProperty, value); ShowHideSortingImage(value); }
        }


        public static readonly BindableProperty SortByProperty = BindableProperty.Create(
        propertyName: nameof(SortByField),
        returnType: typeof(String),
        declaringType: typeof(StemmonsListView),
        defaultValue: string.Empty,
        defaultBindingMode: BindingMode.TwoWay
        );
        public string SortByField
        {
            get { return (string)base.GetValue(SortByProperty); }
            set { base.SetValue(SortByProperty, value); ShowHideSortingImage(value); }
        }

        private void ShowHideSortingImage(string value)
        {
            //gridHeader.IsVisible = !string.IsNullOrEmpty(value);
            imgSortBy.IsVisible = !string.IsNullOrEmpty(value);
        }

        private void ShowHideSortingImage(SortOrder value)
        {
            //gridHeader.IsVisible = !string.IsNullOrEmpty(value);
            imgSortBy.IsVisible = !string.IsNullOrEmpty(value.ColumnName);
        }
        #endregion

        #region Dynamic where & Sort Clause
        public static IEnumerable<object> WhereQuery(IEnumerable<object> source, string columnName, string propertyValue)
        {
            try
            {
                var result = source.Where(m => { return m.GetType().GetProperty(columnName).GetValue(m, null) == null ? false : m.GetType().GetProperty(columnName).GetValue(m, null).ToString().ToLower().Contains(propertyValue.ToLower()); }).ToList();
                return result;
            }
            catch (NullReferenceException obj)
            {
                Application.Current.MainPage.DisplayAlert("Opps", "Specified column name not exist", "OK");
                return source;
            }
            catch (Exception ex)
            {
                return source;
            }
        }

        public static IEnumerable<object> GroupedListViewWhereQuery(IEnumerable<object> source, string columnName, string propertyValue)
        {
            List<Grouping<object, object>> returnList = new List<Grouping<object, object>>();
            try
            {
                foreach (var record in source)
                {
                    var headerValues = record.GetType().GetProperty("Key").GetValue(record);
                    var detailValues = (IEnumerable<object>)record;
                    var result = detailValues.Where(m => { return m.GetType().GetProperty(columnName).GetValue(m, null) == null ? false : m.GetType().GetProperty(columnName).GetValue(m, null).ToString().ToLower().StartsWith(propertyValue.ToLower()); }).ToList();
                    if (result != null && result.Count > 0)
                    {
                        returnList.Add(new Grouping<object, object>(headerValues, result));
                    }
                    else
                    {
                        returnList.Add(new Grouping<object, object>(headerValues, new List<object>()));
                    }
                }
                return returnList;

            }
            catch (NullReferenceException ex)
            {
                Application.Current.MainPage.DisplayAlert("Opps", "Specified column name not exist", "OK");
                return source;
            }
            catch (Exception ex)
            {
                return source;
            }
        }


        public static IEnumerable<object> SortGroupedListView(IEnumerable<object> source, string columnName, SortTypes sortTypes)
        {
            List<Grouping<object, object>> returnList = new List<Grouping<object, object>>();
            try
            {

                foreach (var record in source)
                {
                    var headerValues = record.GetType().GetProperty("Key").GetValue(record);
                    var detailValues = (IEnumerable<Object>)record;
                    if (sortTypes == SortTypes.Ascending)
                    {
                        var result = detailValues.OrderBy(m => { return m.GetType().GetProperty(columnName).GetValue(m, null); }).ToList();
                        returnList.Add(new Grouping<object, object>(headerValues, result));
                    }
                    else
                    {
                        var result = detailValues.OrderByDescending(m => { return m.GetType().GetProperty(columnName).GetValue(m, null); }).ToList();
                        returnList.Add(new Grouping<object, object>(headerValues, result));

                    }
                }
                return returnList;

            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Opps", "Specified column name not exist", "OK");
                return source;
            }
        }

        public static IEnumerable<object> SortListView(IEnumerable<object> source, string columnName, SortTypes sortTypes)
        {
            try
            {
                if (sortTypes == SortTypes.Ascending)
                {
                    return source.OrderBy(m => { return m.GetType().GetProperty(columnName).GetValue(m, null); });
                }
                else
                {
                    return source.OrderByDescending(m => { return m.GetType().GetProperty(columnName).GetValue(m, null); });
                }

            }
            catch (NullReferenceException obj)
            {
                Application.Current.MainPage.DisplayAlert("Opps", "Specified column name not exist", "OK");
                return source;
            }
        }


        #endregion

        #region Searching &  Sorting 
        private void TxtSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string SortColumn = "";
            if (!string.IsNullOrWhiteSpace(SearchByField))
            {
                SortColumn = SearchByField;
            }
            else
            {
                SortColumn = SortOrders.ColumnName;
            }

            if (!string.IsNullOrEmpty(SortColumn))
            {
                var result = (IEnumerable<object>)StemmonsListViewStaticItems;
                string searchText = !string.IsNullOrEmpty(e.NewTextValue) ? e.NewTextValue : string.Empty;

                if (GroupHeaderTemplate != null)
                {
                    if (searchText.IsNotNullOrEmpty() && SearchableItems != null)
                        result = GroupedListViewWhereQuery((IEnumerable<object>)SearchableItems, SortColumn, searchText);
                }
                else
                {
                    if (searchText.IsNotNullOrEmpty() && SearchableItems != null)
                        result = WhereQuery((IEnumerable<object>)SearchableItems, SortColumn, searchText);
                }

                lstStemmons.ItemsSource = result;
            }
        }

        private void TapGestureRecognizerForSort_Tapped(object sender, EventArgs e)
        {
            string SortColumn = "";
            if (!string.IsNullOrWhiteSpace(SearchByField))
            {
                SortColumn = SearchByField;
            }
            else
            {
                SortColumn = SortOrders.ColumnName;
            }

            if (!string.IsNullOrEmpty(SortColumn))
            {
                var source = imgSortBy.Text;
                string sortOrder = source;
                imgSortBy.Text = sortOrder == sortAscedingPath ? sortDescendingPath : sortAscedingPath;

                if (GroupHeaderTemplate != null)
                {
                    var res = SortGroupedListView((IEnumerable<object>)StemmonsListViewStaticItems, SortColumn, SortOrders.SortTypes);
                    lstStemmons.ItemsSource = res;
                }
                else
                {
                    var res = SortListView((IEnumerable<object>)StemmonsListViewStaticItems, SortColumn, SortOrders.SortTypes);
                    lstStemmons.ItemsSource = res;
                }
            }
        }
        #endregion
    }
}