using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Popcorn.UserControls.FluidLayout
{
    internal class FluidLayoutTile
    {
        #region Delegates

        public delegate void PositionChangedHandler(object sender, FluidLayoutEventArgs.PositionChanged e);

        public delegate void SizeChangedHandler(object sender, FluidLayoutEventArgs.SizeChanged e);

        #endregion

        private UserControl _element;

        public FluidLayoutTile(UserControl element)
        {
            Element = element;
        }

        public UserControl Element
        {
            get { return _element; }
            set
            {
                _element = value;
                _element.SizeChanged += ElementOnSizeChanged;
            }
        }

        public int Width
        {
            get { return Convert.ToInt32(Element.Width); }
            set { Element.Width = value; }
        }

        public int Height
        {
            get { return Convert.ToInt32(Element.Height); }
            set { Element.Height = value; }
        }

        public int Top
        {
            get { return Convert.ToInt32(Element.Margin.Top); }
            set
            {
                Element.Margin = new Thickness(Left, value, 0.0, 0.0);
                UpdatePosition();
            }
        }

        public Tile.Position Position
        {
            get
            {
                return new Tile.Position
                {
                    Top = Top,
                    Left = Left
                };
            }
            set
            {
                Element.Margin = new Thickness(value.Left, value.Top, 0.0, 0.0);
                UpdatePosition();
            }
        }

        public Tile.Dimensions Dimensions
        {
            get
            {
                return new Tile.Dimensions
                {
                    Height = Height,
                    Width = Width
                };
            }
            set
            {
                Element.Height = value.Height;
                Element.Width = value.Width;
            }
        }

        public int Left
        {
            get { return Convert.ToInt32(Element.Margin.Left); }
            set
            {
                Element.Margin = new Thickness(value, Top, 0.0, 0.0);
                UpdatePosition();
            }
        }

        public Tile.Position NewPosition { get; set; }
        public bool RunningAnimation { get; set; }

        public bool HasMethod(string method)
        {
            return Element.GetType().GetMethod(method) != null;
        }

        public object CallMethod(string method, object[] param = null)
        {
            if (HasMethod(method))
            {
                MethodInfo m = Element.GetType().GetMethod(method);
                return m.Invoke(this, param);
            }
            return null;
        }

        public bool HasProperty(string property)
        {
            return Element.GetType().GetProperty(property) != null;
        }

        public void SetProperty(string property, object value)
        {
            if (HasProperty(property))
            {
                Element.GetType().GetProperty(property).SetValue(Element, value, null);
            }
        }

        public object GetProperty(string property)
        {
            if (HasProperty(property))
            {
                return Element.GetType().GetProperty(property).GetValue(Element);
            }
            return null;
        }

        private void ElementOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdateSize(sizeChangedEventArgs.PreviousSize);
        }

        public event PositionChangedHandler OnPositionChanged;

        public event SizeChangedHandler OnSizeChanged;

        private void UpdateSize(Size previousSize)
        {
            if (OnSizeChanged == null)
                return;

            OnSizeChanged(this, new FluidLayoutEventArgs.SizeChanged(previousSize, new Size(Width, Height)));
        }

        private void UpdatePosition()
        {
            if (OnPositionChanged == null)
                return;
            OnPositionChanged(this, new FluidLayoutEventArgs.PositionChanged(Position));
        }

        public void AnimatePosition(Thickness toTickness, TimeSpan speed, Action completed = null)
        {
            if (Math.Abs(toTickness.Top - Top) > 1 || Math.Abs(toTickness.Left - Left) > 1)
            {
                var thicknessAnimation = new ThicknessAnimation
                {
                    From = new Thickness(Left, Top, 0, 0),
                    To = toTickness,
                    Duration = new Duration(speed)
                };
                Timeline.SetDesiredFrameRate(thicknessAnimation, 24);
                RunningAnimation = true;
                thicknessAnimation.Completed += (EventHandler)((o, e) =>
                {
                    RunningAnimation = false;
                    if (completed != null)
                    {
                        completed();
                    }
                });
                Element.BeginAnimation(FrameworkElement.MarginProperty, thicknessAnimation);
            }
        }

        public void ResetLayout()
        {
            Element.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
            Element.VerticalAlignment = VerticalAlignment.Top;
            Element.HorizontalAlignment = HorizontalAlignment.Left;
        }
    }
}
