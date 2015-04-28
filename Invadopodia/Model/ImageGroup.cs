using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Invadopodia.Model
{
    public class ImageGroup : ViewModelBase
    {
        public ImageGroup()
        {
            Rectangles = new ObservableCollection<RectangularSelection>();
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                Set(() => Index, ref index, value);
            }
        }

        private BitmapImage imageFirst;
        public BitmapImage ImageFirst
        {
            get
            {
                return imageFirst;
            }
            set
            {
                Set(() => ImageFirst, ref imageFirst, value);
            }
        }

        private BitmapImage imageSecond;
        public BitmapImage ImageSecond
        {
            get
            {
                return imageSecond;
            }
            set
            {
                Set(() => ImageSecond, ref imageSecond, value);
            }
        }

        private ObservableCollection<RectangularSelection> rectangles;
        public ObservableCollection<RectangularSelection> Rectangles
        {
            get
            {
                return rectangles;
            }
            set
            {
                Set(() => Rectangles, ref rectangles, value);
            }
        }

        string folderFirst = "\\actin\\";
        string folderSecond = "\\gelatin\\";

        public void Crop(string folder)
        {
            CreateOutputFolder(folder);
            for (int i = 0; i < Rectangles.Count; i++)
            {
                RectangularSelection rectangle = Rectangles[i];
                WriteableBitmap first = new WriteableBitmap(ImageFirst);
                WriteableBitmap second = new WriteableBitmap(ImageSecond);

                WriteableBitmap croppedFirst = first.Crop((int) rectangle.RealX, (int)rectangle.RealY, (int)rectangle.RealWidth, (int)rectangle.RealHeight);
                WriteableBitmap croppedSecond = second.Crop((int)rectangle.RealX, (int)rectangle.RealY, (int)rectangle.RealWidth, (int)rectangle.RealHeight);

                SaveImage(croppedFirst, folder + folderFirst + Index.ToString() + " " + (i + 1).ToString() + ".tif");
                SaveImage(croppedSecond, folder + folderSecond + Index.ToString() + " " + (i + 1).ToString() + ".tif");
            }
            MessageBox.Show("Crop complete.");
        }

        void SaveImage(BitmapSource image, string filename)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(stream);
                    stream.Close();
                }
            }
        }

        private void CreateOutputFolder(string folder)
        {
            Directory.CreateDirectory(folder + folderFirst);
            Directory.CreateDirectory(folder + folderSecond);
        }

    }
}
