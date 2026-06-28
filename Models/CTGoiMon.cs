using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("CTGOIMON")]
public class CTGoiMon
{
    [Column("MaPhieuGoiMon")]
    public int MaPhieuGoiMon { get; set; }

    [ForeignKey(nameof(MaPhieuGoiMon))]
    public virtual PhieuGoiMon PhieuGoiMon { get; set; } = null!;

    [Column("MaMonAn")]
    public int MaMonAn { get; set; }

    [ForeignKey(nameof(MaMonAn))]
    public virtual MonAn MonAn { get; set; } = null!;

    [Column("SoLuong")]
    public int SoLuong { get; set; }

    [Column("GhiChu")]
    public string? GhiChu { get; set; }

    [Column("DonGia")]
    public long DonGia { get; set; }
}
