using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyNhaHang.Controls;

public class PaginatedView : ContentControl
{
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public object? ButtonContent
    {
        get => GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }
    public object? MainContent
    {
        get => GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
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
    public ICommand? PrevPageCommand
    {
        get => (ICommand?)GetValue(PrevPageCommandProperty);
        set => SetValue(PrevPageCommandProperty, value);
    }
    public ICommand? NextPageCommand
    {
        get => (ICommand?)GetValue(NextPageCommandProperty);
        set => SetValue(NextPageCommandProperty, value);
    }
    public IEnumerable? PageSizes
    {
        get => (IEnumerable?)GetValue(PageSizesProperty);
        set => SetValue(PageSizesProperty, value);
    }
    public IEnumerable? PageNumbers
    {
        get => (IEnumerable?)GetValue(PageNumbersProperty);
        set => SetValue(PageNumbersProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register(nameof(Title), typeof(string), typeof(PaginatedView), new PropertyMetadata(""));

    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register(nameof(ButtonContent), typeof(object), typeof(PaginatedView), new PropertyMetadata(null));

    public static readonly DependencyProperty MainContentProperty =
        DependencyProperty.Register(nameof(MainContent), typeof(object), typeof(PaginatedView), new PropertyMetadata(null));

    public static readonly DependencyProperty PageSizeProperty =
        DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(PaginatedView), new PropertyMetadata(10));

    public static readonly DependencyProperty PageNumberProperty =
        DependencyProperty.Register(nameof(PageNumber), typeof(int), typeof(PaginatedView), new PropertyMetadata(1));

    public static readonly DependencyProperty TotalInfoTextProperty =
        DependencyProperty.Register(nameof(TotalInfoText), typeof(string), typeof(PaginatedView), new PropertyMetadata(""));

    public static readonly DependencyProperty PrevPageCommandProperty =
        DependencyProperty.Register(nameof(PrevPageCommand), typeof(ICommand), typeof(PaginatedView), new PropertyMetadata(null));

    public static readonly DependencyProperty NextPageCommandProperty =
        DependencyProperty.Register(nameof(NextPageCommand), typeof(ICommand), typeof(PaginatedView), new PropertyMetadata(null));

    public static readonly DependencyProperty PageSizesProperty =
        DependencyProperty.Register(nameof(PageSizes), typeof(IEnumerable), typeof(PaginatedView), new PropertyMetadata(null));

    public static readonly DependencyProperty PageNumbersProperty =
        DependencyProperty.Register(nameof(PageNumbers), typeof(IEnumerable), typeof(PaginatedView), new PropertyMetadata(null));

    static PaginatedView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginatedView), new FrameworkPropertyMetadata(typeof(PaginatedView)));
    }
}
