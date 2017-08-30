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
using System.Windows.Shapes;
using LiveCharts.Configurations;
using LiveCharts.Definitions.Points;
using LiveCharts.Definitions.Series;
using LiveCharts.Dtos;
using LiveCharts.Series;
using LiveCharts.Wpf.PointViews;

namespace LiveCharts.Wpf
{
    /// <summary>
    /// Use the column series to plot horizontal bars in a cartesian chart
    /// </summary>
    public class ColumnSeries : Series, IColumnSeriesView
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of ColumnSeries class
        /// </summary>
        public ColumnSeries()
        {
            Core = new ColumnCore(this);
            InitializeDefuaults();
        }

        /// <summary>
        /// Initializes a new instance of ColumnSeries class, using a given mapper
        /// </summary>
        public ColumnSeries(BiDimensinalMapper configuration)
        {
            Core = new ColumnCore(this);
            Configuration = configuration;
            InitializeDefuaults();
        }

        #endregion

        #region Private Properties

        #endregion

        #region Properties

        /// <summary>
        /// The maximum column width property
        /// </summary>
        public static readonly DependencyProperty MaxColumnWidthProperty = DependencyProperty.Register(
            "MaxColumnWidth", typeof (double), typeof (ColumnSeries), new PropertyMetadata(35d));
        /// <summary>
        /// Gets or sets the MaxColumnWidht in pixels, the column width will be capped at this value.
        /// </summary>
        public double MaxColumnWidth
        {
            get { return (double) GetValue(MaxColumnWidthProperty); }
            set { SetValue(MaxColumnWidthProperty, value); }
        }

        /// <summary>
        /// The column padding property
        /// </summary>
        public static readonly DependencyProperty ColumnPaddingProperty = DependencyProperty.Register(
            "ColumnPadding", typeof (double), typeof (ColumnSeries), new PropertyMetadata(2d));
        /// <summary>
        /// Gets or sets the padding between the columns in the series.
        /// </summary>
        public double ColumnPadding
        {
            get { return (double) GetValue(ColumnPaddingProperty); }
            set { SetValue(ColumnPaddingProperty, value); }
        }

        /// <summary>
        /// The labels position property
        /// </summary>
        public static readonly DependencyProperty LabelsPositionProperty = DependencyProperty.Register(
            "LabelsPosition", typeof (BarLabelPosition), typeof (ColumnSeries), 
            new PropertyMetadata(default(BarLabelPosition), EnqueueUpdateCallback));
        /// <summary>
        /// Gets or sets where the label is placed
        /// </summary>
        public BarLabelPosition LabelsPosition
        {
            get { return (BarLabelPosition) GetValue(LabelsPositionProperty); }
            set { SetValue(LabelsPositionProperty, value); }
        }

        /// <summary>
        /// The shares position property
        /// </summary>
        public static readonly DependencyProperty SharesPositionProperty = DependencyProperty.Register(
            "SharesPosition", typeof(bool), typeof(ColumnSeries), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether this column shares space with all the column series in the same position
        /// </summary>
        public bool SharesPosition
        {
            get { return (bool) GetValue(SharesPositionProperty); }
            set { SetValue(SharesPositionProperty, value); }
        }

        #endregion

        /// <inheritdoc cref="ISeriesView.GetPointView"/>
        protected override ChartPointView GetPointView(ChartPoint point, string label)
        {
            var pbv = (ColumnPointView) point.View;

            if (pbv == null)
            {
                pbv = new ColumnPointView
                {
                    Rectangle = new Rectangle(),
                    Data = new CoreRectangle()
                };

                Core.Chart.View.AddToDrawMargin(pbv.Rectangle);
            }
            else
            {
                point.SeriesView.Core.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.Rectangle);
                point.SeriesView.Core.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.Label);
            }

            pbv.Rectangle.Fill = Fill;
            pbv.Rectangle.StrokeThickness = StrokeThickness;
            pbv.Rectangle.Stroke = Stroke;
            pbv.Rectangle.StrokeDashArray = StrokeDashArray;

            pbv.Rectangle.Visibility = Visibility;
            Panel.SetZIndex(pbv.Rectangle, Panel.GetZIndex(this));

            //if (Core.Chart.View.RequiresHoverShape && pbv.HoverShape == null)
            //{
            //    pbv.HoverShape = new Rectangle
            //    {
            //        Fill = Brushes.Transparent,
            //        StrokeThickness = 0
            //    };

            //    Panel.SetZIndex(pbv.HoverShape, int.MaxValue);
            //    Core.Chart.View.EnableHoveringFor(pbv.HoverShape);
            //    Core.Chart.View.AddToDrawMargin(pbv.HoverShape);
            //}

            //if (pbv.HoverShape != null) pbv.HoverShape.Visibility = Visibility;

            if (DataLabels)
            {
                pbv.Label = UpdateLabelContent(new DataLabelViewModel
                {
                    FormattedText = label,
                    Point = point
                }, pbv.Label);
            }

            if (!DataLabels && pbv.Label != null)
            {
                Core.Chart.View.RemoveFromDrawMargin(pbv.Label);
                pbv.Label = null;
            }

            if (point.Stroke != null) pbv.Rectangle.Stroke = (Brush)point.Stroke;
            if (point.Fill != null) pbv.Rectangle.Fill = (Brush)point.Fill;

            pbv.LabelPosition = LabelsPosition;

            return pbv;
        }

        private void InitializeDefuaults()
        {
            SetCurrentValue(StrokeThicknessProperty, 0d);
            SetCurrentValue(MaxColumnWidthProperty, 35d);
            SetCurrentValue(ColumnPaddingProperty, 2d);
            SetCurrentValue(LabelsPositionProperty, BarLabelPosition.Top);

            Func<ChartPoint, string> defaultLabel = x => Core.CurrentYAxis.GetFormatter()(x.Y);
            SetCurrentValue(LabelPointProperty, defaultLabel);

            DefaultFillOpacity = 1;
        }
    }
}
