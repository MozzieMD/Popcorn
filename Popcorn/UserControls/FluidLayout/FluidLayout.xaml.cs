using Popcorn.Model.Subtitle;
using Popcorn.UserControls.Subtitles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Popcorn.UserControls.FluidLayout
{
    /// <summary>
    /// Interaction logic for FluidLayout.xaml
    /// </summary>
    public partial class FluidLayout
    {
        public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register("Animation",
                                                                                                  typeof(bool),
                                                                                                  typeof(FluidLayout));

        public static readonly DependencyProperty AnimationSpeedProperty = DependencyProperty.Register(
            "AnimationSpeed", typeof(int), typeof(FluidLayout));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FluidLayout), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));


        private readonly Queue<UserControl> _addQueue = new Queue<UserControl>();
        private readonly Queue<FluidLayoutTile> _animationQueue = new Queue<FluidLayoutTile>();

        private readonly DispatcherTimer _resizeTimer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500),
            IsEnabled = false
        };

        private List<FluidLayoutTile> _childs = new List<FluidLayoutTile>();

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        public FluidLayout()
        {
            InitializeComponent();
            DataContext = this;
            _resizeTimer.Tick += ResizeTimerTick;
        }

        public bool Animation
        {
            get { return (bool)GetValue(AnimationProperty); }
            set { SetValue(AnimationProperty, value); }
        }

        public int AnimationSpeed
        {
            get { return (int)GetValue(AnimationSpeedProperty); }
            set { SetValue(AnimationSpeedProperty, value); }
        }

        public void Add(UserControl uc)
        {
            var e = new FluidLayoutTile(uc);
            e.ResetLayout();
            e.OnSizeChanged += TileOnOnSizeChanged;
            FluidLayoutGrid.Children.Add(e.Element);
            _childs.Add(e);
            Refresh();
        }

        public void Add(List<UserControl> ucs)
        {
            if (_childs.Count(c => c.RunningAnimation) == 0)
            {
                foreach (FluidLayoutTile e in ucs.Select(uc => new FluidLayoutTile(uc)))
                {
                    e.ResetLayout();
                    e.OnSizeChanged += TileOnOnSizeChanged;
                    FluidLayoutGrid.Children.Add(e.Element);
                    _childs.Add(e);
                }
                Refresh();
            }
            else
            {
                foreach (UserControl uc in ucs)
                {
                    _addQueue.Enqueue(uc);
                }
            }
        }

        public void Remove(UserControl uc)
        {
            _childs.Remove(_childs.FirstOrDefault(child => Equals(child.Element, uc)));
            FluidLayoutGrid.Children.Remove(
                FluidLayoutGrid.Children.Cast<UserControl>().FirstOrDefault(control => Equals(control, uc)));
            Refresh();
        }

        public void Remove(List<UserControl> ucs)
        {
            foreach (UserControl uc in ucs)
            {
                _childs.Remove(_childs.FirstOrDefault(child => Equals(child.Element, uc)));
                FluidLayoutGrid.Children.Remove(
                    FluidLayoutGrid.Children.Cast<UserControl>().FirstOrDefault(control => Equals(control, uc)));
            }
            Refresh();
        }

        public void RemoveByProperty(string property)
        {
            var remove = new List<FluidLayoutTile>(_childs.Where(c => c.HasProperty(property)));
            foreach (FluidLayoutTile tile in remove)
            {
                _childs.Remove(tile);
                FluidLayoutGrid.Children.Remove(
                    FluidLayoutGrid.Children.Cast<UserControl>().FirstOrDefault(control => Equals(control, tile.Element)));
            }
            Refresh();
        }

        public void RemoveByPropertyValue(string property, object value)
        {
            var remove = new List<FluidLayoutTile>(_childs.Where(c => c.HasProperty(property)));
            foreach (FluidLayoutTile tile in remove)
            {
                if (Equals(tile.GetProperty(property), value))
                {
                    _childs.Remove(tile);
                    FluidLayoutGrid.Children.Remove(
                        FluidLayoutGrid.Children.Cast<UserControl>().FirstOrDefault(control => Equals(control, tile.Element)));
                }
            }
            Refresh();
        }

        public void RemoveAll()
        {
            _childs.Clear();
            FluidLayoutGrid.Children.Clear();
            Refresh();
        }

        public void Refresh()
        {
            MakePos();
            foreach (FluidLayoutTile child in _childs)
            {
                MoveTo(child);
            }
        }

        public void SortByProperty(string property, bool reverse = false)
        {
            var order =
                new List<FluidLayoutTile>(
                    _childs.OrderBy(c => c.HasProperty(property) ? c.GetProperty(property) : null));
            if (reverse) order.Reverse();
            _childs = order;
            Refresh();
        }

        public void SortByMethod(string method, bool reverse = false)
        {
            var order =
                new List<FluidLayoutTile>(_childs.OrderBy(kvp => kvp.HasMethod(method) ? kvp.CallMethod(method) : null));
            if (reverse) order.Reverse();
            _childs = order;
            Refresh();
        }

        private void AnimationComplete()
        {
            if (_childs.Count(c => c.RunningAnimation) == 0)
            {
                var ucs = new List<UserControl>();
                foreach (UserControl uc in _addQueue.ToList())
                {
                    ucs.Add(uc);
                    _addQueue.Dequeue();
                }
                Add(ucs);
                foreach (FluidLayoutTile child in _animationQueue.ToList())
                {
                    MoveTo(child);
                    _animationQueue.Dequeue();
                }
            }
        }

        private void TileOnOnSizeChanged(object sender, FluidLayoutEventArgs.SizeChanged sizeChangedEventArgs)
        {
            Refresh();
        }

        private void MoveTo(FluidLayoutTile child)
        {
            if (_childs.Count(c => c.RunningAnimation) == 0)
            {
                if (Animation)
                {
                    child.AnimatePosition(new Thickness(child.NewPosition.Left, child.NewPosition.Top, 0, 0),
                                          new TimeSpan(0, 0, 0, 0, AnimationSpeed), AnimationComplete);
                }
                else
                {
                    child.Position = child.NewPosition;
                }
            }
            else
            {
                _animationQueue.Enqueue(child);
            }
        }

        private int[] MatrixTrimWidth(int[] a, int[] b)
        {
            if (a[0] >= b[0] && a[0] < b[1] || a[1] >= b[0] && a[1] < b[1])
            {
                if (a[0] >= b[0] && a[0] < b[1])
                {
                    a[0] = b[1];
                }
                else
                {
                    a[1] = b[0];
                }
            }
            return a;
        }

        private List<int[]> MatrixJoin(List<int[]> mtx, int[] cell)
        {
            List<int[]> tMtx = mtx;
            tMtx.Add(cell);
            tMtx.Sort(MatrixSortX);
            var mtxJoin = new List<int[]>();


            for (int i = 0, imax = tMtx.Count; i < imax; i++)
            {
                if (mtxJoin.Count > 0
                    && mtxJoin[mtxJoin.Count - 1][1] == tMtx[i][0]
                    && mtxJoin[mtxJoin.Count - 1][2] == tMtx[i][2])
                {
                    mtxJoin[mtxJoin.Count - 1][1] = tMtx[i][1];
                }
                else
                {
                    mtxJoin.Add(tMtx[i]);
                }
            }
            return mtxJoin;
        }

        private List<int[]> UpdateAttachArea(List<int[]> mtx, int[] point, int[] size)
        {
            List<int[]> tMtx = mtx;
            tMtx.Sort(MatrixSortDepth);
            int[] cell = { point[0], point[0] + size[0], point[1] + size[1] };
            for (int i = 0, imax = tMtx.Count; i < imax; i++)
            {
                if (tMtx.Count - 1 >= i)
                {
                    if (cell[0] <= tMtx[i][0] && tMtx[i][1] <= cell[1])
                    {
                        tMtx.RemoveAt(i);
                    }
                    else
                    {
                        tMtx[i] = MatrixTrimWidth(tMtx[i], cell);
                    }
                }
            }
            return MatrixJoin(tMtx, cell);
        }

        private int MatrixSortDepth(int[] a, int[] b)
        {
            return ((a[2] == b[2] && a[0] > b[0]) || a[2] > b[2]) ? 1 : -1;
        }

        private int MatrixSortX(int[] a, int[] b)
        {
            return (a[0] > b[0]) ? 1 : -1;
        }

        private int[] GetAttachPoint(List<int[]> mtx, int width)
        {
            List<int[]> tMtx = mtx;
            tMtx.Sort(MatrixSortDepth);
            int max = tMtx[tMtx.Count - 1][2];
            for (int i = 0, imax = tMtx.Count; i < imax; i++)
            {
                if (tMtx[i][2] >= max) break;
                if (tMtx[i][1] - tMtx[i][0] >= width)
                {
                    return new[] { tMtx[i][0], tMtx[i][2] };
                }
            }
            return new[] { 0, max };
        }

        private void MakePos()
        {
            int width = Convert.ToInt32(ActualWidth);
            var matrix = new List<int[]> { new[] { 0, width, 0 } };
            int hmax = 0;
            foreach (FluidLayoutTile child in _childs)
            {
                var size = new[] { child.Width, child.Height };
                int[] point = GetAttachPoint(matrix, size[0]);
                matrix = UpdateAttachArea(matrix, point, size);
                hmax = Math.Max(hmax, point[1] + size[1]);
                child.NewPosition = new Tile.Position { Left = point[0], Top = point[1] };
            }
            Height = hmax;
        }

        private void FluidLayoutControlSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            _resizeTimer.IsEnabled = true;
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        private void ResizeTimerTick(object sender, System.EventArgs eventArgs)
        {
            _resizeTimer.IsEnabled = false;
            Refresh();
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as FluidLayout;
            if (control != null)
                control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var newItems = newValue as ObservableCollection<Subtitle>;
            if (newItems != null)
            {
                var ucs = new List<UserControl>();
                foreach (var item in newItems)
                {
                    var uc = new TileSubtitle();
                    uc.DataContext = item;
                    ucs.Add(uc);
                }
                RemoveAll();
                Add(ucs);
            }
        }
    }
}