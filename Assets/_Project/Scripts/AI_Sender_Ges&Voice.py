# pyrefly: ignore [missing-import]
import numpy as np
# pyrefly: ignore [missing-import]
import mediapipe as mp
import time
import tensorflow as tf
from tensorflow.keras.models import load_model
import keyboard
import socket
# pyrefly: ignore [missing-import]
import cv2  # Vẫn cần cv2 để đọc camera nhưng không dùng để hiển thị UI
import json
import queue
import sys
import threading
# pyrefly: ignore [missing-import]
import sounddevice as sd
# pyrefly: ignore [missing-import]
from vosk import Model, KaldiRecognizer

# --- CẤU HÌNH ---  
MODEL_PATH = "model_camera/sigil_model.keras"
LABELS = ['Neutral', 'Fireball',  'Thunderbolt', 'WindBlade']
THRESHOLD = 0.5 
MAX_FRAMES = 100 

# --- CẤU HÌNH UDP SENDER VÀ MẠNG ---
UDP_IP = "127.0.0.1" 
UDP_PORT = 11000
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# --- KHỞI TẠO HÀNG ĐỢI ÂM THANH & BIẾN BỘ ĐỆM THỜI GIAN ---
audio_queue = queue.Queue()
latest_voice_word = "none"
voice_timestamp = 0.0 

# 1. LOAD MODEL GESTURE (KERAS) & TỐI ƯU HÓA (WARM-UP)
print("🧠 Đang tải bộ não AI cử chỉ...")
try:
    model = load_model(MODEL_PATH)
    @tf.function(input_signature=[tf.TensorSpec(shape=[1, 100, 63], dtype=tf.float32)])
    def predict_fn(x):
        return model(x, training=False)
    
    print("🔥 Đang làm nóng AI cử chỉ (Warm-up)...")
    dummy_input = tf.zeros([1, 100, 63], dtype=tf.float32)
    predict_fn(dummy_input) 
    print("✅ AI Cử chỉ đã sẵn sàng!")
except Exception as e:
    print(f"❌ LỖI LOAD MODEL GESTURE: {e}")
    exit()

# 2. LOAD MODEL GIỌNG NÓI (VOSK)
print("🎤 Đang tải mô hình giọng nói Vosk...")
try:
    vosk_model = Model("model_voice")
    print("✅ Mô hình giọng nói Vosk đã sẵn sàng!")
except Exception as e:
    print(f"❌ LỖI LOAD MODEL VOSK: {e}")
    exit()

# 3. Cấu hình MediaPipe Bàn Tay
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

is_recording = False
voice_listening = False
should_reset_audio = False
sequence = []

# --- LUỒNG XỬ LÝ NHẬN DIỆN GIỌNG NÓI NGẦM LIÊN TỤC ---
def audio_callback(indata, frames, time_info, status):
    if status:
        print(status, file=sys.stderr)
    audio_queue.put(bytes(indata))

def voice_recognition_thread():
    global latest_voice_word, voice_timestamp
    
    print("\n======================= MICROPHONE LIST =======================")
    print(sd.query_devices())
    print("===============================================================\n")
    
    device_info = sd.query_devices(None, "input")
    samplerate = int(device_info["default_samplerate"])
    print(f"🎛️ [HỆ THỐNG] Đã cấu hình đồng bộ Mic theo tần số gốc: {samplerate}Hz")

    # ĐÃ MỞ RỘNG: Cho phép Vosk bắt được cả từ wind và blade tách rời
    keywords_json = '["fireball", "thunderbolt", "windblade", "wind", "blade", "[unk]"]'
    
    with sd.RawInputStream(samplerate=samplerate, blocksize=4000, device=None, dtype='int16', channels=1, callback=audio_callback):
        rec = KaldiRecognizer(vosk_model, samplerate, keywords_json)
        print("🎙️ [THÔNG BÁO] Luồng thu âm ngầm đã kích hoạt thành công.")
        
        last_printed_text = ""
        while True:
            data = audio_queue.get()
            
            global should_reset_audio, voice_listening
            if should_reset_audio:
                while not audio_queue.empty():
                    try:
                        audio_queue.get_nowait()
                    except queue.Empty:
                        break
                rec.Reset()
                should_reset_audio = False
            
            if voice_listening:
                if rec.AcceptWaveform(data):
                    result = json.loads(rec.Result())
                    text = result.get("text", "").lower()
                else:
                    partial = json.loads(rec.PartialResult())
                    text = partial.get("partial", "").lower()
                
                if text.strip() and text != last_printed_text:
                    print(f"🎵 [VOSK NGHE THẤY]: \"{text}\"")
                    last_printed_text = text
                
                # Khớp từ khóa logic thông minh
                if "fireball" in text:
                    latest_voice_word = "fireball"
                    voice_timestamp = time.time()
                elif "thunderbolt" in text:
                    latest_voice_word = "thunderbolt"
                    voice_timestamp = time.time()
                # BẪY TỪ KHÓA LINH HOẠT: Khắc phục triệt độ lỗi [unk] của chiêu WindBlade
                elif "windblade" in text or "wind" in text or "blade" in text:
                    latest_voice_word = "windblade"
                    voice_timestamp = time.time()
                
# Khởi động luồng xử lý giọng nói
threading.Thread(target=voice_recognition_thread, daemon=True).start()

# Khởi tạo Camera
cap = cv2.VideoCapture(0)

