using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("PHIEUGOIMON")]
public class PhieuGoiMon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("MaPhieuGoiMon")]
    public int MaPhieuGoiMon { get; set; }

    [Column("ThoiGianGoi")]
    public DateTime ThoiGianGoi { get; set; }

    [Column("TongTienTamTinh")]
    public long TongTienTamTinh { get; set; }

    [Column("MaTrangThai")]
    public string MaTrangThai { get; set; } = null!;

    [ForeignKey(nameof(MaTrangThai))]
    public virtual TrangThai TrangThai { get; set; } = null!;

    [Column("MaNhanVien")]
    public int MaNhanVien { get; set; }

    [ForeignKey(nameof(MaNhanVien))]
    public virtual NhanVien NhanVien { get; set; } = null!;

    [Column("MaBan")]
    public int MaBan { get; set; }

    [ForeignKey(nameof(MaBan))]
    public virtual Ban Ban { get; set; } = null!;

    public virtual ICollection<CTGoiMon> CTGoiMons { get; set; } = new List<CTGoiMon>();
}
