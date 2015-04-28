using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Invadopodia.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Invadopodia.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ImageList = new ObservableCollection<ImageGroup>();
            if (IsInDesignMode)
            {
            }
            else
            {
            }
        }

        private ObservableCollection<ImageGroup> imageList;
        public ObservableCollection<ImageGroup> ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                Set(() => ImageList, ref imageList, value);
            }
        }

        private string imageFolder = "No folder selected.";
        public string ImageFolder
        {
            get
            {
                return imageFolder;
            }
            set
            {
                Set(() => ImageFolder, ref imageFolder, value);
            }
        }

        private RelayCommand openFolderCommand;
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return openFolderCommand ?? (openFolderCommand = new RelayCommand(ExecuteOpenFolderCommand));
            }
        }
        private void ExecuteOpenFolderCommand()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ImageFolder = dialog.SelectedPath;
                LoadImagesFromFolder(ImageFolder);
            }
        }

        private RelayCommand undoCommand;
        public RelayCommand UndoCommand
        {
            get
            {
                return undoCommand ?? (undoCommand = new RelayCommand(ExecuteUndoCommand));
            }
        }
        private void ExecuteUndoCommand()
        {
            if (SelectedImageGroup != null)
            {
                if (SelectedImageGroup.Rectangles.Count > 0)
                    SelectedImageGroup.Rectangles.RemoveAt(SelectedImageGroup.Rectangles.Count - 1);
            }
        }

        private RelayCommand cropCommand;
        public RelayCommand CropCommand
        {
            get
            {
                return cropCommand ?? (cropCommand = new RelayCommand(ExecuteCropCommand));
            }
        }
        private void ExecuteCropCommand()
        {
            if (SelectedImageGroup != null)
                SelectedImageGroup.Crop(ImageFolder);
        }

        private void LoadImagesFromFolder(string folder)
        {
            ImageList.Clear();
            string[] files = Directory.GetFiles(folder, "*.tif");
            for (int fileIndex = 0; fileIndex < files.Length - 1; fileIndex += 2)
            {
                ImageGroup tmpGroup = new ImageGroup();
                tmpGroup.ImageFirst = new BitmapImage(new System.Uri(files[fileIndex]));;
                tmpGroup.ImageSecond = new BitmapImage(new System.Uri(files[fileIndex + 1]));;
                tmpGroup.Index = (fileIndex / 2) + 1;
                ImageList.Add(tmpGroup);
            }
            if (ImageList.Count > 0)
                SelectedImageGroup = ImageList[0];   
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
                MessengerInstance.Send<ImageGroup>(SelectedImageGroup);
            }
        }

    }
}