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

// Bước 1: Nhận D1 (D1: Mã bàn, Phụ thu, Thời gian thanh toán, Tổng tiền,
// Danh sách các món ăn có trong hóa đơn thanh toán) từ người dùng.
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

            // Bước 2: Đọc D2 (D2: danh sách bàn ăn) từ CSDL bàn.
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

        // Bước 6: Đọc D4 (D4: Danh sách phiếu gọi món) từ CSDL phiếu gọi món.
        // Bước 7: Kiểm tra tồn tại ít nhất 1 phiếu gọi món có trạng thái
        // Đang chế biến hoặc Đã phục vụ (D4) ứng với "mã bàn" (D1) và chưa được thanh toán.
        // Nếu không thì tới bước 11.
        // Bước 8: Đọc D3 (D3: Danh sách món ăn) từ D4
        // Bước 9: Kiểm tra "tên món ăn" (D1) có thuộc D3 hay không? Nếu không thì tới bước 11.
        // Không cần đọc. Chúng ta query trực tiếp qua bảng liên kết với phiếu gọi món.
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
        // Bước 3: Kiểm tra "mã bàn" (D1) có thuộc D2 hay không? Nếu không thuộc thì tới bước 11.
        ValidateAllProperties();

        if (HasErrors)
        {
            var errors = GetErrors().Select(e => e.ErrorMessage).ToList();
            MessageBox.Show(string.Join("\n", errors), "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            // Bước 11: Kết thúc
            return;
        }

        if (InvoiceDetails.Count == 0)
        {
            MessageBox.Show("Bàn này không có phiếu gọi món nào chưa thanh toán và đủ điều kiện.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            // Bước 11: Kết thúc.
            return;
        }

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                // Bước 10: Lưu D5☰D1 xuống CSDL Hóa đơn.
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
                // Bước 11: Kết thúc.
                return;
            }
            catch (DbUpdateException ex) when (ex.IsPrimaryKeyViolation() && attempt < maxRetries - 1)
            {
                await GenerateNextMaHoaDonAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                // Bước 11: Kết thúc.
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
