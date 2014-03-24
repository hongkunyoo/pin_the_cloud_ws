using PintheCloudWS.Common;
using PintheCloudWS.Converters;
using PintheCloudWS.Locale;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private const string SELECTED_EXPLORER_INDEX_KEY = "SELECTED_EXPLORER_INDEX_KEY";
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        
        private Frame HiddenFrame = null;
        private SpotViewItem SpotViewItem = null;

        private List<Explorer> ExplorerList = new List<Explorer>
        {
            new Explorer() { Title = AppResources.Pick, ClassType = typeof(PickPage) },
            new Explorer() { Title = AppResources.Pin, ClassType = typeof(PinPage) },
        };


        public class Explorer
        {
            public string Title { get; set; }

            public Type ClassType { get; set; }

            public override string ToString()
            {
                return Title;
            }
        }



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

            this.HiddenFrame = new Windows.UI.Xaml.Controls.Frame();
            this.HiddenFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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
            this.SpotViewItem = e.Parameter as SpotViewItem;

            uiSpotNameText.Text = this.SpotViewItem.SpotName;
            uiAccountNameText.Text = this.SpotViewItem.AccountName;

            this.PopulateExplorerList();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        #region UI Methods

        private void uiSettingButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }


        private void uiExplorerList_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (uiExplorerList.SelectedItem != null)
            {
                App.ApplicationSessions[SELECTED_EXPLORER_INDEX_KEY] = uiExplorerList.SelectedIndex;
                ListViewItem selectedListBoxItem = uiExplorerList.SelectedItem as ListViewItem;
                Explorer explorer = selectedListBoxItem.Content as Explorer;
                this.LoadExplorer(explorer.ClassType, this.SpotViewItem);
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// This method is responsible for loading the individual input and output sections for each scenario.  This 
        /// is based on navigating a hidden Frame to the ScenarioX.xaml page and then extracting out the input
        /// and output sections into the respective UserControl on the main page.
        /// </summary>
        /// <param name="scenarioName"></param>
        public void LoadExplorer(Type explorerClass, object parameter)
        {
            // Load the ScenarioX.xaml file into the Frame.
            this.HiddenFrame.Navigate(explorerClass, parameter);

            // Get the top element, the Page, so we can look up the elements
            // that represent the input and output sections of the ScenarioX file.
            Page hiddenPage = HiddenFrame.Content as Page;

            // Get each element.
            UIElement contentGrid = hiddenPage.FindName("uiContentGrid") as UIElement;

            if (contentGrid == null)
            {
                // Malformed input section.
                return;
            }

            // Find the LayoutRoot which parents the input and output sections in the main page.
            Panel panel = hiddenPage.FindName("uiLayoutRoot") as Panel;

            if (panel != null)
            {
                // Get rid of the content that is currently in the intput and output sections.
                panel.Children.Remove(contentGrid);

                // Populate the input and output sections with the newly loaded content.
                uiExplorerUserControl.Content = contentGrid;
            }
            else
            {
                // Malformed Scenario file.
            }
        }


        private void PopulateExplorerList()
        {
            ObservableCollection<object> ExplorerBindingList = new ObservableCollection<object>();

            // Populate the ListBox with the list of scenarios as defined in Constants.cs.
            foreach (Explorer s in ExplorerList)
            {
                ListViewItem item = new ListViewItem();
                item.Content = s;
                item.Name = s.ClassType.FullName;
                item.FontSize = 22;
                item.Foreground = new SolidColorBrush(ColorHexStringToBrushConverter.GetColorFromHexString("919FA6"));
                ExplorerBindingList.Add(item);
            }

            // Bind the ListBox to the scenario list.
            uiExplorerList.ItemsSource = ExplorerBindingList;

            // Starting scenario is the first or based upon a previous selection.
            int startingScenarioIndex = -1;

            if (App.ApplicationSessions.Contains(SELECTED_EXPLORER_INDEX_KEY))
            {
                int selectedScenarioIndex = Convert.ToInt32(App.ApplicationSessions[SELECTED_EXPLORER_INDEX_KEY]);
                startingScenarioIndex = selectedScenarioIndex;
            }
            uiExplorerList.SelectedIndex = startingScenarioIndex != -1 ? startingScenarioIndex : 0;
        }

        #endregion

    }
}