print("\n--- HỆ THỐNG PHỐI HỢP CỬ CHỈ & GIỌNG NÓI CHUẨN XÁC ---")
print("👉 Đè giữ 'E' trong game: Hệ thống ghi nhận cử chỉ của tay.")
print("👉 Nói thần chú bất cứ lúc nào trong khi giữ phím E.")
print("👉 Nhả 'E': AI tự động tính toán đồng bộ combo và bắn sang Unity.")
print("------------------------------------------------------------\n")

with HandLandmarker.create_from_options(options) as landmarker:
    try:
        while cap.isOpened():
            ret, frame = cap.read()
            if not ret: break
            
            frame = cv2.flip(frame, 1)
            rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)
            
            try:
                e_pressed = keyboard.is_pressed('e')
            except:
                e_pressed = False

            if e_pressed and not is_recording:
                is_recording = True
                voice_listening = True
                should_reset_audio = True
                latest_voice_word = "none"
                voice_timestamp = 0.0
                latest_hand_landmarks = None
                sequence = []
                print("🟢 [OPEN] Đang gồng giữ E để làm cử chỉ + đọc chú...")

            # SỬA LỖI 3: KHI NHẢ PHÍM E -> TRÌ HOÃN 0.35 GIÂY CHO BUFFER ÂM THANH CHẠY HẾT
            elif not e_pressed and is_recording:
                is_recording = False
                print("🔴 [CLOSE] Đã nhả phím E. Chờ 0.35 giây để luồng âm thanh xử lý nốt từ cuối...")
                
                time.sleep(0.35) # Nghỉ 350 miligiây để Vosk Thread kịp cập nhật từ khóa cuối cùng bạn nói
                
                voice_listening = False  # Ngừng nhận diện âm thanh
                
                print("🔄 Đang tiến hành đối chiếu Combo phép...")
                
                # 1. Xác định giọng nói
                if time.time() - voice_timestamp <= 3.0:
                    voice_spell = latest_voice_word
                else:
                    voice_spell = "none"

                # 2. Xác định cử chỉ
                gesture_detected = False
                gesture_idx = 0
                gesture_name = "none"
                gesture_conf = 0.0
                
                if len(sequence) > 5:
                    input_seq = np.array(sequence)
                    if len(input_seq) > MAX_FRAMES:
                        input_seq = input_seq[:MAX_FRAMES]
                    else:
                        padding = np.zeros((MAX_FRAMES - len(input_seq), 63))
                        input_seq = np.vstack((input_seq, padding))
                    input_tensor = tf.convert_to_tensor([input_seq], dtype=tf.float32)
                    
                    res = predict_fn(input_tensor).numpy()[0]
                    gesture_idx = np.argmax(res)
                    gesture_conf = res[gesture_idx]
                    
                    # Một cử chỉ hợp lệ khi vượt ngưỡng threshold và không phải là nhãn Neutral (idx 0)
                    if gesture_conf > THRESHOLD and gesture_idx > 0:
                        gesture_detected = True
                        gesture_name = LABELS[gesture_idx]
                
                # 3. Phân loại 3 trường hợp và gửi sang Unity
                if gesture_detected and voice_spell != "none":
                    # Trường hợp A: Có cả cử chỉ và giọng nói
                    print(f"⚡ [KẾT QUẢ] Cả hai: Cử chỉ {gesture_name} ({gesture_conf:.0%}) + Giọng nói '{voice_spell}'")
                    packet_string = f"{gesture_idx},{voice_spell}"
                    sock.sendto(packet_string.encode('utf-8'), (UDP_IP, UDP_PORT))
                    
                elif gesture_detected and voice_spell == "none":
                    # Trường hợp B: Chỉ có cử chỉ
                    print(f"⚡ [KẾT QUẢ] Chỉ cử chỉ: {gesture_name} ({gesture_conf:.0%}) | Giọng nói: 'none'")
                    packet_string = f"{gesture_idx},none"
                    sock.sendto(packet_string.encode('utf-8'), (UDP_IP, UDP_PORT))
                    
                elif not gesture_detected and voice_spell != "none":
                    # Trường hợp C: Chỉ có giọng nói
                    print(f"⚡ [KẾT QUẢ] Chỉ giọng nói: '{voice_spell}' | Cử chỉ: none")
                    packet_string = f"0,{voice_spell}"  # Gửi cử chỉ 0 (Neutral) kèm theo giọng nói
                    sock.sendto(packet_string.encode('utf-8'), (UDP_IP, UDP_PORT))
                    
                else:
                    # Không nhận được gì hợp lệ
                    print("⚠️ Không nhận dạng được cử chỉ hay giọng nói!")
                
                # Reset bộ đệm giọng nói sau khi đã dùng
                latest_voice_word = "none"
                voice_timestamp = 0.0

            # Chỉ chạy MediaPipe hand landmarker và append landmarks khi đang nhấn giữ E
            if is_recording:
                landmarker.detect_async(mp_image, int(time.time() * 1000))
                
                if latest_hand_landmarks:
                    for hand_landmarks in latest_hand_landmarks:
                        wrist = hand_landmarks[0]
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
                    latest_hand_landmarks = None # Reset để tránh đọc lặp lại frame cũ
                    
            time.sleep(0.01) 

    except KeyboardInterrupt:
        print("\n🛑 Đã tắt AI Sender.")

cap.release()