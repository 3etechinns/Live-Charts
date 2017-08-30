﻿//The MIT License(MIT)

//Copyright(c) 2016 Alberto Rodríguez Orozco & LiveCharts Contributors

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LiveCharts.Charts;
using LiveCharts.Data;
using LiveCharts.Definitions.Points;
using LiveCharts.Definitions.Series;

namespace LiveCharts.Wpf.PointViews
{
    /// <summary>
    /// Column point view class.
    /// </summary>
    /// <seealso cref="LiveCharts.Wpf.PointViews.PointView" />
    /// <seealso cref="LiveCharts.Definitions.Points.IRectanglePointView" />
    public class ColumnPointView : PointView, IRectanglePointView
    {
        public RectangleData Data { set => throw new NotImplementedException(); }
        public double ZeroReference { set => throw new NotImplementedException(); }

        public override void Draw(ChartPoint previousDrawn, int index, ISeriesView series, ChartCore chart)
        {
            var candleSeries = (CandleSeries)series;

            // map the series properties to the drawn point.
            CandleVisualShape.Stroke = candleSeries.Stroke;
            CandleVisualShape.StrokeThickness = candleSeries.StrokeThickness;
            CandleVisualShape.Visibility = candleSeries.Visibility;
            Panel.SetZIndex(CandleVisualShape, Panel.GetZIndex(candleSeries));

            // initialize or update the label.
            if (candleSeries.DataLabels)
            {
                Label = candleSeries.UpdateLabelContent(
                    new DataLabelViewModel
                    {
                        FormattedText = DesignerProperties.GetIsInDesignMode(candleSeries)
                            ? "'label'"
                            : candleSeries.LabelPoint(ChartPoint),
                        Point = ChartPoint
                    }, Label);
            }

            // erase data label if it is not required anymore.
            if (!candleSeries.DataLabels && Label != null)
            {
                // notice UpdateLabelContent() added the label to the UI, we need to remove it.
                chart.View.RemoveFromDrawMargin(Label);
                Label = null;
            }


            // register the area where the point interacts with the user (hover and click).
            ChartPoint.ResponsiveArea =
                new ResponsiveRectangle(
                    High, Left,
                    Width, Math.Abs(Low - High));


            //if (IsNew)
            {
                Canvas.SetTop(Rectangle, ZeroReference);
                Canvas.SetLeft(Rectangle, Data.Left);

                Rectangle.Width = Data.Width;
                Rectangle.Height = 0;
            }

            if (Label != null && double.IsNaN(Canvas.GetLeft(Label)))
            {
                Canvas.SetTop(Label, ZeroReference);
                Canvas.SetLeft(Label, ChartPoint.ChartLocation.X);
            }
          
            Func<double> getY = () =>
            {
                double y;

#pragma warning disable 618
                if (LabelPosition == BarLabelPosition.Parallel || LabelPosition == BarLabelPosition.Merged)
#pragma warning restore 618
                {
                    if (Transform == null)
                        Transform = new RotateTransform(270);

                    y = Data.Top + Data.Height/2 + Label.ActualWidth*.5;
                    Label.RenderTransform = Transform;
                }
                else if (LabelPosition == BarLabelPosition.Perpendicular)
                {
                    y = Data.Top + Data.Height/2 - Label.ActualHeight * .5;
                }
                else
                {
                    if (ZeroReference > Data.Top)
                    {
                        y = Data.Top - Label.ActualHeight;
                        if (y < 0) y = Data.Top;
                    }
                    else
                    {
                        y = Data.Top + Data.Height;
                        if (y + Label.ActualHeight > chart.View.DrawMarginHeight) y -= Label.ActualHeight;
                    }
                }

                return y;
            };

            Func<double> getX = () =>
            {
                double x;

#pragma warning disable 618
                if (LabelPosition == BarLabelPosition.Parallel || LabelPosition == BarLabelPosition.Merged)
#pragma warning restore 618
                {
                    x = Data.Left + Data.Width/2 - Label.ActualHeight/2;
                }
                else if (LabelPosition == BarLabelPosition.Perpendicular)
                {
                    x = Data.Left + Data.Width/2 - Label.ActualWidth/2;
                }
                else
                {
                    x = Data.Left + Data.Width / 2 - Label.ActualWidth / 2;
                    if (x < 0)
                        x = 2;
                    if (x + Label.ActualWidth > chart.View.DrawMarginWidth)
                        x -= x + Label.ActualWidth - chart.View.DrawMarginWidth + 2;
                }

                return x;
            };

            if (chart.View.DisableAnimations)
            {
                Rectangle.Width = Data.Width;
                Rectangle.Height = Data.Height;

                Canvas.SetTop(Rectangle, Data.Top);
                Canvas.SetLeft(Rectangle, Data.Left);

                if (Label != null)
                {
                    Label.UpdateLayout();

                    Canvas.SetTop(Label, getY());
                    Canvas.SetLeft(Label, getX());
                }

                return;
            }

            var animSpeed = chart.View.AnimationsSpeed;

            if (Label != null)
            {
                Label.UpdateLayout();

                Label.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(getX(), animSpeed));
                Label.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(getY(), animSpeed));
            }

            Rectangle.BeginAnimation(Canvas.LeftProperty, 
                new DoubleAnimation(Data.Left, animSpeed));
            Rectangle.BeginAnimation(Canvas.TopProperty,
                new DoubleAnimation(Data.Top, animSpeed));

            Rectangle.BeginAnimation(FrameworkElement.WidthProperty,
                new DoubleAnimation(Data.Width, animSpeed));
            Rectangle.BeginAnimation(FrameworkElement.HeightProperty,
                new DoubleAnimation(Data.Height, animSpeed));
        }

        public override void Erase(ChartCore chart)
        {
            chart.View.RemoveFromDrawMargin(Rectangle);
            chart.View.RemoveFromDrawMargin(Label);
        }

        public override void OnHover()
        {
            var copy = Rectangle.Fill.Clone();
            copy.Opacity -= .15;
            Rectangle.Fill = copy;
        }

        public override void OnHoverLeave()
        {
            if (Rectangle == null) return;

            if (ChartPoint.Fill != null)
            {
                Rectangle.Fill = (Brush) ChartPoint.Fill;
            }
            else
            {
                Rectangle.Fill = ((Series) ChartPoint.SeriesView).Fill;
            }
        }

        public override void OnSelection()
        {
            throw new NotImplementedException();
        }

        public override void OnSelectionLeave()
        {
            throw new NotImplementedException();
        }
    }
}
