import csv
from collections import Counter

DATA_PATH = "gesture_data.csv"

def count_data():
    try:
        with open(DATA_PATH, mode='r', newline='') as f:
            reader = csv.reader(f)
            header = next(reader, None) # Bỏ qua dòng tiêu đề
            
            # Dùng set để lưu các bộ (label, sequence_id) duy nhất
            # Vì 1 hành động có nhiều frame, ta chỉ đếm số hành động (sequence)
            unique_sequences = set()
            
            for row in reader:
                if row:
                    label = row[0]
                    seq_id = row[1]
                    unique_sequences.add((label, seq_id))
            
            # Đếm số lượng mỗi label
            counts = Counter(label for label, _ in unique_sequences)
            
            print("\n📊 THỐNG KÊ DỮ LIỆU:")
            print("-" * 30)
            if not counts:
                print("⚠️ File trống hoặc chưa có dữ liệu.")
            else:
                total = 0
                for label, count in counts.items():
                    print(f"   - {label}: {count} mẫu")
                    total += count
                print("-" * 30)
                print(f"⚡ TỔNG CỘNG: {total} mẫu")
                
    except FileNotFoundError:
        print(f"❌ Không tìm thấy file '{DATA_PATH}'. Hãy chắc chắn bạn đã chạy generateData.py trước.")

if __name__ == "__main__":
    count_data()