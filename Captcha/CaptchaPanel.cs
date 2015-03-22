using System.Speech.Synthesis;

namespace Captcha
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using Core;
    using System.IO;
    using System.Windows.Media.Imaging;
    using System.Diagnostics;

    public class CaptchaPanel : FrameworkElement
    {
        static CaptchaPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptchaPanel), new FrameworkPropertyMetadata(typeof(CaptchaPanel)));
        }

        public static DependencyProperty TextLengthProperty = DependencyProperty.Register("TextLength", typeof(uint), typeof(CaptchaPanel), new PropertyMetadata((uint)6));
        public static DependencyProperty MinimumFontSizeProperty = DependencyProperty.Register("MinimumFontSize", typeof(uint), typeof(CaptchaPanel),new PropertyMetadata((uint)36));
        public static DependencyProperty MaximumFontSizeProperty = DependencyProperty.Register("MaximumFontSize", typeof(uint), typeof(CaptchaPanel), new PropertyMetadata((uint)48));
        public static DependencyProperty PenThicknessProperty = DependencyProperty.Register("PenThickness", typeof(double), typeof(CaptchaPanel), new PropertyMetadata((double)1));
        public static DependencyProperty MinimumRotateTransformAngleProperty = DependencyProperty.Register("MinimumRotateTransformAngle", typeof(int), typeof(CaptchaPanel), new PropertyMetadata((int)-30));
        public static DependencyProperty MaximumRotateTransformAngleProperty = DependencyProperty.Register("MaximumRotateTransformAngle", typeof(int), typeof(CaptchaPanel), new PropertyMetadata((int)30));
        public static DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(FontFamily), typeof(CaptchaPanel), new PropertyMetadata((FontFamily)new FontFamily("Verdana")));
        public static DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(CaptchaPanel), new PropertyMetadata((Thickness)new Thickness(10,10,10,10)));
        public static DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(CaptchaPanel), new PropertyMetadata((Brush)Brushes.Transparent));
        public static DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(CaptchaPanel), new PropertyMetadata((Brush)Brushes.Transparent));
        public static DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(CaptchaPanel), new PropertyMetadata((double)0));


        public uint TextLength
        {
            get { return (uint)GetValue(TextLengthProperty); }
            set { SetValue(TextLengthProperty, value); }
        }

        public uint MinimumFontSize
        {
            get { return (uint)GetValue(MinimumFontSizeProperty); }
            set { SetValue(MinimumFontSizeProperty, value); }
        }

        public uint MaximumFontSize
        {
            get { return (uint)GetValue(MaximumFontSizeProperty); }
            set { SetValue(MaximumFontSizeProperty, value); }
        }

        public double PenThickness
        {
            get { return (double)GetValue(PenThicknessProperty); }
            set { SetValue(PenThicknessProperty, value); }
        }

        public int MinimumRotateTransformAngle
        {
            get { return (int)GetValue(MinimumRotateTransformAngleProperty); }
            set { SetValue(MinimumRotateTransformAngleProperty, value); }
        }

        public int MaximumRotateTransformAngle
        {
            get { return (int)GetValue(MaximumRotateTransformAngleProperty); }
            set { SetValue(MaximumRotateTransformAngleProperty, value); }
        }

        public FontFamily Font
        {
            get { return (FontFamily)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        private VisualCollection _children;
        private DefaultRandomNumberGenerator _rng = null;
        private char[] _text;
        private Size _childSize = new Size(0, 0);

        public CaptchaPanel()
        {
        }

        public override void BeginInit()
        {
            _children = new VisualCollection(this);
            _rng = new DefaultRandomNumberGenerator();
            
            base.BeginInit();
        }

        public override void EndInit()
        {
            base.EndInit();
        }

        public void Generate()
        {
            using (var gen = new DefaultPasswordGenerator(new CryptoRandomNumberGenerator()))
            {
                var val = gen.Generate(1, TextLength == 0 ? 6 : TextLength, true, true, true, false, null, 2, 2, 2, PasswordStrengthEnum.NotDefined);
                _text = val[0].Value;
            }

            _children.Clear();
            _children.Add(CreateVisual());
            InvalidateVisual();
        }

        public void Say()
        {
            if(_text==null) return;
            using(var sp = new SpeechSynthesizer())
            {
                var p = new PromptBuilder();
                p.StartSentence();
                p.StartStyle(new PromptStyle(PromptRate.Slow));
                p.AppendBreak(PromptBreak.Small);
                p.AppendTextWithHint(new string(_text), SayAs.SpellOut);
                p.EndStyle();
                p.EndSentence();
                sp.Speak(p);
                p.ClearContent();
            }
            
        }

        public bool IsValid(string value, bool useCase)
        {
            return string.Equals(new string(_text), value, useCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
        }

        public void SaveToFile(string filename)
        {
            SaveToFile(filename,(int)this.Width,(int)this.Height);
        }

        public void SaveToFile(string filename, int width, int height)
        {
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(_children[0]);
            
            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            using (Stream st = File.Create(filename))
                png.Save(st);
        }

        private DrawingVisual CreateVisual()
        {
            var drawingVisual = new DrawingVisual();

            DrawingContext drawingContext = drawingVisual.RenderOpen();

            var point = new Point(Padding.Left, Padding.Top);
            if (_text != null)
                for (int i = 0; i < _text.Length; i++)
                {
                    var fcolor1 = Color.FromRgb((byte)_rng.GetRandomNumber(0, 255), (byte)_rng.GetRandomNumber(0, 255), (byte)_rng.GetRandomNumber(0, 255));
                    var fcolor2 = Color.FromRgb((byte)_rng.GetRandomNumber(0, 255), (byte)_rng.GetRandomNumber(0, 255), (byte)_rng.GetRandomNumber(0, 255));
                    var fangle = _rng.GetRandomNumber(MinimumRotateTransformAngle, MaximumRotateTransformAngle);
                    var textBrush = new LinearGradientBrush(fcolor1, fcolor2, fangle);
                    var penColor = Color.FromRgb((byte)_rng.GetRandomNumber(0, 255), (byte)_rng.GetRandomNumber(0, 255), (byte)_rng.GetRandomNumber(0, 255));
                    var pen = new Pen(new SolidColorBrush(penColor), PenThickness)
                    {
                        DashStyle = DashStyles.DashDotDot,
                        DashCap = PenLineCap.Round,
                        EndLineCap = PenLineCap.Round
                    };
                    
                    var ft = new FormattedText(_text[i].ToString(),
                                              CultureInfo.CurrentCulture,
                                              FlowDirection.LeftToRight,
                                              new Typeface(this.Font == null ? "Verdana" : Font.ToString()),
                                              _rng.GetRandomNumber((int)MinimumFontSize, (int)MaximumFontSize),
                                              textBrush);

                    var geo = ft.BuildGeometry(point);

                    geo.Transform = new RotateTransform(_rng.GetRandomNumber(MinimumRotateTransformAngle, MaximumRotateTransformAngle), point.X + (geo.Bounds.Width/2),
                                                        point.Y + (geo.Bounds.Height/2));

                    drawingContext.DrawGeometry(textBrush, pen, geo);

                    point.Offset(ft.WidthIncludingTrailingWhitespace, 0);
                }

                drawingContext.Close();

            _childSize = drawingVisual.ContentBounds.Size;
            return drawingVisual;
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

        //protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    Generate();
        //    e.Handled = true;
        //    base.OnMouseLeftButtonDown(e);
        //}

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_childSize.Width > 0 && _childSize.Width + Padding.Right * 2 < MaxWidth)
                this.Width = _childSize.Width + Padding.Right*2;
            if(_childSize.Height > 0 && _childSize.Height + Padding.Bottom*2 < MaxHeight)
                this.Height = _childSize.Height + Padding.Bottom * 2;

            var bgRect = new Rect(0, 0, this.Width, this.Height);
            if (!bgRect.IsEmpty)
                drawingContext.DrawRoundedRectangle(Background, new Pen(BorderBrush, BorderThickness), bgRect, 3, 3);

            base.OnRender(drawingContext);
        }

    }
}