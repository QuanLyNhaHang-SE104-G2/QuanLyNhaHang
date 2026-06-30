using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class TiepNhanHoaDonViewModel : InMemoryPaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private async Task<List<string>> GetAllowedTrangThaisForPaymentAsync(AppDbContext context)
    {
        return await context.GetTrangThaiThanhToanAsync();
    }

    protected override string EntityLabel => "món ăn";

    [ObservableProperty]
    private int _maHoaDon;

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn bàn ăn.")]
    private int? _selectedMaBan;

    [ObservableProperty]
    private List<Ban> _bans = [];

    [ObservableProperty]
    private System.DateTime _thoiGianThanhToan = System.DateTime.Now;

    [ObservableProperty]
    private string _phuThuText = "0 VND";

    [ObservableProperty]
    private string _tongTienText = "0 VND";

    [ObservableProperty]
    private ObservableCollection<HoaDonDetailItemViewModel> _invoiceDetails = [];

    [ObservableProperty]
    private ObservableCollection<HoaDonDetailItemViewModel> _pagedItems = [];

    private long _phuThuVal;
    private long _tongTienVal;

    public TiepNhanHoaDonViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        MaHoaDon = 0;
        SelectedMaBan = null;
        ThoiGianThanhToan = System.DateTime.Now;
        PhuThuText = "0 VND";
        TongTienText = "0 VND";
        _phuThuVal = 0;
        _tongTienVal = 0;
        InvoiceDetails.Clear();
    }

    public async Task InitializeFormAsync()
    {
        ClearFields();

        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var allowedStates = await GetAllowedTrangThaisForPaymentAsync(context);

            var eligibleTableIds = await context.PhieuGoiMon
                .AsNoTracking()
                .Where(p => p.MaHoaDon == null && allowedStates.Contains(p.MaTrangThai))
                .Select(p => p.MaBan)
                .Distinct()
                .ToListAsync();

            Bans = await context.Ban
                .AsNoTracking()
                .Where(b => eligibleTableIds.Contains(b.MaBan))
                .OrderBy(b => b.TenBan)
                .ToListAsync();
        }

        await GenerateNextMaHoaDonAsync();

        OnPageChanged();
    }

    private async Task GenerateNextMaHoaDonAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        int? maxId = await context.HoaDon
            .Select(h => (int?)h.MaHoaDon)
            .MaxAsync();
        MaHoaDon = (maxId ?? 0) + 1;
    }

    protected override void OnPageChanged()
    {
        TotalItems = InvoiceDetails.Count;
        UpdatePaginationInfo();

        var pageElements = InvoiceDetails.GetPage(PageNumber, PageSize).ToList();
        PagedItems = new ObservableCollection<HoaDonDetailItemViewModel>(pageElements);
    }

    partial void OnSelectedMaBanChanged(int? value)
    {
        if (value.HasValue)
        {
            LoadTableDetailsAsync(value.Value).SafeFireAndForget(onError: ex =>
                MessageBox.Show($"Lỗi tải thông tin bàn: {ex.Message}", "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error));
        }
        else
        {
            ClearTableDetails();
        }
    }

    private async Task LoadTableDetailsAsync(int maBan)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var selectedTable = await context.Ban
            .Include(b => b.LoaiBan)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.MaBan == maBan);

        _phuThuVal = selectedTable?.LoaiBan?.PhuThu ?? 0;
        PhuThuText = $"{_phuThuVal:N0} VND";

        var allowedStates = await GetAllowedTrangThaisForPaymentAsync(context);

        var orders = await context.PhieuGoiMon
            .Include(p => p.CTGoiMons)
                .ThenInclude(ct => ct.MonAn)
                    .ThenInclude(m => m.DonViTinh)
            .AsNoTracking()
            .Where(p => p.MaBan == maBan && p.MaHoaDon == null && allowedStates.Contains(p.MaTrangThai))
            .OrderBy(p => p.ThoiGianGoi)
            .ToListAsync();

        var details = new List<HoaDonDetailItemViewModel>();
        int stt = 1;
        long totalDishesAmount = 0;

        foreach (var order in orders)
        {
            foreach (var ct in order.CTGoiMons)
            {
                long itemTotal = ct.SoLuong * ct.DonGia;
                totalDishesAmount += itemTotal;
                details.Add(new HoaDonDetailItemViewModel
                {
                    STT = stt++,
                    TenMonAn = ct.MonAn?.TenMonAn ?? "",
                    SoLuong = ct.SoLuong,
                    TenDonViTinh = ct.MonAn?.DonViTinh?.TenDonViTinh ?? "",
                    DonGia = ct.DonGia,
                    DonGiaText = $"{ct.DonGia:N0} VND",
                    ThoiGianGoiText = order.ThoiGianGoi.ToString("yyyy-MM-dd HH:mm"),
                    ThanhTien = itemTotal,
                    ThanhTienText = $"{itemTotal:N0} VND"
                });
            }
        }

        InvoiceDetails = new ObservableCollection<HoaDonDetailItemViewModel>(details);
        _tongTienVal = totalDishesAmount + _phuThuVal;
        TongTienText = $"{_tongTienVal:N0} VND";

        OnPageChanged();
    }

    private void ClearTableDetails()
    {
        _phuThuVal = 0;
        PhuThuText = "0 VND";
        InvoiceDetails.Clear();
        _tongTienVal = 0;
        TongTienText = "0 VND";
        OnPageChanged();
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        await InitializeFormAsync();
    }

    [RelayCommand]
    private async Task AcceptAsync(Window? window)
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            var errors = GetErrors().Select(e => e.ErrorMessage).ToList();
            MessageBox.Show(string.Join("\n", errors), "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (InvoiceDetails.Count == 0)
        {
            MessageBox.Show("Bàn này không có phiếu gọi món nào chưa thanh toán và đủ điều kiện.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                using var context = await _dbContextFactory.CreateDbContextAsync();

                var newHoaDon = new HoaDon
                {
                    MaHoaDon = MaHoaDon,
                    ThoiGianThanhToan = System.DateTime.Now,
                    PhuThu = _phuThuVal,
                    TongTien = _tongTienVal
                };

                var allowedStates = await GetAllowedTrangThaisForPaymentAsync(context);

                var orders = await context.PhieuGoiMon
                    .Where(p => p.MaBan == SelectedMaBan!.Value && p.MaHoaDon == null && allowedStates.Contains(p.MaTrangThai))
                    .ToListAsync();

                foreach (var order in orders)
                {
                    order.MaHoaDon = newHoaDon.MaHoaDon;
                }

                await context.HoaDon.AddAsync(newHoaDon);
                await context.SaveChangesAsync();

                MessageBox.Show("Lập hóa đơn thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
                return;
            }
            catch (DbUpdateException ex) when (ex.IsPrimaryKeyViolation() && attempt < maxRetries - 1)
            {
                await GenerateNextMaHoaDonAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        MessageBox.Show("Không thể lưu hóa đơn do xung đột mã liên tục. Vui lòng thử lại.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    [RelayCommand]
    private void Cancel(Window? window)
    {
        if (window != null)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
