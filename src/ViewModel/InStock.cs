namespace ViewModel
{
    public enum SearchInStock
    {
        Nevermind = 0,
        Yes = 1,
        No = 2,
    }

    public class InStock
    {
        public InStock(SearchInStock searchInStock)
        {
            SearchInStock = searchInStock;
        }

        public SearchInStock SearchInStock { get; set; }

        public string InStockName
        {
            get
            {
                if (SearchInStock == SearchInStock.Yes)
                {
                    return "В наличии";
                }
                else if (SearchInStock == SearchInStock.No)
                {
                    return "Отсутствует";
                }
                else
                {
                    return "Выбрать всё";
                }
            }
        }
    }
}