using DICOMReceiver.Models;
using DICOMReceiver.Models.Dicom;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;
using FellowOakDicom.Imaging.Render;
using FellowOakDicom.IO;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace DICOMReceiver.View
{
    /// <summary>
    /// Interaction logic for ImageViewerControl.xaml
    /// </summary

    public partial class ImageViewerControl : Window
    {
        private Point _lastDownPosition;
        private DicomImage _image;
        private int _frameNumber;

        private SKBitmap _currentView;
        private IEnumerable<string> _filesEnumerator;
        private string _loadedImageFile;
        public DicomImage ImageToDisplay
        {
            get => _image;
            set
            {
                if (_image != value)
                {
                    _image = value;
                    _frameNumber = 0;
                    DisplayImage();
                }
            }
        }

        public ImageViewerControl(IEnumerable<string> filesToLoad)
        {
            new DicomSetupBuilder()
                .RegisterServices(s => s
                    .AddFellowOakDicom()
                    .AddTranscoderManager<NativeTranscoderManager>()
                    .AddImageManager<SkiaSharpImageManager>())
                .Build();
            InitializeComponent();
            _filesEnumerator = filesToLoad;
            Update();
        }
        private void Update(int counter = 0)
        {
            if (String.IsNullOrWhiteSpace(_loadedImageFile))
                _loadedImageFile = _filesEnumerator.FirstOrDefault();
            int index = _filesEnumerator.ToList().IndexOf(_loadedImageFile) + counter;
            if (index >= 0 || index < _filesEnumerator.Count())
            {
                var file = _filesEnumerator.ElementAtOrDefault(index);
                if (!string.IsNullOrWhiteSpace(file))
                {
                    var ff = DicomFile.Open(file, FileReadOption.Default);
                    if (ff.Dataset.Contains(DicomTag.PixelData))
                    {
                        _loadedImageFile = file;
                        ImageToDisplay = new DicomImage(ff.Dataset);
                        _frameNumber = 0;
                        DisplayImage();
                    }
                }
            }
        }
        private void DisplayImage()
        {
            if (_image == null)
            {
                _currentView = null;
                SkiaImageView.InvalidateVisual();
                return;
            }

            var img = _image.RenderImage(_frameNumber);
            _currentView = img.AsSKBitmap();
            SkiaImageView.InvalidateVisual();
        }

        private void ImageView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                // pan
            }
            else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                // window
                var point = e.GetPosition(SkiaImageView);
                var delta = point - _lastDownPosition;
                _lastDownPosition = point;

                _image.WindowCenter = _image.WindowCenter + delta.X * 10;
                _image.WindowWidth = _image.WindowWidth + delta.Y * 10;
                DisplayImage();
            }
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            _image.ShowOverlays = true;
            DisplayImage();
        }

        private void CheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            _image.ShowOverlays = false;
            DisplayImage();
        }

        private void ImageView_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                // scroll down
                if (_image.NumberOfFrames > (_frameNumber + 1))
                {
                    _frameNumber++;
                    DisplayImage();
                }
            }
            else if (e.Delta < 0)
            {
                if (_frameNumber > 0)
                {
                    _frameNumber--;
                    DisplayImage();
                }
            }
            e.Handled = true;
        }

        private void ImageView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _lastDownPosition = e.GetPosition(SkiaImageView);
        }

        private void SkiaImageView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;

            canvas.Clear(SKColors.White);

            if (_currentView != null)
            {
                float scale = Math.Min((float)info.Width / _currentView.Width,
                               (float)info.Height / _currentView.Height);
                float x = (info.Width - scale * _currentView.Width) / 2;
                float y = (info.Height - scale * _currentView.Height) / 2;
                var destRect = new SKRect(x, y, x + scale * _currentView.Width,
                                                   y + scale * _currentView.Height);
                canvas.DrawBitmap(_currentView, destRect);
            }
        }

        private void lstImageFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                ImageToDisplay = e.Source is ListBoxItem item ?
                    new DicomImage((string)item.Content) :
                    new DicomImage((string)listBox.SelectedItem);
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RepeatButton btn)
            {
                int counter = 0;
                switch (btn.Content)
                {
                    case "Next":
                        counter = +1;
                        break;
                    case "Prev":
                        counter = -1;
                        break;
                    default:
                        break;
                }
                Update(counter);
            }
        }

       
    }

}
