GAME DESIGN DOCUMENT: CƠ CHẾ CỐT LÕI (CORE MECHANICS)
Dự án: Sigil of Will - Single-player Gesture-based and Voice Action RPG

I. Vòng Lặp Trạng Thái (State Loop) & Điều Khiển
Game được thiết kế xoay quanh hai trạng thái chuyển đổi mượt mà để giải quyết việc sử dụng luân phiên chuột và cử chỉ tay.

1. Trạng Thái Khám Phá (Exploration Mode)
Kích hoạt: Trạng thái mặc định khi không có kẻ địch trong phạm vi.

Mục tiêu: Di chuyển, tìm đường, tương tác môi trường.

Điều khiển:

WASD: Di chuyển nhân vật.

Chuột (Mouse Delta): Xoay camera tự do 360 độ. Left-click/Right-click có thể gán cho tương tác nhẹ (nhặt đồ, mở cửa).

Cảm biến AI (MediaPipe): Ở trạng thái nghỉ ngơi (Standby) để tiết kiệm tài nguyên hệ thống

2. Trạng Thái Chiến Đấu (Combat Mode)
Kích hoạt: Tự động kích hoạt khi bước vào "vùng đụng độ" (Combat Zone/Arena) hoặc bị quái vật phát hiện (Aggro).

Mục tiêu: Sinh tồn và tiêu diệt mục tiêu bằng phép thuật.

Điều khiển & Chuyển đổi:

Auto-Lock Camera: Camera lập tức vô hiệu hóa đầu vào của chuột. Thay vào đó, Camera sử dụng thuật toán (ví dụ: Quaternion.Slerp trong Unity) để tự động khóa góc nhìn vào kẻ địch nguy hiểm nhất/gần nhất. Dưới chân quái vật xuất hiện vòng tròn báo hiệu đang bị khóa mục tiêu (Target Indicator).

Tay phải (Buông chuột): Người chơi đưa tay phải lên vùng nhận diện của webcam để chuẩn bị thi triển các "Sigil" đồng thời với việc đọc thần chú (fireball, thunderbolt, windblade).

Tay trái (Bàn phím): Vẫn đặt trên WASD để điều khiển nhân vật di chuyển và lùi/né, nhảy.

II. Hệ Thống Thể Lực và Năng Lượng (Stamina & Mana System)

Stamina và Mana là cốt lõi để cân bằng nhịp độ, buộc người chơi phải chiến đấu có nhịp điệu (Rhythm) thay vì spam chiêu loạn xạ.

Hiển thị: Hai thanh năng lượng UI rõ ràng trên màn hình. Màu xanh lá là Mana còn xanh dương là Stamina

Tiêu hao Stamina:

Dodge/Backstep/Sprint (Phím Shift), Jump (Phím Space): Tiêu tốn một khối lượng Stamina cố định. Hết Stamina không thể né.

Khi tung chiêu sẽ tiêu tốn 1 lượng mana tùy vào chiêu thức được tung ra

Hồi phục Stamina:

Tự động hồi phục nhanh chóng khi nhân vật ở Trạng Thái None (không giữ phím cast chiêu, tay buông thõng hoặc không đưa vào camera) VÀ không thực hiện hành động né tránh.

Hình phạt (Exhaustion): Nếu Stamina cạn kiệt (về 0), nhân vật rơi vào trạng thái không thể cast phép hay né tránh, chỉ có thể đi bộ , trở thành mục tiêu dễ bị tấn công.

Hồi phục Mana: 

- Hồi phục chậm hơn stamina nhưng không bị ảnh hưởng bởi trạng thái nhân vật ( trừ khi cast chiêu còn không sẽ hồi phục kết cả khi đang thực hiện hành động chạy, nhảy, né ) 

III. Hệ Thống Thi Triển Phép Thuật (Gesture & Voice Casting)
Sử dụng AI kết hợp vật lý để định hình phong cách chiến đấu.

1. Cơ chế Kích hoạt (The Trigger)
Để bắt đầu đọc cử chỉ (ngăn AI nhận diện nhầm khi người chơi vung tay vô tình), người chơi BẮT BUỘC phải đè phím "E" bằng tay trái, đồng thời tay phải thực hiện cử chỉ và.

Khi nhả phím "E", hành động tung chiêu lập tức kết thúc/hủy bỏ.

2. Bộ Kỹ Năng (The Sigil Arsenal)
Hệ thống sử dụng mạng LSTM và MediaPipe để nhận diện chuỗi hành động thời gian thực:

[ẤN CHÚ 1] Fireball (Cầu Lửa):

Cử chỉ: Tụ lực tay -> Đẩy mạnh ra phía trước.

Ngôn ngữ : Đọc fireball

Đặc tính: Sát thương cao, tốc độ bay chậm. 

[ẤN CHÚ 2] WindBlade (Phong Nhẫn):

Cử chỉ: Vung tay chém ngang hoặc dọc dứt khoát.

Ngôn ngữ : Đọc windblade

Đặc tính: Chém lưỡi dao gió (melee attack) gây sát thương diện rộng (AoE) hình nón phía trước. Cực kỳ hiệu quả để càn quét bầy quái vật nhỏ lao tới đông đúc (Swarm).

[ẤN CHÚ 3] Thunderbolt (Lôi Kích):

Cử chỉ: Đưa tay chỉ thẳng vào màn hình (hướng về phía quái vật).

Ngôn ngữ : Đọc thunderbolt

Đặc tính: Bắn ra tia sét đánh trúng mục tiêu ngay lập tức (Hitscan). Sát thương cực lớn nhưng tiêu tốn rất nhiều mana.

IV. Nhịp Độ & Vòng Lặp Trận Đấu (Combat Loop)
Một trận chiến điển hình (đặc biệt là Đánh Boss) sẽ diễn ra theo vòng lặp sau:

Nhận diện: Game tự động khóa mục tiêu. Người chơi buông chuột, chuẩn bị tay phải và miệng để đọc thần chú.

Quan sát & Né tránh: Tay trái dùng WASD + Shift để né các đòn tấn công của Boss (tiêu tốn stamina). 

Phản công (Casting): Tìm khoảng trống (Window of Opportunity), đè phím "E", tụ lực tay phải đẩy ra và niệm chú trong thời gian giữ phím E.

Hồi phục (None State): Thả phím "E", để hoàn thành việc cast chiêu, chiêu thức sẽ được nhân vật thi triển
