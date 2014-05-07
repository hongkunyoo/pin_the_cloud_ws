using PintheCloudWS.Common;
using PintheCloudWS.Converters;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Managers;
using PintheCloudWS.Models;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 기본 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234237에 나와 있습니다.

namespace PintheCloudWS.Pages
{
    /// <summary>
    /// 대부분의 응용 프로그램에 공통되는 특성을 제공하는 기본 페이지입니다.
    /// </summary>
    public sealed partial class ExplorerPage : PtcPage
    {
        //private const string SELECTED_EXPLORER_INDEX_KEY = "SELECTED_EXPLORER_INDEX_KEY";

        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private FileObjectViewModel PickFileObjectViewModel = new FileObjectViewModel();
        private List<FileObjectViewItem> PickSelectedFileList = new List<FileObjectViewItem>();

        private FileObjectViewModel PinFileObjectViewModel = new FileObjectViewModel();
        private List<FileObject> CurrentFileObjectList = new List<FileObject>();
        private List<FileObjectViewItem> PinSelectedFileList = new List<FileObjectViewItem>();

        private SpotViewItem CurrentSpotViewItem = null;
        private SpotObject CurrentSpot = null;

        //private Frame HiddenFrame = null;
        //private SpotViewItem CurrentSpotViewItem = null;

        //private List<Explorer> ExplorerList = new List<Explorer>
        //{
        //    new Explorer() { Title = AppResources.Pick, ClassType = typeof(PickPage) },
        //    new Explorer() { Title = AppResources.Pin, ClassType = typeof(PinPage) },
        //};

        //public class Explorer
        //{
        //    public string Title { get; set; }

        //    public Type ClassType { get; set; }

        //    public override string ToString()
        //    {
        //        return Title;
        //    }
        //}



        /// <summary>
        /// 이는 강력한 형식의 뷰 모델로 변경될 수 있습니다.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        public ExplorerPage()
        {
            this.InitializeComponent();

            // Set Datacontext
            uiPickFileList.DataContext = this.PickFileObjectViewModel;
            uiPinFileListGridView.DataContext = this.PinFileObjectViewModel;
        }


        #region NavigationHelper 등록

        /// 이 섹션에서 제공되는 메서드는 NavigationHelper에서
        /// 페이지의 탐색 메서드에 응답하기 위해 사용됩니다.
        /// 
        /// 페이지별 논리는 다음에 대한 이벤트 처리기에 있어야 합니다.  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// 및 <see cref="GridCS.Common.NavigationHelper.SaveState"/>입니다.
        /// 탐색 매개 변수는 LoadState 메서드 
        /// 및 이전 세션 동안 유지된 페이지 상태에서 사용할 수 있습니다.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedTo(e);

            // Get Parameter
            this.CurrentSpotViewItem = e.Parameter as SpotViewItem;

            // Set this explorer page.
            uiSpotNameText.Text = this.CurrentSpotViewItem.SpotName;
            uiAccountNameText.Text = this.CurrentSpotViewItem.AccountName;
            Switcher.SetStorageTo(Switcher.GetMainStorage().GetStorageName());
            this.CurrentSpot = App.SpotManager.GetSpotObject(this.CurrentSpotViewItem.SpotId);

            uiExplorerList.SelectedIndex = EventHelper.PICK_PIVOT;
            this.SetPickPivot(AppResources.Loading);
            this.SetPinPivot(AppResources.Loading);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion



        #region UI Methods

        private void uiExplorerList_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            // Set View model for dispaly. One time loading.
            App.ApplicationSessions[PIVOT_KEY] = uiExplorerList.SelectedIndex;

