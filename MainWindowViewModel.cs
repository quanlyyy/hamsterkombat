using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ekz.Models;
using Ekz.Services;

namespace Ekz.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();
    [ObservableProperty]
    private Product? _selectedProduct;

    [ObservableProperty]
    private ObservableCollection<Zakaz> _zakazList = new();
    [ObservableProperty]
    private Zakaz? _selectedZakaz;

    [ObservableProperty]
    private string _statusMessage = string.Empty;
    [ObservableProperty]
    private bool _isLoading;

    public MainWindowViewModel()
    {
        _dbService = new DatabaseService();
        Task.Run(async () => await LoadAllData());
    }

    private async Task LoadAllData()
    {
        await LoadProducts();
        await LoadZakaz();
    }

    // ========== Товары ==========
    [RelayCommand]
    private async Task LoadProducts()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Загрузка товаров...";
            var list = await _dbService.GetProducts();
            Products.Clear();
            foreach (var p in list)
                Products.Add(p);
            StatusMessage = $"Товаров загружено: {Products.Count}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки товаров: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task AddProduct()
    {
        try
        {
            var newProduct = new Product { Name = "Новый товар", Price = 0, Quantity = 0 };
            var newId = await _dbService.AddProduct(newProduct);
            newProduct.Id = newId;
            Products.Add(newProduct);
            StatusMessage = $"Товар добавлен (ID={newId})";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка добавления: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteProduct()
    {
        if (SelectedProduct == null)
        {
            StatusMessage = "Сначала выберите товар (кнопка 'Выбрать')";
            return;
        }
        try
        {
            bool success = await _dbService.DeleteProduct(SelectedProduct.Id);
            if (success)
            {
                Products.Remove(SelectedProduct);
                StatusMessage = "Товар удалён";
            }
            else StatusMessage = "Ошибка удаления";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveProducts()
    {
        try
        {
            foreach (var p in Products)
            {
                if (p.Id == 0)
                    await _dbService.AddProduct(p);
                else
                    await _dbService.UpdateProduct(p);
            }
            StatusMessage = "Все изменения товаров сохранены";
            await LoadProducts();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сохранения: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SelectProduct(Product product)
    {
        SelectedProduct = product;
        StatusMessage = $"Выбран товар: {product.Name} (ID={product.Id})";
    }

    // ========== Заказы ==========
    [RelayCommand]
    private async Task LoadZakaz()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Загрузка заказов...";
            var list = await _dbService.GetZakaz();
            ZakazList.Clear();
            foreach (var z in list)
                ZakazList.Add(z);
            StatusMessage = $"Заказов загружено: {ZakazList.Count}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки заказов: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task AddZakaz()
    {
        try
        {
            var newZakaz = new Zakaz { KlId = 1, Summa = 0, PrId = 1, ProductName = "Не выбран" };
            var newId = await _dbService.AddZakaz(newZakaz);
            newZakaz.IdZakaza = newId;
            ZakazList.Add(newZakaz);
            StatusMessage = $"Заказ добавлен (ID={newId})";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка добавления заказа: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteZakaz()
    {
        if (SelectedZakaz == null)
        {
            StatusMessage = "Сначала выберите заказ (кнопка 'Выбрать')";
            return;
        }
        try
        {
            bool success = await _dbService.DeleteZakaz(SelectedZakaz.IdZakaza);
            if (success)
            {
                ZakazList.Remove(SelectedZakaz);
                StatusMessage = "Заказ удалён";
            }
            else StatusMessage = "Ошибка удаления заказа";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveZakaz()
    {
        try
        {
            foreach (var z in ZakazList)
            {
                if (z.IdZakaza == 0)
                    await _dbService.AddZakaz(z);
                else
                    await _dbService.UpdateZakaz(z);
            }
            StatusMessage = "Все изменения заказов сохранены";
            await LoadZakaz();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сохранения заказов: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SelectZakaz(Zakaz zakaz)
    {
        SelectedZakaz = zakaz;
        StatusMessage = $"Выбран заказ #{zakaz.IdZakaza}";
    }
}