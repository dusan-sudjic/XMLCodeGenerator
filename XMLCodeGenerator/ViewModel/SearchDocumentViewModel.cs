using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.View;

namespace XMLCodeGenerator.ViewModel
{
    public class SearchDocumentViewModel: INotifyPropertyChanged
    {
        private string _searchParameter = "";
        public string SearchParameter
        {
            get => _searchParameter;
            set
            {
                if (value != _searchParameter)
                {
                    _searchParameter = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _searchWord = "";
        public string SearchWord
        {
            get => _searchWord;
            set
            {
                if (value != _searchWord)
                {
                    _searchWord = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _occurrencesOfSearchParameter = 0;
        public int OccurrencesOfSearchParameter
        {
            get => _occurrencesOfSearchParameter;
            set
            {
                if (value != _occurrencesOfSearchParameter)
                {
                    _occurrencesOfSearchParameter = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isSeacrhActive = false;
        public bool IsSearchActive
        {
            get => _isSeacrhActive;
            set
            {
                if (value != _isSeacrhActive)
                {
                    _isSeacrhActive = value;
                    OnPropertyChanged();
                }
            }
        }
        private ElementViewModel _selectedSearchResult = null;
        public ElementViewModel SelectedSearchResult
        {
            get => _selectedSearchResult;
            set
            {
                if (value != _selectedSearchResult)
                {
                    if (_selectedSearchResult != null)
                        _selectedSearchResult.IsSelected = false;
                    _selectedSearchResult = value;
                    if (_selectedSearchResult != null)
                        _selectedSearchResult.IsSelected = true;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ElementViewModel> SearchResults { get; set; } = new();
        public DocumentViewModel Document { get => MainWindow.Document; }
        public SearchDocumentViewModel()
        {
            
        }
        public void SearchDocument(int selectedTabIndex)
        {
            resetDocumentSearch();
            List<ElementViewModel> results = new();
            if (SearchParameter.Trim().Equals(",") || SearchParameter.Trim().Length == 0 || SearchParameter.Trim().Equals("[") || SearchParameter.Trim().Equals("]"))
                return;
            string[] parameters = SearchParameter.Split(' ');
            switch (selectedTabIndex)
            {
                case 0: { results = Document.CimClasses.SearchElement(parameters, skip: true); break; }
                case 1: { results = Document.FunctionDefinitions.SearchElement(parameters, skip: true); break; }
                case 2: { results = Document.PreProcessProcedures.SearchElement(parameters, skip: true); break; }
                case 3: { results = Document.RewritingProcedures.SearchElement(parameters, skip: true); break; }
            }
            foreach (var item in results)
            {
                item.IsHighlighted = true;
                SearchResults.Add(item);
            }
            IsSearchActive = true;
            SearchWord = SearchParameter;
            OccurrencesOfSearchParameter = SearchResults.Count;
        }
        private void resetDocumentSearch()
        {
            foreach (var item in SearchResults)
                item.IsHighlighted = false;
            SearchResults.Clear();
        }
        public void ResetSearch()
        {
            resetDocumentSearch();
            SearchParameter = "";
            IsSearchActive = false;
            SearchWord = "";
            OccurrencesOfSearchParameter = 0;
        }
        public void DownArrowClicked()
        {
            if (!SearchResults.Any())
                return;
            if (SelectedSearchResult == null || SelectedSearchResult == SearchResults[SearchResults.Count - 1])
                SelectedSearchResult = SearchResults[0];
            else
                SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) + 1];
        }
        public void UpArrowClicked()
        {
            if (!SearchResults.Any())
                return;
            if (SelectedSearchResult == null || SelectedSearchResult == SearchResults[0])
                SelectedSearchResult = SearchResults[SearchResults.Count - 1];
            else
                SelectedSearchResult = SearchResults[SearchResults.IndexOf(SelectedSearchResult) - 1];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