            switch (uiExplorerList.SelectedIndex)
            {
                case EventHelper.PICK_PIVOT:
                    // Change edit view mode
                    string currentEditViewMode = uiPickFileListEditViewImageButton.ImageSource;
                    if (currentEditViewMode.Equals(VIEW_IMAGE_URI))  // Edit mode
                        ApplicationBar.Buttons.Add(this.PickDeleteAppBarButton);
                    else if (currentEditViewMode.Equals(EDIT_IMAGE_URI))  // View mode
                        ApplicationBar.Buttons.Remove(this.PickDeleteAppBarButton);

                    // Set Pick Pivot UI
                    ApplicationBar.Buttons.Remove(this.PinFileAppBarButton);
                    for (int i = 0; i < AppBarMenuItems.Length; i++)
                        ApplicationBar.MenuItems.Remove(this.AppBarMenuItems[i]);
                    uiPickPivotImage.Source = new BitmapImage(new Uri(PICK_PIVOT_HIGHLIGHT_IMAGE_URI, UriKind.Relative));
                    uiPinPivotImage.Source = new BitmapImage(new Uri(PIN_PIVOT_IMAGE_URI, UriKind.Relative));
                    uiPivotTitleGrid.Background = new SolidColorBrush(ColorHexStringToBrushConverter.GetColorFromHexString(PICK_PIVOT_TITLE_GRID_COLOR_HEX_STRING));
                    uiCurrentCloudModeImage.Visibility = Visibility.Collapsed;

                    this.SetPickPivot(AppResources.Loading);
                    break;


                case EventHelper.PIN_PIVOT:
                    // Set Pin Pivot UI
                    IStorageManager iStorageManager = Switcher.GetCurrentStorage();
                    ApplicationBar.Buttons.Remove(this.PickDeleteAppBarButton);
                    ApplicationBar.Buttons.Add(this.PinFileAppBarButton);
                    for (int i = 0; i < AppBarMenuItems.Length; i++)
                        ApplicationBar.MenuItems.Add(this.AppBarMenuItems[i]);
                    uiPickPivotImage.Source = new BitmapImage(new Uri(PICK_PIVOT_IMAGE_URI, UriKind.Relative));
                    uiPinPivotImage.Source = new BitmapImage(new Uri(PIN_PIVOT_HIGHLIGHT_IMAGE_URI, UriKind.Relative));
                    uiPivotTitleGrid.Background = new SolidColorBrush(ColorHexStringToBrushConverter.GetColorFromHexString(iStorageManager.GetStorageColorHexString()));
                    uiCurrentCloudModeImage.Source = new BitmapImage(new Uri(iStorageManager.GetStorageImageUri(), UriKind.Relative));
                    uiCurrentCloudModeImage.Visibility = Visibility.Visible;

                    this.SetPinPivot(AppResources.Loading);
                    break;
            }

            //if (uiExplorerList.SelectedItem != null)
            //{
            //    App.ApplicationSessions[SELECTED_EXPLORER_INDEX_KEY] = uiExplorerList.SelectedIndex;
            //    ListViewItem selectedListBoxItem = uiExplorerList.SelectedItem as ListViewItem;
            //    Explorer explorer = selectedListBoxItem.Content as Explorer;
            //    this.LoadExplorer(explorer.ClassType, this.CurrentSpotViewItem);
            //}
        }


