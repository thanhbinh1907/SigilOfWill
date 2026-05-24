import cv2
import mediapipe as mp
import time
import csv
import os
import numpy as np
import sys
import os

def resource_path(relative_path):
    """ Lấy path đúng khi chạy exe hoặc chạy python """
    try:
        base_path = sys._MEIPASS  # PyInstaller temp folder
    except Exception:
        base_path = os.path.abspath(".")
    return os.path.join(base_path, relative_path)

# --- CẤU HÌNH ---
LABEL = "Neutral"  # Tên chiêu thức hiện tại
DATA_PATH = "gesture_data.csv"
RECORDING = False 
latest_hand_landmarks = None # Biến để truyền dữ liệu từ callback ra màn hình

# Khởi tạo MediaPipe Tasks
BaseOptions = mp.tasks.BaseOptions
HandLandmarker = mp.tasks.vision.HandLandmarker
HandLandmarkerOptions = mp.tasks.vision.HandLandmarkerOptions
VisionRunningMode = mp.tasks.vision.RunningMode

# --- TỰ ĐỘNG LẤY SEQUENCE_ID TIẾP THEO ---
def get_next_sequence_id(file_path):
    if not os.path.exists(file_path):
        return 1
    try:
        with open(file_path, mode='r') as f:
            lines = list(csv.reader(f))
            if len(lines) <= 1: return 1
            return int(lines[-1][1]) + 1
    except:
        return 1

current_sequence_id = get_next_sequence_id(DATA_PATH)

def normalize_landmarks(landmarks):
    """Chuẩn hóa 21 tọa độ khớp tay"""
    wrist = landmarks[0]
    palm_size = np.sqrt((landmarks[5].x - wrist.x)**2 + (landmarks[5].y - wrist.y)**2)
    if palm_size == 0: palm_size = 0.01 
    
    normalized = []
    for lm in landmarks:
        normalized.extend([
            (lm.x - wrist.x) / palm_size,
            (lm.y - wrist.y) / palm_size,
            (lm.z - wrist.z) / palm_size
        ])
    return normalized

# Tạo file CSV với cột sequence_id
if not os.path.exists(DATA_PATH):
    with open(DATA_PATH, mode='w', newline='') as f:
        header = ['label', 'sequence_id'] + [f'{axis}{i}' for i in range(21) for axis in ['x', 'y', 'z']]
        csv.writer(f).writerow(header)

def result_callback(result, output_image, timestamp_ms):
    global RECORDING, latest_hand_landmarks, current_sequence_id
    latest_hand_landmarks = result.hand_landmarks
    
    if RECORDING and result.hand_landmarks:
        for hand_landmarks in result.hand_landmarks:
            data = normalize_landmarks(hand_landmarks)
            with open(DATA_PATH, mode='a', newline='') as f:
                # Lưu kèm LABEL và ID của chuỗi hành động
                csv.writer(f).writerow([LABEL, current_sequence_id] + data)

# Cấu hình Landmarker
options = HandLandmarkerOptions(
    base_options=BaseOptions(
        model_asset_path=resource_path('hand_landmarker.task')
    ),
    running_mode=VisionRunningMode.LIVE_STREAM,
    num_hands=1,
    result_callback=result_callback
)

cap = cv2.VideoCapture(0)
with HandLandmarker.create_from_options(options) as landmarker:
    print(f"Sẵn sàng! Chiêu: {LABEL} | ID bắt đầu: {current_sequence_id}")

    while cap.isOpened():
        success, frame = cap.read()
        if not success: break
        
        frame = cv2.flip(frame, 1)
        h, w, _ = frame.shape
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
        
        # Gửi dữ liệu tới AI
        landmarker.detect_async(mp_image, int(time.time() * 1000))

        # --- VẼ CHẤM XANH TRACKING (Bằng OpenCV trực tiếp) ---
        if latest_hand_landmarks:
            for hand_landmarks in latest_hand_landmarks:
                for lm in hand_landmarks:
                    # Chuyển tọa độ tỉ lệ (0-1) sang pixel
                    cx, cy = int(lm.x * w), int(lm.y * h)
                    # Vẽ chấm xanh lá cây lên màn hình
                    cv2.circle(frame, (cx, cy), 5, (0, 255, 0), -1)

        # --- HIỂN THỊ UI ---
        status_txt = f"REC - SEQ: {current_sequence_id}" if RECORDING else f"READY - ID: {current_sequence_id}"
        color = (0, 0, 255) if RECORDING else (0, 255, 0)
        cv2.putText(frame, status_txt, (10, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, color, 2)

        cv2.imshow('Sigil of Will - Data Collector', frame)
        
        key = cv2.waitKey(1) & 0xFF
        if key == 27: # ESC
            break
        elif key == ord('r') or key == ord('R'): 
            if RECORDING:
                RECORDING = False
                current_sequence_id += 1 # Kết thúc 1 hành động, tăng ID cho lần sau
                print(f"Dừng ghi. Tiếp theo sẽ là ID: {current_sequence_id}")
            else:
                RECORDING = True
                print(f"Đang ghi Sequence ID: {current_sequence_id}...")

cap.release()
cv2.destroyAllWindows()