using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("CTGOIMON")]
public class CTGoiMon
{
    [Column("MaPhieuGoiMon")]
    public string MaPhieuGoiMon { get; set; } = null!;

    [ForeignKey(nameof(MaPhieuGoiMon))]
    public virtual PhieuGoiMon PhieuGoiMon { get; set; } = null!;

    [Column("MaMonAn")]
    public string MaMonAn { get; set; } = null!;

    [ForeignKey(nameof(MaMonAn))]
    public virtual MonAn MonAn { get; set; } = null!;

    [Column("SoLuong")]
    public int SoLuong { get; set; }

    [Column("GhiChu")]
    public string? GhiChu { get; set; }

    [Column("DonGia")]
    public decimal DonGia { get; set; }
}
