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
                CanvasWidth = 696;
                canvasHeight = 520;
            }
        }

        private bool IsMouseDown;
        private Point FirstPoint;
        private RelayCommand<Point> mouseDownCommand;
        public RelayCommand<Point> MouseDownCommand
        {
            get
            {
                return mouseDownCommand ?? (mouseDownCommand = new RelayCommand<Point>(ExecuteMouseDownCommand));
            }
        }
        private void ExecuteMouseDownCommand(Point e)
        {
            if (SelectedImageGroup != null)
            {
                SelectedRectangle = new RectangularSelection();
                SelectedImageGroup.Rectangles.Add(SelectedRectangle);
                FirstPoint = e;
                IsMouseDown = true;
            }
        }

        private RelayCommand<Point> mouseUpCommand;
        public RelayCommand<Point> MouseUpCommand
        {
            get
            {
                return mouseUpCommand ?? (mouseUpCommand = new RelayCommand<Point>(ExecuteMouseUpCommand));
            }
        }
        private void ExecuteMouseUpCommand(Point e)
        {
            IsMouseDown = false;
        }

        private RelayCommand<Point> mouseMoveCommand;
        public RelayCommand<Point> MouseMoveCommand
        {
            get
            {
                return mouseMoveCommand ?? (mouseMoveCommand = new RelayCommand<Point>(ExecuteMouseMoveCommand));
            }
        }
        private void ExecuteMouseMoveCommand(Point e)
        {
            if (IsMouseDown)
            {
                System.Windows.Point mousePoint = e;
                SelectedRectangle.X = (FirstPoint.X < mousePoint.X) ? FirstPoint.X : mousePoint.X;
                SelectedRectangle.Y = (FirstPoint.Y < mousePoint.Y) ? FirstPoint.Y : mousePoint.Y;
                SelectedRectangle.Width = (FirstPoint.X < mousePoint.X) ? (mousePoint.X - FirstPoint.X) : (FirstPoint.X - mousePoint.X);
                //SelectedRectangle.Height = (FirstPoint.Y < mousePoint.Y) ? (mousePoint.Y - FirstPoint.Y) : (FirstPoint.Y - mousePoint.Y);
                SelectedRectangle.Height = SelectedRectangle.Width;

                SelectedRectangle.RealX = SelectedRectangle.X * (SelectedImageGroup.ImageFirst.PixelWidth / CanvasWidth);
                SelectedRectangle.RealY = SelectedRectangle.Y * (SelectedImageGroup.ImageFirst.PixelHeight / canvasHeight);
                SelectedRectangle.RealWidth = SelectedRectangle.Width * (SelectedImageGroup.ImageFirst.PixelWidth / CanvasWidth);
                SelectedRectangle.RealHeight = SelectedRectangle.Height * (SelectedImageGroup.ImageFirst.PixelHeight / canvasHeight);
            }
        }

        private int canvasWidth;
        public int CanvasWidth
        {
            get
            {
                return canvasWidth;
            }
            set
            {
                Set(() => CanvasWidth, ref canvasWidth, value);
            }
        }

        private int canvasHeight;
        public int CanvasHeight
        {
            get
            {
                return canvasHeight;
            }
            set
            {
                Set(() => CanvasHeight, ref canvasHeight, value);
            }
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
