using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyNhaHang.Controls;

public class ControlTable : ContentControl
{
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public IEnumerable DishesSource
    {
        get => (IEnumerable)GetValue(DishesSourceProperty);
        set => SetValue(DishesSourceProperty, value);
    }

    public ICommand AddRowCommand
    {
        get => (ICommand)GetValue(AddRowCommandProperty);
        set => SetValue(AddRowCommandProperty, value);
    }

    public int PageSize
    {
        get => (int)GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }

    public int PageNumber
    {
        get => (int)GetValue(PageNumberProperty);
        set => SetValue(PageNumberProperty, value);
    }

    public string TotalInfoText
    {
        get => (string)GetValue(TotalInfoTextProperty);
        set => SetValue(TotalInfoTextProperty, value);
    }

    public ICommand PrevPageCommand
    {
        get => (ICommand)GetValue(PrevPageCommandProperty);
        set => SetValue(PrevPageCommandProperty, value);
    }

    public ICommand NextPageCommand
    {
        get => (ICommand)GetValue(NextPageCommandProperty);
        set => SetValue(NextPageCommandProperty, value);
    }

    public IEnumerable PageSizes
    {
        get => (IEnumerable)GetValue(PageSizesProperty);
        set => SetValue(PageSizesProperty, value);
    }

    public IEnumerable PageNumbers
    {
        get => (IEnumerable)GetValue(PageNumbersProperty);
        set => SetValue(PageNumbersProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ControlTable), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(ControlTable), new PropertyMetadata(null));

    public static readonly DependencyProperty DishesSourceProperty =
        DependencyProperty.Register(nameof(DishesSource), typeof(IEnumerable), typeof(ControlTable), new PropertyMetadata(null));

    public static readonly DependencyProperty AddRowCommandProperty =
        DependencyProperty.Register(nameof(AddRowCommand), typeof(ICommand), typeof(ControlTable), new PropertyMetadata(null));

    public static readonly DependencyProperty PageSizeProperty =
        DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(ControlTable), new PropertyMetadata(10));

    public static readonly DependencyProperty PageNumberProperty =
        DependencyProperty.Register(nameof(PageNumber), typeof(int), typeof(ControlTable), new PropertyMetadata(1));

    public static readonly DependencyProperty TotalInfoTextProperty =
        DependencyProperty.Register(nameof(TotalInfoText), typeof(string), typeof(ControlTable), new PropertyMetadata(""));

    public static readonly DependencyProperty PrevPageCommandProperty =
        DependencyProperty.Register(nameof(PrevPageCommand), typeof(ICommand), typeof(ControlTable), new PropertyMetadata(null));

    public static readonly DependencyProperty NextPageCommandProperty =
        DependencyProperty.Register(nameof(NextPageCommand), typeof(ICommand), typeof(ControlTable), new PropertyMetadata(null));

    public static readonly DependencyProperty PageSizesProperty =
        DependencyProperty.Register(nameof(PageSizes), typeof(IEnumerable), typeof(ControlTable), new PropertyMetadata(null));

    public static readonly DependencyProperty PageNumbersProperty =
        DependencyProperty.Register(nameof(PageNumbers), typeof(IEnumerable), typeof(ControlTable), new PropertyMetadata(null));

    static ControlTable()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlTable), new FrameworkPropertyMetadata(typeof(ControlTable)));
    }
}
