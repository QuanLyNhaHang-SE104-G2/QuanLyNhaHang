using System;
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

public partial class CapNhatPhieuGoiMonViewModel : InMemoryPaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private int _initialMaPhieuGoiMon;

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
    private DateTime _thoiGianGoi = DateTime.Now;

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

    public CapNhatPhieuGoiMonViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    partial void OnMaPhieuGoiMonChanged(int value)
    {
        _ = LoadOrderAsync(value);
    }

    public async Task LoadOrderAsync(int maPhieuGoiMon)
    {
        _initialMaPhieuGoiMon = maPhieuGoiMon;

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                Bans = await context.Ban.AsNoTracking().OrderBy(b => b.TenBan).ToListAsync();
                NhanViens = await context.NhanVien.AsNoTracking().OrderBy(n => n.TenNhanVien).ToListAsync();
                TrangThais = await context.TrangThai.AsNoTracking().OrderBy(t => t.TenTrangThai).ToListAsync();

                var order = await context.PhieuGoiMon.AsNoTracking().FirstOrDefaultAsync(p => p.MaPhieuGoiMon == maPhieuGoiMon);
                if (order != null)
                {
                    SelectedMaBan = order.MaBan;
                    SelectedMaNhanVien = order.MaNhanVien;
                    SelectedMaTrangThai = order.MaTrangThai;
                    ThoiGianGoi = order.ThoiGianGoi;

                    var orderedDishIds = await context.CTGoiMon
                        .Where(c => c.MaPhieuGoiMon == maPhieuGoiMon)
                        .Select(c => c.MaMonAn)
                        .ToListAsync();

                    ActiveMonAns = await context.MonAn
                        .Include(m => m.DonViTinh)
                        .AsNoTracking()
                        .Where(m => m.MaTinhTrang == "DangBan" || orderedDishIds.Contains(m.MaMonAn))
                        .OrderBy(m => m.TenMonAn)
                        .ToListAsync();

                    var details = await context.CTGoiMon
                        .AsNoTracking()
                        .Where(c => c.MaPhieuGoiMon == maPhieuGoiMon)
                        .ToListAsync();

                    // Temporarily unhook event during loading to avoid excessive recalculations
                    OrderDetails.Clear();
                    int stt = 1;
                    foreach (var c in details)
                    {
                        var rowVm = new CTGoiMonItemViewModel(ActiveMonAns)
                        {
                            STT = stt++,
                            SelectedMaMonAn = c.MaMonAn,
                            SoLuong = c.SoLuong.ToString(),
                            GhiChu = c.GhiChu ?? "",
                            DonGia = c.DonGia,
                            DonGiaText = $"{c.DonGia:N0} VND"
                        };
                        rowVm.OnItemChanged += RecalculateTotal;
                        OrderDetails.Add(rowVm);
                    }
                }
            }

            RecalculateTotal();
            OnPageChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải thông tin phiếu gọi món: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
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

        int targetPage = (int)Math.Ceiling((double)OrderDetails.Count / PageSize);
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
        await LoadOrderAsync(_initialMaPhieuGoiMon);
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

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var existingOrder = await context.PhieuGoiMon.FirstOrDefaultAsync(p => p.MaPhieuGoiMon == MaPhieuGoiMon);
                if (existingOrder == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu gọi món cần cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (existingOrder.MaHoaDon.HasValue)
                {
                    MessageBox.Show("Không thể cập nhật phiếu gọi món này vì phiếu đã được xuất hóa đơn thanh toán.", "Lỗi cập nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                existingOrder.MaBan = SelectedMaBan!.Value;
                existingOrder.MaNhanVien = SelectedMaNhanVien!.Value;
                existingOrder.MaTrangThai = SelectedMaTrangThai;
                existingOrder.ThoiGianGoi = ThoiGianGoi;
                existingOrder.TongTienTamTinh = total;

                // Remove existing order details
                var oldDetails = context.CTGoiMon.Where(c => c.MaPhieuGoiMon == MaPhieuGoiMon);
                context.CTGoiMon.RemoveRange(oldDetails);

                // Add new order details
                foreach (var row in activeRows)
                {
                    context.CTGoiMon.Add(new CTGoiMon
                    {
                        MaPhieuGoiMon = MaPhieuGoiMon,
                        MaMonAn = row.SelectedMaMonAn!.Value,
                        SoLuong = row.GetSoLuongParsed(),
                        GhiChu = row.GhiChu,
                        DonGia = row.DonGia
                    });
                }

                await context.SaveChangesAsync();
            }

            MessageBox.Show("Cập nhật phiếu gọi món thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
