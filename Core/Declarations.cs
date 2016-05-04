//The MIT License(MIT)

//copyright(c) 2016 Alberto Rodriguez

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;

namespace LiveCharts
{
    public enum AxisLimitsMode
    {
        Stretch, HalfSparator, OneSeparator
    }

    public enum AxisPosition
    {
        LeftBottom, RightTop
    }

    public enum SeparationState
    {
        Remove,
        Keep,
        InitialAdd
    }

    public enum ZoomingOptions
    {
        None,
        X,
        Y,
        Xy
    }

    public enum LegendLocation
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }

    public struct LvcPoint
    {
        public LvcPoint(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        public LvcPoint(LvcPoint point): this()
        {
            X = point.X;
            Y = point.Y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public static LvcPoint operator +(LvcPoint p1, LvcPoint p2)
        {
            return new LvcPoint(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static LvcPoint operator -(LvcPoint p1, LvcPoint p2)
        {
            return new LvcPoint(p1.X - p2.X, p1.Y - p2.Y);
        }
    }

    public struct LvcSize
    {
        public LvcSize(double width, double heigth) : this()
        {
            Width = width;
            Height = heigth;
        }

        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class LvcRectangle
    {
        private double _left;
        private double _top;
        private double _width;
        private double _height;

        public LvcRectangle()
        {
            
        }

        public LvcRectangle(double left, double top, double width, double height) : this()
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public event Action<double> SetTop;
        public event Action<double> SetLeft;
        public event Action<double> SetWidth;
        public event Action<double> SetHeight;

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                if (SetLeft != null) SetLeft.Invoke(value);
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                if (SetTop != null) SetTop.Invoke(value);
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                if (SetWidth != null) SetWidth.Invoke(value);
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                if (SetHeight != null) SetHeight.Invoke(value);
            }
        }
    }

    public interface IChartUpdater
    {
        ChartCore Chart { get; set; }
        void Run(bool restartView = false);
        void Cancel();
        void UpdateFrequency();
    }

    public interface IObservableChartPoint
    {
        event Action<object> PointChanged;
    }

    public interface ICartesianSeries
    {
        AxisLimitsMode XAxisMode { get; set; }
        AxisLimitsMode YAxisMode { get; set; }
    }
}