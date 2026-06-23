import subprocess
import shutil
import os
import sys

# Đảm bảo in các ký tự Unicode tiếng Việt và emoji không bị lỗi trên console Windows
if hasattr(sys.stdout, 'reconfigure'):
    sys.stdout.reconfigure(encoding='utf-8')
if hasattr(sys.stderr, 'reconfigure'):
    sys.stderr.reconfigure(encoding='utf-8')

def build():
    # 1. Xác định các đường dẫn
    current_dir = os.path.dirname(os.path.abspath(__file__))
    script_path = os.path.join(current_dir, "AI_Sender_Ges&Voice.py")
    dist_dir = os.path.join(current_dir, "dist")
    build_dir = os.path.join(current_dir, "build")
    spec_path = os.path.join(current_dir, "AI_Sender_Ges_Voice.spec")
    
    # 2. Xóa các thư mục build cũ nếu có để tránh xung đột
    for path in [dist_dir, build_dir, spec_path]:
        if os.path.exists(path):
            try:
                if os.path.isdir(path):
                    shutil.rmtree(path)
                else:
                    os.remove(path)
            except Exception as e:
                print(f"[CANH BAO] Khi don dep thu muc cu {path}: {e}")
                
    print("[INFO] Dang chay PyInstaller de dong goi AI (qua trinh nay co the mat vai phut)...")
    
    # 3. Chạy lệnh PyInstaller
    # --onedir: Đóng gói thành một thư mục chứa file .exe và các thư viện .dll đi kèm (tối ưu nhất cho TensorFlow)
    # --name: Đặt tên file exe đầu ra là AI_Sender
    # --collect-all: Đảm bảo PyInstaller copy đầy đủ DLL, assets của vosk và mediapipe
    # --exclude-module: Loại bỏ PyTorch (torch, torchvision, torchaudio) vì dự án sử dụng TensorFlow, giúp giảm ~3.6GB dung lượng đóng gói
    cmd = [
        "pyinstaller",
        "--onedir",
        "--name=AI_Sender_Ges_Voice",
        "--collect-all=vosk",
        "--collect-all=mediapipe",
        "--exclude-module=torch",
        "--exclude-module=torchvision",
        "--exclude-module=torchaudio",
        script_path
    ]
    
    print(f"Đang thực thi: {' '.join(cmd)}")
    # Chạy lệnh sử dụng môi trường python hiện hành (có pip cài đặt pyinstaller)
    result = subprocess.run(cmd, cwd=current_dir, shell=True)
    if result.returncode != 0:
        print("[LOI] Khi chay PyInstaller!")
        sys.exit(1)
        
    print("[SUCCESS] PyInstaller bien dich hoan thanh!")
    
    # 4. Copy các file model vào thư mục đầu ra
    ai_dist_dir = os.path.join(dist_dir, "AI_Sender_Ges_Voice")
    
    models_to_copy = [
        ("model_camera", "dir"),
        ("model_voice", "dir"),
        ("hand_landmarker.task", "file")
    ]
    
    for item, item_type in models_to_copy:
        src = os.path.join(current_dir, item)
        dst = os.path.join(ai_dist_dir, item)
        
        if not os.path.exists(src):
            print(f"[CANH BAO] Khong tim thay nguon {src}")
            continue
            
        print(f"[COPY] Dang sao chep {item} -> {dst}...")
        try:
            if item_type == "dir":
                if os.path.exists(dst):
                    shutil.rmtree(dst)
                shutil.copytree(src, dst)
            else:
                if os.path.exists(dst):
                    os.remove(dst)
                shutil.copy2(src, dst)
        except Exception as e:
            print(f"[LOI] Khi copy {item}: {e}")
            sys.exit(1)
            
    # 5. Dọn dẹp thư mục build tạm thời để giải phóng dung lượng
    if os.path.exists(build_dir):
        print("[INFO] Dang don dep thu muc build tam thoi...")
        try:
            shutil.rmtree(build_dir)
            print("[SUCCESS] Da don dep thu muc build!")
        except Exception as e:
            print(f"[CANH BAO] Khong the tu dong xoa thu muc build: {e}")
            
    print("\n[SUCCESS] Dong goi AI thanh cong! Thu muc ket qua nam tai: Model_SOW/dist/AI_Sender_Ges_Voice")

if __name__ == "__main__":
    build()
