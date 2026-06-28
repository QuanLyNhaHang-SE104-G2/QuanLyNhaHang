using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("NHANVIEN")]
public class NhanVien
{
    [Key]
    [Column("MaNhanVien")]
    public string MaNhanVien { get; set; } = null!;

    [Column("TenNhanVien")]
    public string TenNhanVien { get; set; } = null!;
}
