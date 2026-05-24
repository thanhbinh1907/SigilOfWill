import numpy as np
import mediapipe as mp
import time
import tensorflow as tf
from tensorflow.keras.models import load_model
import keyboard
import socket
import cv2  # Vẫn cần cv2 để đọc camera nhưng không dùng để hiển thị UI

# --- CẤU HÌNH ---
MODEL_PATH = "model_camera/sigil_model.keras"
LABELS = ['Neutral', 'Fireball',  'Thunderbolt', 'WindBlade']
THRESHOLD = 0.5 
MAX_FRAMES = 100 

# --- CẤU HÌNH eUDP SENDER ---
UDP_IP = "127.0.0.1" # Gửi nội bộ (Localhost)
UDP_PORT = 11000
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# 1. LOAD MODEL & TỐI ƯU HÓA (WARM-UP)
print("🧠 Đang tải bộ não AI...")
try:
    model = load_model(MODEL_PATH)
    
    @tf.function(input_signature=[tf.TensorSpec(shape=[1, 100, 63], dtype=tf.float32)])
    def predict_fn(x):
        return model(x, training=False)
    
    print("🔥 Đang làm nóng AI (Warm-up)...")
    dummy_input = tf.zeros([1, 100, 63], dtype=tf.float32)
    predict_fn(dummy_input) 
    print("✅ AI Sẵn sàng gửi dữ liệu tới Unity!")
    
except Exception as e:
    print(f"❌ LỖI LOAD MODEL: {e}")
    exit()

# 2. Cấu hình MediaPipe
BaseOptions = mp.tasks.BaseOptions
HandLandmarker = mp.tasks.vision.HandLandmarker
HandLandmarkerOptions = mp.tasks.vision.HandLandmarkerOptions
VisionRunningMode = mp.tasks.vision.RunningMode

latest_hand_landmarks = None
def result_callback(result, output_image, timestamp_ms):
    global latest_hand_landmarks
    latest_hand_landmarks = result.hand_landmarks

options = HandLandmarkerOptions(
    base_options=BaseOptions(model_asset_path='hand_landmarker.task'),
    running_mode=VisionRunningMode.LIVE_STREAM,
    num_hands=1,
    result_callback=result_callback
)

# Biến trạng thái
is_recording = False
sequence = []

cap = cv2.VideoCapture(0)

print("\n--- HỆ THỐNG CHẠY NGẦM ---")
print("👉 Đè 'E' trong game (hoặc ở cửa sổ này) để niệm chú.")
print("👉 Nhấn Ctrl+C ở terminal này để thoát.")
print("--------------------------\n")

# Bắt đầu luồng
with HandLandmarker.create_from_options(options) as landmarker:
    try:
        while cap.isOpened():
            ret, frame = cap.read()
            if not ret: break
            
            # Xử lý hình ảnh gửi cho MediaPipe (Không hiển thị)
            frame = cv2.flip(frame, 1)
            rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)
            
            # Detect Async
            landmarker.detect_async(mp_image, int(time.time() * 1000))

            # Kiểm tra phím E
            try:
                e_pressed = keyboard.is_pressed('e')
            except:
                e_pressed = False

            # 1. BẮT ĐẦU NIỆM
            if e_pressed and not is_recording:
                is_recording = True
                sequence = []
                print("🟢 Đang đọc cử chỉ...")

            # 2. KẾT THÚC & DỰ ĐOÁN
            elif not e_pressed and is_recording:
                is_recording = False
                
                if len(sequence) > 5: 
                    # Xử lý dữ liệu Numpy
                    input_seq = np.array(sequence)
                    if len(input_seq) > MAX_FRAMES:
                        input_seq = input_seq[:MAX_FRAMES]
                    else:
                        padding = np.zeros((MAX_FRAMES - len(input_seq), 63))
                        input_seq = np.vstack((input_seq, padding))
                    input_tensor = tf.convert_to_tensor([input_seq], dtype=tf.float32)
                    
                    # Dự đoán
                    res = predict_fn(input_tensor).numpy()[0]
                    idx = np.argmax(res)
                    conf = res[idx]
                    
                    if conf > THRESHOLD:
                        spell_name = LABELS[idx]
                        print(f"⚡ Tung chiêu: {spell_name} (Độ tin cậy: {conf:.0%}) -> Gửi UDP: {idx}")
                        
                        # ---> GỬI ID TỚI UNITY <---
                        sock.sendto(str(idx).encode(), (UDP_IP, UDP_PORT))
                    else:
                        print(f"⚠️ Cử chỉ không rõ ràng (Độ tin cậy: {conf:.0%})")
                else:
                    print("⚠️ Thời gian giữ quá ngắn!")

            # 3. THU THẬP DỮ LIỆU
            if is_recording and latest_hand_landmarks:
                for hand_landmarks in latest_hand_landmarks:
                    wrist = hand_landmarks[0]
                    # Chống chia cho 0
                    dx = hand_landmarks[5].x - wrist.x
                    dy = hand_landmarks[5].y - wrist.y
                    palm_size = np.sqrt(dx**2 + dy**2)
                    if palm_size < 0.001: palm_size = 0.001

                    frame_data = []
                    for lm in hand_landmarks:
                        frame_data.extend([
                            (lm.x - wrist.x)/palm_size, 
                            (lm.y - wrist.y)/palm_size, 
                            (lm.z - wrist.z)/palm_size
                        ])
                    sequence.append(frame_data)
                    
            # Giới hạn FPS nhỏ để không ăn 100% CPU vòng lặp while (MediaPipe chạy Async)
            time.sleep(0.01) 

    except KeyboardInterrupt:
        print("\n🛑 Đã tắt AI Sender.")

# Dọn dẹp
cap.release()