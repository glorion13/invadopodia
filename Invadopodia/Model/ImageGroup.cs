using AForge.Imaging;
using AForge.Imaging.Filters;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
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
            Index = index;
            folderFirst = "\\" + firstKeyword + "\\";
            folderSecond = "\\" + secondKeyword + "\\";

            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);

            OriginalImageFirst = filter.Apply(ConvertBitmapImageToBitmap(new BitmapImage(new System.Uri(firstFile))));
            OriginalImageSecond = filter.Apply(ConvertBitmapImageToBitmap(new BitmapImage(new System.Uri(secondFile))));
            ImageFirst = ConvertBitmapToBitmapImage(OriginalImageFirst);
            ImageSecond = ConvertBitmapToBitmapImage(OriginalImageSecond);
        }

        private Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
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

        public struct Gamma
        {
            public Gamma(double value)
            {
                Value = value;
            }
            public double Value;
        }
        public struct Contrast
        {
            public Contrast(int value)
            {
                Value = value;
            }
            public int Value;
        }
        public struct Brightness
        {
            public Brightness(int value)
            {
                Value = value;
            }
            public int Value;
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

        private Bitmap originalImageFirst;
        public Bitmap OriginalImageFirst
        {
            get
            {
                return originalImageFirst;
            }
            set
            {
                Set(() => OriginalImageFirst, ref originalImageFirst, value);
            }
        }

        private Bitmap originalImageSecond;
        public Bitmap OriginalImageSecond
        {
            get
            {
                return originalImageSecond;
            }
            set
            {
                Set(() => OriginalImageSecond, ref originalImageSecond, value);
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
        string secondKeyword = "pla";

        string folderFirst;
        string folderSecond;

        public void Crop(string folder)
        {
            try
            {
                CreateOutputFolders(folder);
                Bitmap unmanagedFirst = ConvertBitmapImageToBitmap(ImageFirst);
                Bitmap unmanagedSecond = ConvertBitmapImageToBitmap(ImageSecond);
                for (int i = 0; i < Rectangles.Count; i++)
                {
                    if ((Rectangles[i].RealWidth > 0.0) && (Rectangles[i].RealHeight) > 0.0)
                    {
                        Crop filter = new Crop(new Rectangle((int)Rectangles[i].RealX, (int)Rectangles[i].RealY, (int)Rectangles[i].RealWidth, (int)Rectangles[i].RealHeight));
                        MessageBox.Show($"{Rectangles[i].RealX} kai {Rectangles[i].RealWidth}");
                        //Grayscale filter2 = new Grayscale(0.2125, 0.7154, 0.0721);
                        Bitmap first = filter.Apply(unmanagedFirst);
                        Bitmap second = filter.Apply(unmanagedSecond);
                        //first = filter2.Apply(first);
                        //second = filter2.Apply(second);

                        first.Save($"{folder}{folderFirst}{Index} {firstKeyword} {i + 1}.tif", ImageFormat.Tiff);
                        second.Save($"{folder}{folderSecond}{Index} {secondKeyword} {i + 1}.tif", ImageFormat.Tiff);
                        //SaveImage(first, String.Concat(folder, folderFirst, Index.ToString(), " ", firstKeyword, " ", (i + 1).ToString(), ".tif") );
                        //SaveImage(second, String.Concat(folder, folderSecond, Index.ToString(), " ", secondKeyword, " ", (i + 1).ToString(), ".tif"));
                    }
                }
                MessageBox.Show("Crop complete.");
            }
            catch
            {
                MessageBox.Show("Error when cropping.");
            }
        }

        /*private void SaveImage(BitmapSource image, string filename)
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
        }*/

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
            //BackupFolders(folder);
            CreateOutputFolder(folder, folderFirst);
            CreateOutputFolder(folder, folderSecond);
        }

    }
}
