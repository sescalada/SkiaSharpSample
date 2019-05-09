using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaSharpDemo
{
    public class GradientBar : SKCanvasView
    {
        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(float), typeof(GradientBar), 0f, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty StrokeWidthProperty =
            BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(GradientBar), 24f, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty BackColorProperty =
            BindableProperty.Create(nameof(BackColor), typeof(Color), typeof(GradientBar), Color.Black, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty FrontColorFromProperty =
            BindableProperty.Create(nameof(FrontColorFrom), typeof(Color), typeof(GradientBar), Color.Yellow, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty FrontColorToProperty =
           BindableProperty.Create(nameof(FrontColorTo), typeof(Color), typeof(GradientBar), Color.Orange, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty DrawLabelProperty =
            BindableProperty.Create(nameof(DrawLabel), typeof(bool), typeof(GradientBar), false, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty UseRoundedBordersProperty =
            BindableProperty.Create(nameof(UseRoundedBorders), typeof(bool), typeof(GradientBar), true, propertyChanged: OnPropertyChanged);

        public float Progress
        {
            get => (float)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public float StrokeWidth
        {
            get => (float)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public Color BackColor
        {
            get => (Color)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }

        public Color FrontColorFrom
        {
            get => (Color)GetValue(FrontColorFromProperty);
            set => SetValue(FrontColorFromProperty, value);
        }

        public Color FrontColorTo
        {
            get => (Color)GetValue(FrontColorToProperty);
            set => SetValue(FrontColorToProperty, value);
        }

        public bool DrawLabel
        {
            get => (bool)GetValue(DrawLabelProperty);
            set => SetValue(DrawLabelProperty, value);
        }

        public bool UseRoundedBorders
        {
            get => (bool)GetValue(UseRoundedBordersProperty);
            set => SetValue(UseRoundedBordersProperty, value);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            base.OnPaintSurface(args);

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            canvas.Save();

            DrawBackLine(info, canvas);
            DrawFrontLine(info, canvas);
            DrawLabelIfRequired(info, canvas);

            canvas.Restore();
        }

        private void DrawBackLine(SKImageInfo info, SKCanvas canvas)
        {
            InnerDrawLine(info, canvas, BackColor, null, 1f);
        }

        private void DrawFrontLine(SKImageInfo info, SKCanvas canvas)
        {
            if (Progress > 0)
            {
                var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(info.Width * Progress, 0),
                    new[] { FrontColorFrom.ToSKColor(), FrontColorTo.ToSKColor() },
                    new[] { 0f, 1f },
                    SKShaderTileMode.Clamp);

                InnerDrawLine(info, canvas, Color.Default, shader, Progress);
            }
            else
            {
                InnerDrawLine(info, canvas, FrontColorTo, null, Progress);
            }
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldVal, object newVal)
        {
            var bar = bindable as GradientBar;
            bar?.InvalidateSurface();
        }

        private void InnerDrawLine(
            SKImageInfo info, 
            SKCanvas canvas,
            Color color,
            SKShader shader,
            float widthFactor)
        {
            using (var paint = new SKPaint())
            {
                paint.StrokeWidth = StrokeWidth;
                paint.IsStroke = true;
                paint.IsAntialias = true;
                paint.StrokeCap = UseRoundedBorders ? SKStrokeCap.Round : SKStrokeCap.Butt;

                if (shader != null)
                {
                    paint.Shader = shader;
                }
                else
                {
                    paint.Color = color.ToSKColor();
                }

                var drawWidth = info.Width - (StrokeWidth * 2);

                canvas.DrawLine(
                    StrokeWidth,
                    info.Height / 2,
                    StrokeWidth + drawWidth * widthFactor,
                    info.Height / 2,
                    paint);
            };
        }

        private void DrawLabelIfRequired(SKImageInfo info, SKCanvas canvas)
        {
            if (!DrawLabel)
            {
                return; 
            }

            using (var paint = new SKPaint())
            {
                paint.TextSize = StrokeWidth;
                paint.IsAntialias = true;
                paint.Color = Color.Black.ToSKColor();
                paint.IsStroke = false;

                var drawWidth = info.Width - (StrokeWidth * 2);

                canvas.DrawText(
                    Progress.ToString("N1"),
                    StrokeWidth + drawWidth * Progress, 
                    info.Height / 2 + StrokeWidth / 2,
                    paint);
            }
        }
    }
}