using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("HOADON")]
public class HoaDon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("MaHoaDon")]
    public int MaHoaDon { get; set; }

    [Column("ThoiGianThanhToan")]
    public DateTime ThoiGianThanhToan { get; set; }

    [Column("TongTien")]
    public long TongTien { get; set; }

    [Column("PhuThu")]
    public long PhuThu { get; set; }

    public virtual ICollection<PhieuGoiMon> PhieuGoiMons { get; set; } = new List<PhieuGoiMon>();
}
