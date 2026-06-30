using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyNhaHang.Controls;

public class EditablePaginatedView : ContentControl
{
    public string AddRowButtonText
    {
        get => (string)GetValue(AddRowButtonTextProperty);
        set => SetValue(AddRowButtonTextProperty, value);
    }
    public bool IsAddRowButtonVisible
    {
        get => (bool)GetValue(IsAddRowButtonVisibleProperty);
        set => SetValue(IsAddRowButtonVisibleProperty, value);
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

    public static readonly DependencyProperty AddRowButtonTextProperty =
        DependencyProperty.Register(nameof(AddRowButtonText), typeof(string), typeof(EditablePaginatedView), new PropertyMetadata("Thêm dòng"));
    public static readonly DependencyProperty IsAddRowButtonVisibleProperty =
        DependencyProperty.Register(nameof(IsAddRowButtonVisible), typeof(bool), typeof(EditablePaginatedView), new PropertyMetadata(true));
    public static readonly DependencyProperty AddRowCommandProperty =
        DependencyProperty.Register(nameof(AddRowCommand), typeof(ICommand), typeof(EditablePaginatedView), new PropertyMetadata(null));
    public static readonly DependencyProperty PageSizeProperty =
        DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(EditablePaginatedView), new PropertyMetadata(10));
    public static readonly DependencyProperty PageNumberProperty =
        DependencyProperty.Register(nameof(PageNumber), typeof(int), typeof(EditablePaginatedView), new PropertyMetadata(1));
    public static readonly DependencyProperty TotalInfoTextProperty =
        DependencyProperty.Register(nameof(TotalInfoText), typeof(string), typeof(EditablePaginatedView), new PropertyMetadata(""));
    public static readonly DependencyProperty PrevPageCommandProperty =
        DependencyProperty.Register(nameof(PrevPageCommand), typeof(ICommand), typeof(EditablePaginatedView), new PropertyMetadata(null));
    public static readonly DependencyProperty NextPageCommandProperty =
        DependencyProperty.Register(nameof(NextPageCommand), typeof(ICommand), typeof(EditablePaginatedView), new PropertyMetadata(null));
    public static readonly DependencyProperty PageSizesProperty =
        DependencyProperty.Register(nameof(PageSizes), typeof(IEnumerable), typeof(EditablePaginatedView), new PropertyMetadata(null));
    public static readonly DependencyProperty PageNumbersProperty =
        DependencyProperty.Register(nameof(PageNumbers), typeof(IEnumerable), typeof(EditablePaginatedView), new PropertyMetadata(null));

    static EditablePaginatedView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(EditablePaginatedView), new FrameworkPropertyMetadata(typeof(EditablePaginatedView)));
    }
}
