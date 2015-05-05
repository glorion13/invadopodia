using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
            folderFirst = "\\" + firstKeyword + "\\";
            folderSecond = "\\" + secondKeyword + "\\";
        }

        public ImageGroup(string firstFile, string secondFile, int index)
        {
            Rectangles = new ObservableCollection<RectangularSelection>();
            ImageFirst = new BitmapImage(new System.Uri(firstFile));
            ImageSecond = new BitmapImage(new System.Uri(secondFile));
            Index = index;
            folderFirst = "\\" + firstKeyword + "\\";
            folderSecond = "\\" + secondKeyword + "\\";
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

        string firstKeyword = "actin";
        string secondKeyword = "gelatin";

        string folderFirst;
        string folderSecond;

        public void Crop(string folder)
        {
            CreateOutputFolders(folder);
            for (int i = 0; i < Rectangles.Count; i++)
            {
                WriteableBitmap first = CropBitmapFromRectangle(ImageFirst, Rectangles[i]);
                WriteableBitmap second = CropBitmapFromRectangle(ImageSecond, Rectangles[i]);

                SaveImage(first, String.Concat(folder, folderFirst, Index.ToString(), " ", firstKeyword, " ", (i + 1).ToString(), ".tif") );
                SaveImage(second, String.Concat(folder, folderSecond, Index.ToString(), " ", secondKeyword, " ", (i + 1).ToString(), ".tif"));
            }
            MessageBox.Show("Crop complete.");
        }

        private WriteableBitmap CropBitmapFromRectangle(BitmapSource bitmap, RectangularSelection rectangle)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);
            return writeableBitmap.Crop((int)rectangle.RealX, (int)rectangle.RealY, (int)rectangle.RealWidth, (int)rectangle.RealHeight);
        }

        private void SaveImage(BitmapSource image, string filename)
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

        private void BackupFolders(string folder)
        {
            string dateString = string.Concat(DateTime.Now.Day.ToString(), "-", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month), "-", DateTime.Now.Year.ToString(), " ", DateTime.Now.Hour.ToString(), "h-", DateTime.Now.Minute.ToString(), "m-", DateTime.Now.Second.ToString() + "s");
            BackupFolder(folder + folderFirst, folder + "\\" + dateString + " " + firstKeyword);
            BackupFolder(folder + folderSecond, folder + "\\" + dateString + " " + secondKeyword);
        }

        private void BackupFolder(string folder, string newFolder)
        {
            if (Directory.Exists(folder))
                Directory.Move(folder, newFolder);
        }

        private void CreateOutputFolder(string folder, string subfolder)
        {
            Directory.CreateDirectory(folder + subfolder);
        }

        private void CreateOutputFolders(string folder)
        {
            BackupFolders(folder);
            CreateOutputFolder(folder, folderFirst);
            CreateOutputFolder(folder, folderSecond);
        }

    }
}
