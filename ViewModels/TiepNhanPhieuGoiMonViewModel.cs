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

public partial class TiepNhanPhieuGoiMonViewModel : InMemoryPaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    protected override string EntityLabel => "món ăn";

    [ObservableProperty]
    private int _maPhieuGoiMon;

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn bàn ăn.")]
    private int? _selectedMaBan;

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn nhân viên phục vụ.")]
    private int? _selectedMaNhanVien;

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn trạng thái phiếu.")]
    private string _selectedMaTrangThai = "";

    [ObservableProperty]
    private System.DateTime _thoiGianGoi = System.DateTime.Now;

    [ObservableProperty]
    private string _tongTienTamTinhText = "0 VND";

    [ObservableProperty]
    private List<Ban> _bans = [];

    [ObservableProperty]
    private List<NhanVien> _nhanViens = [];

    [ObservableProperty]
    private List<TrangThai> _trangThais = [];

    [ObservableProperty]
    private List<MonAn> _activeMonAns = [];

    [ObservableProperty]
    private ObservableCollection<CTGoiMonItemViewModel> _orderDetails = [];

    [ObservableProperty]
    private ObservableCollection<CTGoiMonItemViewModel> _pagedItems = [];

    [ObservableProperty]
    private CTGoiMonItemViewModel? _selectedOrderDetail;

    public TiepNhanPhieuGoiMonViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        MaPhieuGoiMon = 0;
        SelectedMaBan = null;
        SelectedMaNhanVien = null;
        SelectedMaTrangThai = "";
        ThoiGianGoi = System.DateTime.Now;
        TongTienTamTinhText = "0 VND";
        SelectedOrderDetail = null;
        OrderDetails.Clear();
    }

    public async Task InitializeFormAsync()
    {
        ClearFields();

        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            Bans = await context.Ban.AsNoTracking().OrderBy(b => b.TenBan).ToListAsync();
            NhanViens = await context.NhanVien.AsNoTracking().OrderBy(n => n.TenNhanVien).ToListAsync();
            TrangThais = await context.TrangThai.AsNoTracking().OrderBy(t => t.TenTrangThai).ToListAsync();
            ActiveMonAns = await context.MonAn
                .Include(m => m.DonViTinh)
                .AsNoTracking()
                .Where(m => m.MaTinhTrang == "DangBan")
                .OrderBy(m => m.TenMonAn)
                .ToListAsync();
        }

        if (NhanViens.Count > 0) SelectedMaNhanVien = NhanViens[0].MaNhanVien;
        if (TrangThais.Count > 0) SelectedMaTrangThai = TrangThais[0].MaTrangThai;

        await GenerateNextMaPhieuGoiMonAsync();

        OnPageChanged();
    }

    private async Task GenerateNextMaPhieuGoiMonAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        int? maxId = await context.PhieuGoiMon
            .Select(p => (int?)p.MaPhieuGoiMon)
            .MaxAsync();
        MaPhieuGoiMon = (maxId ?? 0) + 1;
    }

    private void RecalculateTotal()
    {
        long total = 0;
        foreach (var row in OrderDetails)
        {
            int qty = row.GetSoLuongParsed();
            total += qty * row.DonGia;
        }
        TongTienTamTinhText = $"{total:N0} VND";
    }

    protected override void OnPageChanged()
    {
        TotalItems = OrderDetails.Count;
        UpdatePaginationInfo();

        var pageElements = OrderDetails.GetPage(PageNumber, PageSize).ToList();
        PagedItems = new ObservableCollection<CTGoiMonItemViewModel>(pageElements);
    }

    [RelayCommand]
    private void AddRow()
    {
        var rowVm = new CTGoiMonItemViewModel(ActiveMonAns)
        {
            STT = OrderDetails.Count + 1,
            SelectedMaMonAn = null,
            SoLuong = "1",
            GhiChu = "",
            DonGia = 0,
            DonGiaText = ""
        };
        rowVm.OnItemChanged += RecalculateTotal;
        OrderDetails.Add(rowVm);

        int targetPage = (int)System.Math.Ceiling((double)OrderDetails.Count / PageSize);
        if (PageNumber != targetPage)
        {
            PageNumber = targetPage;
        }
        else
        {
            OnPageChanged();
        }
        RecalculateTotal();
    }

    [RelayCommand]
    private void DeleteRow(CTGoiMonItemViewModel? row)
    {
        if (row != null)
        {
            row.OnItemChanged -= RecalculateTotal;
            OrderDetails.Remove(row);

            int stt = 1;
            foreach (var item in OrderDetails)
            {
                item.STT = stt++;
            }

            OnPageChanged();
            RecalculateTotal();
        }
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

        foreach (var row in OrderDetails)
        {
            row.ValidateRow();
            if (row.HasErrors)
            {
                MessageBox.Show("Vui lòng sửa các lỗi nhập liệu trong danh sách món ăn.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        var activeRows = OrderDetails.Where(row => row.SelectedMaMonAn.HasValue).ToList();
        if (activeRows.Count == 0)
        {
            MessageBox.Show("Phiếu gọi món phải chọn ít nhất một món ăn.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        long total = activeRows.Sum(row => row.GetSoLuongParsed() * row.DonGia);

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                using var context = await _dbContextFactory.CreateDbContextAsync();
                var newOrder = new PhieuGoiMon
                {
                    MaPhieuGoiMon = MaPhieuGoiMon,
                    MaBan = SelectedMaBan!.Value,
                    MaNhanVien = SelectedMaNhanVien!.Value,
                    MaTrangThai = SelectedMaTrangThai,
                    ThoiGianGoi = ThoiGianGoi,
                    TongTienTamTinh = total
                };

                foreach (var row in activeRows)
                {
                    newOrder.CTGoiMons.Add(new CTGoiMon
                    {
                        MaPhieuGoiMon = MaPhieuGoiMon,
                        MaMonAn = row.SelectedMaMonAn!.Value,
                        SoLuong = row.GetSoLuongParsed(),
                        GhiChu = row.GhiChu,
                        DonGia = row.DonGia
                    });
                }

                await context.PhieuGoiMon.AddAsync(newOrder);
                await context.SaveChangesAsync();

                MessageBox.Show("Tiếp nhận phiếu gọi món thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
                return;
            }
            catch (DbUpdateException ex) when (ex.IsPrimaryKeyViolation() && attempt < maxRetries - 1)
            {
                await GenerateNextMaPhieuGoiMonAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        MessageBox.Show("Không thể lưu phiếu gọi món do xung đột mã liên tục. Vui lòng thử lại.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
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
