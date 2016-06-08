using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Invadopodia.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WPFFolderBrowser;

namespace Invadopodia.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
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
                MessengerInstance.Send(IsSquareSelection);
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
        private async void ExecuteOpenFolderCommand()
        {
            WPFFolderBrowserDialog dialog = new WPFFolderBrowserDialog();
            dialog.Title = "Please select a directory.";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                ImageFolder = dialog.FileName;
                MetroWindow metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
                var controller = await metroWindow.ShowProgressAsync("Please wait...", "Progress message");
                controller.SetProgress(0);
                await LoadImagesFromFolder(ImageFolder, controller);
                await controller.CloseAsync();
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

        private async Task LoadImagesFromFolder(string folder, ProgressDialogController controller)
        {
            ImageList.Clear();
            string[] files = Directory.GetFiles(folder, "*.tif");
            for (int fileIndex = 0; fileIndex < files.Length - 1; fileIndex += 2)
            {
                string firstFile = null;
                string secondFile = null;
                if (files[fileIndex].IndexOf("actin", StringComparison.OrdinalIgnoreCase) >= 0)
                    firstFile = files[fileIndex];
                else if (files[fileIndex + 1].IndexOf("actin", StringComparison.OrdinalIgnoreCase) >= 0)
                    firstFile = files[fileIndex + 1];

                if (files[fileIndex].IndexOf("pla", StringComparison.OrdinalIgnoreCase) >= 0)
                    secondFile = files[fileIndex];
                else if (files[fileIndex + 1].IndexOf("pla", StringComparison.OrdinalIgnoreCase) >= 0)
                    secondFile = files[fileIndex + 1];
                //string firstFile = files[fileIndex].Contains("actin") ? files[fileIndex] : (files[fileIndex + 1].Contains("actin") ? files[fileIndex + 1] : null);
                //string secondFile = files[fileIndex].Contains("pla") ? files[fileIndex] : (files[fileIndex + 1].Contains("pla") ? files[fileIndex + 1] : null);
                int index = (fileIndex / 2) + 1;
                if (firstFile == null || secondFile == null)
                {
                    MessageBox.Show("The image structure in the selected folder does not match the requirements. Make sure no images are open in another program.");
                    break;
                }
                else
                {
                    LoadingProgressValue = (double)fileIndex / (double)(files.Length - 1);
                    controller.SetProgress((double)fileIndex / (double)(files.Length - 1));
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

        private double loadingProgressValue;
        public double LoadingProgressValue
        {
            get
            {
                return loadingProgressValue;
            }
            set
            {
                Set(() => LoadingProgressValue, ref loadingProgressValue, value);
            }
        }

        private bool isLoadingBarVisible = false;
        public bool IsLoadingBarVisible
        {
            get
            {
                return isLoadingBarVisible;
            }
            set
            {
                Set(() => IsLoadingBarVisible, ref isLoadingBarVisible, value);
            }
        }

    }
}