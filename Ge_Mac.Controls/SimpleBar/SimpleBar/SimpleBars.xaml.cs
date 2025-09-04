using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleBars
{
	/// <summary>
	/// Interaction logic for MainControl.xaml
	/// </summary>
	public partial class SimpleBar
	{
        private double minimum = 0;
        public double Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        private double maximum = 100;
        public double Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        private double value = 0.000001;
        private double limitValue = 0;
        public double Value
        {
            get { return this.value; }
            set
            {
                bool hasChanged = false;
                limitValue = LimitValue(value);
                double aValue = value;
                hasChanged |= aValue != this.value;
                this.value = aValue;
                if (hasChanged)
                {
                    SetValueColour();
                    SetSlider();
                    if (autoTextData)
                        SetAutoTextData();
                    hasChanged = false;
                }
            }
        }

        private Orientation orientation = Orientation.Horizontal;

        public Orientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        private Brush stroke = Brushes.Black;
        public Brush Stroke
        {
            get { return stroke; }
            set
            {
                stroke = value;
                rectOutline.Stroke = stroke;
            }
        }

        private Brush fill = Brushes.Green;
        public Brush Fill
        {
            get { return fill; }
            set
            {
                fill = value;
                rectSlider.Fill = fill;
            }
        }

        private bool levelColours = false;

        public bool LevelColours
        {
            get { return levelColours; }
            set { levelColours = value; }
        }

        private Brush normalFill = Brushes.Green;

        public string NormalColourName = "Green";

        private Brush lowValueFill = Brushes.Blue;

        public string LowColourName = "Blue";

        private double lowValueLevel = 0;

        public double LowValueLevel
        {
            get { return lowValueLevel; }
            set { lowValueLevel = value; }
        }

        private Brush highValueFill = Brushes.Orange;

        public string HighColourName = "Orange";

        private double highValueLevel = 80;

        public double HighValueLevel
        {
            get { return highValueLevel; }
            set { highValueLevel = value; }
        }

        private Brush highAlarmFill = Brushes.Red;

        public string HighAlarmName = "Red";

        private double highAlarmLevel = 100;

        public double HighAlarmLevel
        {
            get { return highAlarmLevel; }
            set { highAlarmLevel = value; }
        }

        private Brush backFill = Brushes.White;
        public Brush BackFill
        {
            get { return backFill; }
            set
            {
                backFill = value;
                rectBackground.Fill = backFill;
            }
        }

        private Brush textColour = Brushes.Black;

        public Brush TextColour
        {
            get { return textColour; }
            set
            {
                textColour = value;
                lblData.Foreground = textColour;
            }
        }

        private string text = string.Empty;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                if (autoTextData)
                    SetAutoTextData();
                else
                    lblData.Content = text;
            }
        }

        public enum TextSizes { VerySmall, Small, Medium, Large, VeryLarge, Custom };

        private TextSizes textSize = TextSizes.Medium;

        public TextSizes TextSize
        {
            get { return textSize; }
            set { textSize = value; }
        }

        private double fontSize;

        public double CustomTextSize
        {
            get { return fontSize; }
            set
            {
                textSize = TextSizes.Custom;
                fontSize = value;
                lblData.FontSize = fontSize;
            }
        }

        private VerticalAlignment textVerticalAlignment = VerticalAlignment.Center;

        public VerticalAlignment TextVerticalAlignment
        {
            get { return textVerticalAlignment; }
            set
            {
                textVerticalAlignment = value;
                lblData.VerticalAlignment = value;
            }
        }

        private HorizontalAlignment textHorizontalAlignment = HorizontalAlignment.Center;

        public HorizontalAlignment TextHorizontalAlignment
        {
            get { return textHorizontalAlignment; }
            set
            {
                textHorizontalAlignment = value;
                lblData.HorizontalAlignment = value;
            }
        }

        private bool autoTextData = false;

        public bool AutoTextData
        {
            get { return autoTextData; }
            set { autoTextData = value; }
        }

        private string textFormat = string.Empty;

        public string TextFormat
        {
            get { return textFormat; }
            set { textFormat = value; }
        }
        
        public SimpleBar()
		{
			this.InitializeComponent();
            this.SnapsToDevicePixels = true;
            rectOutline.SnapsToDevicePixels = true;
            rectSlider.SnapsToDevicePixels = true;
            rectBackground.SnapsToDevicePixels = true;
            orientation = Orientation.Vertical;
            BuildGradients();
		}

        private GradientBrush BuildGradient(Color color1, Color color2)
        {
            LinearGradientBrush aGradient = new LinearGradientBrush();
            if (orientation == Orientation.Vertical)
            {
                aGradient.StartPoint = new Point(0, 0);
                aGradient.EndPoint = new Point(1, 0);
            }
            else
            {
                aGradient.StartPoint = new Point(0, 0);
                aGradient.EndPoint = new Point(0, 1);
            }
            aGradient.GradientStops.Add(new GradientStop(color1, 0.0));
            aGradient.GradientStops.Add(new GradientStop(color2, 0.5));
            aGradient.GradientStops.Add(new GradientStop(color1, 1.0));
            return aGradient;
        }

        private Brush BuildGradient(string colourName)
        {
            Brush b = null;
            if (colourName.ToLower() == "green")
            {
                b = BuildGradient(Colors.Green, Colors.LightGreen);
            }
            if (colourName.ToLower() == "blue")
            {
                b = BuildGradient(Colors.Blue, Colors.LightBlue);
            }
            if (colourName.ToLower() == "orange")
            {
                Color DarkOrange = Color.FromArgb(0xFF, 0xFF, 0x80, 0x0);
                Color LightOrange = Color.FromArgb(0xFF, 0xFF, 0xD0, 0x80);
                b = BuildGradient(DarkOrange, LightOrange);
            }
            if (colourName.ToLower() == "red")
            {
                Color LightRed = Color.FromArgb(0xFF, 0xFF, 0x80, 0x80);
                b = BuildGradient(Colors.Red, LightRed);
            }
            return b;
        }


        public void BuildGradients()
        {
            lowValueFill = BuildGradient(LowColourName);
            normalFill = BuildGradient(NormalColourName);
            highValueFill = BuildGradient(HighColourName);
            highAlarmFill = BuildGradient(HighAlarmName);
            Fill = normalFill;
        }

        private double LimitValue(double aValue)
        {
            if (aValue > maximum)
                aValue = maximum;
            if (aValue < minimum)
                aValue = minimum;
            return aValue;
        }

        private void CheckValue()
        {
            Value = LimitValue(value);
        }

        private void CheckMinimum()
        {
            if (minimum > maximum)
                minimum = maximum;
            CheckValue();
        }

        private void CheckMaximum()
        {
            if (maximum < minimum)
                maximum = minimum;
            CheckValue();
        }

        private void SetAutoTextData()
        {
            lblData.Content = text + value.ToString(textFormat);
        }

        private void SetValueColour()
        {
            if (levelColours)
            {
                if (value < lowValueLevel)
                    Fill = lowValueFill;
                if ((value >= lowValueLevel) && (value < highValueLevel))
                    Fill = normalFill;
                if ((value >= highValueLevel) && (value < highAlarmLevel))
                    Fill = highValueFill;
                if (value >= highAlarmLevel)
                    Fill = highAlarmFill;
            }
        }

        private void SetSlider()
        {
            Point pt = ValueToPosition();
            if (orientation == Orientation.Horizontal)
                rectSlider.Width = pt.X;
            else
                rectSlider.Height = pt.Y;
        }

        private void SetControlDimensions()
        {
            if (orientation == Orientation.Horizontal)
            {
                rectSlider.HorizontalAlignment = HorizontalAlignment.Left;
                rectSlider.VerticalAlignment = VerticalAlignment.Stretch;
                rectSlider.Height = rectBackground.ActualHeight;
            }
            else
            {
                rectSlider.HorizontalAlignment = HorizontalAlignment.Stretch;
                rectSlider.VerticalAlignment = VerticalAlignment.Bottom;
                rectSlider.Width = rectBackground.ActualWidth;
            }
            SetSlider();
            SetTextSize();
        }

        private void SetTextSize()
        {
            double minDimension = rectBackground.ActualWidth;
            if (rectBackground.ActualHeight < minDimension)
                minDimension = rectBackground.ActualHeight;
            switch (textSize)
            {
                case TextSizes.Custom:
                    break;
                case TextSizes.VerySmall:
                    fontSize = minDimension / 10;
                    break;
                case TextSizes.Small:
                    fontSize = minDimension / 7;
                    break;
                case TextSizes.Medium:
                    fontSize = minDimension / 5;
                    break;
                case TextSizes.Large:
                    fontSize = minDimension / 3;
                    break;
                case TextSizes.VeryLarge:
                    fontSize = minDimension / 2;
                    break;
                default:
                    fontSize = minDimension / 6;
                    break;
            }
            if (fontSize > 0)
                lblData.FontSize = fontSize;
        }

        private Point ValueToPosition()
        {
            Point pt = new Point(0, 0);
            if (orientation == Orientation.Horizontal)
                pt.X = (limitValue - minimum) * (rectBackground.ActualWidth) / (maximum - minimum);
            else
                pt.Y = (limitValue - minimum) * (rectBackground.ActualHeight) / (maximum - minimum);
            if (pt.X > rectBackground.ActualWidth) pt.X = rectBackground.ActualWidth;
            if (pt.X < 0) pt.X = 0;
            if ((limitValue > 0) && (pt.X < 3)) pt.X = 3;
            return pt;
        }

        private void rectSlider_Loaded(object sender, RoutedEventArgs e)
        {
            SetControlDimensions();
        }

        private void SimpleBars_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetControlDimensions();
        }

        private void SimpleBars_Loaded(object sender, RoutedEventArgs e)
        {
            SetControlDimensions();
            BuildGradients();
        }
    }
}