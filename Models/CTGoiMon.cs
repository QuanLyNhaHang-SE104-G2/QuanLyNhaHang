using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("CTGOIMON")]
public class CTGoiMon
{
    [Column("MaPhieuGoiMon")]
    public int MaPhieuGoiMon { get; set; }

    [Column("MaMonAn")]
    public int MaMonAn { get; set; }

    [Column("SoLuong")]
    public int SoLuong { get; set; }

    [Column("GhiChu")]
    public string? GhiChu { get; set; }

    [Column("DonGia")]
    public long DonGia { get; set; }

    [ForeignKey(nameof(MaPhieuGoiMon))]
    public virtual PhieuGoiMon PhieuGoiMon { get; set; } = null!;

    [ForeignKey(nameof(MaMonAn))]
    public virtual MonAn MonAn { get; set; } = null!;
}
