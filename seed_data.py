import os
import glob
import sqlite3
import subprocess
import shutil

def run_db_migration():
    print("Running database migration to generate SQLite DB...")
    # Run dotnet ef database update to apply migrations and build schemas
    subprocess.run(["dotnet", "ef", "database", "update"], check=True)

def seed_database(db_path):
    print(f"Seeding database at: {db_path}")
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    # 1. Clear existing records to avoid duplication
    cursor.execute("DELETE FROM CTGOIMON;")
    cursor.execute("DELETE FROM PHIEUGOIMON;")
    cursor.execute("DELETE FROM HOADON;")
    cursor.execute("DELETE FROM NHANVIEN;")
    cursor.execute("DELETE FROM BAN;")
    cursor.execute("DELETE FROM MONAN;")
    
    # 2. Seed 3 NHANVIEN
    employees = [
        (1, "Mai P."),
        (2, "Hoàng T."),
        (3, "Linh N.")
    ]
    cursor.executemany("INSERT INTO NHANVIEN (MaNhanVien, TenNhanVien) VALUES (?, ?);", employees)
    print("Seeded 3 employees.")
    
    # 3. Seed BAN (nested loop over areas and LOAIBAN)
    cursor.execute("SELECT MaLoaiBan, TenLoaiBan FROM LOAIBAN;")
    loai_bans = cursor.fetchall()
    
    areas = ["Khu A", "Khu B", "Khu C"]
    table_counter = 1
    tables = []
    
    for area in areas:
        for loai_ban in loai_bans:
            ma_loai_ban, ten_loai_ban = loai_ban
            ma_ban = table_counter
            ten_ban = f"Bàn {table_counter} ({ten_loai_ban})"
            tables.append((ma_ban, ten_ban, area, 4, ma_loai_ban))
            table_counter += 1
            
    cursor.executemany("INSERT INTO BAN (MaBan, TenBan, KhuVuc, SoChoNgoi, MaLoaiBan) VALUES (?, ?, ?, ?, ?);", tables)
    print(f"Seeded {len(tables)} tables.")
    
    # 4. Seed MONAN (nested loop over QD_LOAIMON_DVT and TINHTRANG)
    cursor.execute("SELECT MaLoaiMonAn, MaDonViTinh FROM QD_LOAIMON_DVT;")
    valid_combos = cursor.fetchall()
    
    cursor.execute("SELECT MaTinhTrang, TenTinhTrang FROM TINHTRANG;")
    statuses = cursor.fetchall()
    
    dish_counter = 1
    dishes = []
    
    for combo in valid_combos:
        ma_loai_mon, ma_dvt = combo
        for status in statuses:
            ma_status, ten_status = status
            ma_mon = dish_counter
            
            # Map Category code to nice Vietnamese display name
            cat_display = {"KhaiVi": "Khai vị", "Chinh": "Món chính", "TrangMieng": "Tráng miệng", "DoUong": "Nước uống"}.get(ma_loai_mon, ma_loai_mon)
            unit_display = {"Dia": "Đĩa", "Phan": "Phần", "Chai": "Chai", "Ly": "Ly"}.get(ma_dvt, ma_dvt)
            
            ten_mon = f"{cat_display} {unit_display} {dish_counter}"
            # Arbitrary price starting from 20000, incrementing by 5000
            price = 20000 + (dish_counter * 5000)
            
            dishes.append((ma_mon, ten_mon, price, ma_status, ma_loai_mon, ma_dvt))
            dish_counter += 1
            
    cursor.executemany("INSERT INTO MONAN (MaMonAn, TenMonAn, DonGia, MaTinhTrang, MaLoaiMonAn, MaDonViTinh) VALUES (?, ?, ?, ?, ?, ?);", dishes)
    print(f"Seeded {len(dishes)} dishes.")

    # 5. Seed HOADON
    invoices = [
        (1, "2026-06-29 16:00:00", 320000, 0)
    ]
    cursor.executemany("INSERT INTO HOADON (MaHoaDon, ThoiGianThanhToan, TongTien, PhuThu) VALUES (?, ?, ?, ?);", invoices)
    print("Seeded 1 historical invoice.")

    # 6. Seed PHIEUGOIMON
    orders = [
        # Table 1 (Bàn 1 (Thường)): 3 unpaid slips, 2 eligible (DangCheBien, DaPhucVu), 1 ineligible (ChoBep)
        (1, 1, None, 1, "DangCheBien", "2026-06-29 12:00:00", 100000),
        (2, 1, None, 1, "DaPhucVu", "2026-06-29 12:30:00", 150000),
        (3, 1, None, 1, "ChoBep", "2026-06-29 13:00:00", 50000),

        # Table 2 (Bàn 2 (VIP)): 1 unpaid slip, eligible (DaPhucVu)
        (4, 2, None, 2, "DaPhucVu", "2026-06-29 14:00:00", 80000),

        # Table 3 (Bàn 3 (VVIP)): 4 paid slips, linked to MaHoaDon = 1
        (5, 3, 1, 3, "DaPhucVu", "2026-06-29 10:00:00", 80000),
        (6, 3, 1, 3, "DaPhucVu", "2026-06-29 10:30:00", 80000),
        (7, 3, 1, 3, "DaPhucVu", "2026-06-29 11:00:00", 80000),
        (8, 3, 1, 3, "DaPhucVu", "2026-06-29 11:15:00", 80000)
    ]
    cursor.executemany("INSERT INTO PHIEUGOIMON (MaPhieuGoiMon, MaBan, MaHoaDon, MaNhanVien, MaTrangThai, ThoiGianGoi, TongTienTamTinh) VALUES (?, ?, ?, ?, ?, ?, ?);", orders)
    print("Seeded 8 order slips.")

    # 7. Seed CTGOIMON
    details = [
        # Slip 1
        (1, 1, 2, "", 50000),
        # Slip 2
        (2, 2, 3, "", 50000),
        # Slip 3
        (3, 3, 1, "", 50000),
        # Slip 4
        (4, 4, 2, "", 40000),
        # Slip 5
        (5, 1, 2, "", 40000),
        # Slip 6
        (6, 2, 2, "", 40000),
        # Slip 7
        (7, 3, 2, "", 40000),
        # Slip 8
        (8, 4, 2, "", 40000)
    ]
    cursor.executemany("INSERT INTO CTGOIMON (MaPhieuGoiMon, MaMonAn, SoLuong, GhiChu, DonGia) VALUES (?, ?, ?, ?, ?);", details)
    print("Seeded 8 order detail items.")

    conn.commit()
    conn.close()

def main():
    # 1. Run migrations to create database in project root
    run_db_migration()
    
    root_db = "QuanLyNhaHang.db"
    if not os.path.exists(root_db):
        print("Root DB file not found after migration update!")
        return
        
    # 2. Seed the root database
    seed_database(root_db)
    
    # 3. Locate build target output directories and copy the preseeded DB
    build_dirs = glob.glob(os.path.join("bin", "Debug", "*"))
    for build_dir in build_dirs:
        if os.path.isdir(build_dir):
            dest_db = os.path.join(build_dir, "QuanLyNhaHang.db")
            print(f"Copying preseeded DB to build folder: {dest_db}")
            shutil.copy2(root_db, dest_db)
            
            # Clean up target shm/wal files to avoid conflict
            for shm_wal in ["QuanLyNhaHang.db-shm", "QuanLyNhaHang.db-wal"]:
                dest_lock = os.path.join(build_dir, shm_wal)
                if os.path.exists(dest_lock):
                    os.remove(dest_lock)

if __name__ == "__main__":
    main()
