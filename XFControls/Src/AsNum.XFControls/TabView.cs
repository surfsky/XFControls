using AsNum.XFControls.Behaviors;
using AsNum.XFControls.Binders;
using AsNum.XFControls.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace AsNum.XFControls
{
    /// <summary>
    /// 
    /// </summary>
    public enum TabViewPositions
    {
        /// <summary>顶部</summary>
        Top,
        /// <summary>底部</summary>
        Bottom,
        /// <summary>左</summary>
        Left,
        /// <summary>右</summary>
        Right
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TabViewTransitionTypes
    {
        /// <summary>无动画</summary>
        None,
        /// <summary>淡入淡出</summary>
        Fade
    }

    /// <summary>
    /// 
    /// </summary>
    class TabViewAnimationTypeFactory
    {
        public static SelectChangeBehaviorBase GetBehavior(TabViewTransitionTypes transitionType)
        {
            switch (transitionType)
            {
                case TabViewTransitionTypes.None:
                    return new VisibilityBehavior();
                case TabViewTransitionTypes.Fade:
                    return new FadeBehavior();
            }
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 选项卡
    /// </summary>
    [ContentProperty("Pages")]
    public class TabView : Grid
    {
        // private
        private bool IsInnerUpdate = false;
        private Grid PageContainer = null;/// 子视图容器
        private ScrollView TabBarScroller = null;/// 标签容器的父容器, 如果标签过多，可以滚
        private StackLayout TabBarInner = null;/// 内层标签容器
        private ContentView TabBar = null;/// 外层标签容器

        // BindableProperty
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(TabView), null, propertyChanged: ItemsSourceChanged);
        public static readonly BindableProperty TabBarControlTemplateProperty = BindableProperty.Create("TabBarControlTemplate", typeof(ControlTemplate), typeof(TabView), new TabViewTabBarControlTemplate(), BindingMode.Default, propertyChanged: TabBarControlChanged);
        public static readonly BindableProperty TabTemplateProperty = BindableProperty.Create("TabTemplate", typeof(DataTemplate), typeof(TabView));
        public static readonly BindableProperty TabTemplateSelectorProperty = BindableProperty.Create("TabTemplateSelector", typeof(DataTemplateSelector), typeof(TabView), null);
        public static readonly BindableProperty TabControlTemplateProperty = BindableProperty.Create("TabControlTemplate", typeof(ControlTemplate), typeof(TabView), new TabViewTabControlTemplate());
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(TabView), null);
        public static readonly BindableProperty ItemTemplateSelectorProperty = BindableProperty.Create("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(TabView), null);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(TabView), null, BindingMode.TwoWay, propertyChanged: SelectedItemChanged);
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create("SelectedIndex", typeof(int), typeof(TabView), -1, propertyChanged: SelectedIndexChanged);
        public static readonly BindableProperty TabWidthRequestProperty = BindableProperty.Create("TabWidthRequest", typeof(double), typeof(TabView), 80D);
        public static readonly BindableProperty TabHeightRequestProperty = BindableProperty.Create("TabHeightRequest", typeof(double), typeof(TabView), 40D);
        public static readonly BindableProperty TabPositionProperty = BindableProperty.Create("TabPosition", typeof(TabViewPositions), typeof(TabView), TabViewPositions.Top, propertyChanged: TabPositionChanged);
        public static readonly BindableProperty TabBarBackgroundColorProperty = BindableProperty.Create("TabBarBackgroundColor", typeof(Color), typeof(TabView), Color.FromHex("EEEEEE"));
        public static readonly BindableProperty TransitionTypeProperty = BindableProperty.Create("TransitionType", typeof(TabViewTransitionTypes), typeof(TabView), TabViewTransitionTypes.Fade, propertyChanged: TransitionChanged);


        #region Property
        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tv = (TabView)bindable;
            tv.WrapItemsSource();
            var a = tv.SelectedItem;
            //var p = tv.TabPages.ElementAtOrDefault(tv.SelectedIndex) ?? tv.TabPages.FirstOrDefault();
            var p = tv.TabPages.OrderBy(t => ((TabPageView)t).Index).ElementAtOrDefault(tv.SelectedIndex) ?? tv.TabPages.FirstOrDefault();
            tv.TabSelectedCmd.Execute(p);
        }

        /// <summary>
        /// 标签条的控件模板
        /// </summary>
        public ControlTemplate TabBarControlTemplate
        {
            get { return (ControlTemplate)this.GetValue(TabBarControlTemplateProperty); }
            set { this.SetValue(TabBarControlTemplateProperty, value); }
        }

        private static void TabBarControlChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tv = (TabView)bindable;
            tv.TabBar.ControlTemplate = (ControlTemplate)newValue;
        }

        /// <summary>
        /// 标签头的数据模板
        /// </summary>
        public DataTemplate TabTemplate
        {
            get { return (DataTemplate)GetValue(TabTemplateProperty); }
            set { SetValue(TabTemplateProperty, value); }
        }

        /// <summary>
        /// 标签头模板选择器
        /// </summary>
        public DataTemplateSelector TabTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TabTemplateSelectorProperty); }
            set { SetValue(TabTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// 标签头的控件模板
        /// </summary>
        public ControlTemplate TabControlTemplate
        {
            get { return (ControlTemplate)this.GetValue(TabControlTemplateProperty); }
            set { this.SetValue(TabControlTemplateProperty, value); }
        }

        /// <summary>
        /// 标签页数据模板
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// 标签页模板选择器
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }


        /// <summary>
        /// 选中的数据
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null)
                return;

            var tv = (TabView)bindable;
            if (tv.IsInnerUpdate)
                return;

            var p = tv.TabPages.FirstOrDefault(t => t.BindingContext.Equals(newValue)) ?? tv.TabPages.FirstOrDefault();
            tv.UpdateSelected((TabPageView)p);
        }


        /// <summary>
        /// 当前选中的序号
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)this.GetValue(SelectedIndexProperty); }
            set { this.SetValue(SelectedIndexProperty, value); }
        }

        private static void SelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tv = (TabView)bindable;
            if (tv.IsInnerUpdate)
                return;

            //RaiseChild 会改变 Children (TabPages) 的顺序, 所以,不能直接按顺序取
            //var page = (TabPageView)(tv.TabPages.ElementAtOrDefault((int)newValue) ?? tv.TabPages.FirstOrDefault());
            var page = (TabPageView)(tv.TabPages.OrderBy(t => ((TabPageView)t).Index).ElementAtOrDefault((int)newValue) ?? tv.TabPages.FirstOrDefault());
            tv.UpdateSelected(page);
        }


        /// <summary>
        /// 标签头宽度 默认80
        /// </summary>
        public double TabWidthRequest
        {
            get { return (double)this.GetValue(TabWidthRequestProperty); }
            set { this.SetValue(TabWidthRequestProperty, value); }
        }


        /// <summary>
        /// 标签头高度,默认40
        /// </summary>
        public double TabHeightRequest
        {
            get { return (double)this.GetValue(TabHeightRequestProperty); }
            set { this.SetValue(TabHeightRequestProperty, value); }
        }

        /// <summary>
        /// 标签条的位置,默认 Top
        /// </summary>
        public TabViewPositions TabPosition
        {
            get { return (TabViewPositions)(this.GetValue(TabPositionProperty)); }
            set { this.SetValue(TabPositionProperty, value);  }
        }

        private static void TabPositionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tv = (TabView)bindable;
            tv.UpdatePosition();
        }


        /// <summary>
        /// 标签条的背景颜色,默认透明
        /// </summary>
        public Color TabBarBackgroundColor
        {
            get { return (Color)this.GetValue(TabBarBackgroundColorProperty); }
            set { this.SetValue(TabBarBackgroundColorProperty, value); }
        }

        /// <summary>
        /// 转场动画类型,默认 Fade
        /// </summary>
        public TabViewTransitionTypes TransitionType
        {
            get { return (TabViewTransitionTypes)this.GetValue(TransitionTypeProperty); }
            set { this.SetValue(TransitionTypeProperty, value); }
        }

        private static void TransitionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tv = (TabView)bindable;
            foreach (var p in tv.TabPages)
            {
                tv.SetTransition((TabPageView)p);
            }
        }

        /// <summary>
        /// 使用 Pages 就不能使用 ItemsSource
        /// </summary>
        public ObservableCollection<TabPageView> Pages { get; } = new ObservableCollection<TabPageView>();

        /// <summary>
        /// 标签头的 Tap 触发命令，内部使用，用于切换标签
        /// </summary>
        private ICommand TabSelectedCmd { get; }

        /// <summary>
        /// 标签集合, View 即 TabPageView
        /// </summary>
        public IReadOnlyCollection<View> TabPages { get; private set; }


        /// <summary>
        /// 当前选中的标签
        /// </summary>
        public TabPageView CurrentTabPage { get; private set; }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        public TabView()
        {
            this.PrepareLayout();
            this.TabSelectedCmd = new Command(o => {this.UpdateSelected((TabPageView)o);});
            this.WrapItemsSource();
            this.Pages.CollectionChanged += Pages_CollectionChanged;
        }

        private void Pages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.ItemsSource == null)
                this.ItemsSource = this.Pages;
            else if (!this.ItemsSource.Equals(this.Pages))
                throw new Exception("Can't set TabView Pages when ItemsSource is not null");
        }

        private void UpdateSelected(TabPageView o)
        {
            if (o == null)
                return;

            this.IsInnerUpdate = true;
            var item = (TabPageView)o;
            if (item.Index == this.SelectedIndex &&
                item.BindingContext != null &&
                item.BindingContext.Equals(this.SelectedItem))
                return;

            if (this.CurrentTabPage != null)
            {
                this.CurrentTabPage.IsSelected = false;
                this.NotifySelected(this.CurrentTabPage.BindingContext, false);
            }


            item.IsSelected = true;
            if (item.BindingContext != null && !item.BindingContext.Equals(this.SelectedItem))
                this.SelectedItem = item.BindingContext;
            this.NotifySelected(item.BindingContext, true);
            if (item.Index != this.SelectedIndex)
                this.SelectedIndex = item.Index;
            this.CurrentTabPage = item;
            //IOS下,必须加上这一句,否则会导至 item 不可操作
            this.PageContainer.RaiseChild(item);
            this.IsInnerUpdate = false;
        }


        private void NotifySelected(object data, bool isSelected)
        {
            if (data != null && (data is ISelectable))
            {
                var s = (ISelectable)data;
                if (s.IsSelected != isSelected)
                {
                    s.IsSelected = isSelected;
                    s.NotifyOfPropertyChange(nameof(s.IsSelected));
                    var cmd = isSelected ? s.SelectedCommand : s.UnSelectedCommand;
                    if (cmd != null)
                    {
                        cmd.Execute(null);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private TabPageView GetTab(object data, int idx)
        {
            TabPageView item;
            if (data is TabPageView)
                item = (TabPageView)data;
            else
            {
                item = new TabPageView()
                {
                    Index = idx,
                    BindingContext = data
                };

                #region headView 用方法重构掉
                View headView = null;

                if (this.TabTemplate != null || this.TabTemplateSelector != null)
                {
                    //优先使用 TemplateSelector
                    if (this.TabTemplateSelector != null)
                    {
                        // SelectTemplate 的第二个参数，即 TemplateSelector 的 OnSelectTemplate 方法的 container 参数
                        headView = (View)this.TabTemplateSelector.SelectTemplate(data, item).CreateContent();
                    }
                    else if (this.TabTemplate != null)
                        headView = (View)this.TabTemplate.CreateContent();

                    if (headView != null)
                    {
                        //上下文
                        headView.BindingContext = data;
                    }
                }

                if (headView == null)
                    headView = new Label() { Text = "Tab" };


                //item.Header = headView;
                item.Header = new ContentView()
                {
                    Content = headView,
                    BindingContext = item
                };

                //添加手势
                TapBinder.SetCmd(headView, this.TabSelectedCmd);
                TapBinder.SetParam(headView, item);
                #endregion

                #region bodyView
                View bodyView = null;
                if (this.ItemTemplate != null || this.ItemTemplateSelector != null)
                {
                    if (this.ItemTemplateSelector != null)
                    {
                        bodyView = (View)this.ItemTemplateSelector.SelectTemplate(data, item).CreateContent();
                    }
                    else if (this.ItemTemplate != null)
                    {
                        bodyView = (View)this.ItemTemplate.CreateContent();
                    }

                    if (bodyView != null)
                        bodyView.BindingContext = data;
                }
                if (bodyView == null)
                    bodyView = new Label() { Text = "Body" };

                item.Content = bodyView;
                #endregion
            }


            item.Header.SetBinding(ContentView.ControlTemplateProperty, new Binding(nameof(this.TabControlTemplate), source: this));
            item.Header.SetBinding(View.WidthRequestProperty, new Binding("TabWidthRequest", source: this));
            item.Header.SetBinding(View.HeightRequestProperty, new Binding("TabHeightRequest", source: this));
            item.SetBinding(TabPageView.TabPositionProperty, new Binding(nameof(TabPosition), source: this));
            this.SetTransition(item);
            return item;
        }

        private void SetTransition(TabPageView tab)
        {
            var behavior = TabViewAnimationTypeFactory.GetBehavior(this.TransitionType);
            behavior.SetBinding(SelectChangeBehaviorBase.IsSelectedProperty, new Binding("IsSelected", BindingMode.TwoWay, source: tab));
            tab.Content.Behaviors.Add(behavior);
        }

        /// <summary>
        /// 准备布局
        /// </summary>
        private void PrepareLayout()
        {
            this.RowSpacing = 0;
            this.ColumnSpacing = 0;

            #region 九宫格
            this.RowDefinitions = new RowDefinitionCollection() {
                new RowDefinition() { Height = GridLength.Auto },
                new RowDefinition(),
                new RowDefinition() { Height = GridLength.Auto }
            };

            this.ColumnDefinitions = new ColumnDefinitionCollection() {
                new ColumnDefinition() {Width = GridLength.Auto },
                new ColumnDefinition(),
                new ColumnDefinition() {Width = GridLength.Auto }
            };
            #endregion

            #region 
            this.PageContainer = new Grid();
            this.TabBar = new ContentView();
            this.TabBar.SetBinding(ContentView.BackgroundColorProperty, new Binding(nameof(this.TabBarBackgroundColor), source: this));

            this.TabBarScroller = new ScrollView();
            this.TabBar.Content = this.TabBarScroller;

            this.TabBarInner = new StackLayout()
            {
                Spacing = 0
            };
            this.TabBarScroller.Content = this.TabBarInner;

            this.Children.Add(this.PageContainer);
            this.Children.Add(this.TabBar);

            this.UpdateTabPosition();
            this.UpdateChildrenPosition();
            #endregion

            this.TabPages = new ReadOnlyCollection<View>(this.PageContainer.Children);
        }


        /// <summary>
        /// 更新标签、主体位置
        /// </summary>
        private void UpdatePosition()
        {
            this.BatchBegin();
            this.UpdateTabPosition();
            this.UpdateChildrenPosition();
            this.BatchCommit();
        }

        /// <summary>
        /// 更新标签位置
        /// </summary>
        private void UpdateTabPosition()
        {
            int row = 0, col = 0, colSpan = 1, rowSpan = 1;
            ScrollOrientation orientation = ScrollOrientation.Horizontal;
            StackOrientation orientation2 = StackOrientation.Horizontal;
            switch (this.TabPosition)
            {
                case TabViewPositions.Top:
                    row = 0;
                    col = 0;
                    colSpan = 3;
                    rowSpan = 1;
                    orientation = ScrollOrientation.Horizontal;
                    orientation2 = StackOrientation.Horizontal;
                    break;
                case TabViewPositions.Bottom:
                    row = 2;
                    col = 0;
                    colSpan = 3;
                    rowSpan = 1;
                    orientation = ScrollOrientation.Horizontal;
                    orientation2 = StackOrientation.Horizontal;
                    break;
                case TabViewPositions.Left:
                    row = 0;
                    col = 0;
                    rowSpan = 3;
                    colSpan = 1;
                    orientation = ScrollOrientation.Vertical;
                    orientation2 = StackOrientation.Vertical;
                    break;
                case TabViewPositions.Right:
                    row = 0;
                    col = 2;
                    rowSpan = 3;
                    colSpan = 1;
                    orientation = ScrollOrientation.Vertical;
                    orientation2 = StackOrientation.Vertical;
                    break;
            }

            this.TabBarScroller.Orientation = orientation;
            this.TabBarScroller.HorizontalOptions = LayoutOptions.Fill;
            this.TabBarScroller.VerticalOptions = LayoutOptions.Fill;

            this.TabBarInner.Orientation = orientation2;
            if (this.TabBarInner.Orientation == StackOrientation.Horizontal)
            {
                this.TabBarInner.HorizontalOptions = LayoutOptions.Center;
                this.TabBarInner.VerticalOptions = LayoutOptions.Center;
            }
            else
            {
                this.TabBarInner.HorizontalOptions = LayoutOptions.Center;
                this.TabBarInner.VerticalOptions = LayoutOptions.Start;
            }

            Grid.SetRow(this.TabBar, row);
            Grid.SetColumn(this.TabBar, col);
            Grid.SetRowSpan(this.TabBar, rowSpan);
            Grid.SetColumnSpan(this.TabBar, colSpan);
        }


        /// <summary>
        /// 更新主体位置
        /// </summary>
        private void UpdateChildrenPosition()
        {
            int row = 0, col = 0, colSpan = 0, rowSpan = 0;

            switch (this.TabPosition)
            {
                case TabViewPositions.Top:
                    row = 1;
                    col = 0;
                    colSpan = 3;
                    rowSpan = 2;
                    break;
                case TabViewPositions.Bottom:
                    row = 0;
                    col = 0;
                    colSpan = 3;
                    rowSpan = 2;
                    break;
                case TabViewPositions.Left:
                    row = 0;
                    col = 1;
                    rowSpan = 3;
                    colSpan = 2;
                    break;
                case TabViewPositions.Right:
                    row = 0;
                    col = 0;
                    rowSpan = 3;
                    colSpan = 2;
                    break;
            }
            Grid.SetRow(this.PageContainer, row);
            Grid.SetColumn(this.PageContainer, col);
            Grid.SetRowSpan(this.PageContainer, rowSpan);
            Grid.SetColumnSpan(this.PageContainer, colSpan);
        }




        #region 数据源变动事件
        /// <summary>
        /// 订阅数据源变化通知
        /// </summary>
        private void WrapItemsSource()
        {
            new NotifyCollectionWrapper(
                this.ItemsSource,
                add: (datas, idx) => this.Add(datas, idx),
                remove: (datas, idx) => this.Remove(datas, idx),
                reset: () => this.Reset());
        }

        private void Add(object d, int i)
        {
            var v = this.GetTab(d, i);
            this.TabBarInner.Children.Add(v.Header);
            this.PageContainer.Children.Add(v);
        }

        private void Insert(object d, int i)
        {
            var v = this.GetTab(d, i);
            this.TabBarInner.Children.Insert(i, v.Header);
            this.PageContainer.Children.Insert(i, v);///////
        }

        private void Remove(int i)
        {
            this.TabBarInner.Children.RemoveAt(i);
            this.PageContainer.Children.RemoveAt(i);
        }

        object o = new object();
        private void Add(IList datas, int idx)
        {
            System.Threading.Monitor.Enter(o);
            var c = this.TabBarInner.Children.Count;

            foreach (var d in datas)
            {
                var i = idx++;
                if (i < c)
                {
                    this.Insert(d, i);
                }
                else
                {
                    this.Add(d, i);
                }
            }
            System.Threading.Monitor.Exit(o);
        }

        private void Remove(IList datas, int idx)
        {
            for (var i = idx; i < datas.Count; i++)
            {
                this.Remove(i);
            }
        }

        private void Reset()
        {
            this.TabBarInner.Children.Clear();
            this.PageContainer.Children.Clear();

            if (this.ItemsSource != null)
            {
                var idx = 0;
                foreach (var d in this.ItemsSource)
                {
                    this.Add(d, idx++);
                }
            }
        }
        #endregion

    }


}