import cv2
import numpy as np
import mediapipe as mp
import time
import os
import sys
import math

# Đảm bảo in các ký tự Unicode tiếng Việt không bị lỗi trên console Windows
if hasattr(sys.stdout, 'reconfigure'):
    sys.stdout.reconfigure(encoding='utf-8')

def rotate_y(x, y, z, angle):
    """Xoay quanh trục Y (Yaw)"""
    cos_a = math.cos(angle)
    sin_a = math.sin(angle)
    rx = x * cos_a - z * sin_a
    rz = x * sin_a + z * cos_a
    ry = y
    return rx, ry, rz

def rotate_x(x, y, z, angle):
    """Xoay quanh trục X (Pitch)"""
    cos_a = math.cos(angle)
    sin_a = math.sin(angle)
    rx = x
    ry = y * cos_a - z * sin_a
    rz = y * sin_a + z * cos_a
    return rx, ry, rz

def project_3d(x, y, z, cx, cy, f, d):
    """Chiếu phối cảnh 3D về màn hình 2D"""
    # d là khoảng cách từ camera ảo tới tâm vật thể
    # f là tiêu cự (focal length multiplier)
    denom = d + rz
    if abs(denom) < 0.001:
        denom = 0.001 if denom >= 0 else -0.001
    sx = int(cx + (rx * f) / denom)
    sy = int(cy + (ry * f) / denom)
    return sx, sy

