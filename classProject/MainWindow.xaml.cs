using classProject.Helper;
using Mvvm;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace classProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public classViewModel classMvvm;
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = classMvvm = new classViewModel();






        }
        public class classViewModel : ViewModelBase
        {


            private  ObservableCollection<dog> _listDog;
            public  ObservableCollection<dog> listDog
            {
                get
                {
                    return _listDog;
                }
                set
                {
                    if (_listDog != value)
                    {
                        _listDog = value;
                        if (_listDog.Count > 0)
                        {
                            _listDog.OrderBy(x => x.breed);
                            
                            selecetdDog = _listDog.FirstOrDefault();
                        }
                        OnPropertyChanged();
                    }
                }
            }

            // Decided to keep original record list to have a better control for searching feature  
            private ObservableCollection<dog> _originalListDog;
            public ObservableCollection<dog> originalListDog
            {
                get
                {
                    return _originalListDog;
                }
                set
                {
                    if (_originalListDog != value)
                    {
                        _originalListDog = value;
                      
                        OnPropertyChanged();
                    }
                }
            }


            // Decided to use UpdateSourceTrigger to avoid search button, having a search for each chnage in the textbox   
            private string _searchText;
            public string searchText
            {
                get
                {
                    return _searchText;
                }
                set
                {
                    if (_searchText != value)
                    {
                        _searchText = value;
                        if (string.IsNullOrWhiteSpace(_searchText))
                        {
                            listDog = originalListDog ;
                            isAsceding = true;
                        }
                        else
                        {
                            listDog = new ObservableCollection<dog>(originalListDog.Where(x => x.breed.ToUpper().Contains(_searchText.ToUpper())));
                            OnPropertyChanged();
                        }
                        
                    }
                }
            }

            // Defined my own class for dog  
            private dog _selecetdDog;
            public dog selecetdDog
            {
                get
                {
                    return _selecetdDog;
                }
                set
                {
                    if (_selecetdDog != value)
                    {
                        _selecetdDog = value;
                        if (_selecetdDog != null)
                        {
                            textPart1 = _selecetdDog.breed;
                            textPart2 = _selecetdDog.subBreed;

                            getRandomImage(_selecetdDog.breed);// Getting a random photo for each selected dog breed 
                        }
                        OnPropertyChanged();
                    }
                }
            }


            // Making async. call for getting random photo
            public async void getRandomImage(string breed)
            {
                var client = new RestClient();

                var request = new RestRequest("https://dog.ceo/api/breed/" + breed + "/images/random");
                client.Timeout = -1;
                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                bool success = ((int)restResponse.StatusCode) == 200;
                if (success)
                {
                    dynamic o = JsonConvert.DeserializeObject(restResponse.Content);
                  
                    randomPhoto = o["message"].ToString();
                        
                    
                }
                else
                {
                    randomPhoto = "No photo Found !";
                }
            }

            private string _textPart1;
            public string textPart1
            {
                get
                {
                    return _textPart1;
                }
                set
                {
                    if (_textPart1 != value)
                    {
                        _textPart1 = value;
                        OnPropertyChanged();
                    }
                }
            }
            private string _randomPhoto;
            
            // As photos are availabel through Uri passing the link as a string for image source 
            public string randomPhoto
            {
                get
                {
                    return _randomPhoto;
                }
                set
                {
                    if (_randomPhoto != value)
                    {
                        _randomPhoto = value;
                        OnPropertyChanged();
                    }
                }
            }

            private string _textPart2;
            public string textPart2
            {
                get
                {
                    return _textPart2;
                }
                set
                {
                    if (_textPart2 != value)
                    {
                        _textPart2 = value;
                        OnPropertyChanged();
                    }
                }
            }
            // Kept this variable for making an order of the list ascending/descending 
            private bool _isAsceding;
            public bool isAsceding
            {
                get
                {
                    return _isAsceding;
                }
                set
                {
                    if (_isAsceding != value)
                    {
                        _isAsceding = value;
                        OnPropertyChanged();
                    }
                }
            }



            public ViewModelCommand searchDog { get; set; }

            public classViewModel() : base()
            {
                listDog = new ObservableCollection<dog>();
                getDogList();
               


            }

         
            // Making async. call to get the list of dog breeds
            public async void getDogList()
            {

                listDog.Clear();
                var client = new RestClient();

                var request = new RestRequest("https://dog.ceo/api/breeds/list/all");
                client.Timeout = -1;
                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                bool success = ((int)restResponse.StatusCode) == 200;
                if (success)
                {
                    dynamic o = JsonConvert.DeserializeObject(restResponse.Content);

                    
                    foreach(dynamic each in o["message"])
                    {
                        string first = string.Empty;
                        string second = string.Empty;
                        dog sample = new dog();
                        string sampleString = each.ToString();
                        var splitString = sampleString.Split(':');
                        if (splitString.Length == 2)
                        {
                             first = splitString[0].Replace("\"", "");
                             second = splitString[1].Replace("\"", "");
                        }
                        
                       
                            sample.breed = first;
                            sample.subBreed = second;
                            listDog.Add(sample);

                        
                        
                        
                    }



                  


                }
                listDog = new ObservableCollection<dog>(listDog.OrderBy(x => x.breed));
                originalListDog = listDog;
                isAsceding = true;


            }
        }


        // sorting the list of breeds ascending/descending
        private void View_Click(object sender, RoutedEventArgs e)
        {

            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (classMvvm.isAsceding && classMvvm.listDog.Count>0)
            {
                classMvvm.listDog =  new ObservableCollection<dog>(classMvvm.listDog.OrderByDescending(x => x.breed));
                classMvvm.isAsceding = false;
                
            }
            else if (!classMvvm.isAsceding && classMvvm.listDog.Count > 0)
            {
                classMvvm.listDog = new ObservableCollection<dog>(classMvvm.listDog.OrderBy(x => x.breed));
                classMvvm.isAsceding = true;

            }

        }
    }
}
