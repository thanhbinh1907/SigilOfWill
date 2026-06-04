# SỔ TAY PHÁT TRIỂN DỰ ÁN (PROJECT DEVELOPMENT NOTES)

Tài liệu này ghi chép lại toàn bộ quy hoạch cấu trúc dự án, các sửa đổi tính năng gần đây, cùng đánh giá thiết kế OOP để hỗ trợ phát triển dài hạn.

---

## 1. QUY HOẠCH CẤU TRÚC THƯ MỤC (PROJECT STRUCTURE)

Dự án đã được phân chia rõ ràng thành 2 phân vùng chính để tránh chồng chéo tài nguyên:

* **`/Assets/_Project/` (Phân vùng đồ tự làm):** Chứa tất cả tài nguyên, kịch bản, hoạt ảnh và màn chơi do chính bạn tự viết/lắp ghép.
  * **`Art/`**: Hình ảnh, Sprite UI, mô hình 3D tự thiết kế.
  * **`Prefabs/`**: Các Prefab lắp ráp (Player, Enemies, UI...).
  * **`Scenes/`**: Các màn chơi chính (`Cathedral_01`, `Graveyard_01`, `Scene_Main_Menu_01`...).
  * **`Scripts/`**: Toàn bộ mã nguồn tự viết (sử dụng PascalCase không khoảng trắng).
  * **`Settings/`**: Các file cấu hình hệ thống (ví dụ: `PlayerControl.inputactions` đặt trong `Settings/Input/`).
* **`/Assets/Addons/` (Phân vùng thư viện ngoài):** Chứa tất cả các gói tải về từ Unity Asset Store (như `TextMesh Pro`, `Piloto Studio`, `LowPolyRPGWeapons`...).
  * **`Animation_Packs/`**: Các gói hoạt ảnh tải về (`ExplosiveLLC`, `Nephilite`...).
  * **`SFX_Packs/`**: Các gói âm thanh tải về (`Deadly Kombat`, `Footsteps`...).
  * **`VFX_Packs/`**: Các gói hiệu ứng hình ảnh (`FantasySpells`, `Fire Effects`...).
  * **`Environment_Packs/`**: Các gói môi trường lâu đài, hầm ngục (`PolygonDungeon`, `PolygonElvenRealm`...).

### ⚠️ Quy tắc VÀNG khi dọn dẹp thư mục:
* **BẮT BUỘC** thực hiện việc kéo-thả di chuyển thư mục trực tiếp bên trong cửa sổ **Project** của Unity Editor. 
* **KHÔNG** di chuyển file ở ngoài Windows Explorer để tránh làm mất file `.meta` ẩn $\rightarrow$ dẫn đến lỗi **Missing Script** hay mất liên kết.
* Đối với cài đặt FBX (Rig Humanoid, Loop Time, Root Motion...) hay chèn sự kiện Animation: chỉ cần di chuyển file FBX trong Unity, cài đặt này sẽ được bảo toàn nguyên vẹn nhờ file `.meta` di chuyển cùng.

---

## 2. CÁC TÍNH NĂNG VÀ SỬA ĐỔI GẦN ĐÂY (RECENT CHANGES)

### 📌 Hệ thống Loading Screen & Dịch chuyển (Transition):
* **Loading Screen ngẫu nhiên:** Tự động lấy ngẫu nhiên hình nền từ danh sách `loadingScreenBackgrounds` cấu hình trên `WorldSaveGameManager`.
* **Khóa thời gian tối thiểu 5s:** Giữ màn hình load ít nhất 5 giây để che giấu độ trễ nạp cảnh và khởi tạo thực thể.
* **Icon nhấp nháy (Pulsing Icon):** Tạo hiệu ứng nhấp nháy nhẹ (alpha từ 0.1 đến 1.0) cho icon loading ở góc dưới bên phải bằng `CanvasGroup` và hàm `Mathf.PingPong` chạy thực tế (`Time.unscaledTime`).
* **Khóa di chuyển khi load:** Khắc phục lỗi kẹt cứng chuyển động (lock locomotion) của nhân vật khi vừa khởi động xong map bằng cách tắt Root Motion và khôi phục di chuyển trong animation `Empty`.

### 📌 Logic màn chơi & Cản lưu game (Gameplay Save restrictions):
* **Kích hoạt bục (Item Altar Altar):** Gán lớp `ItemRequirementInteractable` để kiểm tra danh sách 6 cây trượng yêu cầu trong Inventory trước khi bật lửa và cổng dịch chuyển. Trạng thái kích hoạt được đồng bộ tự động vào RAM/File save qua từ điển `activatedInteractables`.
* **Cổng dịch chuyển (Teleport Portal):** Đặt tọa độ spawn và nạp cảnh đích độc lập.
* **Chặn Lưu trong khu vực Boss:** 
  * Viết phương thức `IsAnyBossFightActive()` tại `WorldAIManager` kiểm tra xem có trận đấu nào đang diễn ra (`bossFightIsActive == true`) hay không.
  * Sửa hàm `SaveGame()` để thoát sớm nếu Boss chưa bị tiêu diệt và đang chiến đấu, ngăn chặn lưu ghi đè ở autosave hoặc khi thoát game giữa chừng.

### 📌 Dọn dẹp cảnh báo Compiler (Clean Warnings):
* **`WorldAIManager.cs`**: Thay thế hàm lỗi thời `FindObjectsOfType<T>(true)` thành `FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)` tối ưu hơn.
* **`CharacterFootStepSFXMaker.cs`**: Sử dụng biến khoảng cách dò đất `distanceToGround` thay vì viết cứng giá trị `0.05f` trong lệnh Raycast.

---

## 3. ĐÁNH GIÁ KIẾN TRÚC OOP & PHÒNG NGỪA PHỒNG FILE (CODE BLOAT)

* **Thiết kế Kế thừa & Thành phần:** Lớp cha `CharacterManager` nắm giữ HP/Mana/Stamina cơ bản. Lớp con `PlayerManager` đóng vai trò điều phối trung tâm (Mediator) liên kết các Manager thành phần nhỏ (`PlayerLocomotionManager`, `PlayerStatsManager`...). Đây là thiết kế chuẩn OOP.
* **Nguy cơ phồng file (God Class) cần lưu ý:**
  1. `PlayerManager.cs`: Hàm gán dữ liệu save/load (`SaveGameDataToCurrentCharacterData`) có thể bị phồng khi có thêm nhiều chỉ số. Nên tách riêng thành một lớp Mapper nếu số lượng chỉ số tăng quá nhiều.
  2. `WorldSaveGameManager.cs`: Đang gánh cả logic lưu trữ, chuyển cảnh lẫn hiệu ứng hiển thị UI. Nên tách phần hiệu ứng UI màn hình load ra một script riêng để manager này chỉ chuyên tâm xử lý dữ liệu lưu trữ vật lý.
  3. `WorldItemDatabase.cs`: Nếu số lượng vũ khí lớn, việc khai báo mảng trong C# sẽ rất nặng. Hãy cân nhắc dùng **ScriptableObjects** cho từng vũ khí để tải động thông minh.
