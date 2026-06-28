using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("PHIEUGOIMON")]
public class PhieuGoiMon
{
    [Key]
    [Column("MaPhieuGoiMon")]
    public string MaPhieuGoiMon { get; set; } = null!;

    [Column("ThoiGianGoi")]
    public DateTime ThoiGianGoi { get; set; }

    [Column("TongTienTamTinh")]
    public decimal TongTienTamTinh { get; set; }

    [Column("MaTrangThai")]
    public string MaTrangThai { get; set; } = null!;

    [ForeignKey(nameof(MaTrangThai))]
    public virtual TrangThai TrangThai { get; set; } = null!;

    [Column("MaNhanVien")]
    public string MaNhanVien { get; set; } = null!;

    [ForeignKey(nameof(MaNhanVien))]
    public virtual NhanVien NhanVien { get; set; } = null!;

    [Column("MaBan")]
    public string MaBan { get; set; } = null!;

    [ForeignKey(nameof(MaBan))]
    public virtual Ban Ban { get; set; } = null!;

    public virtual ICollection<CTGoiMon> CTGoiMons { get; set; } = new List<CTGoiMon>();
}
