using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("TRANGTHAI")]
public class TrangThai
{
    [Key]
    [Column("MaTrangThai")]
    public string MaTrangThai { get; set; } = null!;

    [Column("TenTrangThai")]
    public string TenTrangThai { get; set; } = null!;

    [Column("DuocThanhToan")]
    public bool DuocThanhToan { get; set; }
}
