using AForge.Imaging;
using AForge.Imaging.Filters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Invadopodia.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace Invadopodia.ViewModel
{
    public class ImageViewModel : ViewModelBase
    {
        public ImageViewModel()
        {
            if (IsInDesignMode)
            {
            }
            else
            {
                MessengerInstance.Register<ImageGroup>(this, g => SelectedImageGroup = g);
                MessengerInstance.Register<bool>(this, b => IsSquareSelection = b);
                CanvasSize.Width = 348;
                CanvasSize.Height = 260;
                GammaValueFirst = 1.0;
                GammaValueSecond = 1.0;
            }
        }

        private bool isSquareSelection;
        public bool IsSquareSelection
        {
            get
            {
                return isSquareSelection;
            }
            set
            {
                Set(() => IsSquareSelection, ref isSquareSelection, value);
            }
        }

        private void AdjustImageFirst()
        {
            if (SelectedImageGroup != null)
            {
                UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage(SelectedImageGroup.OriginalImageFirst);
                BrightnessCorrection brightnessFilter = new BrightnessCorrection(BrightnessValueFirst);
                brightnessFilter.ApplyInPlace(unmanagedImage);
                ContrastCorrection contrastFilter = new ContrastCorrection(ContrastValueFirst);
                contrastFilter.ApplyInPlace(unmanagedImage);
                GammaCorrection gammaFilter = new GammaCorrection(GammaValueFirst);
                gammaFilter.ApplyInPlace(unmanagedImage);
                SelectedImageGroup.ImageFirst = ConvertBitmapToBitmapImage(unmanagedImage.ToManagedImage());
            }
        }
        private void AdjustImageSecond()
        {
            if (SelectedImageGroup != null)
            {
                UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage(SelectedImageGroup.OriginalImageSecond);
                BrightnessCorrection brightnessFilter = new BrightnessCorrection(BrightnessValueSecond);
                brightnessFilter.ApplyInPlace(unmanagedImage);
                ContrastCorrection contrastFilter = new ContrastCorrection(ContrastValueSecond);
                contrastFilter.ApplyInPlace(unmanagedImage);
                GammaCorrection gammaFilter = new GammaCorrection(GammaValueSecond);
                gammaFilter.ApplyInPlace(unmanagedImage);
                SelectedImageGroup.ImageSecond = ConvertBitmapToBitmapImage(unmanagedImage.ToManagedImage());
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private double gammaValueFirst;
        public double GammaValueFirst
        {
            get
            {
                return gammaValueFirst;
            }
            set
            {
                Set(() => GammaValueFirst, ref gammaValueFirst, value);
                AdjustImageFirst();
            }
        }
        private int contrastValueFirst;
        public int ContrastValueFirst
        {
            get
            {
                return contrastValueFirst;
            }
            set
            {
                Set(() => ContrastValueFirst, ref contrastValueFirst, value);
                AdjustImageFirst();
            }
        }
        private int brightnessValueFirst;
        public int BrightnessValueFirst
        {
            get
            {
                return brightnessValueFirst;
            }
            set
            {
                Set(() => BrightnessValueFirst, ref brightnessValueFirst, value);
                AdjustImageFirst();
            }
        }

        private double gammaValueSecond;
        public double GammaValueSecond
        {
            get
            {
                return gammaValueSecond;
            }
            set
            {
                Set(() => GammaValueSecond, ref gammaValueSecond, value);
                AdjustImageSecond();
            }
        }
        private int contrastValueSecond;
        public int ContrastValueSecond
        {
            get
            {
                return contrastValueSecond;
            }
            set
            {
                Set(() => ContrastValueSecond, ref contrastValueSecond, value);
                AdjustImageSecond();
            }
        }
        private int brightnessValueSecond;
        public int BrightnessValueSecond
        {
            get
            {
                return brightnessValueSecond;
            }
            set
            {
                Set(() => BrightnessValueSecond, ref brightnessValueSecond, value);
                AdjustImageSecond();
            }
        }

        private bool IsMouseDown;
        private System.Windows.Point FirstPoint;
        private RelayCommand<System.Windows.Point> mouseDownCommand;
        public RelayCommand<System.Windows.Point> MouseDownCommand
        {
            get
            {
                return mouseDownCommand ?? (mouseDownCommand = new RelayCommand<System.Windows.Point>(ExecuteMouseDownCommand));
            }
        }
        private void ExecuteMouseDownCommand(System.Windows.Point e)
        {
            if (SelectedImageGroup != null)
            {
                SelectedRectangle = new RectangularSelection();
                SelectedImageGroup.Rectangles.Add(SelectedRectangle);
                FirstPoint = e;
                IsMouseDown = true;
            }
        }

        private RelayCommand<System.Windows.Point> mouseUpCommand;
        public RelayCommand<System.Windows.Point> MouseUpCommand
        {
            get
            {
                return mouseUpCommand ?? (mouseUpCommand = new RelayCommand<System.Windows.Point>(ExecuteMouseUpCommand));
            }
        }
        private void ExecuteMouseUpCommand(System.Windows.Point e)
        {
            IsMouseDown = false;
        }

        private RelayCommand<System.Windows.Point> mouseMoveCommand;
        public RelayCommand<System.Windows.Point> MouseMoveCommand
        {
            get
            {
                return mouseMoveCommand ?? (mouseMoveCommand = new RelayCommand<System.Windows.Point>(ExecuteMouseMoveCommand));
            }
        }
        private void ExecuteMouseMoveCommand(System.Windows.Point e)
        {
            if (IsMouseDown)
            {
                System.Windows.Point mousePoint = e;
                SelectedRectangle.X = (FirstPoint.X < mousePoint.X) ? FirstPoint.X : mousePoint.X;
                SelectedRectangle.Y = (FirstPoint.Y < mousePoint.Y) ? FirstPoint.Y : mousePoint.Y;
                SelectedRectangle.Width = (FirstPoint.X < mousePoint.X) ? (mousePoint.X - FirstPoint.X) : (FirstPoint.X - mousePoint.X);
                if (IsSquareSelection)
                    SelectedRectangle.Height = SelectedRectangle.Width;
                else
                    SelectedRectangle.Height = (FirstPoint.Y < mousePoint.Y) ? (mousePoint.Y - FirstPoint.Y) : (FirstPoint.Y - mousePoint.Y);

                SelectedRectangle.RealX = SelectedRectangle.X * (SelectedImageGroup.ImageFirst.PixelWidth / CanvasSize.Width);
                SelectedRectangle.RealY = SelectedRectangle.Y * (SelectedImageGroup.ImageFirst.PixelHeight / CanvasSize.Height);
                SelectedRectangle.RealWidth = SelectedRectangle.Width * (SelectedImageGroup.ImageFirst.PixelWidth / CanvasSize.Width);
                if (IsSquareSelection)
                    SelectedRectangle.RealHeight = SelectedRectangle.RealWidth;
                else
                    SelectedRectangle.RealHeight = SelectedRectangle.Height * (SelectedImageGroup.ImageFirst.PixelHeight / CanvasSize.Height);
            }
        }

        private System.Windows.Size CanvasSize;
        private void AdjustRectanglePosition(RectangularSelection rectangle, System.Windows.Size newCanvasSize, System.Windows.Size oldCanvasSize)
        {
            double factorX = newCanvasSize.Width / oldCanvasSize.Width;
            double factorY = newCanvasSize.Height / oldCanvasSize.Height;
            rectangle.X *= factorX;
            rectangle.Y *= factorY;
            rectangle.Width *= factorX;
            rectangle.Height *= factorY;
        }

        private RelayCommand<SizeChangedEventArgs> resizeCanvasCommand;
        public RelayCommand<SizeChangedEventArgs> ResizeCanvasCommand
        {
            get
            {
                return resizeCanvasCommand ?? (resizeCanvasCommand = new RelayCommand<SizeChangedEventArgs>(ExecuteResizeCanvasCommand));
            }
        }
        private void ExecuteResizeCanvasCommand(SizeChangedEventArgs e)
        {
            CanvasSize = e.NewSize;
            if (SelectedImageGroup != null)
                foreach (RectangularSelection rectangle in SelectedImageGroup.Rectangles)
                    AdjustRectanglePosition(rectangle, e.NewSize, e.PreviousSize);
        }
        
        private RectangularSelection selectedRectangle;
        public RectangularSelection SelectedRectangle
        {
            get
            {
                return selectedRectangle;
            }
            set
            {
                Set(() => SelectedRectangle, ref selectedRectangle, value);
            }
        }

        private ImageGroup selectedImageGroup;
        public ImageGroup SelectedImageGroup
        {
            get
            {
                return selectedImageGroup;
            }
            set
            {
                Set(() => SelectedImageGroup, ref selectedImageGroup, value);
            }
        }

    }


}
