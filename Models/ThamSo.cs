using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("THAMSO")]
public class ThamSo
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("SoChoNgoiToiThieu")]
    public int SoChoNgoiToiThieu { get; set; }
}