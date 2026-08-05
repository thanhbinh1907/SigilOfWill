import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os
import sys

# Đảm bảo in các ký tự Unicode tiếng Việt và emoji không bị lỗi trên console Windows
if hasattr(sys.stdout, 'reconfigure'):
    sys.stdout.reconfigure(encoding='utf-8')
if hasattr(sys.stderr, 'reconfigure'):
    sys.stderr.reconfigure(encoding='utf-8')

FILE_PATH = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "gesture_data.csv"))

def analyze_gestures():
    try:
        # 1. Đọc dữ liệu
        df = pd.read_csv(FILE_PATH)
        print(f"📂 Đã đọc file '{FILE_PATH}' với {len(df)} dòng dữ liệu.\n")

        # 2. Tính số frame cho mỗi lần múa (Sequence)
        # Gom nhóm theo Label và ID để đếm số dòng (mỗi dòng là 1 frame)
        seq_counts = df.groupby(['label', 'sequence_id']).size().reset_index(name='frame_count')

        # 3. Tính thống kê chi tiết cho từng chiêu
        stats = seq_counts.groupby('label')['frame_count'].agg(
            Total_Samples='count',  # Tổng số mẫu
            Mean_Frames='mean',     # Trung bình
            Min_Frames='min',       # Ngắn nhất
            Max_Frames='max',       # Dài nhất
            Std_Dev='std'           # Độ lệch chuẩn (Độ ổn định)
        ).round(2) # Làm tròn 2 số thập phân

        print("📊 BẢNG THỐNG KÊ CHI TIẾT:")
        print("=" * 80)
        # Định dạng in cho đẹp
        print(stats.to_string())
        print("=" * 80)
        
        # 4. Đánh giá độ ổn định
        print("\n🧐 NHẬN XÉT SƠ BỘ:")
        for label, row in stats.iterrows():
            avg = row['Mean_Frames']
            std = row['Std_Dev']
            stability = "Rất Ổn định" if std < 5 else "Hơi thất thường" if std < 15 else "Rất lộn xộn"
            
            print(f"   - {label}: Trung bình {avg} frames (~{avg/30:.1f}s). Độ ổn định: {stability} (Lệch {std} frames)")

        # 5. Vẽ biểu đồ phân bố (Histogram)
        plt.figure(figsize=(12, 6))
        sns.histplot(data=seq_counts, x='frame_count', hue='label', kde=True, element="step")
        plt.title('Phân bố độ dài các chiêu thức (Số Frames)')
        plt.xlabel('Số lượng Frame')
        plt.ylabel('Số lượng Mẫu')
        plt.axvline(x=100, color='r', linestyle='--', label='MAX_FRAMES (100)')
        plt.legend()
        plt.grid(True, alpha=0.3)
        plt.show()

    except FileNotFoundError:
        print(f"❌ Không tìm thấy file '{FILE_PATH}'. Hãy kiểm tra lại tên file.")
    except Exception as e:
        print(f"❌ Có lỗi xảy ra: {e}")

if __name__ == "__main__":
    analyze_gestures()