def main():
    current_dir = os.path.dirname(os.path.abspath(__file__))
    model_path = os.path.join(current_dir, "hand_landmarker.task")
    
    if not os.path.exists(model_path):
        print(f"[LOI] Không tìm thấy file model: {model_path}!")
        return

    BaseOptions = mp.tasks.BaseOptions
    HandLandmarker = mp.tasks.vision.HandLandmarker
    HandLandmarkerOptions = mp.tasks.vision.HandLandmarkerOptions
    VisionRunningMode = mp.tasks.vision.RunningMode

    latest_result = []

    def result_callback(result, output_image, timestamp_ms):
        nonlocal latest_result
        if result.hand_landmarks:
            latest_result = result.hand_landmarks
        else:
            latest_result = []

    options = HandLandmarkerOptions(
        base_options=BaseOptions(model_asset_path=model_path),
        running_mode=VisionRunningMode.LIVE_STREAM,
        num_hands=1,
        result_callback=result_callback
    )

    cap = cv2.VideoCapture(0)
    if not cap.isOpened():
        print("[LOI] Không thể mở Camera!")
        return

    # Các đoạn nối xương bàn tay (Connections)
    connections = [
        # Ngón cái
        (0, 1), (1, 2), (2, 3), (3, 4),
        # Ngón trỏ
        (0, 5), (5, 6), (6, 7), (7, 8),
        # Ngón giữa
        (0, 9), (9, 10), (10, 11), (11, 12),
        # Ngón áp út
        (0, 13), (13, 14), (14, 15), (15, 16),
        # Ngón út
        (0, 17), (17, 18), (18, 19), (19, 20),
        # Lòng bàn tay
        (5, 9), (9, 13), (13, 17)
    ]

    print("\n=======================================================")
    print("🔮 KHỞI ĐỘNG HOLOGRAPHIC 3D MAGIC HAND VISUALIZER 🔮")
    print("-> Đang liên kết Camera và phân tích chiều sâu AI...")
    print("-> Nhấn 'B' để bật/tắt hiển thị camera nền bên trái.")
    print("-> Nhấn 'Q' hoặc 'ESC' để đóng trình mô phỏng.")
    print("=======================================================\n")

    show_camera_bg = True
    yaw_angle = 0.0

    with HandLandmarker.create_from_options(options) as landmarker:
        while cap.isOpened():
            ret, frame = cap.read()
            if not ret:
                break

            frame = cv2.flip(frame, 1)
            h, w, c = frame.shape
            
            # Cắt ảnh camera sang hình vuông hoặc tỷ lệ chuẩn để hiển thị song song
            # Chúng ta sẽ hiển thị 2 bảng điều khiển kích thước 480x480 song song
            target_size = 480
            frame_resized = cv2.resize(frame, (target_size, target_size))

            rgb_frame = cv2.cvtColor(frame_resized, cv2.COLOR_BGR2RGB)
            mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)

            # Phân tích bất đồng bộ
            landmarker.detect_async(mp_image, int(time.time() * 1000))

            # 1. Khởi tạo Panel trái (2D Overlay)
            left_panel = frame_resized.copy() if show_camera_bg else np.zeros((target_size, target_size, 3), dtype=np.uint8)
            
            # Vẽ grid mờ cho Panel trái nếu ở chế độ nền đen
            if not show_camera_bg:
                for y in range(0, target_size, 40):
                    cv2.line(left_panel, (0, y), (target_size, y), (30, 15, 5), 1)
                for x in range(0, target_size, 40):
                    cv2.line(left_panel, (x, 0), (x, target_size), (30, 15, 5), 1)

            # 2. Khởi tạo Panel phải (3D Hologram Viewport)
            right_panel = np.zeros((target_size, target_size, 3), dtype=np.uint8)
            
            # Vẽ lưới radar/vòng tròn tọa độ 3D ở nền Panel phải
            view_cx, view_cy = target_size // 2, target_size // 2
            cv2.circle(right_panel, (view_cx, view_cy), 200, (40, 20, 10), 1, cv2.LINE_AA)
            cv2.circle(right_panel, (view_cx, view_cy), 120, (40, 20, 10), 1, cv2.LINE_AA)
            cv2.line(right_panel, (view_cx - 210, view_cy), (view_cx + 210, view_cy), (40, 20, 10), 1)
            cv2.line(right_panel, (view_cx, view_cy - 210), (view_cx, view_cy + 210), (40, 20, 10), 1)
            cv2.putText(right_panel, "HOLOGRAPHIC 3D VIEWPORT (AUTO-ROTATE)", (20, 30), 
                        cv2.FONT_HERSHEY_SIMPLEX, 0.4, (0, 200, 255), 1, cv2.LINE_AA)

            # Tự động xoay góc Y của viewport 3D theo thời gian
            yaw_angle = time.time() * 1.0 # Tốc độ xoay
            pitch_angle = 0.35 # Góc nhìn từ trên xuống (khoảng 20 độ)

            if latest_result:
                hand_landmarks = latest_result[0]
                
                # --- VẼ CHO PANEL TRÁI (2D OVERLAY CÓ ĐỘ SÂU) ---
                # Sắp xếp vẽ theo chiều sâu để tạo hiệu ứng 3D (Painter's Algorithm)
                # Tính độ sâu trung bình của từng khớp và từng xương nối
                draw_queue = []
                
                # Khớp xương 2D quy ra pixel
                coords_2d = []
                for lm in hand_landmarks:
                    cx, cy = int(lm.x * target_size), int(lm.y * target_size)
                    coords_2d.append((cx, cy))
                
                # Thêm các khớp xương vào hàng đợi vẽ (loại="joint", z, vị trí, index)
                for idx, lm in enumerate(hand_landmarks):
                    draw_queue.append({
                        "type": "joint",
                        "z": lm.z,
                        "pt": coords_2d[idx]
                    })
                
                # Thêm các đường nối xương vào hàng đợi vẽ (loại="bone", z_trung_binh, pt1, pt2)
                for start_idx, end_idx in connections:
                    z_avg = (hand_landmarks[start_idx].z + hand_landmarks[end_idx].z) / 2.0
                    draw_queue.append({
                        "type": "bone",
                        "z": z_avg,
                        "pt1": coords_2d[start_idx],
                        "pt2": coords_2d[end_idx]
                    })
                
                # Sắp xếp hàng đợi vẽ theo Z giảm dần (Z lớn vẽ trước - ở xa vẽ trước, Z nhỏ vẽ sau - ở gần đè lên)
                draw_queue.sort(key=lambda item: item["z"], reverse=True)
                
                # Thực hiện vẽ đè lên Panel trái
                for element in draw_queue:
                    # z thông thường dao động từ -0.15 (gần) đến 0.15 (xa)
                    # Quy đổi scale dựa trên độ sâu z
                    z_val = element["z"]
                    scale = 1.0 - z_val * 4.0
                    scale = max(0.4, min(2.5, scale)) # Giới hạn tỉ lệ
                    
                    if element["type"] == "bone":
                        pt1, pt2 = element["pt1"], element["pt2"]
                        # Hiệu ứng phát sáng neon xanh cyan
                        cv2.line(left_panel, pt1, pt2, (255, 120, 0), max(1, int(6 * scale)), cv2.LINE_AA) # Quầng sáng ngoài
                        cv2.line(left_panel, pt1, pt2, (255, 255, 210), max(1, int(2 * scale)), cv2.LINE_AA) # Lõi sáng trong
                    elif element["type"] == "joint":
                        pt = element["pt"]
                        # Khớp phát sáng neon cam/vàng
                        cv2.circle(left_panel, pt, max(1, int(8 * scale)), (0, 160, 255), -1, cv2.LINE_AA)
                        cv2.circle(left_panel, pt, max(1, int(4 * scale)), (255, 255, 255), -1, cv2.LINE_AA)

                # Vẽ vòng tròn ma pháp ở tâm lòng bàn tay trên Panel trái
                palm_center_2d = coords_2d[9]
                cv2.circle(left_panel, palm_center_2d, 25, (0, 215, 255), 1, cv2.LINE_AA)
                cv2.line(left_panel, (palm_center_2d[0]-6, palm_center_2d[1]), (palm_center_2d[0]+6, palm_center_2d[1]), (0, 215, 255), 1)
                cv2.line(left_panel, (palm_center_2d[0], palm_center_2d[1]-6), (palm_center_2d[0], palm_center_2d[1]+6), (0, 215, 255), 1)


                # --- VẼ CHO PANEL PHẢI (XOAY PHỐI CẢNH 3D HOLOGRAPHIC) ---
                # Trọng tâm xoay là khớp số 9 (lòng bàn tay) để tay không bị bay lệch ra khỏi khung hình khi xoay
                ref_lm = hand_landmarks[9]
                
                # Tính toán tọa độ 3D xoay chiều và chiếu phối cảnh
                coords_3d_proj = []
                focal_length = 500.0  # Tiêu cự camera ảo
                cam_dist = 0.5        # Khoảng cách camera ảo
                
                for lm in hand_landmarks:
                    # Lấy tọa độ tương đối so với trọng tâm lòng bàn tay
                    rx = lm.x - ref_lm.x
                    ry = lm.y - ref_lm.y
                    rz = lm.z - ref_lm.z
                    
                    # 1. Xoay quanh trục Y (Xoay ngang)
                    rx, ry, rz = rotate_y(rx, ry, rz, yaw_angle)
                    # 2. Xoay quanh trục X (Nghiêng nhìn từ trên xuống)
                    rx, ry, rz = rotate_x(rx, ry, rz, pitch_angle)
                    
                    # Chiếu phối cảnh 3D -> 2D
                    denom = cam_dist + rz
                    if abs(denom) < 0.001:
                        denom = 0.001
                    sx = int(view_cx + (rx * focal_length) / denom)
                    sy = int(view_cy + (ry * focal_length) / denom)
                    
                    coords_3d_proj.append({
                        "x": sx,
                        "y": sy,
                        "z": rz # Lưu lại độ sâu rz để depth sort cho Viewport 3D
                    })

                # Vẽ vòng tròn ma pháp 3D ở đế (Hologram Stand Ring) dưới bàn tay
                # Ta vẽ một hình tròn nằm trên mặt phẳng XZ (Y cố định = 0.12 bên dưới lòng bàn tay)
                ring_points_3d = []
                num_segments = 24
                ring_radius = 0.10
                ring_y = 0.12
                for i in range(num_segments):
                    ang = (2 * math.pi * i) / num_segments
                    rx = ring_radius * math.cos(ang)
                    rz = ring_radius * math.sin(ang)
                    ry = ring_y
                    
                    # Xoay theo hệ tọa độ chung của Viewport
                    rx, ry, rz = rotate_y(rx, ry, rz, yaw_angle)
                    rx, ry, rz = rotate_x(rx, ry, rz, pitch_angle)
                    
                    # Chiếu phối cảnh
                    denom = cam_dist + rz
                    if abs(denom) < 0.001:
                        denom = 0.001
                    sx = int(view_cx + (rx * focal_length) / denom)
                    sy = int(view_cy + (ry * focal_length) / denom)
                    ring_points_3d.append((sx, sy))

                # Nối các điểm tạo thành vòng tròn 3D phát sáng màu xanh neon mờ
                for i in range(num_segments):
                    pt1 = ring_points_3d[i]
                    pt2 = ring_points_3d[(i + 1) % num_segments]
                    cv2.line(right_panel, pt1, pt2, (200, 100, 0), 2, cv2.LINE_AA) # Xanh dương nhạt

                # Tiến hành Depth Sort cho Panel phải
                draw_queue_3d = []
                for idx, pt_proj in enumerate(coords_3d_proj):
                    draw_queue_3d.append({
                        "type": "joint",
                        "z": pt_proj["z"],
                        "pt": (pt_proj["x"], pt_proj["y"])
                    })
                
                for start_idx, end_idx in connections:
                    z_avg = (coords_3d_proj[start_idx]["z"] + coords_3d_proj[end_idx]["z"]) / 2.0
                    draw_queue_3d.append({
                        "type": "bone",
                        "z": z_avg,
                        "pt1": (coords_3d_proj[start_idx]["x"], coords_3d_proj[start_idx]["y"]),
                        "pt2": (coords_3d_proj[end_idx]["x"], coords_3d_proj[end_idx]["y"])
                    })
                
                # Sắp xếp vẽ từ xa tới gần
                draw_queue_3d.sort(key=lambda item: item["z"], reverse=True)
                
                for element in draw_queue_3d:
                    z_val = element["z"]
                    # Tỉ lệ phóng to thu nhỏ dựa trên Z
                    scale = 1.0 - z_val * 4.0
                    scale = max(0.4, min(2.5, scale))
                    
                    if element["type"] == "bone":
                        pt1, pt2 = element["pt1"], element["pt2"]
                        # Hologram màu xanh dương phát sáng (Neon Blue Glow)
                        cv2.line(right_panel, pt1, pt2, (230, 80, 0), max(1, int(5 * scale)), cv2.LINE_AA) # Quầng sáng xanh biển
                        cv2.line(right_panel, pt1, pt2, (255, 230, 200), max(1, int(2 * scale)), cv2.LINE_AA) # Lõi trắng
                    elif element["type"] == "joint":
                        pt = element["pt"]
                        # Điểm khớp màu xanh ngọc / trắng
                        cv2.circle(right_panel, pt, max(1, int(7 * scale)), (200, 200, 0), -1, cv2.LINE_AA)
                        cv2.circle(right_panel, pt, max(1, int(3 * scale)), (255, 255, 255), -1, cv2.LINE_AA)

            else:
                # Nếu không nhận diện thấy tay, hiển thị thông báo
                cv2.putText(right_panel, "WAITING FOR HAND GESTURE...", (view_cx - 110, view_cy), 
                            cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 100, 255), 1, cv2.LINE_AA)

            # Ghép song song 2 Panel thành một cửa sổ lớn kích thước 960x480
            combined_window = np.hstack((left_panel, right_panel))

            # Hiển thị cửa sổ ghép
            cv2.imshow("Sigil Of Will - Holographic 3D Hand Visualizer", combined_window)

            key = cv2.waitKey(1)
            if key & 0xFF == ord('b') or key & 0xFF == ord('B'):
                show_camera_bg = not show_camera_bg
                print(f"[INFO] Đã chuyển đổi hiển thị camera nền: {show_camera_bg}")
            elif key & 0xFF == ord('q') or key == 27:
                break

    cap.release()
    cv2.destroyAllWindows()
    print("👋 Đã đóng trình mô phỏng 3D.")

if __name__ == "__main__":
    main()
