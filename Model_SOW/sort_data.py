import pandas as pd
import os

FILE_PATH = 'gesture_data.csv'

def sort_csv():
    if not os.path.exists(FILE_PATH):
        print(f"❌ Không tìm thấy file {FILE_PATH}")
        return

    print("🔄 Đang đọc file...")
    # 1. Đọc dữ liệu
    df = pd.read_csv(FILE_PATH)
    
    # In ra trật tự cũ để so sánh
    print("📌 Các nhãn hiện có (chưa sắp xếp):", df['label'].unique())

    print("🔄 Đang sắp xếp...")
    # 2. Sắp xếp dữ liệu
    # Ưu tiên 1: Theo Tên chiêu (label) -> Gom Fireball về 1 chỗ, Windblade về 1 chỗ...
    # Ưu tiên 2: Theo ID chuỗi (sequence_id) -> Để các frame trong 1 lần múa nằm cạnh nhau
    df_sorted = df.sort_values(by=['label', 'sequence_id'])

    # 3. Lưu đè lại file cũ (bỏ cột index thừa)
    df_sorted.to_csv(FILE_PATH, index=False)

    print("✅ Đã sắp xếp xong!")
    print("📌 Thứ tự mới trong file:", df_sorted['label'].unique())
    print(f"💾 Dữ liệu đã được lưu lại vào: {FILE_PATH}")

if __name__ == "__main__":
    # Cài thư viện nếu chưa có: pip install pandas
    sort_csv()