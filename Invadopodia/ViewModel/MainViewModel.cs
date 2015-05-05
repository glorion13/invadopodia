using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Invadopodia.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WPFFolderBrowser;

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
            WPFFolderBrowserDialog dialog = new WPFFolderBrowserDialog();
            dialog.Title = "Please select a directory.";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                ImageFolder = dialog.FileName;
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
                string firstFile = files[fileIndex].Contains("actin") ? files[fileIndex] : (files[fileIndex + 1].Contains("actin") ? files[fileIndex + 1] : null);
                string secondFile = files[fileIndex].Contains("gelatin") ? files[fileIndex] : (files[fileIndex + 1].Contains("gelatin") ? files[fileIndex + 1] : null);
                int index = (fileIndex / 2) + 1;
                if (firstFile == null || secondFile == null)
                {
                    MessageBox.Show("The image structure in the selected folder does not match the requirements.");
                    break;
                }
                else
                {
                    ImageGroup newImageGroup = new ImageGroup(firstFile, secondFile, index);
                    ImageList.Add(newImageGroup);
                }
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