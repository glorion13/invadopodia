using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invadopodia.Model
{
    public class RectangularSelection : ViewModelBase
    {
        public RectangularSelection()
        {

        }

        public RectangularSelection(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        private double x;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                Set(() => X, ref x, value);
            }
        }

        private double y;
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                Set(() => Y, ref y, value);
            }
        }

        private double width;
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                Set(() => Width, ref width, value);
            }
        }


        private double height;
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                Set(() => Height, ref height, value);
            }
        }

        private double realX;
        public double RealX
        {
            get
            {
                return realX;
            }
            set
            {
                Set(() => RealX, ref realX, value);
            }
        }

        private double realY;
        public double RealY
        {
            get
            {
                return realY;
            }
            set
            {
                Set(() => RealY, ref realY, value);
            }
        }

        private double realWidth;
        public double RealWidth
        {
            get
            {
                return realWidth;
            }
            set
            {
                Set(() => RealWidth, ref realWidth, value);
            }
        }


        private double realHeight;
        public double RealHeight
        {
            get
            {
                return realHeight;
            }
            set
            {
                Set(() => RealHeight, ref realHeight, value);
            }
        }

    }
}
