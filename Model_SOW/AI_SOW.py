import cv2
import numpy as np
import mediapipe as mp
import time
import tensorflow as tf
from tensorflow.keras.models import load_model
import keyboard  # Thư viện xử lý phím mượt hơn cv2

# --- CẤU HÌNH ---
MODEL_PATH = "model/sigil_model.keras"
LABELS = ['Fireball','Neutral', 'Thunderbolt', 'WindBlade']
THRESHOLD = 0.5 
MAX_FRAMES = 100 

# 1. LOAD MODEL & TỐI ƯU HÓA (WARM-UP)
print("🧠 Đang tải bộ não AI...")
try:
    model = load_model(MODEL_PATH)
    
    # Định nghĩa hàm dự đoán nhanh (Graph Execution)
    @tf.function(input_signature=[tf.TensorSpec(shape=[1, 100, 63], dtype=tf.float32)])
    def predict_fn(x):
        return model(x, training=False)
    
    # --- WARM-UP (QUAN TRỌNG ĐỂ CHỐNG LAG) ---
    print("🔥 Đang làm nóng động cơ AI (Warm-up)...")
    dummy_input = tf.zeros([1, 100, 63], dtype=tf.float32)
    predict_fn(dummy_input) # Chạy thử 1 lần để TensorFlow compile graph
    print("✅ Hệ thống đã sẵn sàng 100%!")
    
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
final_result = "Ready"
confidence_score = 0.0
result_color = (255, 255, 255)

cap = cv2.VideoCapture(0)

print("\n--- HƯỚNG DẪN ---")
print("👉 GIỮ phím SPACE để Niệm (Thu dữ liệu).")
print("👉 THẢ phím SPACE để Bắn (Dự đoán).")
print("------------------\n")

# Bắt đầu luồng MediaPipe
with HandLandmarker.create_from_options(options) as landmarker:
    while cap.isOpened():
        ret, frame = cap.read()
        if not ret: break
        
        # Xử lý hình ảnh
        frame = cv2.flip(frame, 1)
        h, w, _ = frame.shape
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)
        
        # Gửi sang MediaPipe (Async - Không chặn luồng chính)
        landmarker.detect_async(mp_image, int(time.time() * 1000))

        # --- LOGIC GIỮ SPACE (Non-blocking) ---
        # Kiểm tra trạng thái phím Space an toàn
        try:
            space_pressed = keyboard.is_pressed('space')
        except:
            space_pressed = False

        # 1. BẮT ĐẦU NIỆM (Vừa ấn Space)
        if space_pressed and not is_recording:
            is_recording = True
            sequence = []
            final_result = "Recording..."
            confidence_score = 0.0
            print("🟢 Bắt đầu niệm...")

        # 2. KẾT THÚC & DỰ ĐOÁN (Vừa thả Space)
        elif not space_pressed and is_recording:
            is_recording = False
            print(f"🔴 Đang tính toán... (Frames: {len(sequence)})")
            
            if len(sequence) > 5: 
                # Xử lý dữ liệu (Cực nhanh nhờ Numpy)
                input_seq = np.array(sequence)
                if len(input_seq) > MAX_FRAMES:
                    input_seq = input_seq[:MAX_FRAMES]
                else:
                    padding = np.zeros((MAX_FRAMES - len(input_seq), 63))
                    input_seq = np.vstack((input_seq, padding))
                
                # Tạo Tensor & Scale
                input_tensor = tf.convert_to_tensor([input_seq], dtype=tf.float32)
                
                # DỰ ĐOÁN (Nhanh tức thì do đã Warm-up)
                res = predict_fn(input_tensor).numpy()[0]
                idx = np.argmax(res)
                conf = res[idx]
                
                # In kết quả ra console để debug
                debug_str = " | ".join([f"{LABELS[i]}: {res[i]:.0%}" for i in range(len(LABELS))])
                print(f"   📊 {debug_str}")

                if conf > THRESHOLD:
                    final_result = f"{LABELS[idx]}"
                    confidence_score = conf
                    
                    # Đổi màu chữ
                    if "Fire" in final_result: result_color = (0, 0, 255)       # Đỏ
                    elif "Thunder" in final_result: result_color = (0, 255, 255)# Vàng
                    elif "Wind" in final_result: result_color = (0, 255, 0)     # Xanh lá
                    else: result_color = (200, 200, 200)
                else:
                    final_result = "Unknown"
                    result_color = (128, 128, 128)
            else:
                final_result = "Too short!"
                result_color = (0, 0, 255)

        # --- THU THẬP DỮ LIỆU ---
        if is_recording:
            if latest_hand_landmarks:
                for hand_landmarks in latest_hand_landmarks:
                    # Logic chuẩn hóa
                    wrist = hand_landmarks[0]
                    palm_size = np.sqrt((hand_landmarks[5].x - wrist.x)**2 + (hand_landmarks[5].y - wrist.y)**2)
                    if palm_size == 0: palm_size = 0.01

                    frame_data = []
                    for lm in hand_landmarks:
                        frame_data.extend([
                            (lm.x - wrist.x)/palm_size, 
                            (lm.y - wrist.y)/palm_size, 
                            (lm.z - wrist.z)/palm_size
                        ])
                    sequence.append(frame_data)

        # --- VẼ GIAO DIỆN ---
        # Vẽ khớp tay
        if latest_hand_landmarks:
            for hand_landmarks in latest_hand_landmarks:
                for lm in hand_landmarks:
                    cx, cy = int(lm.x * w), int(lm.y * h)
                    cv2.circle(frame, (cx, cy), 5, (0, 255, 0), -1)

        # UI Trạng thái
        if is_recording:
            # Hiệu ứng đang ghi (Viền đỏ + Chấm đỏ)
            cv2.rectangle(frame, (0, 0), (w, h), (0, 0, 255), 4)
            cv2.circle(frame, (40, 40), 15, (0, 0, 255), -1) 
            cv2.putText(frame, f"REC ({len(sequence)})", (70, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
            cv2.putText(frame, "HOLD SPACE TO CAST", (w//2 - 150, h - 30), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 0, 255), 2)
        else:
            # Hiển thị kết quả to ở giữa màn hình
            cv2.putText(frame, final_result, (50, h//2), cv2.FONT_HERSHEY_SIMPLEX, 2, result_color, 4)
            if confidence_score > 0:
                cv2.putText(frame, f"Conf: {confidence_score:.0%}", (50, h//2 + 60), cv2.FONT_HERSHEY_SIMPLEX, 0.8, result_color, 1)
            
            cv2.putText(frame, "Hold SPACE to start", (10, h - 20), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 255, 255), 2)

        cv2.imshow('Sigil of Will - Spacebar Mode', frame)
        if cv2.waitKey(1) & 0xFF == 27: break # ESC để thoát

cap.release()
cv2.destroyAllWindows()