        private void uiRefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            switch (uiExplorerPivot.SelectedIndex)
            {
                case EventHelper.PICK_PIVOT:
                    this.PickFileObjectViewModel.IsDataLoaded = false;
                    this.SetPickPivot(AppResources.Refreshing);
                    break;


                case EventHelper.PIN_PIVOT:
                    this.PinFileObjectViewModel.IsDataLoaded = false;
                    TaskHelper.AddTask(TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName(), StorageExplorer.Refresh());
                    this.SetPinPivot(AppResources.Refreshing);
                    break;
            }
        }

        #endregion



        #region Private Methods

        private void SetPickPivot(string message)
        {
            // If internet is on, refresh
            // Otherwise, show internet unavailable message.
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!this.PickFileObjectViewModel.IsDataLoaded)
                    this.SetPickFileListAsync(message);
            }
            else
            {
                uiPickFileList.Visibility = Visibility.Collapsed;
                uiPickFileListMessage.Text = AppResources.InternetUnavailableMessage;
                uiPickFileListMessage.Visibility = Visibility.Visible;
            }
        }


        private async void SetPickFileListAsync(string message)
        {
            // Show Progress Indicator
            base.SetProgressRing(uiProgressRing, true);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                uiPickFileList.Visibility = Visibility.Collapsed;
                uiPickFileListMessage.Text = message;
                uiPickFileListMessage.Visibility = Visibility.Visible;
            });

            try
            {
                // Get files from the spot and set it to list.
                List<FileObject> fileList = await this.CurrentSpot.ListFileObjectsAsync();
                this.PickFileObjectViewModel.IsDataLoaded = true;
                if (fileList.Count > 0)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPickFileList.Visibility = Visibility.Visible;
                        uiPickFileListMessage.Visibility = Visibility.Collapsed;
                        this.PickFileObjectViewModel.SetItems(fileList, false);
                    });
                }
                else
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPickFileList.Visibility = Visibility.Collapsed;
                        uiPickFileListMessage.Text = AppResources.NoFileInSpotMessage;
                        uiPickFileListMessage.Visibility = Visibility.Visible;
                    });
                }
            }
            catch
            {
                this.PickFileObjectViewModel.IsDataLoaded = true;
                uiPickFileList.Visibility = Visibility.Collapsed;
                uiPickFileListMessage.Text = AppResources.BadLoadingFileMessage;
                uiPickFileListMessage.Visibility = Visibility.Visible;
            }


            // Hide Progress Indicator
            base.SetProgressRing(uiProgressRing, false);
        }


        private async void SetPinPivot(string message)
        {
            // If it wasn't already signed in, show signin button.
            // Otherwise, load files
            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            if (!iStorageManager.IsSignIn())  // wasn't signed in.
            {
                this.PinSelectedFileList.Clear();

                uiPinFileListGrid.Visibility = Visibility.Collapsed;
                uiPinFileSignInPanel.Visibility = Visibility.Visible;
            }
            else  // already signed in.
            {
                uiPinFileListGrid.Visibility = Visibility.Visible;
                uiPinFileSignInPanel.Visibility = Visibility.Collapsed;

                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    if (!this.PinFileObjectViewModel.IsDataLoaded)
                        this.SetPinFileListAsync(iStorageManager, message, null);
                }
                else
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPinFileListGridView.Visibility = Visibility.Collapsed;
                        uiPinFileMessage.Text = AppResources.InternetUnavailableMessage;
                        uiPinFileMessage.Visibility = Visibility.Visible;
                    });
                }
            }
        }


        private async void SetPinFileListAsync(IStorageManager iStorageManager, string message, FileObjectViewItem folder)
        {
            // Set Mutex true and Show Process Indicator
            // Clear selected file and set pin button false.
            base.SetProgressRing(uiProgressRing, true);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                uiPinFileListGridView.Visibility = Visibility.Collapsed;
                uiPinFileMessage.Text = message;
                uiPinFileMessage.Visibility = Visibility.Visible;
            });
            this.PinSelectedFileList.Clear();

            try
            {
                // Wait Signin and Sync task
                await TaskHelper.WaitSignInTask(iStorageManager.GetStorageName());
                await TaskHelper.WaitTask(TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName());

                // Get files from current folder in the cloud.
                // If it is not null, set items.
                // Otherwise, show message
                if (folder == null)
                    this.CurrentFileObjectList = StorageExplorer.GetFilesFromRootFolder();
                else
                    this.CurrentFileObjectList = StorageExplorer.GetTreeForFolder(this.GetCloudStorageFileObjectById(folder.Id));
                if (this.CurrentFileObjectList != null)
                {
                    // If didn't change cloud mode while loading, set it to list.
                    // Set file list visible and current path.
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.PinFileObjectViewModel.IsDataLoaded = true;
                        uiPinFileListGridView.Visibility = Visibility.Visible;
                        uiPinFileCurrentPath.Text = StorageExplorer.GetCurrentPath();
                        this.PinFileObjectViewModel.SetItems(this.CurrentFileObjectList, true);
                    });


                    // If there exists file, show it.
                    // Otherwise, show no file message.
                    if (this.CurrentFileObjectList.Count > 0)
                    {
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            uiPinFileMessage.Visibility = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            uiPinFileMessage.Text = AppResources.NoFileInFolderMessage;
                        });
                    }
                }
                else
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPinFileListGridView.Visibility = Visibility.Collapsed;
                        uiPinFileMessage.Text = AppResources.BadLoadingFileMessage;
                        uiPinFileMessage.Visibility = Visibility.Visible;
                    });
                }
            }
            catch
            {
                uiPinFileListGridView.Visibility = Visibility.Collapsed;
                uiPinFileMessage.Text = AppResources.BadLoadingFileMessage;
                uiPinFileMessage.Visibility = Visibility.Visible;
            }
            base.SetProgressRing(uiProgressRing, false);
        }


        private FileObject GetCloudStorageFileObjectById(string fileId)
        {
            if (fileId == null) return null;
            for (int i = 0; i < this.CurrentFileObjectList.Count; i++)
            {
                if (this.CurrentFileObjectList[i] == null) return null;
                if (this.CurrentFileObjectList[i].Id == null) return null;
                if (this.CurrentFileObjectList[i].Id.Equals(fileId)) return this.CurrentFileObjectList[i];
            }
            return null;
        }


        ///// <summary>
        ///// This method is responsible for loading the individual input and output sections for each scenario.  This 
        ///// is based on navigating a hidden Frame to the ScenarioX.xaml page and then extracting out the input
        ///// and output sections into the respective UserControl on the main page.
        ///// </summary>
        ///// <param name="scenarioName"></param>
        //public void LoadExplorer(Type explorerClass, object parameter)
        //{
        //    // Load the ScenarioX.xaml file into the Frame.
        //    this.HiddenFrame.Navigate(explorerClass, parameter);

        //    // Get the top element, the Page, so we can look up the elements
        //    // that represent the input and output sections of the ScenarioX file.
        //    Page hiddenPage = HiddenFrame.Content as Page;

        //    // Get each element.
        //    UIElement contentGrid = hiddenPage.FindName("uiContentGrid") as UIElement;

        //    if (contentGrid == null)
        //    {
        //        // Malformed input section.
        //        return;
        //    }

        //    // Find the LayoutRoot which parents the input and output sections in the main page.
        //    Panel panel = hiddenPage.FindName("uiLayoutRoot") as Panel;

        //    if (panel != null)
        //    {
        //        // Get rid of the content that is currently in the intput and output sections.
        //        panel.Children.Remove(contentGrid);

        //        // Populate the input and output sections with the newly loaded content.
        //        uiExplorerUserControl.Content = contentGrid;
        //    }
        //    else
        //    {
        //        // Malformed Scenario file.
        //    }
        //}


        //private void PopulateExplorerList()
        //{
        //    ObservableCollection<object> ExplorerBindingList = new ObservableCollection<object>();

        //    // Populate the ListBox with the list of scenarios as defined in Constants.cs.
        //    foreach (Explorer s in ExplorerList)
        //    {
        //        ListViewItem item = new ListViewItem();
        //        item.Content = s;
        //        item.Name = s.ClassType.FullName;
        //        item.FontSize = 22;
        //        item.Foreground = new SolidColorBrush(ColorHexStringToBrushConverter.GetColorFromHexString("919FA6"));
        //        ExplorerBindingList.Add(item);
        //    }

        //    // Bind the ListBox to the scenario list.
        //    uiExplorerList.ItemsSource = ExplorerBindingList;

        //    // Starting scenario is the first or based upon a previous selection.
        //    int startingScenarioIndex = -1;

        //    if (App.ApplicationSessions.Contains(SELECTED_EXPLORER_INDEX_KEY))
        //    {
        //        int selectedScenarioIndex = Convert.ToInt32(App.ApplicationSessions[SELECTED_EXPLORER_INDEX_KEY]);
        //        startingScenarioIndex = selectedScenarioIndex;
        //    }
        //    uiExplorerList.SelectedIndex = startingScenarioIndex != -1 ? startingScenarioIndex : 0;
        //}


        private void uiPickDeleteAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
        }


        private void uiPinAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
        }

        #endregion

    }
}
